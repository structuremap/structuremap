using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
            new [] {typeof (string)});

        private readonly Queue<string> _descriptions = new Queue<string>();
        private readonly string _title;

        public override string Message
        {
            get
            {
                var writer = new StringWriter();
                //writer.WriteLine("StructureMap Context from inner to outer:");

                writer.WriteLine(_title);

                var i = 0;
                _descriptions.Each(x => writer.WriteLine(++i + ".) " + x));

                return writer.ToString();
            }
        }

        public void Push(string description)
        {
            _descriptions.Enqueue(description);
        }

        public StructureMapException(string message) : base(message)
        {
            _title = message;
            Push(message);
        }

        public StructureMapException(string message, Exception innerException) : base(message, innerException)
        {
            _title = "Failure at: \"{0}\"".ToFormat(message);
            Push(message);
        }

        public string Title
        {
            get { return _title; }
        }

    }
}