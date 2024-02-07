using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using Newtonsoft.Json;

namespace api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos
{
    public class QueryContactInfoCW : Property
    {
        public string? mailingTel { get; set; }
        public string? mobileNo { get; set; }
        public string? residenceTelNo { get; set; }
        public string? emailAddress { get; set; }
        public string? embossName { get; set; }
        public string? panNo { get; set; }
    }

    public class CWContactInfoResponseDto : BaseRespModel
    {
        public QueryContactInfoCW? Data { get; set; }
    }

    public class CWContactInfoRequestDto : Property
    {
        public string? cardNumber { get; set; }
        public string? sequenceNo { get; set; }
    }
}


namespace api.svici.sys.Utilities.Data.Dtos.SV_BO_Dtos.SV
{
    public class MultiRef
    {
        [JsonProperty("@id")]
        public string? id { get; set; }

        [JsonProperty("@soapenc:root")]
        public string? soapencroot { get; set; }

        [JsonProperty("@soapenv:encodingStyle")]
        public string? soapenvencodingStyle { get; set; }

        [JsonProperty("@xsi:type")]
        public string? xsitype { get; set; }

        [JsonProperty("@xmlns:soapenc")]
        public string? xmlnssoapenc { get; set; }

        [JsonProperty("@xmlns:ns2")]
        public string? xmlnsns2 { get; set; }
        public AlternateEmail? alternateEmail { get; set; }
        public AnnualIncome? annualIncome { get; set; }
        public BigID? bigID { get; set; }
        public BillingOption? billingOption { get; set; }
        public Crn? crn { get; set; }
        public Dob? dob { get; set; }
        public EmailAddress? emailAddress { get; set; }
        public EmbossName? embossName { get; set; }
        public EmerContcPers? emerContcPers { get; set; }
        public EmerMobile? emerMobile { get; set; }
        public EmerRelation? emerRelation { get; set; }
        public EmerTel? emerTel { get; set; }
        public Fi? fi { get; set; }
        public Gender? gender { get; set; }
        public ImmigVisaExpDate? immigVisaExpDate { get; set; }
        public IndustryCode? industryCode { get; set; }
        public LastUpdateTMS? lastUpdateTMS { get; set; }
        public MailingAddress1? mailingAddress1 { get; set; }
        public MailingAddress2? mailingAddress2 { get; set; }
        public MailingAddress3? mailingAddress3 { get; set; }
        public MailingAddress4? mailingAddress4 { get; set; }
        public MailingCountry? mailingCountry { get; set; }
        public MailingPostcode? mailingPostcode { get; set; }
        public MailingState? mailingState { get; set; }
        public MailingTel? mailingTel { get; set; }
        public MailingTown? mailingTown { get; set; }
        public MaritalStatus? maritalStatus { get; set; }
        public MinisterialDiplomatic? ministerialDiplomatic { get; set; }
        public MobileNo? mobileNo { get; set; }
        public MotherMaidenName? motherMaidenName { get; set; }
        public Name? name { get; set; }
        public Nationality? nationality { get; set; }
        public NatureOfBusiness? natureOfBusiness { get; set; }
        public NewID? newID { get; set; }
        public NewIdIndicator? newIdIndicator { get; set; }
        public Occupation? occupation { get; set; }
        public OfficeAddress1? officeAddress1 { get; set; }
        public OfficeAddress2? officeAddress2 { get; set; }
        public OfficeAddress3? officeAddress3 { get; set; }
        public OfficeAddress4? officeAddress4 { get; set; }
        public OfficeCountry? officeCountry { get; set; }
        public OfficeFax? officeFax { get; set; }
        public OfficeName? officeName { get; set; }
        public OfficePostcode? officePostcode { get; set; }
        public OfficeState? officeState { get; set; }
        public OfficeTel1? officeTel1 { get; set; }
        public OfficeTel2? officeTel2 { get; set; }
        public OfficeTown? officeTown { get; set; }
        public OldIC? oldIC { get; set; }
        public Pan? pan { get; set; }
        public PlaceOfBirth? placeOfBirth { get; set; }
        public PositionHeld? positionHeld { get; set; }
        public PrStatus? prStatus { get; set; }
        public PreferredLanguage? preferredLanguage { get; set; }
        public PrevEmpAddress1? prevEmpAddress1 { get; set; }
        public PrevEmpAddress2? prevEmpAddress2 { get; set; }
        public PrevEmpAddress3? prevEmpAddress3 { get; set; }
        public PrevEmpAddress4? prevEmpAddress4 { get; set; }
        public PrevEmpCountry? prevEmpCountry { get; set; }
        public PrevEmpName? prevEmpName { get; set; }
        public PrevEmpPostcode? prevEmpPostcode { get; set; }
        public PrevEmpTel? prevEmpTel { get; set; }
        public PrevEmpTown? prevEmpTown { get; set; }
        public PrevEmpYear? prevEmpYear { get; set; }
        public Race? race { get; set; }
        public ResidenceAddress1? residenceAddress1 { get; set; }
        public ResidenceAddress2? residenceAddress2 { get; set; }
        public ResidenceAddress3? residenceAddress3 { get; set; }
        public ResidenceAddress4? residenceAddress4 { get; set; }
        public ResidenceCountry? residenceCountry { get; set; }
        public ResidencePostcode? residencePostcode { get; set; }
        public ResidenceSince? residenceSince { get; set; }
        public ResidenceState? residenceState { get; set; }
        public ResidenceTel? residenceTel { get; set; }
        public ResidenceTown? residenceTown { get; set; }
        public ResidenceType? residenceType { get; set; }
        public ResponseCode? responseCode { get; set; }
        public ResponseDescription? responseDescription { get; set; }
        public SequenceNo? sequenceNo { get; set; }
        public SpouseAddress1? spouseAddress1 { get; set; }
        public SpouseAddress2? spouseAddress2 { get; set; }
        public SpouseAddress3? spouseAddress3 { get; set; }
        public SpouseAddress4? spouseAddress4 { get; set; }
        public SpouseCountry? spouseCountry { get; set; }
        public SpouseId? spouseId { get; set; }
        public SpouseName? spouseName { get; set; }
        public SpousePostcode? spousePostcode { get; set; }
        public SpouseTel? spouseTel { get; set; }
        public SpouseTown? spouseTown { get; set; }
        public Title? title { get; set; }
        public WorkNature? workNature { get; set; }
        public AltEmailAddress? altEmailAddress { get; set; }
        public HmeTel? hmeTel { get; set; }
        public OfficeTelExt1? officeTelExt1 { get; set; }
    }

