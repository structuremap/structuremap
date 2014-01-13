using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;

namespace StructureMap
{
    /// <summary>
    /// Main exception for StructureMap.  Use the ErrorCode to aid in troubleshooting
    /// StructureMap problems
    /// </summary>
    [Serializable]
    public class StructureMapException : Exception
    {
        public static readonly ConstructorInfo Constructor =
            typeof(StructureMapException).GetConstructor(new Type[] { typeof(string), typeof(Exception) });

        public static readonly MethodInfo PushMethod = typeof(StructureMapException).GetMethod("Push", new Type[] { typeof(string) });

        private readonly Queue<string> _descriptions = new Queue<string>();
        private readonly string _title;
        private readonly int _errorCode;

        protected StructureMapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _errorCode = info.GetInt32("errorCode");
            var descriptions = info.GetValue("descriptions", typeof (string[])).As<string[]>();

            descriptions.Each(x => _descriptions.Enqueue(x));

            _title = info.GetString("message");
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("errorCode", _errorCode, typeof(int));
            info.AddValue("descriptions", _descriptions.ToArray(), typeof(string[]));
            info.AddValue("message", _title);
            base.GetObjectData(info, context);
        }

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
            _title = message;
            Push(message);
        }

        [Obsolete("Want to eliminate the error code ctor")]
        public StructureMapException(int ErrorCode, params object[] args)
        {
            _errorCode = ErrorCode;
            var msg = string.Format("StructureMap Exception Code:  {0}\n", _errorCode);
            msg += ErrorMessages.GetMessage(ErrorCode, args);

            _title = msg;
            Push(msg);
        }

        [Obsolete("Want to eliminate the error code ctor")]
        public StructureMapException(int ErrorCode, Exception InnerException, params object[] args)
            : base(string.Empty, InnerException)
        {
            _errorCode = ErrorCode;
            var msg = string.Format("StructureMap Exception Code:  {0}\n", _errorCode);
            msg += ErrorMessages.GetMessage(ErrorCode, args);
            _title = msg;
            Push(msg);
        }

        public string Title
        {
            get { return _title; }
        }

        public int ErrorCode
        {
            get { return _errorCode; }
        }


    }
}