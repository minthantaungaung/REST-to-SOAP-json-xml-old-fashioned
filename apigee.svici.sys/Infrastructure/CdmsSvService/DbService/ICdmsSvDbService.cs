using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CardStatusList;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.CustomerEnquiry;

namespace api.svici.sys.Infrastructure.CdmsSvService.DbService
{
    public interface ICdmsSvDbService
    {
        Task<List<CustomerEnquiry>?> GetInfoById(string customerId, string url);
        Task<(List<QueryCardStatusByAcctNoModel>, bool isErrorOccurred)>? GetCardStatusById(string Id, string url);
        Task<List<Dictionary<string, object>>?> KycByCardNumber(string cardNumber, string url);
    }
}
