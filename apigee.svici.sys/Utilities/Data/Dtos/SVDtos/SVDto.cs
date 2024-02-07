using api.svici.sys.Utilities.Data.Dtos.ResponseModels;

namespace api.svici.sys.Utilities.Data.Dtos.SVDtos
{
    public class storeAppFinalRespModel : BaseRespModel
    {
        public storeAppRespModel Data { get; set; }
    }
    public class storeAppRespModel
    {
        public string? applicationId { get; set; }

        //public IList<ResponseModel> Response { get; set; }
    }
    public class storeAppBTRTModel
    {
        public string? uid { get; set; }
        public string? storeId { get; set; }
        public string? templateName { get; set; }
        public string? instId { get; set; }
        public string? agentId { get; set; }
        public string? blockName { get; set; }
        public customerBlock01? customerBlock { get; set; }
        //  public mainBlock01 mainBlock { get; set; }
        public personBlock01? personBlock { get; set; }
        public addressBlock01? addressBlock { get; set; }
        public cardInitBlock01? cardInitBlock { get; set; }
        public accountInitBlock01? accountInitBlock { get; set; }
    }

    public class customerBlock01
    {
        public string? customerId { get; set; }
        public string? nrcId { get; set; }
        public string? documentType { get; set; }
    }

    //public class mainBlock01
    //{
    //    public string? contractId { get; set; }
    //}
    public class SVWIModel
    {
        public string? login { get; set; }
        public string? pwd { get; set; }
    }
    public class personBlock01
    {
        public string? personId { get; set; }
        public string? fristName { get; set; }
        public string? surName { get; set; }
        public string? dateOfBirth { get; set; }
        public string? personProcessMode { get; set; }
        public string? gender { get; set; }
        public string? typeOfPersonId { get; set; }
        public string? personNrcId { get; set; }

    }

    public class addressBlock01
    {
        public string? addressId { get; set; }
        public string? addressType { get; set; }
        public string? addressProcessMode { get; set; }
        public string? addressLineOne { get; set; }
        public string? addressLineTwo { get; set; }
        public string? region { get; set; }
        public string? primaryPhone { get; set; }
        public string? secondaryPhone { get; set; }
        public string? mobilePhone { get; set; }
        public string? email { get; set; }
    }

    public class cardInitBlock01
    {
        public string? cardNumber { get; set; }
        public string? embossName { get; set; }
        public string? regionList { get; set; }
    }

    public class accountInitBlock01
    {
        public string? accountNumber { get; set; }
        public string? accountType { get; set; }
    }

    public class processAppModel
    {
        public string uid { get; set; }
        public string applicationId { get; set; }
        public bool applicationIdSpecified { get; set; }
    }

    #region Update Customer Info (BTRT30)
    public class CustomerBlock02
    {
        public string? documentType { get; set; }
        public string? nrcId { get; set; }
        public string? documentProcessingMode { get; set; }
        public string? customerId { get; set; }
    }

    public class PersonBlock02
    {
        public string? personId { get; set; }
        public string? fristName { get; set; }
        public string? surName { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string? typeOfPersonId { get; set; }
        public string? personNrcId { get; set; }
        public string? personProcessMode { get; set; }
        public string? newFristName { get; set; }
        public string? newSurName { get; set; }
        public DateTime? newDateOfBirth { get; set; }
        public string? newGender { get; set; }
        public string? newMaritalStaus { get; set; }
        public string? newTypeOfPersonId { get; set; }
        public string? newPersonNrcId { get; set; }
    }

    public class AddressBlock02
    {
        public string? addressId { get; set; }
        public string? addressType { get; set; }
        public string? addressProcessMode { get; set; }
        public string? addressLineOne { get; set; }
        public string? newAddressLineOne { get; set; }
        public string? addressLineTwo { get; set; }
        public string? primaryPhone { get; set; }
        public string? secondaryPhone { get; set; }
        public string? mobilePhone { get; set; }
        public string? email { get; set; }
    }

    public class UpdateAppBTRTModel
    {
        public string? uid { get; set; }
        public string? storeId { get; set; }
        public string? templateName { get; set; }
        public string? instId { get; set; }
        public string? agentId { get; set; }
        public string? blockName { get; set; }
        public CustomerBlock02? customerBlock1 { get; set; }
        public PersonBlock02? personBlock { get; set; }
        public AddressBlock02? addressBlock { get; set; }
    }
    #endregion
}
