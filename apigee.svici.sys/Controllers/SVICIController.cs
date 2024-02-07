using api.svici.sys.Infrastructure.StoreService;
using api.svici.sys.Models;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.StoreDtos;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using api.svici.sys.Utilities.Helpers;
using app.api.Infrastructure.StoreService;
using KBZLogger.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace app.api.Controllers
{
    [Authorize]
    [Route("api/credit-mpu")]
    [ApiController]
    public class SVICIController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConnectionString _conStr;
        private readonly LogSetting _auditLog;
        private readonly ILogger<SVICIController> _logger;
        private string? scheme;
        private string? parameter;
        public SVICIController(IHttpClientFactory httpClientFactory, IOptions<ConnectionString> conStr,
            ILogger<SVICIController> log, IOptions<LogSetting> auditLog)
        {
            _logger = log;
            _conStr = conStr.Value;
            _auditLog = auditLog.Value;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("authorize/login", Name = "Login")]
        public async Task<IActionResult> Login(Authenticate data)
        {
            Request.Headers.TryGetValue("LOGID", out var logId);
            AuthFinalRespModel returnResponse = new();
            AuthCreditCardService authService = new();
            StoreResponseService respService = new();
            returnResponse.KBZRefNo = logId;
            string uid = string.Empty;

            _logger.LogInformation($"RefNo : {logId},  " +
                $"Credit_Card_SV_Auth Request data ==============> " +
                $"{JsonConvert.SerializeObject(data)}");

            try
            {
                ResponseServiceConfig getConfig = GetCongfigSetting("SV_Credit_TokenAuth", logId);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) || string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }

                string Authxml = authService.GetAuthDto(getConfig, data);

                if (string.IsNullOrEmpty(Authxml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(logId, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }

                _logger.LogInformation($"RefNo : {logId}  " +
                    $"XML Request ==============> {Authxml}");

                var httpContent = new StringContent(Authxml, Encoding.UTF8, "text/xml");

                var result = _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent).Result;
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {logId} " +
                    $" Credit_Card_SV_Auth Response ==============> {resp}");

                XDocument doc = XDocument.Parse(resp);
                if (result.IsSuccessStatusCode)
                {

                    var q = from node in doc.Descendants("uid") select node;
                    if (q.ToList().Count() != 0)
                        uid = q.FirstOrDefault()!.Value;
                    returnResponse.Data = new AuthRespModel() { uid = uid };

                    _logger.LogInformation($"RefNo : {logId}  " +
                        "Credit Auth Response ==============> " +
                        $" {JsonConvert.SerializeObject(returnResponse)}");

                    return Ok(returnResponse);
                }
                else
                {
                    returnResponse = respService.GetAuthErrorResp(doc, logId);

                    _logger.LogError($"RefNo : {logId}  " +
                        "Credit Auth Response ==============> " +
                        $" {JsonConvert.SerializeObject(returnResponse)}");
                    AuditThread(logId, returnResponse, null);
                    return BadRequest(returnResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {logId} " +
                    $"Credit Auth  Exception Message ============>   " +
                    $"{ex.Message}, InnerException : {ex.InnerException}");

                returnResponse = respService.GetAuthErrorResp(ex, logId);
                AuditThread(logId, returnResponse, ex);

                _logger.LogError($"RefNo : {logId} " +
                   $"Credit Auth Respons Response ============>  " +
                   $"{JsonConvert.SerializeObject(returnResponse)}");

                return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
            }

        }

        [HttpPost("storeApplication/btrt1", Name = "BTRT1")]
        public async Task<IActionResult> BTRT1(BTRT1_Dto data)
        {
            Request.Headers.TryGetValue("LOGID", out var logId);

            _logger.LogInformation($"RefNo : {logId},  " +
                $"BTRT01 Request data ==============>  " +
                $"{JsonConvert.SerializeObject(data)}");

            storeAppFinalRespModel respModel = new();
            StoreBTRT1Service service = new();
            StoreResponseService respService = new();
            string appId = string.Empty;

            try
            {
                respModel.KBZRefNo = logId;

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT01_Credit", logId);

                if (!getConfig.isCheck)
                {
                    respModel.Error = ErrorCodeModel.Unauthorized;
                    respModel.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, respModel);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) ||
                    string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    respModel.Error = ErrorCodeModel.ClientRespError;
                    respModel.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                string xml = service.GetBTRT1(getConfig, data);

                if (string.IsNullOrEmpty(xml))
                {
                    respModel.Error = ErrorCodeModel.FormatError;
                    respModel.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                _logger.LogInformation($"RefNo : {logId}  " +
                    $"BTRT01 XML Request ==========> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {logId} " +
                    $"BTRT01 Raw Response ============> {resp}");

                if (result.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(resp))
                    {
                        respModel = respService.GetErrorResp(logId);
                        AuditThread(logId, respModel, null);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT01 => {respModel}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }

                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;

                    if (q is not null && q.ToList().Count() != 0)
                    {
                        appId = q.FirstOrDefault()!.Value;
                        respModel.Data = new storeAppRespModel()
                        {
                            applicationId = appId
                        };

                        _logger.LogInformation($"RefNo : {logId} " +
                            $"BTRT01 application ID => {JsonConvert.SerializeObject(respModel)}");

                        return Ok(respModel);
                    }
                    else
                    {
                        respModel = respService.GetErrorResp(doc, logId);
                        AuditThread(logId, respModel, null);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT01 => {JsonConvert.SerializeObject(respModel)}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }
                }
                else
                {
                    XDocument doc = XDocument.Parse(resp);
                    respModel = respService.GetErrorResp(doc, logId);
                    AuditThread(logId, respModel, null);

                    _logger.LogError($"RefNo : {logId} " +
                    $"BTRT01 Response ============> {JsonConvert.SerializeObject(respModel)}");

                    return BadRequest(respModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {logId} " +
                    $"BTRT01 Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                respModel = respService.GetErrorResp(ex, logId);
                AuditThread(logId, respModel, ex);

                _logger.LogError($"RefNo : {logId} " +
                   $"BTRT01 Response ============> {JsonConvert.SerializeObject(respModel)}");

                return StatusCode(StatusCodes.Status500InternalServerError, respModel);
            }
        }

        [HttpPost("storeApplication/btrt2", Name = "BTRT2")]
        public async Task<IActionResult> BTRT2(BTRT2_Dto data)
        {
            Request.Headers.TryGetValue("LOGID", out var logId);

            _logger.LogInformation($"RefNo : {logId},  " +
                $"BTRT02 Request data ==============>  " +
                $"{JsonConvert.SerializeObject(data)}");

            storeAppFinalRespModel respModel = new();
            StoreBTRT2Service service = new();
            StoreResponseService respService = new();
            string appId = string.Empty;

            try
            {
                respModel.KBZRefNo = logId;

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT02_Credit", logId);

                if (!getConfig.isCheck)
                {
                    respModel.Error = ErrorCodeModel.Unauthorized;
                    respModel.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, respModel);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) ||
                    string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    respModel.Error = ErrorCodeModel.ClientRespError;
                    respModel.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                string xml = service.GetBTRT2(getConfig, data);

                if (string.IsNullOrEmpty(xml))
                {
                    respModel.Error = ErrorCodeModel.FormatError;
                    respModel.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                _logger.LogInformation($"RefNo : {logId}  " +
                    $"BTRT02 XML Request ==========> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {logId} " +
                    $"BTRT02 Raw Response ============> {resp}");

                if (result.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(resp))
                    {
                        respModel = respService.GetErrorResp(logId);
                        AuditThread(logId, respModel, null);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT02 => {respModel}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }

                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;

                    if (q is not null && q.ToList().Count() != 0)
                    {
                        appId = q.FirstOrDefault()!.Value;
                        respModel.Data = new storeAppRespModel()
                        {
                            applicationId = appId
                        };

                        _logger.LogInformation($"RefNo : {logId} " +
                            $"BTRT02 application ID => {JsonConvert.SerializeObject(respModel)}");

                        return Ok(respModel);
                    }
                    else
                    {
                        respModel = respService.GetErrorResp(doc, logId);
                        AuditThread(logId, respModel, null);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT02 => {JsonConvert.SerializeObject(respModel)}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }
                }
                else
                {
                    XDocument doc = XDocument.Parse(resp);
                    respModel = respService.GetErrorResp(doc, logId);
                    AuditThread(logId, respModel, null);

                    _logger.LogError($"RefNo : {logId} " +
                    $"BTRT02 Response ============> {JsonConvert.SerializeObject(respModel)}");

                    return BadRequest(respModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {logId} " +
                    $"BTRT02 Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                respModel = respService.GetErrorResp(ex, logId);
                AuditThread(logId, respModel, ex);

                _logger.LogError($"RefNo : {logId} " +
                   $"BTRT02 Response ============> {JsonConvert.SerializeObject(respModel)}");

                return StatusCode(StatusCodes.Status500InternalServerError, respModel);
            }
        }

        [HttpPost("storeApplication/btrt3", Name = "BTRT3")]
        public async Task<IActionResult> BTRT3(BTRT3_Dto data)
        {
            Request.Headers.TryGetValue("LOGID", out var logId);

            _logger.LogInformation($"RefNo : {logId},  " +
                $"BTRT03 Request data ==============>  " +
                $"{JsonConvert.SerializeObject(data)}");

            storeAppFinalRespModel respModel = new();
            StoreBTRT3Service service = new();
            StoreResponseService respService = new();
            string appId = string.Empty;

            try
            {
                respModel.KBZRefNo = logId;

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT03_Credit", logId);

                if (!getConfig.isCheck)
                {
                    respModel.Error = ErrorCodeModel.Unauthorized;
                    respModel.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, respModel);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) ||
                    string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    respModel.Error = ErrorCodeModel.ClientRespError;
                    respModel.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                string xml = service.GetBTRT3(getConfig, data);

                if (string.IsNullOrEmpty(xml))
                {
                    respModel.Error = ErrorCodeModel.FormatError;
                    respModel.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                _logger.LogInformation($"RefNo : {logId}  " +
                    $"BTRT03 XML Request ==========> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {logId} " +
                    $"BTRT03 Raw Response ============> {resp}");

                if (result.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(resp))
                    {
                        respModel = respService.GetErrorResp(logId);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT03 => {respModel}");
                        AuditThread(logId, respModel, null);
                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }

                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;

                    if (q is not null && q.ToList().Count() != 0)
                    {
                        appId = q.FirstOrDefault()!.Value;
                        respModel.Data = new storeAppRespModel()
                        {
                            applicationId = appId
                        };

                        _logger.LogInformation($"RefNo : {logId} " +
                            $"BTRT03 application ID => {JsonConvert.SerializeObject(respModel)}");

                        return Ok(respModel);
                    }
                    else
                    {
                        respModel = respService.GetErrorResp(doc, logId);
                        AuditThread(logId, respModel, null);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT03 => {JsonConvert.SerializeObject(respModel)}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }
                }
                else
                {
                    XDocument doc = XDocument.Parse(resp);
                    respModel = respService.GetErrorResp(doc, logId);
                    AuditThread(logId, respModel, null);

                    _logger.LogError($"RefNo : {logId} " +
                    $"BTRT03 Response ============> {JsonConvert.SerializeObject(respModel)}");

                    return BadRequest(respModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {logId} " +
                    $"BTRT03 Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                respModel = respService.GetErrorResp(ex, logId);
                AuditThread(logId, respModel, ex);

                _logger.LogError($"RefNo : {logId} " +
                   $"BTRT03 Response ============> {JsonConvert.SerializeObject(respModel)}");

                return StatusCode(StatusCodes.Status500InternalServerError, respModel);
            }
        }

        [HttpPost("storeApplication/btrt35", Name = "BTRT35")]
        public async Task<IActionResult> BTRT35(BTRT35_Dto data)
        {
            Request.Headers.TryGetValue("LOGID", out var logId);

            _logger.LogInformation($"RefNo : {logId},  " +
                $"BTRT35 Request data ==============>  " +
                $"{JsonConvert.SerializeObject(data)}");

            storeAppFinalRespModel respModel = new();
            StoreBTRT35Service service = new();
            StoreResponseService respService = new();
            string appId = string.Empty;

            try
            {
                respModel.KBZRefNo = logId;

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT35_Credit", logId);

                if (!getConfig.isCheck)
                {
                    respModel.Error = ErrorCodeModel.Unauthorized;
                    respModel.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, respModel);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) ||
                    string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    respModel.Error = ErrorCodeModel.ClientRespError;
                    respModel.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                string xml = service.GetBTRT35(getConfig, data);

                if (string.IsNullOrEmpty(xml))
                {
                    respModel.Error = ErrorCodeModel.FormatError;
                    respModel.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(logId, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                _logger.LogInformation($"RefNo : {logId}  " +
                    $"BTRT35 XML Request ==========> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {logId} " +
                    $"BTRT35 Raw Response ============> {resp}");

                if (result.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(resp))
                    {
                        respModel = respService.GetErrorResp(logId);
                        AuditThread(logId, respModel, null);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT35 => {respModel}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }

                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;

                    if (q is not null && q.ToList().Count() != 0)
                    {
                        appId = q.FirstOrDefault()!.Value;
                        respModel.Data = new storeAppRespModel()
                        {
                            applicationId = appId
                        };

                        _logger.LogInformation($"RefNo : {logId} " +
                            $"BTRT35 application ID => {JsonConvert.SerializeObject(respModel)}");

                        return Ok(respModel);
                    }
                    else
                    {
                        respModel = respService.GetErrorResp(doc, logId);
                        AuditThread(logId, respModel, null);

                        _logger.LogError($"RefNo : {logId} " +
                            $"Failed to post BTRT35 => {JsonConvert.SerializeObject(respModel)}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }
                }
                else
                {
                    XDocument doc = XDocument.Parse(resp);
                    respModel = respService.GetErrorResp(doc, logId);
                    AuditThread(logId, respModel, null);

                    _logger.LogError($"RefNo : {logId} " +
                    $"BTRT35 Response ============> {JsonConvert.SerializeObject(respModel)}");

                    return BadRequest(respModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {logId} " +
                    $"BTRT35 Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                respModel = respService.GetErrorResp(ex, logId);
                AuditThread(logId, respModel, ex);

                _logger.LogError($"RefNo : {logId} " +
                   $"BTRT35 Response ============> {JsonConvert.SerializeObject(respModel)}");

                return StatusCode(StatusCodes.Status500InternalServerError, respModel);
            }
        }

        [HttpGet("QueryApplication/{appid}", Name = "QueryApplication")]
        public async Task<IActionResult> QueryApplication(string appid)
        {
            Request.Headers.TryGetValue("LOGID", out var logId);
            AppQueryRespModel returndata = new();
            DataSet dataset = new();
            returndata.KBZRefNo = logId;

            _logger.LogInformation($"Svici CreditMPU QueryApplication Request ===> " +
                $"KBZRefNo : {logId} " +
                $"RequestPayload : {appid}");

            try
            {
                string sql = $"SELECT * FROM VISTA.VW_APPL_LST_DTL WHERE APP_ID ='{appid}'";
                await using var conn = new OracleConnection(_conStr.VISTA_Oracle);
                await conn.OpenAsync();

                OracleCommand cmd = new(sql, conn);
                OracleDataAdapter adapter = new(cmd);
                adapter.Fill(dataset);

                conn.Close();
                string data = JsonConvert.SerializeObject(dataset.Tables[0]);

                returndata.Data = JsonConvert.DeserializeObject<List<Dictionary<string,object>>>(data);
               
                _logger.LogInformation($"Svici CreditMPU QueryApplication Response ===> " +
                    $"KBZRefNo : {logId} " +
                    $"Response : {JsonConvert.SerializeObject(returndata)}");
                
                return Ok(returndata);
            }
            catch (Exception ex)
            {
                returndata.Error = new()
                {
                    Code = ErrorCodeModel.UnknownException.Code,
                    Message = ex.Message,
                };

                _logger.LogError($"Svici CreditMPU QueryApplication Response ===> " +
                   $"KBZRefNo : {logId} " +
                   $"Response : {JsonConvert.SerializeObject(returndata)}");

                return BadRequest(returndata);
            }

        }

        [NonAction]
        public ResponseServiceConfig GetCongfigSetting(string serviceName, string logId)
        {
            _logger.LogInformation($"RefNo: {logId}, Config Request ============> {serviceName}");
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
        public void AuditThread(string LOGID, object returnResponse, Exception? ex)
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