using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using System.Xml.Linq;

namespace api.svici.sys.Infrastructure.CdmsSvService.ResponseService
{
    public interface ICdmsSvResponseService
    {
        AuthFinalRespModel GetAuthErrorResp(XDocument doc, string LogID);
        AuthFinalRespModel GetAuthErrorResp(Exception ex, string LogID);
        storeAppFinalRespModel GetErrorResp(XDocument doc, string LogID);
        storeAppFinalRespModel GetErrorResp(string LogID);
        storeAppFinalRespModel GetErrorResp(Exception ex, string LogID);
        (int statusCode, GetCardResponseDto GetCardResponse) GetCardResponse(string xmlresp);
    }
}
