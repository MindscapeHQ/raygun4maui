using System.Collections;
using Mindscape.Raygun4Net;

namespace Raygun4Maui;

/// <summary>
/// Resolves the message builders on first enumeration rather than when RaygunMauiClient is created
/// </summary>
internal sealed class LazyMessageBuilders : IEnumerable<IMessageBuilder>
{
    private readonly Lazy<IEnumerable<IMessageBuilder>> _builders;

    public LazyMessageBuilders(Func<IEnumerable<IMessageBuilder>> resolve)
    {
        _builders = new Lazy<IEnumerable<IMessageBuilder>>(resolve);
    }

    public IEnumerator<IMessageBuilder> GetEnumerator()
    {
        return _builders.Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
