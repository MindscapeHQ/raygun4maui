# Issue #37: EXC_BAD_ACCESS in RaygunNetworkMonitor

## Crash Report

```
Exception Type:  EXC_BAD_ACCESS (SIGSEGV)
Exception Subtype: KERN_INVALID_ADDRESS at 0x0000000000000000

Thread 0 Crashed:
0   CoreFoundation         -[__NSDictionaryM objectForKey:] + 136
1   RaygunNetworkMonitor   +[RaygunNetworkMonitor networkRequestStarted:] + 164
2   RaygunNetworkMonitor   _swizzle_resume + 48
```

## Disassembly

### `_swizzle_resume` (at 0x432c)

Called whenever `NSURLSessionTask.resume` is invoked. Passes the task to `networkRequestStarted:`, then calls the original `resume` implementation.

```asm
__swizzle_resume:
0x432c  stp   x22, x21, [sp, #-0x30]!
0x4330  stp   x20, x19, [sp, #0x10]
0x4334  stp   x29, x30, [sp, #0x20]
0x4338  add   x29, sp, #0x20
0x433c  mov   x19, x1                          ; save _cmd
0x4340  adrp  x8, 12
0x4344  ldr   x20, [x8, #0x890]                ; x20 = RaygunNetworkMonitor class
0x4348  bl    _objc_retain                      ; retain task (self)
0x434c  mov   x21, x0                           ; x21 = retained task
0x4350  mov   x0, x20                           ; receiver = RaygunNetworkMonitor
0x4354  mov   x2, x21                           ; arg = task
0x4358  bl    _objc_msgSend$networkRequestStarted:  ; +164 crash offset traces back here
0x435c  adrp  x8, 12
0x4360  ldr   x8, [x8, #0xad8]                 ; x8 = _original_resume_imp
0x4364  mov   x0, x21                           ; self = task
0x4368  mov   x1, x19                           ; _cmd = original selector
0x436c  blr   x8                                ; call original resume
0x4370  mov   x0, x21
0x4374  ldp   x29, x30, [sp, #0x20]
0x4378  ldp   x20, x19, [sp, #0x10]
0x437c  ldp   x22, x21, [sp], #0x30
0x4380  b     _objc_release                     ; release task
```

### `+[RaygunNetworkMonitor networkRequestStarted:]` (at 0x5e6c)

This is where the crash occurs. The method:
1. Gets `task.originalRequest` and checks `shouldIgnore:`
2. Records `CACurrentMediaTime()` as the start time
3. Gets the associated task ID
4. Uses the task ID as a key into the shared `timers` dictionary — **crashes here**