    public class AltEmailAddress
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class AlternateEmail
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class AnnualIncome
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class BigID
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class BillingOption
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Crn
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class CustProfileEnquiryReturn
    {
        [JsonProperty("@href")]
        public string href { get; set; }
    }

    public class Dob
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class EmailAddress
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class EmbossName
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class EmerContcPers
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class EmerMobile
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class EmerRelation
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class EmerTel
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class Fi
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Gender
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class HmeTel
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ImmigVisaExpDate
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class IndustryCode
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class LastUpdateTMS
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MailingAddress1
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MailingAddress2
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class MailingAddress3
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class MailingAddress4
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class MailingCountry
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MailingPostcode
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MailingState
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MailingTel
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MailingTown
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MaritalStatus
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MinisterialDiplomatic
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class MobileNo
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class MotherMaidenName
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }
    public class Name
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Nationality
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class NatureOfBusiness
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class NewID
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class NewIdIndicator
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Ns1CustProfileEnquiryResponse
    {
        [JsonProperty("@soapenv:encodingStyle")]
        public string soapenvencodingStyle { get; set; }

        [JsonProperty("@xmlns:ns1")]
        public string xmlnsns1 { get; set; }
        public CustProfileEnquiryReturn? custProfileEnquiryReturn { get; set; }
    }

    public class Occupation
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeAddress1
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class OfficeAddress2
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeAddress3
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeAddress4
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeCountry
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class OfficeFax
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeName
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class OfficePostcode
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class OfficeState
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class OfficeTel1
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeTel2
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeTelExt1
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class OfficeTown
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class OldIC
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class Pan
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class PlaceOfBirth
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PositionHeld
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PreferredLanguage
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }
    }

    public class PrevEmpAddress1
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpAddress2
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpAddress3
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpAddress4
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpCountry
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpName
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpPostcode
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpTel
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpTown
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrevEmpYear
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class PrStatus
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class Race
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceAddress1
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceAddress2
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceAddress3
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceAddress4
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceCountry
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidencePostcode
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceSince
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceState
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceTel
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class ResidenceTown
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResidenceType
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class ResponseCode
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class ResponseDescription
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class CWGetCard
    {
        [JsonProperty("soapenv:Envelope")]
        public SoapenvEnvelope soapenvEnvelope { get; set; }
    }

    public class SequenceNo
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class SoapenvBody
    {
        [JsonProperty("ns1:custProfileEnquiryResponse")]
        public Ns1CustProfileEnquiryResponse ns1custProfileEnquiryResponse { get; set; }
        public MultiRef? multiRef { get; set; }
    }

    public class SoapenvEnvelope
    {
        [JsonProperty("@xmlns:soapenv")]
        public string xmlnssoapenv { get; set; }

        [JsonProperty("@xmlns:xsd")]
        public string xmlnsxsd { get; set; }

        [JsonProperty("@xmlns:xsi")]
        public string xmlnsxsi { get; set; }

        [JsonProperty("soapenv:Body")]
        public SoapenvBody soapenvBody { get; set; }
    }

    public class SpouseAddress1
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseAddress2
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseAddress3
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseAddress4
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseCountry
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseId
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseName
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpousePostcode
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseTel
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class SpouseTown
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class Title
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("@xsi:nil")]
        public string xsinil { get; set; }
    }

    public class WorkNature
    {
        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }
}