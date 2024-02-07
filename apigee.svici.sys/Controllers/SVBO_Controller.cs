using api.svici.sys.Infrastructure.SV_FE_Services;
using api.svici.sys.Utilities.ActionFilters;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos;
using api.svici.sys.Utilities.Data.Dtos.SV_FE_Dtos;
using app.api.Controllers;
using app.api.Infrastructure.SVService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Text;
using api.svici.sys.Infrastructure.SVBO_Service;
using System.Net;

namespace api.svici.sys.Controllers
{
    [Authorize]
    [Route("api/SVBO")]
    [ApiController]
    [RequestPayloadValidationFilter]
    public class SVBO_Controller : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConnectionString _conStr;
        private readonly ISvFE_DbService _dbFEService;
        private readonly ISV_BO_Service _service;

        private readonly ILogger<SVFE_Controller> _logger;
        public SVBO_Controller(IHttpClientFactory httpClientFactory, IOptions<ConnectionString> conStr,
            ILogger<SVFE_Controller> log, ISvFE_DbService dbFEService, ISV_BO_Service service)
        {
            _logger = log;
            _conStr = conStr.Value;
            _dbFEService = dbFEService;
            _httpClientFactory = httpClientFactory;
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("ContactInfo", Name = "QueryContactInfoByCardNumber")]
        public async Task<IActionResult> QueryContactInfoByCardNumber(string cardNumber)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            QueryContactInfoResponseDto returndata = new();
            returndata.KBZRefNo = kbzRefNo;

            _logger.LogInformation($"QueryContactInfoByCardNumber Request ===> KBZRefNo : {kbzRefNo}, CardNumber : {cardNumber}");

            try
            {
                var response = await _dbFEService.QueryContactInfo(cardNumber, _conStr.VISTA_Oracle!);
                if (response.data != null)
                {
                    returndata.Data = response.data;
                    _logger.LogInformation($" Response ===> KBZRefNo : {kbzRefNo}, Response : {returndata}");
                    return Ok(returndata);
                }
                else
                {
                    returndata.Error = ErrorCodeModel.NoRecordsFound;
                    returndata.Error.Details.Add(new BaseRespErrorDetail() { ErrorDescription = response.errorMsg });
                    _logger.LogError($"QueryContactInfoByCardNumber Response ===> KBZRefNo : {kbzRefNo}, Response : {returndata} \n Error : {response.errorMsg}");
                    return StatusCode(StatusCodes.Status400BadRequest, returndata);
                }
            }
            catch (Exception ex)
            {
                returndata.Error = ErrorCodeModel.DBError;
                returndata.Error.Details.Add(new BaseRespErrorDetail() { ErrorDescription = ex.Message });
                _logger.LogError($"QueryContactInfoByCardNumber Response ===> KBZRefNo : {kbzRefNo}, Response : {returndata} \n Exception : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, returndata);
            }
        }

        [AllowAnonymous]
        [HttpPost("CardWorksProfileEnquiry", Name = "CardWorks_ProfileEnquiry")]
        public async Task<IActionResult> CardWorks_ProfileEnquiry(CWContactInfoRequestDto req)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            _logger.LogInformation($"RefNo : {kbzRefNo}, Request : {req}");

            CWContactInfoResponseDto response = new();
            response.KBZRefNo = kbzRefNo;
            try
            {

                string xml = _service.GetCWCardListXML(req);
                if (string.IsNullOrEmpty(xml))
                {
                    response.Error = ErrorCodeModel.FormatError;
                    response.Error.Details.Add(ErrorCodeModel.TemplateFormatError);
                    _logger.LogInformation($"RefNo: {kbzRefNo}, Response : {response}");
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                _logger.LogInformation($"RefNo: {kbzRefNo}, XML Request : {xml}");
                
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                client.DefaultRequestHeaders.Add("SOAPAction", _conStr.CardWorksSoapService + "/custProfileEnquiry");
                var httpContent = new StringContent(xml, Encoding.UTF8, "text/xml");

                var result = await client.PostAsync(_conStr.CardWorksSoapService, httpContent);
                var resp = await result.Content.ReadAsStringAsync();

                _logger.LogInformation($"RefNo: {kbzRefNo}, SV XML Response : {resp}");
                if (!string.IsNullOrEmpty(resp))
                {
                    var responseData = await _service.GetCardListResponse(resp);
                    if (responseData is null)
                    {
                        response.Error = ErrorCodeModel.ClientRespError;
                        _logger.LogInformation($"RefNo: {kbzRefNo}, Response : {response}");
                        return StatusCode(StatusCodes.Status500InternalServerError, response);
                    }
                    else if (!(typeof(QueryContactInfoCW).GetProperties().Any(p => !string.IsNullOrWhiteSpace(p.GetValue(responseData)?.ToString()))))
                    {
                        response.Error = ErrorCodeModel.NoRecordsFound;
                        _logger.LogInformation($"RefNo: {kbzRefNo}, Response : {response}");
                        return BadRequest(response);
                    }
                        response.Data = responseData;
                    _logger.LogInformation($"RefNo: {kbzRefNo}, Response : {response}");
                    return Ok(response);
                }
                else
                {
                    response.Error = ErrorCodeModel.ClientRespError;
                    _logger.LogError($"RefNo: {kbzRefNo}, Response : {response}, Response was null");
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Raw Exception : {ex}");
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
    }
}
