using System;
using System.Resources;

namespace StructureMap
{
    internal static class ErrorMessages
    {
        [Obsolete("no one has ever liked this approach")]
        public static string GetMessage(int errorCode, params object[] args)
        {
            var msg = "StructureMap Exception Code:  " + errorCode + "\n";

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var type = arg as Type;
                if (type != null)
                {
                    args[i] = type.AssemblyQualifiedName;
                }
            }

            var errorMsg = getMessage(errorCode);
            if (errorMsg == null)
            {
                errorMsg = string.Empty;
            }

            return string.Format(errorMsg, args);
        }

        private static string getMessage(int errorCode)
        {
            var resources = new ResourceManager(typeof (StructureMapException));
            return resources.GetString(errorCode.ToString());
        }
    }
}