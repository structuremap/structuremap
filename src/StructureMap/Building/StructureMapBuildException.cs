using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace StructureMap.Building
{
    [Serializable]
    public class StructureMapBuildException : Exception
    {
        public static Func<IContext, T> Wrap<T>(Func<IContext, T> inner, string description)
        {
            return context =>
            {
                try
                {
                    return inner(context);
                }
                catch (StructureMapBuildException e)
                {
                    e.push(description);
                    throw;
                }
                catch (Exception e)
                {
                    throw new StructureMapBuildException(description, e);
                }
            };
        }

        private readonly Queue<string> _descriptions = new Queue<string>();

        public StructureMapBuildException(string message, Exception innerException) : base(null, innerException)
        {
            _descriptions.Enqueue(message);
        }

        protected StructureMapBuildException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message
        {
            get
            {
                var writer = new StringWriter();
                writer.WriteLine();
                writer.WriteLine("StructureMap Context from inner to outer:");
                var i = 0;
                _descriptions.Each(x => writer.WriteLine(++i + ".) " + x));

                return writer.ToString();
            }
        }

        private void push(string description)
        {
            _descriptions.Enqueue(description);
        }
    }
}