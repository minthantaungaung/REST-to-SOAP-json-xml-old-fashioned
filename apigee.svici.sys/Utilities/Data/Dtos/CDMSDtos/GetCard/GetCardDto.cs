using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard
{
    public class GetCardRequestDto : Property
    {
        [Required]
        public string? CardNo { get; set; } = null!;
    }

    public class GetCardResponseDto : BaseRespModel
    {
        public CardData? Data { get; set; }
    }

    public class CardData
    {
        public List<GetCardResponse> Card { get; set; } = new List<GetCardResponse>();
    }
    public class GetCardResponse
    {
        public string? CardId { get; set; }
        public string? PlasticId { get; set; }
        public string? CardNumber { get; set; }
        public string? MemNumber { get; set; }
        public string? ExpirationDate { get; set; }
        public string? CardMask { get; set; }
        public string? NetworkId { get; set; }
        public string? NetworkDesc { get; set; }
        public string? CardStatus { get; set; }
        public string? CardStatusDesc { get; set; }
        public string? EmbossName { get; set; }
        public string? CardType { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? PrimaryCard { get; set; }
        public string? PrimAtmAcct { get; set; }
        public string? PrimPosAcct { get; set; }
        public string? IssueDate { get; set; }
        public string? PersonId { get; set; }
        public string? CustomerId { get; set; }
    }
}

namespace api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard.SVResp
{
    public class Card
    {
        public string? cardId { get; set; }
        public string? plasticId { get; set; }
        public string? cardNumber { get; set; }
        public string? memNumber { get; set; }
        public string? expirationDate { get; set; }
        public string? cardMask { get; set; }
        public string? networkId { get; set; }
        public string? networkDesc { get; set; }
        public string? cardStatus { get; set; }
        public string? cardStatusDesc { get; set; }
        public string? embossName { get; set; }
        public string? cardType { get; set; }
        public string? productId { get; set; }
        public string? productName { get; set; }
        public string? primAtmAcct { get; set; }
        public string? primPosAcct { get; set; }
        public string? issueDate { get; set; }
    }
    public class SVGetCardList
    {
        [JsonProperty("soap:Envelope")]
        public SoapEnvelope? soapEnvelope { get; set; }
    }
    public class GetCardsListResponse
    {
        [JsonProperty("@xmlns:ns3")]
        public string? xmlnsns3 { get; set; }

        [JsonProperty("@xmlns:ns2")]
        public string? xmlnsns2 { get; set; }

        [JsonProperty("@xmlns")]
        public string? xmlns { get; set; }

        [JsonProperty("ns2:status")]
        public string? ns2status { get; set; }

        [JsonProperty("ns2:errorCode")]
        public string? ns2errorCode { get; set; }

        [JsonProperty("ns2:errorDesc")]
        public string? ns2errorDesc { get; set; }
        public dynamic? card { get; set; }
    }

    public class SoapBody
    {
        public GetCardsListResponse? getCardsListResponse { get; set; }

        [JsonProperty("soap:Fault")]
        public SoapFault? soapFault { get; set; }
    }
    public class SoapFault
    {
        public string? faultcode { get; set; }
        public string? faultstring { get; set; }
    }
    public class SoapEnvelope
    {
        [JsonProperty("@xmlns:soap")]
        public string? xmlnssoap { get; set; }

        [JsonProperty("soap:Body")]
        public SoapBody? soapBody { get; set; }
    }
}
