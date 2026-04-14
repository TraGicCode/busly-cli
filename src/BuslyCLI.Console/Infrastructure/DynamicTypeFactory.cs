using System.Reflection;
using System.Reflection.Emit;

namespace BuslyCLI.Infrastructure;

public static class DynamicTypeFactory
{
    public static Type CreateFromString(string typeAsString)
    {
        var assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var type = moduleBuilder.DefineType(typeAsString,
            TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit |
            TypeAttributes.AutoLayout,
            null).GetTypeInfo().AsType();
        return type;
    }
}
