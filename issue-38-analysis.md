# Issue #38: IgnoredUrls does not support wildcard matching

## Summary

`IgnoredUrls` in `Raygun4MauiSettings` performs exact string equality matching only. Users cannot filter dynamic URLs containing query parameters, tokens, or IDs. There is also no extension point to intercept or modify URLs before they are sent to the RUM API.

## Analysis

### The C# `ShouldIgnore` implementation (exact match only)

**File:** `Raygun4Maui/MauiRUM/EventTrackers/RaygunNetworkTracker.cs:103-114`

```csharp
public static bool ShouldIgnore(string url)
{
    if (string.IsNullOrEmpty(url))
    {
        return true;
    }

    var isUrlInSettingsIgnored = _settings?.IgnoredUrls?.Contains(url) ?? false;
    var isUrlInDefaultIgnored = _defaultIgnoredUrls?.Contains(url) ?? false;

    return isUrlInSettingsIgnored || isUrlInDefaultIgnored;
}
```

`IList<string>.Contains(url)` calls `string.Equals` on each element — this is strict exact-match comparison. A pattern like `"*DOCS_FILE_DOWNLOAD*"` is compared literally and will never match a real URL.

The same issue exists in `RaygunViewTracker.ShouldIgnore` (`RaygunViewTracker.cs:120-123`):

```csharp
public static bool ShouldIgnore(string viewName)
{
    return _settings.IgnoredViews != null && _settings.IgnoredViews.Contains(viewName);
}
```

### The native iOS `shouldIgnore` uses prefix/suffix matching

**File:** `raygun4xamarin/Raygun4Maui.Native.NetworkMonitor.iOS/Shared/RaygunNetworkMonitor.m:431-450`

```objc
+ (bool)shouldIgnore:(NSURLRequest *)request
{
    if (!enabled || request == nil || request.URL == nil || request.URL.relativeString == nil)
    {
        return true;
    }

    NSString* urlString = request.URL.relativeString;

    for (NSString* ignoredUrl in ignoredUrls)
    {
        if ([urlString hasPrefix:ignoredUrl]
         || [urlString hasSuffix:ignoredUrl])
        {
            return true;
        }
    }

    return false;
}
```

The native side uses `hasPrefix:` / `hasSuffix:` — slightly more flexible than exact match but still does not support wildcards or substring matching. Note that the native `ignoredUrls` set is hardcoded to Raygun API endpoints only (line 86-87) and is not user-configurable, so the mismatch in matching strategy is not directly impactful today. User-configured `IgnoredUrls` are only checked on the C# side.

### Dual filtering layers

Network requests pass through two filtering stages:

1. **Native layer** (`RaygunNetworkMonitor.m:shouldIgnore:`) — filters at the swizzle/intercept level using prefix/suffix matching against hardcoded Raygun API URLs. User `IgnoredUrls` are **not** passed to the native layer.

2. **C# layer** (`RaygunNetworkTracker.ShouldIgnore`) — filters after the native layer posts a notification. This is where user-configured `IgnoredUrls` are checked, using exact match.

```
NSURLSessionTask.resume
  → _swizzle_resume
    → networkRequestStarted: (native shouldIgnore: prefix/suffix check)
      → ... request completes ...
        → NSNotification posted
          → RaygunAppleNativeNetworkObserver.OnNetworkRequestOccurredEvent
            → RaygunNetworkTracker.OnNetworkRequestFinishedEvent
              → ShouldIgnore(url)  ← exact match check with user IgnoredUrls
                → if not ignored, event is sent to RUM API
```

### Default ignored URLs

**C# side** (`RaygunNetworkTracker.cs:74-78`):
```csharp
_defaultIgnoredUrls = new List<string>
{
    _settings.RumApiEndpoint.AbsoluteUri,        // "https://api.raygun.com/events"
    _settings.RaygunSettings.ApiEndpoint.AbsoluteUri  // e.g. "https://api.raygun.com/entries"
};
```

These are full absolute URIs and are matched exactly. This works because Raygun's own API calls use predictable URLs. But any user-facing URL with query parameters cannot be matched this way.

**Native side** (`RaygunNetworkMonitor.m:86-87`):
```objc
[ignoredUrls addObject:@"api.raygun.com/entries"];
[ignoredUrls addObject:@"api.raygun.com/events"];
```

These are partial strings matched with prefix/suffix — more lenient but not wildcard-aware.

### The `IgnoredViews` setting has the same issue

`RaygunViewTracker.cs:120-123` also uses `Contains()` for view name matching. While view names are typically static class names (so exact match is more reasonable), wildcard support would still be useful for filtering groups of views by naming convention.

### Visibility of internal types

The issue also requests making network tracker interfaces public. Currently:

- `IRaygunNetworkTracker` — does not exist (no interface defined)
- `RaygunNetworkTracker` — public static class, but no extension points
- `RaygunNetworkTrackerFactory` — does not exist

There is no way for developers to:
- Intercept URLs before they are sent to RUM
- Mask sensitive query parameters
- Implement custom filtering logic beyond `IgnoredUrls`

## Recommendation

### 1. Add wildcard/glob matching to `ShouldIgnore`

Replace exact `Contains()` with pattern matching that treats `*` as a wildcard. The matching should be case-insensitive since URLs are case-insensitive in practice.

Affected methods:
- `RaygunNetworkTracker.ShouldIgnore` (`RaygunNetworkTracker.cs:103`)
- `RaygunViewTracker.ShouldIgnore` (`RaygunViewTracker.cs:120`)

### 2. Consider a URL sanitization callback

Add an optional `Func<string, string>` property (e.g., `UrlSanitizer`) to `Raygun4MauiSettings` that is called before URLs are sent to RUM. This would let developers strip or mask sensitive query parameters without needing to ignore the entire URL.

### 3. Align native and C# ignore strategies

The native `shouldIgnore:` uses prefix/suffix matching while C# uses exact match. If wildcard support is added to the C# layer, consider whether the native layer should also support user-configured patterns (to avoid unnecessary notification overhead for ignored URLs).
