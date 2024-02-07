
using System;

namespace api.svici.sys.Utilities.Data.Dtos.StoreDtos;

public class BTRT35_Dto
{
    public string? uid { get; set; }
    public string? storeId { get; set; }
    public string? templateName { get; set; }
    public string? instId { get; set; }
    public string? agentId { get; set; }
    public string? blockName { get; set; }
    public BTRT35_MainBlock mainBlock { get; set; }
    public BTRT35_CustomerBlock customerBlock { get; set; }
    public BTRT35_PersonBlock personBlock { get; set; }
    public BTRT35_CardBlock cardBlock { get; set; }
    public BTRT35_AccountBlock accountBlock { get; set; }
    public List<BTRT35_AdditionalServiceBlock> additionalServiceBlock { get; set; }
}

public class BTRT35_MainBlock
{
    public string? ContractID { get; set; }
    //public string? BatchID { get; set; }
}

public class BTRT35_CustomerBlock
{
    //public string? DocumentType { get; set; }
    //public string? Number { get; set; }
    public string? CustomerId { get; set; }
}
public class BTRT35_PersonBlock
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


public class BTRT35_CardBlock
{
    public string? CardNumber { get; set; }
    public string? CardType { get; set; }
}
public class BTRT35_AccountBlock
{
    public string? CardNumberfilter { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountType { get; set; }
    public string? CurrencyCode { get; set; }
}

public class BTRT35_AdditionalServiceBlock
{
    public string? ServiceID { get; set; }
    public string? ServiceStartDate { get; set; }
    public string? ServiceEndDate { get; set; }
    public string? ServiceActionFlag { get; set; }
    public CREDIT_SERVICE_BLOCK? creditServiceBlock { get; set; }
    public PERMANENT_ORDER? permanetOrderBlock { get; set; }
}

public class CREDIT_SERVICE_BLOCK
{
    public string? AccountExceedLimit { get; set; }
}
public class PERMANENT_ORDER
{
    public string? ParticipantAccount { get; set; }
    public string? PaymentAmountformula { get; set; }
    public string? StartExecutionDate { get; set; }
}
