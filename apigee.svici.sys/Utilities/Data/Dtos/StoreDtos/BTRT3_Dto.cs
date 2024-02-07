namespace api.svici.sys.Utilities.Data.Dtos.StoreDtos;

public class BTRT3_Dto
{
    public string? uid { get; set; }
    public string? storeId { get; set; }
    public string? templateName { get; set; }
    public string? instId { get; set; }
    public string? agentId { get; set; }
    public string? blockName { get; set; }
    public BTRT3_MainBlock mainBlock { get; set; }
    public BTRT3_CustomerBlock customerBlock { get; set; }
    public BTRT3_PersonBlock personBlock { get; set; }
    public BTRT3_AddressBlock addressBlock { get; set; }
    public BTRT3_CardBlock cardBlock { get; set; }
    public BTRT3_AccountBlock accountBlock { get; set; }
    public List<BTRT3_AdditionalServiceBlock> additionalServiceBlock { get; set; }
}

public class BTRT3_MainBlock
{
    public string? ContractID { get; set; }
    public string? BatchID { get; set; }
}

public class BTRT3_CustomerBlock
{
    //public string? DocumentType { get; set; }
    //public string? Number { get; set; }
    public string? CustomerId { get; set; }
}
public class BTRT3_PersonBlock
{
    public string? PersonId { get; set; }
    public string? FristName { get; set; }
    public string? SurName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? PersonProcessMode { get; set; }
    //public string? SecurityQues { get; set; }
    //public string? SecurityID { get; set; }
    //public string? Gender { get; set; }
    //public string? MartialStatus { get; set; }
    //public string? PersonStyle { get; set; }
    //public string? TypeOfPersonId { get; set; }
    //public string? PersonNrcId { get; set; }

}

public class BTRT3_AddressBlock
{
    public string? AddressId { get; set; }
    public string? AddressType { get; set; }
    public string? AddressProcessMode { get; set; }
    public string? AddressLineOne { get; set; }
    public string? AddressLineTwo { get; set; }
    //public string? Region { get; set; }
    public string? PrimaryPhone { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Email { get; set; }
}

public class BTRT3_CardBlock
{
    public string? CardNumber { get; set; }
    public string? CardType { get; set; }
    public string? EmbossedName { get; set; }
    public string? CycleScheme { get; set; }
    public string? LimitsScheme { get; set; }
    public string? FeeScheme { get; set; }
    public string? RegionList { get; set; }
    public string? DeliveryInstructions { get; set; }
    public string? DeliveryAgentCode { get; set; }
}
public class BTRT3_AccountBlock
{
    public string? AccountNumber { get; set; }
    //public string? AccountType { get; set; }
    //public string? CurrencyCode { get; set; }
}

public class BTRT3_AdditionalServiceBlock
{
    public string? ServiceID { get; set; }
    public string? ServiceStartDate { get; set; }
    public string? ServiceEndDate { get; set; }
    public string? ServiceActionFlag { get; set; }
    public BTRT3_SMSServiceBlock? smsServiceBlock { get; set; }
}

public class BTRT3_SMSServiceBlock
{
    public string? MobilePhone { get; set; }
}
