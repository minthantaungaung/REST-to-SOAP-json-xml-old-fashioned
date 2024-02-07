using api.svici.sys.Utilities.Data.Dtos.ResponseModels;

namespace api.svici.sys.Utilities.Data.Dtos.SV_FE_Dtos
{
    public class QueryCardTranxResponseDto : BaseRespModel
    {
        public List<CardTranxList>? Data { get; set; }
    }

    public class CardTranxList
    {
        public string? CARD_NUM { get; set; }
        public string? ACC_NUM { get; set; }
        public string? TRN_DATE { get; set; }
        public string? TIME { get; set; }
        public string? TERMINAL { get; set; }
        public string? TERMINAL_NAME { get; set; }
        public decimal? TRAN_TYPE { get; set; }
        public string? TRAN_DESC { get; set; }
        public decimal? AMMOUNT { get; set; }
        public decimal? CURRENCY { get; set; }
        public string? CUR_DESP { get; set; }
        public decimal? FEE { get; set; }
        public decimal? CONVERTED_AMT { get; set; }
        public decimal? RESP_CODE { get; set; }
        public string? RESP_DESC { get; set; }
        public decimal? REVERSAL { get; set; }
        public decimal? MSGTYPE { get; set; }
        public decimal? UTRNNO { get; set; }
        public decimal? ATM_RESP { get; set; }
        public string? ATM_RESP_DESC { get; set; }
        public string? ISS_AUTH_ID { get; set; }
        public string? ACQ_AUTH_ID { get; set; }
        public string? ISS_INST { get; set; }
        public string? ISS_INSTITUTION { get; set; }
        public string? ACQ_INST { get; set; }
        public string? ACQ_INSTITUTION { get; set; }
        public string? FROMDATE { get; set; }
        public string? TODATE { get; set; }
    }

    public class QueryCardTranxRequestDto : Property
    {
        public string? CardNumber { get; set; }
        public string? AccNumber { get; set; }
        public string? FromDate { get; set; } = null!;
        public string? ToDate { get; set; } = null!;
    }
}
