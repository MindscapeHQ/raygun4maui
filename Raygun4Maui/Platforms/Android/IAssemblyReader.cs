#nullable enable
using System.Reflection.PortableExecutable;

namespace Raygun4Maui;

internal interface IAssemblyReader : IDisposable
{
    /// <summary>
    /// Try to find the module, and create PEReader for the module if it exists
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns>A reader for the PE or null</returns>
    PEReader? TryGetReader(string moduleName);
}