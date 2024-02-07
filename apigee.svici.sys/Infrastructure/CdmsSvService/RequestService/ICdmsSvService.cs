using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard;
using api.svici.sys.Utilities.Data.Dtos.StoreDtos;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;

namespace api.svici.sys.Infrastructure.CdmsSvService.RequestService
{
    public interface ICdmsSvService
    {
        string GetAuthDto(ResponseServiceConfig setting, Authenticate model);
        string GetApplication(ResponseServiceConfig setting, processAppModel model);
        string GetBTRT01(ResponseServiceConfig setting, storeAppBTRTModel model);
        string GetBTRT02(ResponseServiceConfig setting, storeAppBTRTModel model);
        string GetBTRT30(ResponseServiceConfig setting, UpdateAppBTRTModel model);
        string GetCardListXML(ResponseServiceConfig setting, WsseCredentials wsse, GetCardRequestDto model);
    }
}
