
using Mindscape.Raygun4Net;

namespace Raygun4Maui.MauiRUM;

internal static class RaygunSerializer
{

    public static string Serialize<T>(T @object)
    {
        return SimpleJson.SerializeObject(@object);
    }
}