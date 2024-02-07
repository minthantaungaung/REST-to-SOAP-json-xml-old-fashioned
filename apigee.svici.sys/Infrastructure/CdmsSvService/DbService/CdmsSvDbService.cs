using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CardStatusList;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CustomerEnquiry;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace api.svici.sys.Infrastructure.CdmsSvService.DbService
{
    public class CdmsSvDbService : ICdmsSvDbService
    {
        public async Task<List<CustomerEnquiry>>? GetInfoById(string Id, string url)
        {

            string sqlfilter = string.Empty;
            string parmName = string.Empty;
            if (Id.Length.Equals(9))
            {
                sqlfilter = "rf.iss_customer_id=:customerId";
                parmName = "customerId";
            }
            else if (Id.Length.Equals(17))
            {
                sqlfilter = "rf.ISS_ACCT_NUM=:accountNo";
                parmName = "accountNo";
            }
            else if (Id.Length.Equals(16))
            {
                sqlfilter = "rf.ISS_CARD_NUM=:cardNumber";
                parmName = "cardNumber";
            }
            DataSet dataset = new();

            using var conn = new OracleConnection(url);
            await conn.OpenAsync();

            string sql = $"select rf.ISS_ACCT_NUM,rf.ISS_CARD_NUM,rf.ISS_PERSON_ID,rf.ISS_CONTRACT_ID,rf.ISS_CUSTOMER_ID,rf.ISS_AGENT_ID,rf.INST_ID,chd.PERSON_ID,chd.ADDRESS_ID from vista.iss_ref_tab rf inner join vista.iss_cardhldr_tab chd on rf.ISS_PERSON_ID=chd.PERSON_ID where {sqlfilter}";
            using OracleCommand cmd = new(sql, conn);

            cmd.Parameters.Add(new OracleParameter(parmName, Id));
            using OracleDataAdapter adapter = new(cmd);

            adapter.Fill(dataset);
            conn.Close();
            string data = JsonConvert.SerializeObject(dataset.Tables[0]);

            var Data = JsonConvert.DeserializeObject<List<CustomerEnquiry>>(data);
            return Data;
        }
        public async Task<(List<QueryCardStatusByAcctNoModel>, bool isErrorOccurred)>? GetCardStatusById(string Id, string url)
        {
            bool isError = false;
            try
            {
                DataSet dataset = new();

                using var conn = new OracleConnection(url);
                await conn.OpenAsync();

                //string sql = $"select * from vista.cardslist_bynrc where substr(acct,7,9) = :CIFNo";
                string sql = $"select CARDID,CARDNUMBER,MEM_NO,EXPIRATIONDATE,CARDSTATUS,CARDSTATUSDESC,PRODUCTNAME,ACCT,EMBOSSNAME,ID_NO From vista.cardslist where CUST_ID=:CIFNo";
                using OracleCommand cmd = new(sql, conn);

                cmd.Parameters.Add(new OracleParameter("CIFNo", Id));
                using OracleDataAdapter adapter = new(cmd);

                adapter.Fill(dataset);
                conn.Close();
                string data = JsonConvert.SerializeObject(dataset.Tables[0]);

                var Data = JsonConvert.DeserializeObject<List<QueryCardStatusByAcctNoModel>>(data);
                return (Data, isError);
            }
            catch (Exception ex)
            {
                isError = true;
                return (default, isError);
            }
        }
        public async Task<List<Dictionary<string, object>>?> KycByCardNumber(string cardNumber, string url)
        {
            DataSet dataset = new();
            string sql = "SELECT * FROM vista.crd_lst where CARDNUMBER =:CardNumber";
            await using var conn = new OracleConnection(url);
            await conn.OpenAsync();

            OracleCommand cmd = new(sql, conn);
            cmd.Parameters.Add(new OracleParameter("CardNumber", cardNumber));
            OracleDataAdapter adapter = new(cmd);
            adapter.Fill(dataset);

            conn.Close();
            string data = JsonConvert.SerializeObject(dataset.Tables[0]);
            var kycdata = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data);
            return kycdata;
        }
    }
}
