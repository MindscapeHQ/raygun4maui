using Mindscape.Raygun4Net;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public static class RaygunSessionTracker
  {
    private static readonly TimeSpan MaxSessionLength = TimeSpan.FromMinutes(30);

    private static RaygunIdentifierMessage _currentUser;
    private static DateTime _lastSeenTime;

    public static event Action<RaygunSessionEventArgs>        SessionStarted;
    public static event Action<RaygunSessionChangedEventArgs> SessionChanged;

    public static string SessionId { get; private set; }

    public static RaygunIdentifierMessage CurrentUser
    {
      get => _currentUser;
      set => SetUser(value);
    }

    public static void Init(RaygunIdentifierMessage user)
    {
      _currentUser = user;

      // Listen for events
      RaygunAppEventPublisher.AppInitialised += OnAppInitialised;
      RaygunAppEventPublisher.AppStarted += OnAppStarted;
      RaygunAppEventPublisher.AppResumed += OnAppResumed;
      RaygunAppEventPublisher.AppPaused += OnAppPaused;
      RaygunAppEventPublisher.AppStopped += OnAppStopped;
      RaygunAppEventPublisher.RaygunUserChanged += OnRaygunUserChanged;

      EvaluateSession();
    }

    public static void EnsureSessionStarted()
    {
      EvaluateSession();
    }

    public static void UpdateLastSeenTime()
    {
      _lastSeenTime = DateTime.UtcNow;
    }

    private static void SetUser(RaygunIdentifierMessage newUser)
    {
      // Going from an anonymous user to a known user does NOT warrant a change in session.
      // Instead the user associated with the current session will be updated.

      // Conditions for a change in session:
      //  A:  Anon user -> Known user = NO.
      //  B: Known user ->  Anon user = YES.
      //  C: Known user -> Different Known user = YES.

      // Condition B: User has been reset.
      bool conditionB = _currentUser.IsAnonymous == false && newUser.IsAnonymous;

      // Condition C: Changing between known users.
      bool conditionC = _currentUser.IsAnonymous == false && newUser.IsAnonymous == false
                                                          && string.Equals(_currentUser.Identifier, newUser.Identifier) == false;

      bool changedUser = conditionB || conditionC;

      if (changedUser)
      {
        RotateSession(_currentUser, newUser);
      }

      _currentUser = newUser;
    }

    /*
      App Lifecycle Event Callbacks
    */

    private static void OnAppInitialised(AppInitialised args)
    {
      EvaluateSession();
    }

    private static void OnAppStarted(AppStarted args)
    {
      EvaluateSession();
    }

    private static void OnAppResumed(AppResumed args)
    {
      EvaluateSession();
    }

    private static void OnAppPaused(AppPaused args)
    {
      UpdateLastSeenTime();
    }

    private static void OnAppStopped(AppStopped args)
    {
      UpdateLastSeenTime();
    }
    
    private static void OnRaygunUserChanged(RaygunUserChanged args)
    {
      CurrentUser = args.User;
    }

    /// <summary>
    /// Evaluates the lifecycle of the current session.
    /// If there is no session, one will be started.
    /// If the current session has had no interaction for the last 30 minutes, the session is stopped and a new one is started.
    /// </summary>
    private static void EvaluateSession()
    {
      if (string.IsNullOrEmpty(SessionId))
      {
        SessionId = GenerateNewSessionId();
        SessionStarted?.Invoke(new RaygunSessionEventArgs(SessionId, CurrentUser));
      }
      else if (ShouldRotateSession())
      {
        RotateSession(CurrentUser, CurrentUser);
      }

      UpdateLastSeenTime();
    }

    private static bool ShouldRotateSession()
    {
      return !string.IsNullOrEmpty(SessionId) &&
             _lastSeenTime != DateTime.MinValue &&
             DateTime.UtcNow - _lastSeenTime > MaxSessionLength;
    }

    private static void RotateSession(RaygunIdentifierMessage currentUser, RaygunIdentifierMessage newUser)
    {
      var newSessionId = GenerateNewSessionId();
      SessionChanged?.Invoke(new RaygunSessionChangedEventArgs(SessionId, newSessionId, currentUser, newUser));
      SessionId = newSessionId;
    }

    private static string GenerateNewSessionId()
    {
      return Guid.NewGuid().ToString();
    }
  }