```asm
+[RaygunNetworkMonitor networkRequestStarted:]:
; ---- prologue ----
0x5e6c  stp   x24, x23, [sp, #-0x40]!
0x5e70  stp   x22, x21, [sp, #0x10]
0x5e74  stp   x20, x19, [sp, #0x20]
0x5e78  stp   x29, x30, [sp, #0x30]
0x5e7c  add   x29, sp, #0x30

; ---- retain task ----
0x5e80  mov   x0, x2                            ; x0 = task argument
0x5e84  bl    _objc_retain
0x5e88  mov   x19, x0                           ; x19 = retained task

; ---- check shouldIgnore ----
0x5e8c  adrp  x8, 11
0x5e90  ldr   x20, [x8, #0x890]                 ; x20 = RaygunNetworkMonitor class
0x5e94  bl    _objc_msgSend$originalRequest      ; [task originalRequest]
0x5e98  mov   x29, x29
0x5e9c  bl    _objc_retainAutoreleasedReturnValue
0x5ea0  mov   x21, x0                           ; x21 = originalRequest
0x5ea4  mov   x0, x20
0x5ea8  mov   x2, x21
0x5eac  bl    _objc_msgSend$shouldIgnore:        ; [RaygunNetworkMonitor shouldIgnore:request]
0x5eb0  mov   x20, x0                           ; x20 = shouldIgnore result
0x5eb4  mov   x0, x21
0x5eb8  bl    _objc_release                      ; release originalRequest
0x5ebc  tbnz  w20, #0x0, 0x5f44                 ; if shouldIgnore → jump to epilogue

; ---- record start time ----
0x5ec0  adrp  x8, 11
0x5ec4  ldr   x20, [x8, #0x8c8]                 ; x20 = NSNumber class
0x5ec8  bl    _CACurrentMediaTime               ; d0 = current time
0x5ecc  mov   x0, x20
0x5ed0  bl    _objc_msgSend$numberWithDouble:    ; [NSNumber numberWithDouble:time]
0x5ed4  mov   x29, x29
0x5ed8  bl    _objc_retainAutoreleasedReturnValue
0x5edc  mov   x20, x0                           ; x20 = start time NSNumber

; ---- get associated task ID ----
0x5ee0  adrp  x1, 2
0x5ee4  add   x1, x1, #0xf8                     ; x1 = "RaygunSessionTaskId" key
0x5ee8  mov   x0, x19                           ; x0 = task
0x5eec  bl    _objc_getAssociatedObject          ; taskId = associated object
0x5ef0  mov   x29, x29
0x5ef4  bl    _objc_retainAutoreleasedReturnValue
0x5ef8  mov   x21, x0                           ; x21 = taskId

; ---- nil check on taskId ----
0x5efc  cbz   x0, 0x5f34                        ; if taskId == nil → skip dictionary access

; ---- dictionary access (CRASH SITE) ----
0x5f00  adrp  x23, 11
0x5f04  ldr   x0, [x23, #0xab8]                 ; x0 = timers (NSMutableDictionary)
0x5f08  mov   x2, x21                           ; x2 = taskId (the key)
0x5f0c  bl    _objc_msgSend$objectForKey:        ; [timers objectForKey:taskId]
                                                 ; *** CRASH at return addr 0x5f10 ***
                                                 ; offset from function start: 0x5f10 - 0x5e6c = 0xa4 = 164 ✓
0x5f10  mov   x29, x29
0x5f14  bl    _objc_retainAutoreleasedReturnValue
0x5f18  mov   x22, x0                           ; x22 = existing value
0x5f1c  bl    _objc_release
0x5f20  cbnz  x22, 0x5f34                       ; if already tracked → skip store

; ---- store start time in timers dict ----
0x5f24  ldr   x0, [x23, #0xab8]                 ; x0 = timers
0x5f28  mov   x2, x20                           ; x2 = start time (value)
0x5f2c  mov   x3, x21                           ; x3 = taskId (key)
0x5f30  bl    _objc_msgSend$setObject:forKeyedSubscript:  ; timers[taskId] = start

; ---- cleanup ----
0x5f34  mov   x0, x21
0x5f38  bl    _objc_release                      ; release taskId
0x5f3c  mov   x0, x20
0x5f40  bl    _objc_release                      ; release start time

; ---- epilogue ----
0x5f44  mov   x0, x19
0x5f48  ldp   x29, x30, [sp, #0x30]
0x5f4c  ldp   x20, x19, [sp, #0x20]
0x5f50  ldp   x22, x21, [sp, #0x10]
0x5f54  ldp   x24, x23, [sp], #0x40
0x5f58  b     _objc_release                      ; release task
```

## Analysis

The crash offset +164 from `networkRequestStarted:` maps exactly to `0x5f10` — the return address after `bl _objc_msgSend$objectForKey:` at `0x5f0c`. This confirms the crash occurs inside `[timers objectForKey:taskId]`.

The nil guard at `0x5efc` (`cbz x0, 0x5f34`) correctly prevents calling `objectForKey:` with a nil key. Since the guard passes, the key is non-nil, yet the crash is `KERN_INVALID_ADDRESS at 0x0000000000000000` at +136 bytes deep inside `objectForKey:`. This indicates **corrupted internal dictionary state** — the dictionary's hash table contains null pointers due to concurrent modification from multiple threads.

### Corresponding source (from raygun4xamarin)

```objc
static NSMutableDictionary* timers;  // shared, no synchronization

+ (void)networkRequestStarted:(NSURLSessionTask *)task
{
    if ([RaygunNetworkMonitor shouldIgnore:task.originalRequest])
        return;

    NSNumber* start = @(CACurrentMediaTime());
    NSString* taskId = objc_getAssociatedObject(task, kSessionTaskIdKey);

    if (taskId != nil)
    {
        timers[taskId] = start;  // ← CRASH: concurrent access corrupts dictionary
    }
}
```

## Root Cause

`NSMutableDictionary` is not thread-safe. The `timers` dictionary is read/written from any thread via `_swizzle_resume` (any thread calling `[task resume]`), completion handlers, and `_swizzle_cancel`. Concurrent access corrupts the dictionary's internal hash table, causing `objectForKey:` to dereference a null pointer.

## Fix

Synchronize all access to `timers` using `@synchronized(timers)` or a serial dispatch queue.
