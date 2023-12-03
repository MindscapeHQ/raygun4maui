using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Raygun4Maui.MauiRUM;

internal static class RaygunSerializer
{
    private static readonly StringEnumConverter StringEnumConverter = new StringEnumConverter();
    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
        Converters = new[] { StringEnumConverter }
    };

    public static string Serialize<T>(T @object)
    {
        return JsonConvert.SerializeObject(@object, Settings);
    }
}