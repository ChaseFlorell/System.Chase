using System.Linq;
using System.Reflection;

namespace System.Chase.Internal
{
    internal static class ClassConstructionHelper
    {
        public static T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            paramTypes = paramTypes ?? new Type[0];
            var constructor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, 
                paramTypes, 
                null);

            if (!(constructor is null)) return (T) constructor.Invoke(paramValues);
            
            var errorMessage = paramTypes.Any() 
                ? $"{typeof(T).FullName} must have a non-public constructor `{typeof(T).Name}({string.Join(", ", paramTypes.Select(p => p.Name))})`" 
                : $"{typeof(T).FullName} must have an empty non-public constructor";
                
            throw new Exception(errorMessage);
        }
    }
}
