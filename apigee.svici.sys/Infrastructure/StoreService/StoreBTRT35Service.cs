using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.StoreDtos;

namespace app.api.Infrastructure.StoreService
{
    public class StoreBTRT35Service
    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext<StoreBTRT2Service>();
        public string GetBTRT35(ResponseServiceConfig setting, BTRT35_Dto data)
        {
            string xml = string.Empty;
            string additionalservice = string.Empty;
            try
            {
                var body = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocument"]);
                if (body is null) return xml;

                if(data.additionalServiceBlock != null && data.additionalServiceBlock.Count() != 0)
                {
                    additionalservice = getAdditionalService(setting, data.additionalServiceBlock);
                    if (string.IsNullOrEmpty(additionalservice)) return xml;
                }

                var xmlTemplate = JsonConvert.DeserializeObject(body).ToString();
                if (xmlTemplate != null)
                {
                    #region BTRT2 data binding

                    xml = string.Format(xmlTemplate,
                      data.uid,
                      data.storeId,
                      data.templateName,
                      data.instId,
                      data.agentId,
                      data.blockName,

                      data.mainBlock.ContractID,

                      data.customerBlock.CustomerId,

                      data.personBlock.PersonId,
                      data.personBlock.FristName,
                      data.personBlock.SurName,
                      data.personBlock.DateOfBirth,
                      data.personBlock.PersonProcessMode,

                      data.cardBlock.CardNumber,
                      data.cardBlock.CardType,

                      data.accountBlock.CardNumberfilter,
                      data.accountBlock.AccountNumber,
                      data.accountBlock.AccountType,
                      data.accountBlock.CurrencyCode,
                      additionalservice);
                    #endregion
                }
                return xml;
            }
            catch (Exception ex)
            {
                _logger.Error($"StoreBTRT35Service.GetBTRT35 =======> " +
                  $"Expection msg: {ex.Message}");
                return xml;
            }
        }
        private string getAdditionalService(ResponseServiceConfig setting, List<BTRT35_AdditionalServiceBlock> _list)
        {
            try
            {
                var additionalService1 = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocumentB1"]);
                var additionalService2 = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocumentB2"]);
                var B1Template = JsonConvert.DeserializeObject(additionalService1).ToString();
                var B2Template = JsonConvert.DeserializeObject(additionalService2).ToString();

                XDocument document1 = XDocument.Parse(B1Template);
                XDocument document2 = XDocument.Parse(B2Template);

                string add_xml1 = document1.ToString();
                string add_xml2 = document2.ToString();
                string result = string.Empty;

                foreach (var i in _list)
                {

                    if (i.creditServiceBlock != null)
                    {
                        var newadd_xml1 = add_xml1;
                        newadd_xml1 = string.Format(add_xml1,
                        i.ServiceID,
                        i.ServiceStartDate,
                        i.ServiceEndDate,
                        i.ServiceActionFlag,
                        i.creditServiceBlock.AccountExceedLimit);
                        result += newadd_xml1;
                    }
                    if (i.permanetOrderBlock != null)
                    {
                        var newadd_xml2 = add_xml2;
                        newadd_xml2 = string.Format(newadd_xml2,
                        i.ServiceStartDate,
                        i.ServiceEndDate,
                        i.ServiceActionFlag,
                        i.permanetOrderBlock.ParticipantAccount,
                        i.permanetOrderBlock.PaymentAmountformula,
                        i.permanetOrderBlock.StartExecutionDate);
                        result += newadd_xml2;
                    }

                }
                return result;
            } 
            catch (Exception ex)
            {
                _logger.Error($"StoreBTRT35Service.getAdditionalService =======> " +
                   $"Expection msg: {ex.Message}");
                return null;
            }
        }
    }
}
