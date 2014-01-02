using System;
using System.Runtime.Serialization;
using System.Security;

namespace StructureMap
{
    /// <summary>
    /// Main exception for StructureMap.  Use the ErrorCode to aid in troubleshooting
    /// StructureMap problems
    /// </summary>
    [Serializable]
    public class StructureMapException : Exception
    {
        private readonly int _errorCode;
        private readonly string _msg;

        protected StructureMapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _errorCode = info.GetInt32("errorCode");
            _msg = info.GetString("msg");
        }


        public StructureMapException(int ErrorCode, params object[] args)
        {
            _errorCode = ErrorCode;
            _msg = string.Format("StructureMap Exception Code:  {0}\n", _errorCode);
            _msg += ErrorMessages.GetMessage(ErrorCode, args);
        }

        public StructureMapException(int ErrorCode, Exception InnerException, params object[] args)
            : base(string.Empty, InnerException)
        {
            _errorCode = ErrorCode;
            _msg = string.Format("StructureMap Exception Code:  {0}\n", _errorCode);
            _msg += ErrorMessages.GetMessage(ErrorCode, args);
        }

        public override string Message
        {
            get { return _msg; }
        }

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("errorCode", _errorCode, typeof (int));
            info.AddValue("msg", _msg, typeof (string));

            base.GetObjectData(info, context);
        }
    }
}