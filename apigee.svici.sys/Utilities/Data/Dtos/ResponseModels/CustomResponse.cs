using Newtonsoft.Json;

namespace api.svici.sys.Utilities.Data.Dtos.ResponseModels
{
    public class CommonResponse
    {
        [JsonProperty(Order = 0)]
        public string? KBZRefNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 2)]
        public ErrorResponse? Error { get; set; }
    }

    public class ErrorResponse
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public List<ErrorResponseDetails>? Details { get; set; }

        public static implicit operator string(ErrorResponse v)
        {
            throw new NotImplementedException();
        }
    }

    public class ErrorResponseDetails
    {
        public string? ErrorCode { get; set; }
        public string? ErrorDescription { get; set; }
    }

    public class MainResponse : CommonResponse
    {
        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public dynamic? Data { get; set; }
    }
    public class ResponseModel
    {
        public string KBZRefNo { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public dynamic Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Error Error { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public IList<ErrorDetail> Details { get; set; } = new List<ErrorDetail>();
    }

    public class ErrorDetail
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class CreateMultiJnrResponse : ResponseModel
    {
        public responseDataModel Data { get; set; }
    }
    public class responseDataModel
    {
        public string MSGID { get; set; }
        public string MSGSTAT { get; set; }
        public string BATCH_NUMBER { get; set; }
        public string REF_NUMBER { get; set; }
    }

}
