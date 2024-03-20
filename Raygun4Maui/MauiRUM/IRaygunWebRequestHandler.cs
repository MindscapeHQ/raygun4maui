namespace Raygun4Maui.MauiRUM;

public interface IRaygunWebRequestHandler
{
    Task<bool> IsOnline();

    int Post(string payload);

    Task<int> PostAsync(string payload);
}