using api.svici.sys.Utilities.Data.Dtos.ResponseModels;

namespace api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CustomerEnquiry
{
    public class CustomerEnquiryDto : BaseRespModel
    {
        public List<CustomerEnquiry>? Data { get; set; }
    }
    public class CustomerEnquiry
    {
        public string? iss_acct_num { get; set; }
        public string? iss_card_num { get; set; }
        public string? iss_person_id { get; set; }
        public string? iss_contract_id { get; set; }
        public string? iss_customer_id { get; set; }
        public string? iss_agent_id { get; set; }
        public string? inst_id { get; set; }
        public string? person_id { get; set; }
        public string? address_id { get; set; }
    }



}
