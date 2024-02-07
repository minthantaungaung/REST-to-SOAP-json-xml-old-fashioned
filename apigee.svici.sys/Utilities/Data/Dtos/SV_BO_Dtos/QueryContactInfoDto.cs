using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SV_FE_Dtos;
using Newtonsoft.Json;

namespace api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos
{
    public class QueryContactInfo : Property
    {
        public string? E_MAIL { get; set; }
        public string? PHONE1 { get; set; }
        public string? PHONE2 { get; set; }
        public string? PHONE3 { get; set; }
    }
    public class QueryContactInfoResponseDto : BaseRespModel
    {
        public QueryContactInfo? Data { get; set; }
    }
}