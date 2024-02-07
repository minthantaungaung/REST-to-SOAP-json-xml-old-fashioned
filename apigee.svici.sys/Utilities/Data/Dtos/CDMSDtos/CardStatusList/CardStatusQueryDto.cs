using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using Newtonsoft.Json;

namespace api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CardStatusList
{
    public class SmartVistaQueryCardStatusByNRCResponseModel : BaseRespModel
    {
        public IList<QueryCardStatusByAcctNoModel> Data { get; set; }
    }
    public class QueryCardStatusByAcctNoModel
    {
        public string CardID { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CardStatus { get; set; }
        public string CardStatusDesc { get; set; }
        public string ProductName { get; set; }
        public string Acct { get; set; }
        public string Mem_No { get; set; }
        public string EmbossName { get; set; }

        [JsonProperty("ID_NO")]
        public string NRC { get; set; }
    }
}
