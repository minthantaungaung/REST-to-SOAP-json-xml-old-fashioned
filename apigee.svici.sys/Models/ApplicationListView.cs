using Microsoft.EntityFrameworkCore;

namespace api.svici.sys.Models
{
    public class ApplicationListView
    {
        public int APP_ID { get; set; }
        public string? BATCH_ID { get; set; }
        public string? BTRT_TYP { get; set; }
        public int? SIMPLE_ID { get; set; }
        public string? SIMP_NAME { get; set; }
        public string? CUR_APP_STATUS { get; set; }
        public string? CUR_APP_STATUS_DESC { get; set; }
        public string? ERR_MSG { get; set; }
        public DateTime? REG_DATE { get; set; }
        public DateTime? PROC_DATE { get; set; }
        public string? INST_ID { get; set; }
        public string? AGENT_ID { get; set; }
        public string? CONTRACT_ID { get; set; }
        public string? ID_TYPE { get; set; }
        public string? ID_NO { get; set; }
        public string? CUSTOMER_ID { get; set; }
        public string? PERSON_ID { get; set; }
        public string? PERSON_STYLE { get; set; }
        public string? FIRST_NAME { get; set; }
        public string? SUR_NAME { get; set; }
        public string? FATHER_NAME { get; set; }
        public string? EMERGENCY_CONTACT_NAME { get; set; }
        public string? EMERGENCY_CONTACT_NO { get; set; }
        public string? DOB { get; set; }
        public string? GENDER { get; set; }
        public string? MARITAL_STATUS { get; set; }
        public string? RESIDENCE { get; set; }
        public string? ADDRESS_ID { get; set; }
        public string? ADDRESS_TYPE { get; set; }
        public string? ADDRESS_LINE1 { get; set; }
        public string? ADDRESS_LINE2 { get; set; }
        public string? REGION { get; set; }
        public string? PRIMARY_PHONE { get; set; }
        public string? SECONDARY_PHONE { get; set; }
        public string? MOBILE_PHONE { get; set; }
        public string? EMAIL { get; set; }
        public string? CARD_TYPE { get; set; }
        public string? CARD_NUM { get; set; }
        public string? EMBOSSED_NAME { get; set; }
        public string? CARD_REGION { get; set; }
        public string? DELIVERY_AGENT_CODE { get; set; }
        public string? ACCT_NO { get; set; }
        public string? ACCT_TYPE { get; set; }
        public string? CREDIT_LIMIT { get; set; }
        public string? PAYMENT_FORMULA { get; set; }
        public string? AUTO_DEBIT_AC { get; set; }
    }
}
