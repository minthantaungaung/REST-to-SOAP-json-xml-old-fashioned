using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CardStatusList;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CustomerEnquiry;
using api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos;
using api.svici.sys.Utilities.Data.Dtos.SV_FE_Dtos;

namespace api.svici.sys.Infrastructure.SV_FE_Services
{
    public interface ISvFE_DbService
    {
        Task<(List<CardTranxList>? data, string? errorMsg)> QueryCardTranx(QueryCardTranxRequestDto req, string url);
        Task<(QueryContactInfo? data, string? errorMsg)> QueryContactInfo(string carNo, string url);
    }
}
