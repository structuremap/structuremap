using System;
using System.Reflection;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap.Emitting.Parameters
{
    public static class Methods
    {
        private static readonly Cache<Type, MethodInfo> _parseMethods = new Cache<Type, MethodInfo>(findParse);

        public static readonly MethodInfo GET_PROPERTY =
            ReflectionHelper.GetMethod<IConfiguredInstance>(i => i.GetProperty(null));

        public static MethodInfo CREATE_INSTANCE_ARRAY =
            ReflectionHelper.GetMethod<BuildSession>(i => i.CreateInstanceArray(null, null));

        public static MethodInfo ENUM_PARSE = typeof (Enum).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public,
                                                                      null,
                                                                      new[]
                                                                          {
                                                                              typeof (Type), typeof (string), typeof (bool)
                                                                          }, null);

        public static MethodInfo GET_CHILD =
            ReflectionHelper.GetMethod<IConfiguredInstance>(i => i.GetChild(null, null, null));

        public static MethodInfo GET_CHILDREN_ARRAY =
            ReflectionHelper.GetMethod<IConfiguredInstance>(i => i.GetChildrenArray(null));

        public static MethodInfo GET_TYPE_FROM_HANDLE = typeof (Type).GetMethod("GetTypeFromHandle");
        public static MethodInfo HAS_PROPERTY = ReflectionHelper.GetMethod<IConfiguredInstance>(i => i.HasProperty(null));

        private static MethodInfo findParse(Type t)
        {
            return t.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new[] {typeof (string)}, null);
        }

        public static MethodInfo ParseFor(Type type)
        {
            return _parseMethods.Retrieve(type);
        }
    }
}