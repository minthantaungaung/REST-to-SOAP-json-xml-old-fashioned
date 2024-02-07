using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace api.svici.sys.Utilities.Helpers
{
    public static class ServiceConfig
    {
        public static ResponseServiceConfig InitConfig(string ServiceName, string ServiceUrl, HttpClient httpClient, string scheme, string parameter, string KBZ_REF_NO)
        {
            try
            {
                string URL = ServiceUrl + "getbyid"; //Settings.VALUE(ConfigurationManager.AppSettings["URL"].ToString(), SettingType.URL, Project.CONFIG) + "getbyid";
                HttpRequestMessage requestMsg = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri(URL + "?Id=" + ServiceName) };
                requestMsg.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter);
                requestMsg.Headers.Add("KBZ_REF_NO", KBZ_REF_NO);
                HttpResponseMessage response =  httpClient.Send(requestMsg);
                ResponseServiceConfig resConfig = new ResponseServiceConfig();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResConfig>(response.Content.ReadAsStringAsync().Result);

                    string username = result.Data.userid;
                    dynamic values = JsonConvert.DeserializeObject<Object>(JsonConvert.DeserializeObject<Object>(result.Data.value.ToString()).ToString());
                    dynamic format = result.Data.reqFormatValue.ToString();
                    foreach (var val in values)
                        format = format.Replace("{" + val.Name.ToString() + "}", val.Value.ToString());

                    dynamic Config = new
                    {
                        EndpointAddress = result.Data.serviceUrl.ToString(),
                        RequestFormat = format
                    };

                    resConfig.isCheck = true;
                    resConfig.Config = Config;
                    return resConfig;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    BaseRespModel result = JsonConvert.DeserializeObject<BaseRespModel>(response.Content.ReadAsStringAsync().Result);
                    if (result.Error.Code == "503")
                    {
                        string serviceStatus = "INACTIVE";

                        resConfig.isCheck = true;
                        return resConfig;
                    }
                    else
                    {
                        resConfig.isCheck = false;
                        return resConfig;
                    }

                }
                else throw new Exception(ErrorCodeModel.Config_Call_Error.ErrorDescription);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
