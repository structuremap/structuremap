using System;
using System.Resources;
using System.Runtime.Serialization;

namespace StructureMap
{
    /// <summary>
    /// Main exception for StructureMap.  Use the ErrorCode to aid in troubleshooting
    /// StructureMap problems
    /// </summary>
    [Serializable]
    public class StructureMapException : ApplicationException
    {
        private int _errorCode;
        private string _msg;


        protected StructureMapException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            _errorCode = info.GetInt32("errorCode");
            _msg = info.GetString("msg");
        }


        public StructureMapException(int ErrorCode, params object[] args) : base()
        {
            initialize(ErrorCode, args);
        }

        public StructureMapException(int ErrorCode, Exception InnerException, params object[] args)
            : base(string.Empty, InnerException)
        {
            initialize(ErrorCode, args);
        }

        private void initialize(int errorCode, params object[] args)
        {
            _errorCode = errorCode;
            _msg = "StructureMap Exception Code:  " + _errorCode + "\n";


            string errorMsg = getMessage(ErrorCode);
            if (errorMsg == null)
            {
                errorMsg = string.Empty;
            }

            _msg += string.Format(errorMsg, args);
        }

        private string getMessage(int errorCode)
        {
            ResourceManager resources = new ResourceManager(GetType());

            return resources.GetString(errorCode.ToString());
        }

        public override string Message
        {
            get { return _msg; }
        }

        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("errorCode", _errorCode, typeof (int));
            info.AddValue("msg", _msg, typeof (string));

            base.GetObjectData(info, context);
        }
    }
}