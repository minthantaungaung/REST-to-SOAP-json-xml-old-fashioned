using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api.svici.sys.Utilities.Data.Dtos.ResponseModels
{
    public class Property
    {
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
    public class BaseRespError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }

        public IList<BaseRespErrorDetail> Details { get; set; } = new List<BaseRespErrorDetail>();

        public BaseRespError()
        {
            Details = new List<BaseRespErrorDetail>();
        }
    }

    public class BaseRespErrorDetail
    {
        public string? ErrorCode { get; set; }
        public string? ErrorDescription { get; set; }
    }

    public class BaseRespModel : Property
    {
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string? KBZRefNo { get; set; }

        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public BaseRespError? Error { get; set; }
    }

    public class AuthFinalRespModel : BaseRespModel
    {
        public AuthRespModel? Data { get; set; }
    }
    public class AppQueryRespModel : BaseRespModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic? Data { get; set; }
    }
    public class AuthRespModel
    {
        public string? uid { get; set; }
    }

    #region Demo Param Request
    public class BaseReqModel
    {
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string? DemoParam1 { get; set; }

        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string? DemoParam2 { get; set; }
    }

    #endregion
    public class ErrorCodeModel
    {
        public static BaseRespError Timeout { get { return new BaseRespError { Code = "1002", Message = "Timeout error." }; } }
        public static BaseRespError UnknownException { get { return new BaseRespError { Code = "1004", Message = "Unknown error." }; } }
        public static BaseRespError Maintenance { get { return new BaseRespError { Code = "1005", Message = "Service is in Maintenance mode." }; } }
        public static BaseRespError ForcedUpdate { get { return new BaseRespError { Code = "1006", Message = "Latest version of the app is required to access the API." }; } }
        public static BaseRespError Unauthorized { get { return new BaseRespError { Code = "1000", Message = "Unauthorized." }; } }
        public static BaseRespError NotFoundError { get { return new BaseRespError { Code = "1009", Message = "Target System Not Found." }; } }
        public static BaseRespError ThirdPartyError { get { return new BaseRespError { Code = "1017", Message = "ThirdParty Error!" }; } }
        public static BaseRespError ClientRespError { get { return new BaseRespError { Code = "1016", Message = "ThirdParty Response Error!" }; } }

        public static BaseRespError LoginError { get { return new BaseRespError { Code = "1020", Message = "Application Unauthorized " }; } }
        public static BaseRespError DBError { get { return new BaseRespError { Code = "1015", Message = "Database error." }; } }
        public static BaseRespError NoRecordsFound { get { return new BaseRespError { Code = "1013", Message = "No Records Found!" }; } }
        public static BaseRespError NoRecordsAffect { get { return new BaseRespError { Code = "1012", Message = "No Rows Affected!" }; } }
        public static BaseRespError InvalidRequestPayload { get { return new BaseRespError { Code = "1001", Message = "Invalid Request Payload." }; } }
        public static BaseRespError ValidateError { get { return new BaseRespError { Code = "1001", Message = "Validation error." }; } }
        public static BaseRespError ApplicationUnauthorized { get { return new BaseRespError { Code = "1020", Message = "Application Unauthorized." }; } }
        public static BaseRespError FormatError { get { return new BaseRespError { Code = "FM-SYS", Message = "Formatting error." }; } }

        public static BaseRespErrorDetail Config_Call_Error { get { return new BaseRespErrorDetail { ErrorCode = "CB-CC-S001", ErrorDescription = "Config service call error." }; } }
        public static BaseRespErrorDetail UnauthorizedConfiguration { get { return new BaseRespErrorDetail { ErrorCode = "CB-CONF-S001", ErrorDescription = "Unauthorized Configuration." }; } }
        public static BaseRespErrorDetail TemplateFormatError { get { return new BaseRespErrorDetail { ErrorCode = "FM-TAGSYS", ErrorDescription = "Template Formatting Failed!" }; } }
        public static BaseRespErrorDetail EmptyRequestParameterError { get { return new BaseRespErrorDetail { ErrorCode = "EMP-TAGSYS", ErrorDescription = "The request parameter cannot be empty!" }; } }
    }
}
