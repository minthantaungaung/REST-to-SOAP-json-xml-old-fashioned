using api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;

namespace api.svici.sys.Infrastructure.SVBO_Service
{
    public interface ISV_BO_Service
    {
        string GetCWCardListXML(CWContactInfoRequestDto req);
        Task<QueryContactInfoCW?> GetCardListResponse(string CardNumber);
        string MockResponse();
    }
}