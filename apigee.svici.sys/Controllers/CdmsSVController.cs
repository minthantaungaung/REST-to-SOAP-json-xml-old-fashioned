using api.svici.sys.Infrastructure.StoreService;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.StoreDtos;
using api.svici.sys.Utilities.Helpers;
using app.api.Controllers;
using KBZLogger.Core;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using Microsoft.AspNetCore.Authorization;
using api.svici.sys.Infrastructure.CdmsSvService.RequestService;
using api.svici.sys.Infrastructure.CdmsSvService.ResponseService;
using api.svici.sys.Infrastructure.CdmsSvService.DbService;
using System.Security.Policy;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CustomerEnquiry;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CardStatusList;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard;
using System.Xml;
using System.Dynamic;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using api.svici.sys.Infrastructure.SV_FE_Services;

namespace api.svici.sys.Controllers
{
    [Authorize]
    [Route("api/cdms/")]
    [ApiController]
    public class CdmsSVController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConnectionString _conStr;
        private readonly WsseCredentials _wsse;
        private readonly LogSetting _auditLog;
        private readonly ILogger<SVICIController> _logger;
        private readonly ICdmsSvService _service;
        private readonly ICdmsSvResponseService _respService;
        private readonly ICdmsSvDbService _dbService;
        private string? scheme;
        private string? parameter;
        public CdmsSVController(IHttpClientFactory httpClientFactory, IOptions<ConnectionString> conStr, IOptions<WsseCredentials> wsse, ILogger<SVICIController> log, IOptions<LogSetting> auditLog,
            ICdmsSvService service, ICdmsSvResponseService responseService, ICdmsSvDbService dbService)
        {
            _logger = log;
            _conStr = conStr.Value;
            _auditLog = auditLog.Value;
            _wsse = wsse.Value;
            _service = service;
            _dbService = dbService;
            _respService = responseService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("authorize/login", Name = "GenerateUID")]
        public async Task<IActionResult> GenerateUID(Authenticate data)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            AuthFinalRespModel returnResponse = new();


            returnResponse.KBZRefNo = kbzRefNo;
            string uid = string.Empty;

            _logger.LogInformation($"RefNo : {kbzRefNo},  " +
                $"Credit_Card_SV_Auth Request data ==============> " +
                $"{JsonConvert.SerializeObject(data)}");

            try
            {
                ResponseServiceConfig getConfig = GetCongfigSetting("SV_TokenAuthorize", kbzRefNo);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(kbzRefNo, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) || string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    returnResponse.Error = ErrorCodeModel.ClientRespError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(kbzRefNo, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }

                string Authxml = _service.GetAuthDto(getConfig, data);

                if (string.IsNullOrEmpty(Authxml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(kbzRefNo, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }

                _logger.LogInformation($"RefNo : {kbzRefNo}  " +
                    $"XML Request ==============> {Authxml}");

                var httpContent = new StringContent(Authxml, Encoding.UTF8, "text/xml");

                var result = _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent).Result;
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {kbzRefNo} " +
                    $" Credit_Card_SV_Auth Response ==============> {resp}");

                XDocument doc = XDocument.Parse(resp);
                if (result.IsSuccessStatusCode)
                {

                    var q = from node in doc.Descendants("uid") select node;
                    if (q.ToList().Count() != 0)
                        uid = q.FirstOrDefault()!.Value;
                    returnResponse.Data = new AuthRespModel() { uid = uid };

                    _logger.LogInformation($"RefNo : {kbzRefNo}  " +
                        "Credit Auth Response ==============> " +
                        $" {JsonConvert.SerializeObject(returnResponse)}");

                    return Ok(returnResponse);
                }
                else
                {
                    returnResponse = _respService.GetAuthErrorResp(doc, kbzRefNo);

                    _logger.LogError($"RefNo : {kbzRefNo}  " +
                        "Credit Auth Response ==============> " +
                        $" {JsonConvert.SerializeObject(returnResponse)}");
                    AuditThread(kbzRefNo, returnResponse, null);
                    return BadRequest(returnResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {kbzRefNo} " +
                    $"Credit Auth  Exception Message ============>   " +
                    $"{ex.Message}, InnerException : {ex.InnerException}");

                returnResponse = _respService.GetAuthErrorResp(ex, kbzRefNo);
                AuditThread(kbzRefNo, returnResponse, ex);

                _logger.LogError($"RefNo : {kbzRefNo} " +
                   $"Credit Auth Respons Response ============>  " +
                   $"{JsonConvert.SerializeObject(returnResponse)}");

                return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
            }

        }

