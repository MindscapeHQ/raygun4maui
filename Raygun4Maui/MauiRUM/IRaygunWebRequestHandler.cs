namespace Raygun4Maui.MauiRUM;

public interface IRaygunWebRequestHandler
{
    bool IsOnline();

    int Post(string payload);

    Task<int> PostAsync(string payload);
}