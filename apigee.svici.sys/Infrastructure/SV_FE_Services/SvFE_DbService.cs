using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CardStatusList;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CustomerEnquiry;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos;
using api.svici.sys.Utilities.Data.Dtos.SV_FE_Dtos;
using app.api.Infrastructure.Logger;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace api.svici.sys.Infrastructure.SV_FE_Services
{
    public class SvFE_DbService : ISvFE_DbService
    {
        private readonly ILogger _logger;
        public SvFE_DbService(ILogger<SvFE_DbService> logger)
        {
            _logger = logger;
        }

        public async Task<(List<CardTranxList>? data, string? errorMsg)> QueryCardTranx(QueryCardTranxRequestDto req, string url)
        {
            string? errorMsg = default;
            DataSet dataset = new();

            //string sql = "SELECT CARD_NUM As CARD_NUM, ACC_NUM As ACC_NUM, TRN_DATE As TRN_DATE, TIME As TIME, TERMINAL As TERMINAL, TERMINAL_NAME As TERMINAL_NAME, TRAN_TYPE As TRAN_TYPE, TRAN_DESC As TRAN_DESC, AMMOUNT As AMMOUNT, CURRENCY As CURRENCY, CUR_DESP As CUR_DESP, FEE As FEE, CONVERTED_AMT As CONVERTED_AMT, RESP_CODE As RESP_CODE, RESP_DESC As RESP_DESC, REVERSAL As REVERSAL, MSGTYPE As MSGTYPE, UTRNNO As UTRNNO, ISS_AUTH_ID As ISS_AUTH_ID, ACQ_AUTH_ID As ACQ_AUTH_ID, ATM_RESP As ATM_RESP, ATM_RESP_DESC As ATM_RESP_DESC, ISS_INST As ISS_INST, ISS_INSTITUTION As ISS_INSTITUTION, ACQ_INST As ACQ_INST, ACQ_INSTITUTION As ACQ_INSTITUTION, TRN_DATE As FROMDATE, TRN_DATE As TODATE FROM svista.fe_trans WHERE (CARD_NUM = :CARD_NUM OR ACC_NUM = :ACC_NUM) AND TRN_DATE >= :FROMDATE AND TRN_DATE <= :TODATE ORDER BY TRN_DATE";

            string sql = "SELECT CARD_NUM As CARD_NUM, ACC_NUM As ACC_NUM, TRN_DATE As TRN_DATE, TIME As TIME, TERMINAL As TERMINAL, TERMINAL_NAME As TERMINAL_NAME, TRAN_TYPE As TRAN_TYPE, TRAN_DESC As TRAN_DESC, AMMOUNT As AMMOUNT, CURRENCY As CURRENCY, CUR_DESP As CUR_DESP, FEE As FEE, CONVERTED_AMT As CONVERTED_AMT, RESP_CODE As RESP_CODE, RESP_DESC As RESP_DESC, REVERSAL As REVERSAL, MSGTYPE As MSGTYPE, UTRNNO As UTRNNO, ISS_AUTH_ID As ISS_AUTH_ID, ACQ_AUTH_ID As ACQ_AUTH_ID, ATM_RESP As ATM_RESP, ATM_RESP_DESC As ATM_RESP_DESC, ISS_INST As ISS_INST, ISS_INSTITUTION As ISS_INSTITUTION, ACQ_INST As ACQ_INST, ACQ_INSTITUTION As ACQ_INSTITUTION, TRN_DATE As FROMDATE, TRN_DATE As TODATE FROM svista.fe_trans " +
                $"WHERE (CARD_NUM = '{req.CardNumber}' OR ACC_NUM = '{req.AccNumber}') AND TRN_DATE >= '{req.FromDate}' AND TRN_DATE <= '{req.ToDate}' ORDER BY TRN_DATE";

            await using var conn = new OracleConnection(url);
            await conn.OpenAsync();

            OracleCommand cmd = new(sql, conn);
            //cmd.Parameters.Add(new OracleParameter("CARD_NUM", req.CardNumber));
            //cmd.Parameters.Add(new OracleParameter("ACC_NUM", req.AccNumber));
            //cmd.Parameters.Add(new OracleParameter("FROMDATE", req.FromDate));
            //cmd.Parameters.Add(new OracleParameter("TODATE", req.ToDate));
            OracleDataAdapter adapter = new(cmd);
            adapter.Fill(dataset);
            conn.Close();
            string data = JsonConvert.SerializeObject(dataset.Tables[0]);

            _logger.LogInformation($"Get Card Tranx RawList : {data}");

            var tranxData = JsonConvert.DeserializeObject<List<CardTranxList>>(data);
            if (tranxData == null || tranxData?.Count() == 0)
            {
                errorMsg = "No record found";
                _logger.LogError(errorMsg);
            }
            _logger.LogInformation($"Get Card Tranx List : {tranxData}");
            return (tranxData, errorMsg);
        }

        public async Task<(QueryContactInfo? data, string? errorMsg)> QueryContactInfo(string cardNo, string url)
        {
            string? errorMsg = default;
            DataSet dataset = new();
            string sql = "select at.e_mail,at.phone1, at.phone2, at. phone3 from vista.iss_ref_tab ir inner join vista.iss_cardhldr_tab chd on ir.iss_person_id=chd.person_id inner join vista.addr_tab at on at.address_id=chd.address_id where ir.iss_card_num=:cardNumber";

            await using var conn = new OracleConnection(url);
            await conn.OpenAsync();

            OracleCommand cmd = new(sql, conn);
            cmd.Parameters.Add(new OracleParameter("cardNumber", cardNo));
            OracleDataAdapter adapter = new(cmd);
            adapter.Fill(dataset);
            conn.Close();
            if(dataset.Tables[0].Rows.Count == 0)
            {
                errorMsg = "No record found";
                _logger.LogError(errorMsg);
                return (default, errorMsg);
            }
            string data = JsonConvert.SerializeObject(dataset.Tables[0]);
            //if (!data.Any())
            
            _logger.LogInformation($"Raw Contact Info Data: {data}");

            var Data = JsonConvert.DeserializeObject<List<QueryContactInfo?>?>(data);

            _logger.LogInformation($"Contact Info : {Data}");
            return (Data[0], errorMsg);
        }
    }
}
