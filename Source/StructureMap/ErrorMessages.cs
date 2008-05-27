using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace StructureMap
{
    internal static class ErrorMessages
    {
        public static string GetMessage(int errorCode, params object[] args)
        {
            string msg = "StructureMap Exception Code:  " + errorCode + "\n";

            for (int i = 0; i < args.Length; i++)
            {
                object arg = args[i];
                Type type = arg as Type;
                if (type != null)
                {
                    args[i] = type.AssemblyQualifiedName;
                }
            }

            string errorMsg = getMessage(errorCode);
            if (errorMsg == null)
            {
                errorMsg = string.Empty;
            }

            return string.Format(errorMsg, args);
        }

        private static string getMessage(int errorCode)
        {
            ResourceManager resources = new ResourceManager(typeof(StructureMapException));
            return resources.GetString(errorCode.ToString());
        }
    }
}
