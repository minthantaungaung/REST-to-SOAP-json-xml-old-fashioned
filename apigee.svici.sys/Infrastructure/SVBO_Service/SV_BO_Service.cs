using System.Xml;
using System.Xml.Linq;
using api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos;
using api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos.SV;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using Newtonsoft.Json;

namespace api.svici.sys.Infrastructure.SVBO_Service
{
    public class SV_BO_Service : ISV_BO_Service
    {
        public string GetCWCardListXML(CWContactInfoRequestDto req)
        {
            string path = Path.Combine("Utilities/Data/RequestTemplates/SVBO/CW_Profile_Enquiry.xml");
            XDocument document = XDocument.Load(path);
            string xmlTemplate = document.ToString();

            string xml = string.Empty;
            if (xmlTemplate != null)
            {
                xml = string.Format(xmlTemplate, req.cardNumber, req.sequenceNo);
            }
            return xml;
        }

        public async Task<QueryContactInfoCW?> GetCardListResponse(string resp)
        {
            XDocument doc = XDocument.Parse(resp);
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(doc.ToString());
            var json = JsonConvert.SerializeXmlNode(doc2);
            var model = JsonConvert.DeserializeObject<CWGetCard>(json);
            var info = model?.soapenvEnvelope.soapenvBody.multiRef;
            QueryContactInfoCW data = new();
            data.mobileNo = info?.mobileNo?.text;
            data.mailingTel = info?.mailingTel?.text;
            data.residenceTelNo = info?.residenceTel?.text;
            data.emailAddress = info?.emailAddress?.text;
            data.panNo = info?.pan?.text;               
            return data;
        }

        public string MockResponse()
        {
            string path = Path.Combine("Utilities/Data/RequestTemplates/SVBO/MockResponse.xml");
            XDocument document = XDocument.Load(path);
            string xmlTemplate = document.ToString();
            return xmlTemplate;
        }
    }
}
