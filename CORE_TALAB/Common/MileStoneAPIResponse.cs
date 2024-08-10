namespace CORE_TALAB.Common
{
    public class MileStoneAPIResponse
    {
        public int RetCode { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }

        public MileStoneAPIResponse(int retCode, object data, string messsage)
        {
            this.RetCode = retCode;
            this.Data = data;
            this.Message = messsage;
        }

        public MileStoneAPIResponse(int retCode, string messsage)
        {
            this.RetCode = retCode;
            this.Message = messsage;
        }

        public MileStoneAPIResponse(int retCode)
        {
            this.RetCode = retCode;
        }
    }
}