        [HttpPost("btrt01", Name = "StoreBTRT01")]
        public async Task<IActionResult> StoreBTRT01(storeAppBTRTModel data)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];

            _logger.LogInformation($"RefNo : {kbzRefNo},  BTRT01 Request data ==============>  {JsonConvert.SerializeObject(data)}");

            var respModel = new storeAppFinalRespModel { KBZRefNo = kbzRefNo };

            string appId = string.Empty;

            try
            {
                respModel.KBZRefNo = kbzRefNo;

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT01_MPU_Debit", kbzRefNo);

                if (!getConfig.isCheck)
                {
                    respModel.Error = ErrorCodeModel.Unauthorized;
                    respModel.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(kbzRefNo, respModel, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, respModel);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) || string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    respModel.Error = ErrorCodeModel.ClientRespError;
                    respModel.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(kbzRefNo, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                string xml = _service.GetBTRT01(getConfig, data);

                if (string.IsNullOrEmpty(xml))
                {
                    respModel.Error = ErrorCodeModel.FormatError;
                    respModel.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(kbzRefNo, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                _logger.LogInformation($"RefNo : {kbzRefNo}  BTRT01 XML Request ============> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {kbzRefNo} , BTRT01 Raw Response ============> {resp}");

                if (result.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(resp))
                    {
                        respModel = _respService.GetErrorResp(kbzRefNo);
                        AuditThread(kbzRefNo, respModel, null);

                        _logger.LogError($"RefNo : {kbzRefNo}, Failed to post BTRT01 ============> {respModel}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }

                    XDocument doc = XDocument.Parse(resp);
                    var q = from node in doc.Descendants("applicationId") select node;

                    if (q != null && q.Any())
                    {
                        appId = q.FirstOrDefault()?.Value;
                        respModel.Data = new storeAppRespModel()
                        {
                            applicationId = appId
                        };

                        _logger.LogInformation($"RefNo : {kbzRefNo} " +
                            $"BTRT01 application ID => {JsonConvert.SerializeObject(respModel)}");

                        return Ok(respModel);
                    }
                    else
                    {
                        respModel = _respService.GetErrorResp(doc, kbzRefNo);
                        AuditThread(kbzRefNo, respModel, null);

                        _logger.LogError($"RefNo : {kbzRefNo} " +
                            $"Failed to post BTRT01 => {JsonConvert.SerializeObject(respModel)}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }
                }
                else
                {
                    XDocument doc = XDocument.Parse(resp);
                    respModel = _respService.GetErrorResp(doc, kbzRefNo);
                    AuditThread(kbzRefNo, respModel, null);

                    _logger.LogError($"RefNo : {kbzRefNo} " +
                        $"BTRT01 Response ============> {JsonConvert.SerializeObject(respModel)}");

                    return BadRequest(respModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {kbzRefNo} " +
                    $"BTRT01 Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                respModel = _respService.GetErrorResp(ex, kbzRefNo);
                AuditThread(kbzRefNo, respModel, ex);

                _logger.LogError($"RefNo : {kbzRefNo} " +
                   $"BTRT01 Response ============> {JsonConvert.SerializeObject(respModel)}");

                return StatusCode(StatusCodes.Status500InternalServerError, respModel);
            }
        }

        [HttpPost("btrt02", Name = "StoreBTRT02")]
        public async Task<IActionResult> StoreBTRT02(storeAppBTRTModel data)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];

            _logger.LogInformation($"RefNo : {kbzRefNo},  " +
                $"BTRT02 Request data ==============>  " +
                $"{JsonConvert.SerializeObject(data)}");

            storeAppFinalRespModel respModel = new();

            string appId = string.Empty;

            try
            {
                respModel.KBZRefNo = kbzRefNo;

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT02_MPU_Debit", kbzRefNo);

                if (!getConfig.isCheck)
                {
                    respModel.Error = ErrorCodeModel.Unauthorized;
                    respModel.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(kbzRefNo, respModel, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, respModel);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) ||
                    string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    respModel.Error = ErrorCodeModel.ClientRespError;
                    respModel.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    AuditThread(kbzRefNo, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                string xml = _service.GetBTRT02(getConfig, data);

                if (string.IsNullOrEmpty(xml))
                {
                    respModel.Error = ErrorCodeModel.FormatError;
                    respModel.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(kbzRefNo, respModel, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                _logger.LogInformation($"RefNo : {kbzRefNo}  " +
                    $"BTRT02 XML Request ==========> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {kbzRefNo} " +
                    $"BTRT02 Raw Response ============> {resp}");

                if (result.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(resp))
                    {
                        respModel = _respService.GetErrorResp(kbzRefNo);
                        AuditThread(kbzRefNo, respModel, null);

                        _logger.LogError($"RefNo : {kbzRefNo} " +
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

                        _logger.LogInformation($"RefNo : {kbzRefNo} " +
                            $"BTRT02 application ID => {JsonConvert.SerializeObject(respModel)}");

                        return Ok(respModel);
                    }
                    else
                    {
                        respModel = _respService.GetErrorResp(doc, kbzRefNo);
                        AuditThread(kbzRefNo, respModel, null);

                        _logger.LogError($"RefNo : {kbzRefNo} " +
                            $"Failed to post BTRT02 => {JsonConvert.SerializeObject(respModel)}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }
                }
                else
                {
                    XDocument doc = XDocument.Parse(resp);
                    respModel = _respService.GetErrorResp(doc, kbzRefNo);
                    AuditThread(kbzRefNo, respModel, null);

                    _logger.LogError($"RefNo : {kbzRefNo} " +
                    $"BTRT02 Response ============> {JsonConvert.SerializeObject(respModel)}");

                    return BadRequest(respModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {kbzRefNo} " +
                    $"BTRT02 Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                respModel = _respService.GetErrorResp(ex, kbzRefNo);
                AuditThread(kbzRefNo, respModel, ex);

                _logger.LogError($"RefNo : {kbzRefNo} " +
                   $"BTRT02 Response ============> {JsonConvert.SerializeObject(respModel)}");

                return StatusCode(StatusCodes.Status500InternalServerError, respModel);
            }
        }

        [HttpPost("btrt30", Name = "StoreBTRT30")]
        public async Task<IActionResult> StoreBTRT30(UpdateAppBTRTModel data)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];

            _logger.LogInformation($"RefNo : {kbzRefNo},  " +
                $"BTRT30 Request data ==============>  " +
                $"{JsonConvert.SerializeObject(data)}");

            storeAppFinalRespModel respModel = new();

            string appId = string.Empty;

            try
            {
                respModel.KBZRefNo = kbzRefNo;

                ResponseServiceConfig getConfig = GetCongfigSetting("SV_BTRT30_MPU_Debit", kbzRefNo);

                if (!getConfig.isCheck)
                {
                    respModel.Error = ErrorCodeModel.Unauthorized;
                    respModel.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    _logger.LogError($"RefNo : {kbzRefNo}, Response : {respModel}");
                    return StatusCode(StatusCodes.Status401Unauthorized, respModel);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) ||
                    string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    respModel.Error = ErrorCodeModel.ClientRespError;
                    respModel.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    _logger.LogError($"RefNo : {kbzRefNo}, Response : {respModel}");
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                string xml = _service.GetBTRT30(getConfig, data);

                if (string.IsNullOrEmpty(xml))
                {
                    respModel.Error = ErrorCodeModel.FormatError;
                    respModel.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    _logger.LogError($"RefNo : {kbzRefNo}, Response : {respModel}");
                    return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                }

                _logger.LogInformation($"RefNo : {kbzRefNo}  " +
                    $"BTRT30 XML Request ==========> {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo : {kbzRefNo} " +
                    $"BTRT30 Raw Response ============> {resp}");

                if (result.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(resp))
                    {
                        respModel = _respService.GetErrorResp(kbzRefNo);
                        AuditThread(kbzRefNo, respModel, null);

                        _logger.LogError($"RefNo : {kbzRefNo} " +
                            $"Failed to post BTRT30 => {respModel}");

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

                        _logger.LogInformation($"RefNo : {kbzRefNo} " +
                            $"BTRT30 application ID => {JsonConvert.SerializeObject(respModel)}");

                        return Ok(respModel);
                    }
                    else
                    {
                        respModel = _respService.GetErrorResp(doc, kbzRefNo);
                        AuditThread(kbzRefNo, respModel, null);

                        _logger.LogError($"RefNo : {kbzRefNo} " +
                            $"Failed to post BTRT30 => {JsonConvert.SerializeObject(respModel)}");

                        return StatusCode(StatusCodes.Status500InternalServerError, respModel);
                    }
                }
                else
                {
                    XDocument doc = XDocument.Parse(resp);
                    respModel = _respService.GetErrorResp(doc, kbzRefNo);
                    AuditThread(kbzRefNo, respModel, null);

                    _logger.LogError($"RefNo : {kbzRefNo} " +
                    $"BTRT30 Response ============> {JsonConvert.SerializeObject(respModel)}");

                    return BadRequest(respModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {kbzRefNo} " +
                    $"BTRT30 Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                respModel = _respService.GetErrorResp(ex, kbzRefNo);

                _logger.LogError($"RefNo : {kbzRefNo} " +
                   $"BTRT30 Response ============> {JsonConvert.SerializeObject(respModel)}");

                return StatusCode(StatusCodes.Status500InternalServerError, respModel);
            }
        }

        [NonAction]
        [HttpPost("processApplication", Name = "CDMSProcessApplication")]
        public async Task<IActionResult> CDMSProcessApplication(processAppModel model)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            storeAppFinalRespModel returnResponse = new();

            try
            {

                string appId = string.Empty; string url = string.Empty;

                _logger.LogInformation($" RefNo: {kbzRefNo} , ProcessApplication  Request Starting ====> {JsonConvert.SerializeObject(model)}");
                ResponseServiceConfig getConfig = GetCongfigSetting("SV_ProcessApplication", kbzRefNo);

                if (!getConfig.isCheck)
                {
                    returnResponse.Error = ErrorCodeModel.Unauthorized;
                    returnResponse.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    AuditThread(kbzRefNo, returnResponse, null);
                    return StatusCode(StatusCodes.Status401Unauthorized, returnResponse);
                }
                url = getConfig.Config.EndpointAddress;

                string xml = _service.GetApplication(getConfig, model);
                if (string.IsNullOrEmpty(xml))
                {
                    returnResponse.Error = ErrorCodeModel.FormatError;
                    returnResponse.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    AuditThread(kbzRefNo, returnResponse, null);
                    return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
                }

                _logger.LogInformation($"RefNo: {kbzRefNo} , ProcessApplication Request XML =======> {xml}");
                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                _logger.LogInformation($"RefNo: {kbzRefNo} , Requesting... to SV with Soap Url =======> {url}");
                var result = await _httpClientFactory.CreateClient().PostAsync(url, httpContent);
                var resp = await result.Content.ReadAsStringAsync();
                XDocument doc = XDocument.Parse(resp);

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"RefNo: {kbzRefNo} , ProcessApplication Response =======> {resp} ");

                    var q = from node in doc.Descendants("applicationId") select node;

                    if (q.ToList().Count() != 0)
                        appId = q.FirstOrDefault()!.Value;

                    if (string.IsNullOrEmpty(appId))
                    {
                        _logger.LogError($"RefNo: {kbzRefNo} , AppID not found =======> {resp}");
                        returnResponse.Error = ErrorCodeModel.ClientRespError;
                        returnResponse.Error.Details = new List<BaseRespErrorDetail>
                        {
                            new BaseRespErrorDetail() { ErrorCode = "TAGSYS", ErrorDescription = "AppID not found!" }
                        };
                        return BadRequest(returnResponse);
                    }
                    else
                    {
                        _logger.LogInformation($"RefNo: {kbzRefNo} , Responsed App ID =======> {appId} ");
                        returnResponse.Data.applicationId = appId;
                        return Ok(returnResponse);
                    }
                }
                else
                {
                    returnResponse = _respService.GetErrorResp(doc, kbzRefNo);

                    _logger.LogError($"ProcessApplication Error Response ========> RefNo: {kbzRefNo} , {resp}");
                    AuditThread(kbzRefNo, returnResponse);

                    return BadRequest(returnResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RefNo : {kbzRefNo} " +
                    $"ProcessApplication Exception Message ============>  {ex.Message}, InnerException : {ex.InnerException}");

                returnResponse = _respService.GetErrorResp(ex, kbzRefNo);
                AuditThread(kbzRefNo, returnResponse, ex);

                _logger.LogError($"RefNo : {kbzRefNo} " +
                   $"ProcessApplication Response ============> {JsonConvert.SerializeObject(returnResponse)}");

                return StatusCode(StatusCodes.Status500InternalServerError, returnResponse);
            }
        }

        [HttpGet("enquiry/customer/{customerId}", Name = "CustomerEnquiry")]
        public async Task<IActionResult> CustomerEnquiry(string customerId)
        {
            _logger.LogInformation($"Request customer enquiry => {customerId}");

            CustomerEnquiryDto response = new();
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            response.KBZRefNo = kbzRefNo;

            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    response.Error = ErrorCodeModel.ValidateError;
                    response.Error.Details.Add(ErrorCodeModel.EmptyRequestParameterError);
                }

                var result = await _dbService.GetInfoById(customerId, _conStr.VISTA_Oracle!);
                if (result?.Count() != 0)
                    response.Data = result;
                else
                    response.Error = ErrorCodeModel.NoRecordsFound;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Error = ErrorCodeModel.DBError;
                _logger.LogError($"Response ===> KBZRefNo : {kbzRefNo}, Response : {JsonConvert.SerializeObject(response)} \n Exception : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [HttpGet("enquiry/cardstatus/{customerId}", Name = "QueryCardStatusByCustomerId")]
        public async Task<IActionResult> QueryCardStatusByCustomerId(string customerId)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            AuditThread(kbzRefNo, customerId);

            _logger.LogInformation($"CDMS Sv Api CardStatusByCIFNoDto Request ===> " +
                $"KBZRefNo : {kbzRefNo} RequestPayload : {customerId}");

            SmartVistaQueryCardStatusByNRCResponseModel response = new();
            if (string.IsNullOrEmpty(customerId))
            {
                response.Error = ErrorCodeModel.ValidateError;
                response.Error.Details.Add(ErrorCodeModel.EmptyRequestParameterError);
                return BadRequest(response);
            }

            response.KBZRefNo = kbzRefNo;
            var result = await _dbService.GetCardStatusById(customerId, _conStr.VISTA_Oracle!);

            if (result.isErrorOccurred)
            {
                response.Error = ErrorCodeModel.DBError;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

            if (result.Item1.Any())
                response.Data = result.Item1;
            else
                response.Error = ErrorCodeModel.NoRecordsFound;

            _logger.LogInformation($"CDMS Sv Api CardStatusByCIFNoDto Response ===> " +
                $"KBZRefNo : {kbzRefNo} RequestPayload : {JsonConvert.SerializeObject(response)}");

            AuditThread(kbzRefNo, response);
            return Ok(response);
        }

        [HttpPost("card/list", Name = "GetCard")]
        public async Task<IActionResult> GetCard(GetCardRequestDto request)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            _logger.LogInformation($"RefNo : {kbzRefNo}, RequestPayLoad : {request}");

            GetCardResponseDto response = new();
            response.KBZRefNo = kbzRefNo;
            try
            {
                ResponseServiceConfig getConfig = GetCongfigSetting("SVCMS_GetCard", kbzRefNo);

                if (!getConfig.isCheck)
                {
                    response.Error = ErrorCodeModel.Unauthorized;
                    response.Error.Details.Add(ErrorCodeModel.UnauthorizedConfiguration);
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }

                if (string.IsNullOrEmpty(getConfig.Config.RequestFormat) || string.IsNullOrEmpty(getConfig.Config.EndpointAddress))
                {
                    response.Error = ErrorCodeModel.ClientRespError;
                    response.Error.Details.Add(ErrorCodeModel.Config_Call_Error);
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                string xml = _service.GetCardListXML(getConfig, _wsse, request);

                if (string.IsNullOrEmpty(xml))
                {
                    response.Error = ErrorCodeModel.FormatError;
                    response.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                _logger.LogInformation($"RefNo: {kbzRefNo}, XML Request : {xml}");

                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                var result = await _httpClientFactory.CreateClient().PostAsync(getConfig.Config.EndpointAddress, httpContent);
                var resp = await result.Content.ReadAsStringAsync();
                _logger.LogInformation($"RefNo: {kbzRefNo}, SV XML Response : {resp}");
                if (!string.IsNullOrEmpty(resp))
                {
                    (int statusCode, GetCardResponseDto GetCardResponse) cardInfo = _respService.GetCardResponse(resp);

                    response.Data = cardInfo.GetCardResponse?.Data;
                    response.Error = cardInfo.GetCardResponse?.Error;

                    _logger.LogInformation($"RefNo: {kbzRefNo}, Response : {response}");
                    return StatusCode(cardInfo.statusCode, cardInfo.GetCardResponse);
                }
                else
                {
                    response.Error = ErrorCodeModel.ClientRespError;
                    _logger.LogInformation($"RefNo: {kbzRefNo}, Response : {response}, Response was null");
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Error = ErrorCodeModel.UnknownException;
                response.Error.Details.Add(new BaseRespErrorDetail()
                {
                    ErrorCode = ErrorCodeModel.UnknownException.Code,
                    ErrorDescription = ex.Message
                });
                _logger.LogError($"RefNo: {kbzRefNo}, Response (Exception) : {response}");
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("card/kyc", Name = "KYCByCardNumber")]
        public async Task<IActionResult> KYCByCardNumber(string cardNumber)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            AppQueryRespModel returndata = new();
            returndata.KBZRefNo = kbzRefNo;

            _logger.LogInformation($"KYCByCardNumber Request ===> KBZRefNo : {kbzRefNo} RequestPayload : {cardNumber}");

            try
            {
                returndata.Data = await _dbService.KycByCardNumber(cardNumber, _conStr.VISTA_Oracle!);
                _logger.LogInformation($" Response ===> KBZRefNo : {kbzRefNo}, Response : {JsonConvert.SerializeObject(returndata)}");
                return Ok(returndata);
            }
            catch (Exception ex)
            {
                returndata.Error = ErrorCodeModel.DBError;
                _logger.LogError($"Response ===> KBZRefNo : {kbzRefNo}, Response : {JsonConvert.SerializeObject(returndata)} \n Exception : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError,returndata);
            }

        }

        [NonAction]
        public ResponseServiceConfig GetCongfigSetting(string serviceName, string kbzRefNo)
        {
            _logger.LogInformation($"RefNo: {kbzRefNo}, Config Request ============> {serviceName}");
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
            config.KbzRefNo = kbzRefNo;

            #endregion

            ResponseServiceConfig getConfig = ServiceConfig.InitConfig(config.ServiceName, config.ConfigUrl,
                config.HttpClient, config.Scheme, config.Parameter, config.KbzRefNo);

            _logger.LogInformation($"RefNo: {kbzRefNo} , Config Response =======> {JsonConvert.SerializeObject(getConfig)}");

            return getConfig;
        }

        [NonAction]
        public void AuditThread(string LOGID, object returnResponse, Exception? ex = null)
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
