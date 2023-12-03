using Mindscape.Raygun4Net;
using Raygun4Maui.AppEvents;
using Raygun4Maui.MauiRUM.EventTypes;

namespace Raygun4Maui.MauiRUM.EventTrackers;

public class RaygunSessionTracker
  {
    private readonly TimeSpan _maxSessionLength = TimeSpan.FromMinutes(30);

    private RaygunIdentifierMessage _currentUser;
    private DateTime _lastSeenTime;

    public event EventHandler<RaygunSessionEventArgs>        SessionStarted;
    public event EventHandler<RaygunSessionChangedEventArgs> SessionChanged;

    public string SessionId { get; private set; }

    public RaygunIdentifierMessage CurrentUser
    {
      get => _currentUser;
      set => SetUser(value);
    }

    public RaygunSessionTracker()
    {
    }

    public void Init(RaygunIdentifierMessage user)
    {
      _currentUser = user;

      // Listen for events
      RaygunAppEventPublisher.ListenFor(RaygunAppEventType.AppInitialised, OnAppInitialised);
      RaygunAppEventPublisher.ListenFor(RaygunAppEventType.AppStarted,     OnAppStarted);
      RaygunAppEventPublisher.ListenFor(RaygunAppEventType.AppResumed,     OnAppResumed);
      RaygunAppEventPublisher.ListenFor(RaygunAppEventType.AppPaused,      OnAppPaused);
      RaygunAppEventPublisher.ListenFor(RaygunAppEventType.AppStopped,     OnAppStopped);

      EvaluateSession();
    }

    public void EnsureSessionStarted()
    {
      EvaluateSession();
    }

    public void UpdateLastSeenTime()
    {
      _lastSeenTime = DateTime.UtcNow;
    }

    private void SetUser(RaygunIdentifierMessage newUser)
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
        // RaygunLogger.Debug("RUM detected change in user");
        RotateSession(_currentUser, newUser);
      }

      _currentUser = newUser;
    }

    /*
      App Lifecycle Event Callbacks
    */

    private void OnAppInitialised(IRaygunAppEventArgs args)
    {
      // RaygunLogger.Debug("RaygunSessionTracker - OnAppInitialised");
      EvaluateSession();
    }

    private void OnAppStarted(IRaygunAppEventArgs args)
    {
      // RaygunLogger.Debug("RaygunSessionTracker - OnAppStarted");
      EvaluateSession();
    }

    private void OnAppResumed(IRaygunAppEventArgs args)
    {
      // RaygunLogger.Debug("RaygunSessionTracker - OnAppResumed");
      EvaluateSession();
    }

    private void OnAppPaused(IRaygunAppEventArgs args)
    {
      // RaygunLogger.Debug("RaygunSessionTracker - OnAppPaused");
      UpdateLastSeenTime();
    }

    private void OnAppStopped(IRaygunAppEventArgs args)
    {
      // RaygunLogger.Debug("RaygunSessionTracker - OnAppStopped");
      UpdateLastSeenTime();
    }

    /// <summary>
    /// Evaluates the lifecycle of the current session.
    /// If there is no session, one will be started.
    /// If the current session has had no interaction for the last 30 minutes, the session is stopped and a new one is started.
    /// </summary>
    private void EvaluateSession()
    {
      if (string.IsNullOrEmpty(SessionId))
      {
        // RaygunLogger.Debug("RaygunSessionTracker - Starting new session");
        SessionId = GenerateNewSessionId();
        SessionStarted?.Invoke(this, new RaygunSessionEventArgs(SessionId, CurrentUser));
      }
      else if (ShouldRotateSession())
      {
        RotateSession(CurrentUser, CurrentUser);
      }

      UpdateLastSeenTime();
    }

    private bool ShouldRotateSession()
    {
      return !string.IsNullOrEmpty(SessionId) &&
             _lastSeenTime != DateTime.MinValue &&
             DateTime.UtcNow - _lastSeenTime > _maxSessionLength;
    }

    private void RotateSession(RaygunIdentifierMessage currentUser, RaygunIdentifierMessage newUser)
    {
      // RaygunLogger.Debug("RaygunSessionTracker - Rotating session");
      var newSessionId = GenerateNewSessionId();
      SessionChanged?.Invoke(this, new RaygunSessionChangedEventArgs(SessionId, newSessionId, currentUser, newUser));
      SessionId = newSessionId;
    }

    private static string GenerateNewSessionId()
    {
      return Guid.NewGuid().ToString();
    }
  }