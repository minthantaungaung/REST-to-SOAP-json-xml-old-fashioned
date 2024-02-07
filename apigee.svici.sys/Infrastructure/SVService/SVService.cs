using api.svici.sys.Utilities.Data.Constants;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using app.api.Infrastructure.StoreService;
using KBZLogger.Core;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Net;
using System.Security.Claims;
using System.ServiceModel;
using System.Xml.Linq;

namespace app.api.Infrastructure.SVService
{
    public class SVService
    {
        private static readonly Serilog.ILogger _logger = Log.ForContext<StoreBTRT1Service>();
        public string GetBTRT01(ResponseServiceConfig setting, storeAppBTRTModel model)
        {
            string xml = string.Empty;
            try
            {
                var body = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocument"]);
                if (body is null) return xml;

                var xmlTemplate = JsonConvert.DeserializeObject(body).ToString();

                #region BTRT1 data binding
                xml = string.Format(xmlTemplate,
                    model.uid, //0
                    model.storeId,
                    model.templateName,
                    model.instId,
                    model.agentId,
                    model.blockName,  //5
                                      // 6 -8  -- customer block
                    model.customerBlock.documentType,
                    model.customerBlock.nrcId,
                    model.customerBlock.customerId,
                    // 9-15  -- person block                                                      
                    model.personBlock.fristName,
                    model.personBlock.surName,
                    model.personBlock.dateOfBirth,
                    model.personBlock.personProcessMode,
                    model.personBlock.gender,
                    model.personBlock.typeOfPersonId,
                    model.personBlock.personNrcId,
                                   
                    model.addressBlock.addressType,
                    model.addressBlock.addressProcessMode,
                    model.addressBlock.addressLineOne,
                    model.addressBlock.addressLineTwo,
                    model.addressBlock.region,
                    model.addressBlock.primaryPhone,
                    model.addressBlock.secondaryPhone,
                    model.addressBlock.mobilePhone,
                    model.addressBlock.email,
                    
                    model.cardInitBlock.cardNumber,
                    model.cardInitBlock.embossName,
                    model.cardInitBlock.regionList,
                    
                    model.accountInitBlock.accountNumber,
                    model.accountInitBlock.accountType);
                #endregion
                return xml;
            }
            catch (Exception ex)
            {
                _logger.Error($"Debit StoreBTRT1Service.GetBTRT1 =======> " +
                    $"Exception msg: {ex.Message}");
                return xml;
            }
        }
        public string GetBTRT02(ResponseServiceConfig setting, storeAppBTRTModel model)
        {
            string xml = string.Empty;
            try
            {
                var body = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocument"]);
                if (body is null) return xml;

                var xmlTemplate = JsonConvert.DeserializeObject(body).ToString();

                #region BTRT2 data binding
                xml = string.Format(xmlTemplate,
                        model.uid, //0
                        model.storeId,
                        model.templateName,
                        model.instId,
                        model.agentId,
                        model.blockName,
                        model.customerBlock.customerId,
                        // 7 -14 person block
                        model.personBlock.personId,
                        model.personBlock.fristName,
                        model.personBlock.surName,
                        model.personBlock.dateOfBirth,
                        model.personBlock.personProcessMode,
                        model.personBlock.gender,
                        model.personBlock.typeOfPersonId,
                        model.personBlock.personNrcId,
                        // 15 customer block
                        model.addressBlock.addressId,
                        model.addressBlock.addressType,
                        model.addressBlock.addressProcessMode,
                        model.addressBlock.addressLineOne,
                        model.addressBlock.addressLineTwo,
                        model.addressBlock.region,
                        model.addressBlock.primaryPhone,
                        model.addressBlock.secondaryPhone,
                        model.addressBlock.mobilePhone,
                        model.addressBlock.email,
                        // 23 cardInit block
                        model.cardInitBlock.cardNumber,
                        model.cardInitBlock.embossName,
                        model.cardInitBlock.regionList,
                        // 26 account block
                        model.accountInitBlock.accountNumber,
                        model.accountInitBlock.accountType);
                #endregion
                return xml;
            }
            catch (Exception ex)
            {
                _logger.Error($"Debit StoreBTRT2Service.GetBTRT2 =======> " +
                    $"Exception msg: {ex.Message}");
                return xml;
            }
        }
        public string GetApplication(ResponseServiceConfig setting, processAppModel model)
        {
            string xml = string.Empty;
            try
            {
                var body = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocument"]);
                if (body is null) return xml;

                var xmlTemplate = JsonConvert.DeserializeObject(body).ToString();

                #region process application data binding
                xml = string.Format(xmlTemplate, model.uid, model.applicationId);
                #endregion
                return xml;
            }
            catch (Exception ex)
            {
                _logger.Error($"Debit GetApplicationService.GetApplication =======> " +
                    $"Exception msg: {ex.Message}");
                return xml;
            }
        }
        public string GetBTRT30(ResponseServiceConfig setting, UpdateAppBTRTModel model)
        {
            string xml = string.Empty;
            try
            {
                var body = JsonConvert.SerializeObject(JObject.Parse(setting.Config.RequestFormat)["XmlDocument"]);
                if (body is null) return xml;

                var xmlTemplate = JsonConvert.DeserializeObject(body).ToString();
                #region BTRT30 data binding
                xml = string.Format(xmlTemplate,
                        model.uid,
                        model.storeId,
                        model.templateName,
                        model.instId,
                        model.agentId,
                        model.blockName,
                        // 6 -9  -- customer block
                        model.customerBlock1.documentType,
                        model.customerBlock1.nrcId,
                        model.customerBlock1.documentProcessingMode,
                        model.customerBlock1.customerId,
                        // 10-22  -- person block - 14 fields
                        model.personBlock.personId,
                        model.personBlock.fristName,
                        model.personBlock.surName,
                        model.personBlock.dateOfBirth,
                        model.personBlock.typeOfPersonId,
                        model.personBlock.personNrcId,
                        model.personBlock.personProcessMode,
                        model.personBlock.newFristName,
                        model.personBlock.newSurName,
                        model.personBlock.newDateOfBirth,
                        model.personBlock.newGender,
                        model.personBlock.newMaritalStaus,
                        model.personBlock.newTypeOfPersonId,
                        model.personBlock.newPersonNrcId,
                        //23-32  --address block - 10 fields
                        model.addressBlock.addressId,
                        model.addressBlock.addressType,
                        model.addressBlock.addressProcessMode,
                        model.addressBlock.addressLineOne,
                        model.addressBlock.newAddressLineOne,
                        model.addressBlock.addressLineTwo,
                        model.addressBlock.primaryPhone,
                        model.addressBlock.secondaryPhone,
                        model.addressBlock.mobilePhone,
                        model.addressBlock.email
                    );
                #endregion
                return xml;
            }
            catch (Exception ex)
            {
                _logger.Error($"Debit GetBTRT30Service.GetApplication =======> " +
                    $"Exception msg: {ex.Message}");
                return xml;
            }
        }

        public string GetAuthDt(string xmlTemplate, SVWIModel model)
        {
            //XMLDocuments xml = JsonConvert.DeserializeObject<XMLDocuments>(RequestFormat);
            string xml = string.Empty;
            if (xmlTemplate != null)
            {
                xml = string.Format(xmlTemplate, model.login, model.pwd);
            }
            return xml;
        }
    }
}
