using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.ReflectionHelpers
{
    public static class ReflectionExtencions
    {
        public static IEnumerable<Type> BaseTypes(this Type type)
        {
            Type t = type;
            while (true)
            {
                t = t.BaseType;
                if (t == null) break;
                yield return t;
            }
        }

        static bool AnyBaseType(this Type type, Func<Type, bool> predicate) =>
  type.BaseTypes().Any(predicate);

        static bool IsParticularGeneric(this Type type, Type generic) =>
  type.IsGenericType && type.GetGenericTypeDefinition() == generic;

        static bool IsType(Type type, object ObjectType) =>
  type.IsParticularGeneric(ObjectType.GetType());

        //public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        //{
        //    return
        //      assembly.GetTypes()
        //              .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
        //              .ToArray();
        //}

        public static IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            var types = assembly.GetTypes();
         //   types[2].Namespace
            return types.Where(t => t.Namespace != null).Where(t => t.Namespace.Contains(nameSpace));
        }
    }
}
