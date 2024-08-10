
using CORE_TALAB.Enum.responsesEnums;
using System;

namespace CORE_TALAB.Common
{
    public class HttpFQResponse
    {
        public HttpFQResponse()
        {
            this.Result = (int)ERESULT_ENUM.RS_NOT_OK;
            this._dtResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public HttpFQResponse(ERESULT_ENUM result, DateTime dt)
        {
            this.Result = (int)result;
            this.DtResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            this.DtReceive = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public HttpFQResponse(ERESULT_ENUM result, object message, DateTime dt)
        {
            this.Result = (int)result;
            this.Message = message;
            this.DtResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            this.DtReceive = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public HttpFQResponse(object message, DateTime dt)
        {
            this.Result = (int)ERESULT_ENUM.RS_OK;
            this.Message = message;
            this.DtResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            this.DtReceive = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public HttpFQResponse(object message, object other, DateTime dt)
        {
            this.Result = (int)ERESULT_ENUM.RS_OK;
            if(message != null)
                this.Message = message;
            if(other != null)
                this.Other = other;
            this.DtResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            this.DtReceive = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public HttpFQResponse(ERESULT_ENUM result, object message, object other, DateTime dt)
        {
            this.Result = (int)result;
            this.Message = message;
            this.Other = other;
            this.DtResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            this.DtReceive = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        public HttpFQResponse(ERESULT_ENUM result, object message, object other, object other2, DateTime dt)
        {
            this.Result = (int)result;
            this.Message = message;
            this.Other = other;
            this.OtherEx = other2;
            this.DtResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            this.DtReceive = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        private int _result;
        public int Result
        {
            get {  return _result; }
            set { _result = value; }
        }

        private object _message = null;
        public object Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private object _other = null;
        public object Other
        {
            get { return _other; }
            set { _other = value; }
        }

        private object _otherEx = null;
        public object OtherEx
        {
            get { return _otherEx; }
            set { _otherEx = value; }
        }

        private object _otherEx2 = null;
        public object OtherEx2
        {
            get { return _otherEx2; }
            set { _otherEx2 = value; }
        }

        private string _dtReceive = null;
        public string DtReceive
        {
            get { return _dtReceive; }
            set { _dtReceive = value; }
        }

        private string _dtResponse = null;
        public string DtResponse
        {
            get { return _dtResponse; }
            set { _dtResponse = value; }
        }

        /// <summary>
        /// Make
        /// </summary>
        /// <param name="result"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static HttpFQResponse Make(ERESULT_ENUM result, DateTime dt)
        {
            return new HttpFQResponse(result, dt);
        }

        /// <summary>
        /// Make
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static HttpFQResponse Make(ERESULT_ENUM result, object message, DateTime dt)
        {
            return new HttpFQResponse(result, message, dt);
        }

        /// <summary>
        /// Make
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static HttpFQResponse Make(object message, DateTime dt)
        {
            return new HttpFQResponse(message, dt);
        }

        /// <summary>
        /// Make
        /// </summary>
        /// <param name="message"></param>
        /// <param name="other"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static HttpFQResponse Make(object message, object other, DateTime dt)
        {
            return new HttpFQResponse(message, other, dt);
        }

        /// <summary>
        /// Make
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="other"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static HttpFQResponse Make(ERESULT_ENUM result, object message, object other, DateTime dt)
        {
            return new HttpFQResponse(result, message, other, dt);
        }

        public static HttpFQResponse Make(ERESULT_ENUM result, object message, object other, object other2, DateTime dt)
        {
            return new HttpFQResponse(result, message, other, other2, dt);
        }
    }
}