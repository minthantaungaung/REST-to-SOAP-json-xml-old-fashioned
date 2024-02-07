using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using app.api.Controllers;
using KBZLogger.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Xml;

namespace api.svici.sys.Controllers.Base
{
    public class BaseController : ControllerBase
    {
        public readonly ConnectionString _conStr;
        public readonly LogSetting _auditLog;
        public readonly ILogger<SVController> _logger;
        public string LOGID = null;
        public AuditLogModel logs = null;
        public readonly IHttpClientFactory _httpClientFactory;
        internal string scheme;
        internal string parameter;
        public BaseController(IOptions<ConnectionString> conStr, IOptions<LogSetting> auditlog, ILogger<SVController> log,
            IHttpClientFactory httpClientFactory)
        {
            _conStr = conStr.Value;
            _auditLog = auditlog.Value;
            _logger = log;
            _httpClientFactory = httpClientFactory;
        }

        #region Extension Methods
        [NonAction]
        public string logsAudit(AuditLogModel logs)
        {
            //   var principal = Request.GetRequestContext().Principal as System.Security.Claims.ClaimsPrincipal;

            logs.AuditLogID = Guid.NewGuid().ToString();
            logs.CurrentUrl = Request.GetDisplayUrl();

            if (string.IsNullOrEmpty(logs.KBZMessageID)) logs.KBZMessageID = Guid.NewGuid().ToString();

            logs.LoggedBy = GetClaimUsername();
            logs.LoggedDate = DateTime.Now;

            if (logs.PayLoad != null)
            {
                logs.PayLoad = xmltojson(logs.PayLoad.ToString());
            }

            AuditThread(logs);

            return logs.KBZMessageID;
        }
        [NonAction]
        public void AuditThread(AuditLogModel logs)
        {
            using (var content = new StringContent(JsonConvert.SerializeObject(logs), Encoding.UTF8, "application/json"))
            {
                var result = _httpClientFactory.CreateClient().PostAsync(_conStr.LOGAPI + "AuditLog", content).Result;
            }
        }
        [NonAction]
        public JObject xmltojson(string xml)
        {
            try
            {
                if (!IsValidJson(xml))
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);

                    var jsonText = JsonConvert.SerializeXmlNode(doc);

                    return JObject.Parse(jsonText);
                }
                return JObject.Parse(xml);
            }
            catch
            {
                return null;
            }
        }
        [NonAction]
        public static bool IsValidJson(string stringValue)
        {
            bool returnValue = false;
            string value = null;

            if (!string.IsNullOrEmpty(stringValue))
                value = stringValue.Trim();

            if (!string.IsNullOrWhiteSpace(value) &&
                (value.StartsWith("{") && value.EndsWith("}") || //For object
                 value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    value = stringValue.Trim();

                    var obj = JToken.Parse(value);

                    returnValue = true;
                }
                catch
                {
                    returnValue = false;
                }
            }
            else
            {
                returnValue = false;
            }

            return returnValue;
        }
        [NonAction]
        private string GetClaimUsername()
        {
            //string usernameKey = "client_id";
            string username = string.Empty;

            string usernameKey = "user_name";
            var identity = User.Identity as ClaimsIdentity;
            username = identity.Claims.SingleOrDefault(f => f.Type == usernameKey).Value;

            return username;
        }

        [NonAction]
        public (int, BaseRespModel) ExceptionCall<T>(T returnResponse, Exception ex)
        {

            if (Convert.ToString(ex.Message).Trim().Contains("Incorrect Login and/or Password"))
            {
                BaseRespModel model = (BaseRespModel)(object)returnResponse;
                model.Error = ErrorCodeModel.LoginError;
                model.Error.Details = new List<BaseRespErrorDetail>();
                model.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "SVICI_REF", ErrorDescription = "Incorrect / Invalid  User and Password (Application Login) !" });
                model.KBZRefNo = LOGID;

                #region LOG
                if (_auditLog.APILOG == "Y")
                {
                    logs = new AuditLogModel
                    {
                        HttpCode = HttpStatusCode.BadRequest,
                        HttpVerb = HttpVerbs.POST,
                        SourceUrl = HttpContext.Request.GetEncodedUrl(),
                        PayLoadType = PayLoadType.RESPONSE,
                        LogLevel = KBZLogger.Core.LogLevel.ERROR,
                        KBZMessageID = LOGID,
                        PayLoad = JsonConvert.SerializeObject(returnResponse),
                        Message = ex.Message,
                        Exception = ex.StackTrace
                    };
                    logsAudit(logs);

                }
                if (_auditLog.TEXTLOG == "Y")
                {
                    _logger.LogError($"KBZ Reference : {LOGID} Response JSON:" + JsonConvert.SerializeObject(returnResponse) + "\n Error:" + ex.Message + "::: LogMessage ::::" + ", " + ex.StackTrace);
                }
                #endregion
                return (StatusCodes.Status400BadRequest, model);
            }

            // Wrong user token is used or Token is expired
            if (Convert.ToString(ex.Message).Trim().Contains("Invalid UserID or User is not authorized"))
            {
                BaseRespModel model = (BaseRespModel)(object)returnResponse;
                model.Error = ErrorCodeModel.LoginError;
                model.Error.Details = new List<BaseRespErrorDetail>();
                model.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "SVICI_REF", ErrorDescription = "Invalid UserID or User is not authorized!" });
                model.KBZRefNo = LOGID;
                #region LOG
                if (_auditLog.APILOG == "Y")
                {
                    logs = new AuditLogModel
                    {
                        HttpCode = HttpStatusCode.BadRequest,
                        HttpVerb = HttpVerbs.POST,
                        SourceUrl = HttpContext.Request.GetEncodedUrl(),
                        PayLoadType = PayLoadType.RESPONSE,
                        LogLevel = KBZLogger.Core.LogLevel.ERROR,
                        KBZMessageID = LOGID,
                        PayLoad = JsonConvert.SerializeObject(returnResponse),
                        Message = ex.Message + "",
                        Exception = ex.StackTrace
                    };

                    logsAudit(logs);

                }
                if (_auditLog.TEXTLOG == "Y")
                {
                    _logger.LogError("KBZ Reference : " + LOGID + "Response JSON:" + JsonConvert.SerializeObject(returnResponse) + "\n Error:" + ex.Message + "::: LogMessage ::::" + "" + ", " + ex.StackTrace);
                }
                #endregion
                return (StatusCodes.Status400BadRequest, model);
            }

            if (Convert.ToString(ex.Message).Trim().Contains("Application not processed"))
            {
                BaseRespModel model = (BaseRespModel)(object)returnResponse;
                model.Error = ErrorCodeModel.ClientRespError;
                model.Error.Details = new List<BaseRespErrorDetail>();
                model.Error.Details.Add(new BaseRespErrorDetail()
                {
                    ErrorCode = "SVICI_REF",
                    ErrorDescription = ex.Message
                });
                model.KBZRefNo = LOGID;
                #region LOG
                if (_auditLog.APILOG == "Y")
                {
                    logs = new AuditLogModel
                    {
                        HttpCode = HttpStatusCode.BadRequest,
                        HttpVerb = HttpVerbs.POST,
                        SourceUrl = HttpContext.Request.GetEncodedUrl(),
                        PayLoadType = PayLoadType.RESPONSE,
                        LogLevel = KBZLogger.Core.LogLevel.ERROR,
                        KBZMessageID = LOGID,
                        PayLoad = JsonConvert.SerializeObject(returnResponse),
                        Message = ex.Message,
                        Exception = ex.StackTrace
                    };
                    logsAudit(logs);
                }
                if (_auditLog.TEXTLOG == "Y")
                {
                    _logger.LogError("KBZ Reference : " + LOGID + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                        + "\n Error:" + ex.Message + "::: LogMessage ::::" + "" + ", " + ex.StackTrace);
                }
                #endregion
                return (StatusCodes.Status400BadRequest, model);
            }
            //HttpResponseMessage finalResponse = null;
            if (Convert.ToString(ex.Message).Trim().ToLower().Contains("internal server error"))
            {
                BaseRespModel model = (BaseRespModel)(object)returnResponse;
                model.Error = ErrorCodeModel.ThirdPartyError;
                model.Error.Details = new List<BaseRespErrorDetail>();
                model.Error.Details.Add(new BaseRespErrorDetail()
                {
                    ErrorCode = "SVCW_REF",
                    ErrorDescription = "Internal Server Error on ThirdParty System !"
                });
                model.KBZRefNo = LOGID;
                #region LOG
                if (_auditLog.APILOG == "Y")
                {
                    logs = new AuditLogModel
                    {
                        HttpCode = HttpStatusCode.InternalServerError,
                        HttpVerb = HttpVerbs.POST,
                        SourceUrl = HttpContext.Request.GetEncodedUrl(),
                        PayLoadType = PayLoadType.RESPONSE,
                        LogLevel = KBZLogger.Core.LogLevel.ERROR,
                        KBZMessageID = LOGID,
                        PayLoad = JsonConvert.SerializeObject(returnResponse),
                        Message = ex.Message,
                        Exception = ex.StackTrace
                    };
                    logsAudit(logs);

                }
                if (_auditLog.TEXTLOG == "Y")
                {
                    _logger.LogError("KBZ Reference : " + LOGID + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                        + "\n Error:" + ex.Message + "::: LogMessage ::::" + "" + ", " + ex.StackTrace);
                }
                #endregion
                return (StatusCodes.Status500InternalServerError, model);
            }

            if (Convert.ToString(ex.Message).Trim().ToLower().Contains("timed out") ||
                 Convert.ToString(ex.Message).Trim().ToLower().Contains("timeout"))
            {
                BaseRespModel model = (BaseRespModel)(object)returnResponse;
                model.Error = ErrorCodeModel.Timeout;
                model.Error.Details = new List<BaseRespErrorDetail>();
                model.Error.Details.Add(new BaseRespErrorDetail()
                {
                    ErrorCode = "TAGSYS",
                    ErrorDescription = ex.Message
                });
                model.KBZRefNo = LOGID;
                #region LOG
                if (_auditLog.APILOG == "Y")
                {
                    logs = new AuditLogModel
                    {
                        HttpCode = HttpStatusCode.InternalServerError,
                        HttpVerb = HttpVerbs.POST,
                        SourceUrl = HttpContext.Request.GetEncodedUrl(),
                        PayLoadType = PayLoadType.RESPONSE,
                        LogLevel = KBZLogger.Core.LogLevel.ERROR,
                        KBZMessageID = LOGID,
                        PayLoad = JsonConvert.SerializeObject(returnResponse),
                        Message = ex.Message,
                        Exception = ex.StackTrace
                    };
                    logsAudit(logs);

                }
                if (_auditLog.TEXTLOG == "Y")
                {
                    _logger.LogError("KBZ Reference : " + LOGID + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                        + "\n Error:" + ex.Message + "::: LogMessage ::::" + "" + ", " + ex.StackTrace);
                }
                #endregion
                return (StatusCodes.Status500InternalServerError, model);
            }

            if (Convert.ToString(ex.Message).Trim().ToLower().Contains("not found") ||
             Convert.ToString(ex.Message).Trim().ToLower().Contains("404") ||
             Convert.ToString(ex.Message).Trim().ToLower().Contains("no data"))
            {
                BaseRespModel model = (BaseRespModel)(object)returnResponse;
                model.Error = ErrorCodeModel.NotFoundError;
                model.Error.Details = new List<BaseRespErrorDetail>();
                model.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = ex.Message });
                model.KBZRefNo = LOGID;
                #region LOG
                if (_auditLog.APILOG == "Y")
                {
                    logs = new AuditLogModel
                    {
                        HttpCode = HttpStatusCode.InternalServerError,
                        HttpVerb = HttpVerbs.POST,
                        SourceUrl = HttpContext.Request.GetEncodedUrl(),
                        PayLoadType = PayLoadType.RESPONSE,
                        LogLevel = KBZLogger.Core.LogLevel.ERROR,
                        KBZMessageID = LOGID,
                        PayLoad = JsonConvert.SerializeObject(returnResponse),
                        Message = ex.Message,
                        Exception = ex.StackTrace
                    };
                    logsAudit(logs);

                }
                if (_auditLog.TEXTLOG == "Y")
                {
                    _logger.LogError("KBZ Reference : " + LOGID + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                        + "\n Error:" + ex.Message + "::: LogMessage ::::" + "" + ", " + ex.StackTrace);
                }
                #endregion
                return (StatusCodes.Status500InternalServerError, model);
            }
            //TODO: Underlying connection close
            else
            {
                BaseRespModel model = (BaseRespModel)(object)returnResponse;
                model.Error = ErrorCodeModel.UnknownException;
                model.Error.Details = new List<BaseRespErrorDetail>
                {
                    new BaseRespErrorDetail() { ErrorCode = "1004", ErrorDescription = ex.Message }
                };
                #region LOG
                if (_auditLog.APILOG == "Y")
                {
                    logs = new AuditLogModel
                    {
                        HttpCode = HttpStatusCode.InternalServerError,
                        HttpVerb = HttpVerbs.POST,
                        SourceUrl = HttpContext.Request.GetEncodedUrl(),
                        PayLoadType = PayLoadType.RESPONSE,
                        LogLevel = KBZLogger.Core.LogLevel.ERROR,
                        KBZMessageID = LOGID,
                        PayLoad = JsonConvert.SerializeObject(returnResponse),
                        Message = ex.Message,
                        Exception = ex.StackTrace
                    };
                    logsAudit(logs);

                }
                if (_auditLog.TEXTLOG == "Y")
                {
                    _logger.LogError("KBZ Reference : " + LOGID + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                        + "\n Error:" + ex.Message + "::: LogMessage ::::" + "" + ", " + ex.StackTrace);
                }
                #endregion
                return (StatusCodes.Status500InternalServerError, model);
            }
        }

        [NonAction]
        public (int, BaseRespModel) AggrateExCall<T>(T returnResponse, AggregateException ex)
        {
            BaseRespModel model = (BaseRespModel)(object)returnResponse;
            string ErrorMessage = "";
            foreach (var exp in ex.InnerExceptions)
                ErrorMessage += exp.ToString();
            #region LOG
            if (_auditLog.APILOG == "Y")
            {
                logs = new AuditLogModel
                {
                    HttpCode = HttpStatusCode.InternalServerError,
                    HttpVerb = HttpVerbs.POST,
                    SourceUrl = HttpContext.Request.GetEncodedUrl(),
                    PayLoadType = PayLoadType.RESPONSE,
                    LogLevel = KBZLogger.Core.LogLevel.ERROR,
                    KBZMessageID = LOGID,
                    PayLoad = JsonConvert.SerializeObject(returnResponse),
                    Message = ex.Message,
                    Exception = ErrorMessage
                };
                logsAudit(logs);

            }
            if (_auditLog.TEXTLOG == "Y")
            {
                _logger.LogError("KBZ Reference : " + LOGID + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                    + "\n Error:" + ex.Message + ", " + ErrorMessage);

            }
            #endregion
            model.Error = ErrorCodeModel.UnknownException;
            model.Error.Details = new List<BaseRespErrorDetail>();
            model.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "SYS", ErrorDescription = ErrorMessage });
            model.KBZRefNo = LOGID;
            return (StatusCodes.Status500InternalServerError, model);
        }
        #endregion
    }
}
