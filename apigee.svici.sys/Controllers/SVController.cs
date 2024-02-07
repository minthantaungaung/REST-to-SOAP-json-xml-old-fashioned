using api.svici.sys.Controllers.Base;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using api.svici.sys.Utilities.Helpers;
using app.api.Infrastructure.Logger;
using app.api.Infrastructure.SVService;
using KBZLogger.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Superpower;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Security.Policy;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace app.api.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class SVController : BaseController
    {
        public SVController(IOptions<ConnectionString> conStr, IOptions<LogSetting> auditlog, ILogger<SVController> log,
            IHttpClientFactory httpClientFactory) : base(conStr, auditlog, log, httpClientFactory)
        {
        }

        /// <summary>
        /// Exact Code Flow Copy of SVIC V1 but with .net core
        /// Not currently using by any projects
        /// </summary>

        [HttpPost("authorize/tokenAuthorize", Name = "StoreAuthService")]
        public async Task<IActionResult> AuthController(SVWIModel data)
        {
            AuthFinalRespModel returnResponse = new();
            try
            {
                SVService service = new SVService();
                string uid = string.Empty;
                string url = string.Empty;

                HttpContext.Request.Headers.TryGetValue("LOGID", out var logId);
                LOGID = logId;

                if (string.IsNullOrEmpty(data.pwd) || string.IsNullOrEmpty(data.login))
                {
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details = new List<BaseRespErrorDetail>
                    {
                        new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Web Login credential is not valid from SV!" }
                    };
                    #region LOG
                    if (_auditLog.APILOG == "Y")
                    {
                        logs = new AuditLogModel
                        {
                            HttpCode = HttpStatusCode.BadRequest,
                            HttpVerb = HttpVerbs.POST,
                            SourceUrl = Request.GetEncodedUrl(),
                            PayLoadType = PayLoadType.RESPONSE,
                            LogLevel = KBZLogger.Core.LogLevel.ERROR,
                            KBZMessageID = logId,
                            PayLoad = JsonConvert.SerializeObject(returnResponse),
                            Message = $"Web Login credential is not valid from SV!"
                        };
                        logsAudit(logs);

                    }
                    if (_auditLog.TEXTLOG == "Y")
                    {
                        _logger.LogError("RefNo : " + logId + "Response JSON:" + JsonConvert.SerializeObject(returnResponse) + "\n Error: Web Login credential is not valid from SV!");
                    }
                    #endregion
                    return BadRequest(returnResponse);
                }

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_TokenAuthorize", logId);
                url = getConfig.Config.EndpointAddress;
                

                var body = JsonConvert.SerializeObject(JObject.Parse(getConfig.Config.RequestFormat)["XmlDocument"]);
                var xmlTemplate = JsonConvert.DeserializeObject(body);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }

                string xml = service.GetAuthDt(xmlTemplate, data);
                _logger.LogInformation($"RefNo: {logId} , StoreAuthService Request XML =======> {xml}");
                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                _logger.LogInformation($"RefNo: {logId} , Requesting... to SV with Soap Url =======> {url}");
                var result = await _httpClientFactory.CreateClient().PostAsync(url, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"RefNo: {logId} , Authorization Raw Response: {resp}");

                    _logger.LogInformation($"RefNo: {logId} , StoreAuthorizationthService Response =======> {JsonConvert.SerializeObject(result.Content.ReadAsStringAsync().Result)} ");
                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("uid") select node;
                    if (q.ToList().Count() != 0)
                        uid = q.FirstOrDefault()!.Value;

                    if (string.IsNullOrEmpty(uid))
                    {
                        _logger.LogError($"RefNo: {logId} , uid not found =======> {resp}");
                        returnResponse.Error = ErrorCodeModel.ClientRespError;
                        returnResponse.Error.Details = new List<BaseRespErrorDetail>
                        {
                            new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "The return data is not valid from SV!" }
                        };
                        #region LOG
                        if (_auditLog.APILOG == "Y")
                        {
                            logs = new AuditLogModel
                            {
                                HttpCode = HttpStatusCode.BadRequest,
                                HttpVerb = HttpVerbs.POST,
                                SourceUrl = Request.GetEncodedUrl(),
                                PayLoadType = PayLoadType.RESPONSE,
                                LogLevel = KBZLogger.Core.LogLevel.ERROR,
                                KBZMessageID = logId,
                                PayLoad = JsonConvert.SerializeObject(returnResponse),
                                Message = "The return data is not valid from SV!"
                            };
                            logsAudit(logs);

                        }
                        if (_auditLog.TEXTLOG == "Y")
                        {
                            _logger.LogError("KBZ Reference : " + logId + "Response JSON:" +
                                JsonConvert.SerializeObject(returnResponse) + "\n Error: The return data is not valid from SV!");
                        }
                        #endregion
                        return BadRequest(returnResponse);
                    }
                    else
                    {
                        returnResponse.Data = new AuthRespModel { uid = uid };
                        _logger.LogInformation($"RefNo: {logId} , ApiAuthorizationService Responsed uid =======> {uid} ");
                        return Ok(returnResponse);
                    }
                }
                else
                {
                    _logger.LogError($"RefNo: {logId} , Authorization Raw Response: {resp}");

                    returnResponse.KBZRefNo = logId;
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details = new List<BaseRespErrorDetail>
                    {
                        new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" }
                    };
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
                            KBZMessageID = logId,
                            PayLoad = JsonConvert.SerializeObject(returnResponse),
                            Message = "The return data is not valid from SV! ::"
                        };
                        logsAudit(logs);

                    }
                    if (_auditLog.TEXTLOG == "Y")
                    {
                        _logger.LogError("KBZ Reference : " + logId + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                            + "\n Error:" + "The return data is not valid from SV!");
                    }
                    _logger.LogError($"RefNo: {logId} , Authorization Error Response ========> {resp}");
                    #endregion
                    return BadRequest(returnResponse);
                }
            }
            catch (AggregateException ex)
            {
                var errorRsp = AggrateExCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
            catch (Exception ex)
            {
                var errorRsp = ExceptionCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
        }

        [HttpPost("storeApplication/BTRT01", Name = "BTRT01")]
        public async Task<IActionResult> StoreBTRTO1Application(storeAppBTRTModel data)
        {
            storeAppFinalRespModel returnResponse = new();
            try
            {
                SVService service = new SVService();
                string appId = string.Empty; string url = string.Empty;

                HttpContext.Request.Headers.TryGetValue("LOGID", out var logId);
                LOGID = logId;

                _logger.LogInformation($" RefNo: {logId} , StoreBTRTO1Application  Request Starting ====> {JsonConvert.SerializeObject(data)}");

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT01_MPU_Debit", logId);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }
                url = getConfig.Config.EndpointAddress;

                string xml = service.GetBTRT01(getConfig, data);
                if (string.IsNullOrEmpty(xml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }
                _logger.LogInformation($"RefNo: {logId} , StoreBTRTO1 Request XML =======> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                _logger.LogInformation($"RefNo: {logId} , Requesting... to SV with Soap Url =======> {url}");
                var result = await _httpClientFactory.CreateClient().PostAsync(url, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"RefNo: {logId} , StoreBTRTO1Application Response =======> {JsonConvert.SerializeObject(result.Content.ReadAsStringAsync().Result)} ");
                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;
                    if (q.ToList().Count() != 0)
                        appId = q.FirstOrDefault()!.Value;

                    if (string.IsNullOrEmpty(appId))
                    {
                        _logger.LogError($"RefNo: {logId} , AppID not found =======> {resp}");
                        returnResponse.Error = ErrorCodeModel.ClientRespError;
                        returnResponse.Error.Details = new List<BaseRespErrorDetail>
                        {
                            new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" }
                        };
                        return BadRequest(returnResponse);
                    }
                    else
                    {
                        _logger.LogInformation($"RefNo: {logId} , Responsed App ID =======> {appId} ");
                        returnResponse.Data.applicationId = appId;
                        return Ok(returnResponse);
                    }
                }
                else
                {
                    returnResponse.KBZRefNo = logId;
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details = new List<BaseRespErrorDetail>();
                    returnResponse.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" });
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
                            KBZMessageID = logId,
                            PayLoad = JsonConvert.SerializeObject(returnResponse),
                            Message = "The return data is not valid from SV! ::"
                        };
                        logsAudit(logs);

                    }
                    if (_auditLog.TEXTLOG == "Y")
                    {
                        _logger.LogError("KBZ Reference : " + logId + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                            + "\n Error:" + "The return data is not valid from SV!");
                    }
                    _logger.LogError($"StoreBTRTO1Application Error Response ========> RefNo: {logId} , {resp}");
                    #endregion
                    return BadRequest(returnResponse);
                }
            }
            catch (AggregateException ex)
            {
                var errorRsp = AggrateExCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
            catch (Exception ex)
            {
                var errorRsp = ExceptionCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
        }

        [HttpPost("storeApplication/BTRT02", Name = "BTRT02")]
        public async Task<IActionResult> StoreBTRTO2Application(storeAppBTRTModel data)
        {
            storeAppFinalRespModel returnResponse = new();
            try
            {
                SVService service = new SVService();
                string appId = string.Empty; string url = string.Empty;

                HttpContext.Request.Headers.TryGetValue("LOGID", out var logId);
                LOGID = logId;
                _logger.LogInformation($" RefNo: {logId} , StoreBTRTO2Application  Request Starting ====> {JsonConvert.SerializeObject(data)}");

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT02_MPU_Debit", logId);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }
                url = getConfig.Config.EndpointAddress;

                string xml = service.GetBTRT02(getConfig, data);
                if (string.IsNullOrEmpty(xml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }

                _logger.LogInformation($"RefNo: {logId} , StoreBTRTO2 Request XML =======> {xml}");
                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                _logger.LogInformation($"RefNo: {logId} , Requesting... to SV with Soap Url =======> {url}");
                var result = await _httpClientFactory.CreateClient().PostAsync(url, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"RefNo: {logId} , StoreBTRTO2Application Response =======> {resp} ");
                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;
                    if (q.ToList().Count() != 0)
                        appId = q.FirstOrDefault()!.Value;

                    if (string.IsNullOrEmpty(appId))
                    {
                        _logger.LogError($"RefNo: {logId} , AppID not found =======> {resp}");
                        returnResponse.Error = ErrorCodeModel.ClientRespError;
                        returnResponse.Error.Details = new List<BaseRespErrorDetail>
                        {
                            new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" }
                        };
                        return BadRequest(returnResponse);
                    }
                    else
                    {
                        _logger.LogInformation($"RefNo: {logId} , Responsed App ID =======> {appId} ");
                        returnResponse.Data.applicationId = appId;
                        return Ok(returnResponse);
                    }
                }
                else
                {
                    returnResponse.KBZRefNo = logId;
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details = new List<BaseRespErrorDetail>();
                    returnResponse.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" });
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
                            KBZMessageID = logId,
                            PayLoad = JsonConvert.SerializeObject(returnResponse),
                            Message = "The return data is not valid from SV! ::"
                        };
                        logsAudit(logs);

                    }
                    if (_auditLog.TEXTLOG == "Y")
                    {
                        _logger.LogError("KBZ Reference : " + logId + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                            + "\n Error:" + "The return data is not valid from SV!");
                    }
                    _logger.LogError($"StoreBTRTO2Application Error Response ========> RefNo: {logId} , {resp}");
                    #endregion
                    return BadRequest(returnResponse);
                }
            }
            catch (AggregateException ex)
            {
                var errorRsp = AggrateExCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
            catch (Exception ex)
            {
                var errorRsp = ExceptionCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
        }

        [HttpPost("processApplication/BTRT", Name = "ProcessApplication")]
        public async Task<IActionResult> ProcessApplication(processAppModel model)
        {
            storeAppFinalRespModel returnResponse = new();
            try
            {
                SVService service = new SVService();
                string appId = string.Empty; string url = string.Empty;

                HttpContext.Request.Headers.TryGetValue("LOGID", out var logId);
                LOGID = logId;
                _logger.LogInformation($" RefNo: {logId} , ProcessApplication  Request Starting ====> {JsonConvert.SerializeObject(model)}");
                ResponseServiceConfig getConfig = GetCongfigSetting("SV_ProcessApplication", logId);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }
                url = getConfig.Config.EndpointAddress;

                string xml = service.GetApplication(getConfig, model);
                if (string.IsNullOrEmpty(xml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }

                _logger.LogInformation($"RefNo: {logId} , ProcessApplication Request XML =======> {xml}");
                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                _logger.LogInformation($"RefNo: {logId} , Requesting... to SV with Soap Url =======> {url}");
                var result = await _httpClientFactory.CreateClient().PostAsync(url, httpContent);
                var resp = await result.Content.ReadAsStringAsync();
               
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"RefNo: {logId} , ProcessApplication Response =======> {resp} ");
                    XDocument doc = XDocument.Parse(resp);

                    var q = from node in doc.Descendants("applicationId") select node;

                    if (q.ToList().Count() != 0)
                        appId = q.FirstOrDefault()!.Value;

                    if (string.IsNullOrEmpty(appId))
                    {
                        _logger.LogError($"RefNo: {logId} , AppID not found =======> {resp}");
                        returnResponse.Error = ErrorCodeModel.ClientRespError;
                        returnResponse.Error.Details = new List<BaseRespErrorDetail>
                    {
                        new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" }
                    };
                        return BadRequest(returnResponse);
                    }
                    else
                    {
                        _logger.LogInformation($"RefNo: {logId} , Responsed App ID =======> {appId} ");
                        returnResponse.Data.applicationId = appId;
                        return Ok(returnResponse);
                    }
                }
                else
                {
                    returnResponse.KBZRefNo = logId;
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details = new List<BaseRespErrorDetail>();
                    returnResponse.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" });
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
                            KBZMessageID = logId,
                            PayLoad = JsonConvert.SerializeObject(returnResponse),
                            Message = "The return data is not valid from SV! ::"
                        };
                        logsAudit(logs);

                    }
                    if (_auditLog.TEXTLOG == "Y")
                    {
                        _logger.LogError("KBZ Reference : " + logId + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                            + "\n Error:" + "The return data is not valid from SV!");
                    }
                    _logger.LogError($"ProcessApplication Error Response ========> RefNo: {logId} , {resp}");
                    #endregion

                    return BadRequest(returnResponse);
                }
            }
            catch (AggregateException ex)
            {
                var errorRsp = AggrateExCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
            catch (Exception ex)
            {
                var errorRsp = ExceptionCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
        }

        [HttpPost("mobile/storeApp", Name = "Mobile_BTRT01")]
        public async Task<IActionResult> MobileStoreBTRT01(storeAppBTRTModel model)
        {
            storeAppFinalRespModel returnResponse = new();
            try
            {
                SVService service = new SVService();
                string appId = string.Empty;
                string url = string.Empty;

                HttpContext.Request.Headers.TryGetValue("LOGID", out var logId);
                LOGID = logId;
                _logger.LogInformation($" RefNo: {logId} , MobileStoreBTRTO1Application  Request Starting ====> {JsonConvert.SerializeObject(model)}");


                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT01_MPU_Debit", logId);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }
                url = getConfig.Config.EndpointAddress;

                string xml = service.GetBTRT01(getConfig, model);
                if (string.IsNullOrEmpty(xml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }
                _logger.LogInformation($"RefNo: {logId} , MobileStoreBTRTO1 Request XML =======> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                _logger.LogInformation($"RefNo: {logId} , Requesting... to SV with Soap Url =======> {url}");
                var result = await _httpClientFactory.CreateClient().PostAsync(url, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"RefNo: {logId} , MobileStoreBTRTO1Application Response =======> {JsonConvert.SerializeObject(result.Content.ReadAsStringAsync().Result)} ");
                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;
                    if (q.ToList().Count() != 0)
                        appId = q.FirstOrDefault()!.Value;

                    if (string.IsNullOrEmpty(appId))
                    {
                        _logger.LogError($"RefNo: {logId} , AppID not found =======> {resp}");
                        returnResponse.Error = ErrorCodeModel.ClientRespError;
                        returnResponse.Error.Details = new List<BaseRespErrorDetail>
                        {
                            new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" }
                        };
                        return BadRequest(returnResponse);
                    }
                    else
                    {
                        _logger.LogInformation($"RefNo: {logId} , Responsed App ID =======> {appId} ");
                        returnResponse.Data.applicationId = appId;
                        return Ok(returnResponse);
                    }
                }
                else
                {
                    returnResponse.KBZRefNo = logId;
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details = new List<BaseRespErrorDetail>();
                    returnResponse.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" });
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
                            KBZMessageID = logId,
                            PayLoad = JsonConvert.SerializeObject(returnResponse),
                            Message = "The return data is not valid from SV! ::"
                        };
                        logsAudit(logs);

                    }
                    if (_auditLog.TEXTLOG == "Y")
                    {
                        _logger.LogError("KBZ Reference : " + logId + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                            + "\n Error:" + "The return data is not valid from SV!");
                    }
                    _logger.LogError($"MobileStoreBTRTO1Application Error Response ========> RefNo: {logId} , {resp}");
                    #endregion

                    return BadRequest(returnResponse);
                }
            }
            catch (AggregateException ex)
            {
                var errorRsp = AggrateExCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
            catch (Exception ex)
            {
                var errorRsp = ExceptionCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
        }

        [HttpPost("updateApplication/BTRT30", Name = "BTRT30")]
        public async Task<IActionResult> UpdateApplicationBTRT30(UpdateAppBTRTModel model)
        {
            storeAppFinalRespModel returnResponse = new();
            try
            {
                SVService service = new SVService();
                string appId = string.Empty;
                string url = string.Empty;

                HttpContext.Request.Headers.TryGetValue("LOGID", out var logId);
                LOGID = logId;
                _logger.LogInformation($" RefNo: {logId} , StoreBTRT30Application  Request Starting ====> {JsonConvert.SerializeObject(model)}");

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT30_MPU_Debit", logId);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }
                url = getConfig.Config.EndpointAddress;

                string xml = service.GetBTRT30(getConfig, model);
                if (string.IsNullOrEmpty(xml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThreadV2(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }
                _logger.LogInformation($"RefNo: {logId} , StoreBTRT30Application Request XML =======> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                _logger.LogInformation($"RefNo: {logId} , Requesting... to SV with Soap Url =======> {url}");
                var result = await _httpClientFactory.CreateClient().PostAsync(url, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"RefNo: {logId} , StoreBTRT30Application Response =======> {JsonConvert.SerializeObject(result.Content.ReadAsStringAsync().Result)} ");
                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;
                    if (q.ToList().Count() != 0)
                        appId = q.FirstOrDefault()!.Value;

                    if (string.IsNullOrEmpty(appId))
                    {
                        _logger.LogError($"RefNo: {logId} , AppID not found =======> {resp}");
                        returnResponse.Error = ErrorCodeModel.ClientRespError;
                        returnResponse.Error.Details = new List<BaseRespErrorDetail>
                        {
                            new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" }
                        };
                        return BadRequest(returnResponse);
                    }
                    else
                    {
                        _logger.LogInformation($"RefNo: {logId} , Responsed App ID =======> {appId} ");
                        returnResponse.Data.applicationId = appId;
                        return Ok(returnResponse);
                    }
                }
                else
                {
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details = new List<BaseRespErrorDetail>();
                    returnResponse.Error.Details.Add(new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "Indicate unknown exception from SV !" });
                    returnResponse.KBZRefNo = logId;
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
                            KBZMessageID = logId,
                            PayLoad = JsonConvert.SerializeObject(returnResponse),
                            Message = "The return data is not valid from SV! ::"
                        };
                        logsAudit(logs);

                    }
                    if (_auditLog.TEXTLOG == "Y")
                    {
                        _logger.LogError("KBZ Reference : " + logId + "Response JSON:" + JsonConvert.SerializeObject(returnResponse)
                            + "\n Error:" + "The return data is not valid from SV!");
                    }
                    _logger.LogError($"StoreBTRT30Application Error Response ========> RefNo: {logId} , {resp}");
                    #endregion
                    return BadRequest(returnResponse);
                }
            }
            catch (AggregateException ex)
            {
                var errorRsp = AggrateExCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
            catch (Exception ex)
            {
                var errorRsp = ExceptionCall(returnResponse, ex);
                return StatusCode(errorRsp.Item1, errorRsp.Item2);
            }
        }

        [NonAction]
        public ResponseServiceConfig GetCongfigSetting(string serviceName, string logId)
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            scheme = authHeader.Scheme;
            parameter = authHeader.Parameter!;

            #region  prepare to get config value

            ConfigSetting config = new ConfigSetting();
            config.ServiceName = serviceName;
            config.ConfigUrl = _conStr.CONFIG_URL!;
            config.HttpClient = _httpClientFactory.CreateClient();
            config.Scheme = scheme;
            config.Parameter = parameter;
            config.KbzRefNo = logId;

            #endregion

            ResponseServiceConfig getConfig = ServiceConfig.InitConfig(config.ServiceName, config.ConfigUrl,
                config.HttpClient, config.Scheme, config.Parameter, config.KbzRefNo);

            _logger.LogInformation($"RefNo: {logId} , Config Response =======> {JsonConvert.SerializeObject(getConfig)}");

            return getConfig;
        }

        [NonAction]
        public void AuditThreadV2(string LOGID, object returnResponse, Exception? ex)
        {
            if (_auditLog.APILOG == "N") return;

            var identity = User.Identity as ClaimsIdentity;
            AuditLogModel logs = new()
            {
                AuditLogID = Guid.NewGuid().ToString(),
                CurrentUrl = Request.GetDisplayUrl(),
                LoggedBy = identity?.Claims?.SingleOrDefault(f => f.Type == "user_name")?.Value ?? "",
                LoggedDate = DateTime.Now,
                HttpCode = HttpStatusCode.BadRequest,
                HttpVerb = HttpVerbs.POST,
                SourceUrl = HttpContext.Request.GetEncodedUrl(),
                PayLoadType = PayLoadType.RESPONSE,
                LogLevel = KBZLogger.Core.LogLevel.ERROR,
                KBZMessageID = LOGID,
                PayLoad = JsonConvert.SerializeObject(returnResponse),
                Message = ex is null ? "" : ex.Message,
                Exception = ex is null ? "" : ex.StackTrace
            };

            using (var content = new StringContent(JsonConvert.SerializeObject(logs), Encoding.UTF8, "application/json"))
            {
                var result = _httpClientFactory.CreateClient().PostAsync(_conStr.LOGAPI + "AuditLog", content).Result;
            }
        }

    }
}
