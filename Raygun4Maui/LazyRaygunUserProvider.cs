using Mindscape.Raygun4Net;

namespace Raygun4Maui;

/// <summary>
/// Resolves the user provider on the first GetUser call rather than when RaygunMauiClient is created
/// </summary>
internal sealed class LazyRaygunUserProvider : IRaygunUserProvider
{
    private readonly Lazy<IRaygunUserProvider> _provider;

    public LazyRaygunUserProvider(Func<IRaygunUserProvider> resolve)
    {
        _provider = new Lazy<IRaygunUserProvider>(resolve);
    }

    public RaygunIdentifierMessage GetUser()
    {
        return _provider.Value?.GetUser();
    }
}
