using api.svici.sys.Infrastructure.SV_FE_Services;
using api.svici.sys.Utilities.ActionFilters;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SV_FE_Dtos;
using app.api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace api.svici.sys.Controllers
{
    [Authorize]
    [Route("api/SVFE")]
    [ApiController]
    [RequestPayloadValidationFilter]
    public class SVFE_Controller : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConnectionString _conStr;
        private readonly ISvFE_DbService _dbFEService;
        private readonly ILogger<SVFE_Controller> _logger;
        public SVFE_Controller(IHttpClientFactory httpClientFactory, IOptions<ConnectionString> conStr,
            ILogger<SVFE_Controller> log, ISvFE_DbService dbFEService)
        {
            _logger = log;
            _conStr = conStr.Value;
            _dbFEService = dbFEService;
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        [HttpPost("QueryCardTranx", Name = "QueryCardTranx")]
        public async Task<IActionResult> QueryCardTranx(QueryCardTranxRequestDto req)
        {
            string kbzRefNo = Request.Headers.TryGetValue("KBZ_REF_NO", out var refno) ? refno : Request.Headers["LOGID"];
            QueryCardTranxResponseDto returndata = new();
            returndata.KBZRefNo = kbzRefNo;

            _logger.LogInformation($"QueryCardTranx Request ===> KBZRefNo : {kbzRefNo} RequestPayload : {req}");

            try
            {
                var dataList = await _dbFEService.QueryCardTranx(req, _conStr.VISTA_FE!);
                if (dataList.data != null && dataList.data.Count() != 0)
                {
                    returndata.Data = dataList.data;
                    _logger.LogInformation($" Response ===> KBZRefNo : {kbzRefNo}, Response : {returndata}");
                    return Ok(returndata);
                }
                else
                {
                    returndata.Error = ErrorCodeModel.NoRecordsFound;
                    returndata.Error.Details.Add(new BaseRespErrorDetail() { ErrorDescription = dataList.errorMsg });
                    _logger.LogError($"QueryCardTranx Response ===> KBZRefNo : {kbzRefNo}, Response : {returndata} \n Error : {dataList.errorMsg}");
                    return StatusCode(StatusCodes.Status400BadRequest, returndata);
                }
            }
            catch (Exception ex)
            {
                returndata.Error = ErrorCodeModel.DBError;
                returndata.Error.Details.Add(new BaseRespErrorDetail() { ErrorDescription = ex.Message});
                _logger.LogError($"QueryCardTranx Response ===> KBZRefNo : {kbzRefNo}, Response : {returndata} \n Exception : {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, returndata);
            }

        }
    }
}
