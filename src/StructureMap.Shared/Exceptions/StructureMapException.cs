using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap
{
    /// <summary>
    /// Main exception for StructureMap.  Use the ErrorCode to aid in troubleshooting
    /// StructureMap problems
    /// </summary>
    public class StructureMapException : Exception
    {
        public static readonly ConstructorInfo Constructor =
            typeof (StructureMapException).GetConstructor(new[] {typeof (string), typeof (Exception)});

        public static readonly MethodInfo PushMethod = typeof (StructureMapException).GetMethod("Push",
            new[] {typeof (string), typeof (object[])});

        private readonly Queue<string> _descriptions = new Queue<string>();
        private readonly string _title;

        public override string Message
        {
            get
            {
                var writer = new StringWriter();

                writer.WriteLine(_title);

                if (Context.IsNotEmpty())
                {
                    writer.WriteLine();
                    writer.WriteLine(Context);
                    writer.WriteLine();
                }

                var i = 0;
                _descriptions.Each(x => writer.WriteLine(++i + ".) " + x));

                return writer.ToString();
            }
        }

        public string Context { get; set; }

        public void Push(string description, params object[] parameters)
        {
            _descriptions.Enqueue(description.ToFormat(parameters));
        }


        public StructureMapException(string message) : base(message)
        {
            _title = message;
        }

        public StructureMapException(string message, Exception innerException) : base(message, innerException)
        {
            _title = message;
        }


        public IList<Guid> Instances = new List<Guid>();

        public string Title
        {
            get { return _title; }
        }
    }
}
