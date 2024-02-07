﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;
using System;
using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.StoreDtos;

namespace app.api.Infrastructure.StoreService
{
    public class StoreBTRT3Service
    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext<StoreBTRT2Service>();
        public string GetBTRT3(ResponseServiceConfig setting, BTRT3_Dto data)
        {
            string xml = string.Empty;
            string additionalservice = string.Empty;
            try
            {
                var body = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocument"]);
                if (body is null) return xml;

                if (data.additionalServiceBlock != null && data.additionalServiceBlock.Count() != 0)
                {
                    additionalservice = getAdditionalService(setting, data.additionalServiceBlock);
                    if (string.IsNullOrEmpty(additionalservice)) return xml;
                }

                var xmlTemplate = JsonConvert.DeserializeObject(body).ToString();
                if (xmlTemplate != null)
                {
                    #region BTRT3 data binding
                    xml = string.Format(xmlTemplate,
                        data.uid,
                        data.storeId,
                        data.templateName,
                        data.instId,
                        data.agentId,
                        data.blockName,
                        //main block
                        data.mainBlock.ContractID,
                        data.mainBlock.BatchID,
                        //customer block
                        data.customerBlock.CustomerId,
                        //person block
                        data.personBlock.PersonId,
                        data.personBlock.FristName,
                        data.personBlock.SurName,
                        data.personBlock.DateOfBirth,
                        data.personBlock.PersonProcessMode,
                        //data.personBlock.SecurityQues,
                        //data.personBlock.SecurityID,
                        //data.personBlock.Gender,
                        //data.personBlock.MartialStatus,
                        //data.personBlock.PersonStyle,
                        //data.personBlock.TypeOfPersonId,
                        //data.personBlock.PersonNrcId,
                        //address block
                        data.addressBlock.AddressId,
                        data.addressBlock.AddressType,
                        data.addressBlock.AddressProcessMode,
                        data.addressBlock.AddressLineOne,
                        data.addressBlock.AddressLineTwo,
                        //data.addressBlock.Region,
                        data.addressBlock.PrimaryPhone,
                        data.addressBlock.SecondaryPhone,
                        data.addressBlock.MobilePhone,
                        data.addressBlock.Email,
                        //card block
                        data.cardBlock.CardNumber,
                        data.cardBlock.CardType,
                        data.cardBlock.EmbossedName,
                        data.cardBlock.CycleScheme,
                        data.cardBlock.LimitsScheme,
                        data.cardBlock.FeeScheme,
                        data.cardBlock.RegionList,
                        data.cardBlock.DeliveryInstructions,
                        data.cardBlock.DeliveryAgentCode,
                        //account block
                        data.accountBlock.AccountNumber,
                        //data.accountBlock.AccountType,
                        //data.accountBlock.CurrencyCode,
                       additionalservice);
                    #endregion
                }
                return xml;
            }
            catch (Exception ex)
            {
                _logger.Error($"StoreBTRT3Service.GetBTRT3 =======> " +
                   $"Expection msg: {ex.Message}");
                return xml;
            }
        }
        private string getAdditionalService(ResponseServiceConfig setting, List<BTRT3_AdditionalServiceBlock> _list)
        {
            try
            {
                var additionalService1 = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocumentB1"]);
                var B1Template = JsonConvert.DeserializeObject(additionalService1).ToString();
                XDocument document = XDocument.Parse(B1Template);

                var add_xml = document.ToString();
                string result = string.Empty;

                foreach (var i in _list)
                {
                    string tempTemplate = string.Empty;
                    XDocument tempDoc = document;
                    if (i.smsServiceBlock == null)
                    {
                        var q = from node in tempDoc.Descendants("block")
                                let attr = node.Attribute("name")
                                where attr != null && attr.Value == "SMS_SERVICE_BLOCK"
                                select node;
                        q.ToList().ForEach(x => x.Remove());
                        tempTemplate = tempDoc.ToString();

                        var newtempTemplate = tempTemplate;
                        newtempTemplate = string.Format(newtempTemplate,
                        i.ServiceID,
                        i.ServiceStartDate,
                        i.ServiceEndDate,
                        i.ServiceActionFlag);
                        result += newtempTemplate;
                    }
                    else
                    {
                        var newadd_xml = add_xml;
                        newadd_xml = string.Format(newadd_xml,
                        i.ServiceID,
                        i.ServiceStartDate,
                        i.ServiceEndDate,
                        i.ServiceActionFlag,
                        i.smsServiceBlock?.MobilePhone);
                        result += newadd_xml;
                    }
                }
                return result;
            }catch(Exception ex)
            {
                _logger.Error($"StoreBTRT3Service.getAdditionalService =======> " +
                   $"Expection msg: {ex.Message}");
                return null;
            }
        }
    }
}
