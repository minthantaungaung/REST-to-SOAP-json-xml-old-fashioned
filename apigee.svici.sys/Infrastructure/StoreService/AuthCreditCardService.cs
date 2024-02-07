using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.StoreDtos;

namespace api.svici.sys.Infrastructure.StoreService
{
    public class AuthCreditCardService
    {
        public string GetAuthDto(ResponseServiceConfig setting, Authenticate model)
        {
            string xml = string.Empty;
            try
            {
                var body = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocument"]);
                if (body is null)
                    return xml;
                var xmlTemplate = JsonConvert.DeserializeObject(body).ToString();

                if (xmlTemplate != null)
                {
                    xml = string.Format(xmlTemplate, model.login, model.password);
                }
                return xml;
            }
            catch (Exception ex)
            {
                return xml;
            }
        }
    }
}
