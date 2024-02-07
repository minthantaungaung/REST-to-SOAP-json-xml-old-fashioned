namespace api.svici.sys.Utilities.Data.Dtos.StoreDtos;

public class BTRT2_Dto
{
    public string? uid { get; set; }
    public string? storeId { get; set; }
    public string? templateName { get; set; }
    public string? instId { get; set; }
    public string? agentId { get; set; }
    public string? blockName { get; set; }
    public BTRT2_MainBlock mainBlock { get; set; }
    public BTRT2_CustomerBlock customerBlock { get; set; }
    public BTRT2_PersonBlock personBlock { get; set; }
    public BTRT2_AddressBlock addressBlock { get; set; }
    public BTRT2_CardBlock cardBlock { get; set; }
    public BTRT2_AccountBlock accountBlock { get; set; }
    public List<BTRT2_AdditionalServiceBlock> additionalServiceBlock { get; set; }
}

public class BTRT2_MainBlock
{
    public string? ContractID { get; set; }
    public string? BatchID { get; set; }
}

public class BTRT2_CustomerBlock
{
    //public string? DocumentType { get; set; }
    //public string? Number { get; set; }
    public string? CustomerId { get; set; }
}
public class BTRT2_PersonBlock
{
    public string? PersonId { get; set; }
    public string? FristName { get; set; }
    public string? SurName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNo { get; set; }
    public string? PersonProcessMode { get; set; }
    public string? SecurityQues { get; set; }
    public string? SecurityID { get; set; }
    public string? Gender { get; set; }
    public string? MartialStatus { get; set; }
    public string? Residence { get; set; }
    public string? EmployedFlag { get; set; }
    public string? PersonStyle { get; set; }
    public string? TypeOfPersonId { get; set; }
    public string? PersonNrcId { get; set; }

}

public class BTRT2_AddressBlock
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

public class BTRT2_CardBlock
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
public class BTRT2_AccountBlock
{
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public string? CurrencyCode { get; set; }
}

public class BTRT2_AdditionalServiceBlock
{
    public string? ServiceID { get; set; }
    public string? ServiceStartDate { get; set; }
    public string? ServiceEndDate { get; set; }
    public string? ServiceActionFlag { get; set; }
    public BTRT2_SMSServiceBlock? smsServiceBlock { get; set; }
}

public class BTRT2_SMSServiceBlock
{
    public string? MobilePhone { get; set; }
}
