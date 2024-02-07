using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard;
using api.svici.sys.Utilities.Data.Dtos.CDMSDtos.GetCard.SVResp;
using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using app.api.Infrastructure.StoreService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;

namespace api.svici.sys.Infrastructure.CdmsSvService.ResponseService
{
    public class CdmsSvResponseService : ICdmsSvResponseService
    {
        private static readonly Serilog.ILogger _logger = Serilog.Log.ForContext<StoreBTRT2Service>();
        public storeAppFinalRespModel GetErrorResp(XDocument doc, string LogID)
        {
            storeAppFinalRespModel response = new();
            List<BaseRespErrorDetail> errorDetail = new();
            string errorCode = string.Empty, errorMessage = string.Empty;

            try
            {
                var errCodeQuery = from node in doc.Descendants("faultcode") select node;
                var errMsgQuery = from node in doc.Descendants("faultstring") select node;

                if (errCodeQuery is not null && errCodeQuery.ToList().Count() != 0)
                    errorCode = errCodeQuery.FirstOrDefault()!.Value;

                if (errCodeQuery is not null && errMsgQuery.ToList().Count() != 0)
                {
                    foreach (var item in errMsgQuery.ToList())
                    {
                        errorDetail.Add(new BaseRespErrorDetail
                        {
                            ErrorCode = errorCode,
                            ErrorDescription = item.Value.ToString() ?? "",
                        });
                    }
                }
                else
                {
                    errorDetail.Add(new BaseRespErrorDetail
                    {
                        ErrorCode = "TAGSYS",
                        ErrorDescription = "Indicate unknown exception from SV !"
                    });
                }
            }
            catch (Exception ex)
            {
                errorDetail.Add(new BaseRespErrorDetail
                {
                    ErrorCode = "SVICI_REF",
                    ErrorDescription = ex.Message
                });
            }
            finally
            {
                response.KBZRefNo = LogID;
                response.Error = ErrorCodeModel.ClientRespError;
                response.Error.Details = errorDetail;
            }
            return response;
        }
        public AuthFinalRespModel GetAuthErrorResp(XDocument doc, string LogID)
        {
            AuthFinalRespModel response = new();
            List<BaseRespErrorDetail> errorDetail = new();
            string errorCode = string.Empty, errorMessage = string.Empty;
            XNamespace ns2 = "http://bpc.ru/common/types/v0.1/";
            try
            {
                var errCodeQuery = from node in doc.Descendants("faultcode") select node;
                var errMsgQuery = from node in doc.Descendants("faultstring") select node;

                if (errCodeQuery is not null && errCodeQuery.ToList().Count() != 0)
                    errorCode = errCodeQuery.FirstOrDefault()!.Value;

                if (errCodeQuery is not null && errMsgQuery.ToList().Count() != 0)
                {
                    foreach (var item in errMsgQuery.ToList())
                    {
                        errorDetail.Add(new BaseRespErrorDetail
                        {
                            ErrorCode = errorCode,
                            ErrorDescription = item.Value.ToString() ?? "",
                        });
                    }
                }
                else
                {
                    errorDetail.Add(new BaseRespErrorDetail
                    {
                        ErrorCode = "TAGSYS",
                        ErrorDescription = "Indicate unknown exception from SV !"
                    });
                }
            }
            catch (Exception ex)
            {
                errorDetail.Add(new BaseRespErrorDetail
                {
                    ErrorCode = "SVICI_REF",
                    ErrorDescription = ex.Message
                });
            }
            finally
            {
                response.KBZRefNo = LogID;
                response.Error = ErrorCodeModel.ClientRespError;
                response.Error.Details = errorDetail;
            }
            return response;
        }
        public AuthFinalRespModel GetAuthErrorResp(Exception ex, string LogID)
        {
            AuthFinalRespModel response = new();
            List<BaseRespErrorDetail> errorDetail = new()
            {
                new BaseRespErrorDetail
                {
                    ErrorCode = "SVICI_REF",
                    ErrorDescription = ex.Message
                }
            };

            response.KBZRefNo = LogID;
            response.Error = ErrorCodeModel.ClientRespError;
            response.Error.Details = errorDetail;

            return response;
        }
        public storeAppFinalRespModel GetErrorResp(string LogID)
        {
            storeAppFinalRespModel response = new();
            List<BaseRespErrorDetail> errorDetail = new()
            {
                new BaseRespErrorDetail
                {
                    ErrorCode = "TAGSYS",
                    ErrorDescription = "Indicate unknown exception from SV !"
                }
            };

            response.KBZRefNo = LogID;
            response.Error = ErrorCodeModel.ClientRespError;
            response.Error.Details = errorDetail;

            return response;
        }
        public storeAppFinalRespModel GetErrorResp(Exception ex, string LogID)
        {
            storeAppFinalRespModel response = new();
            List<BaseRespErrorDetail> errorDetail = new()
            {
                new BaseRespErrorDetail
                {
                    ErrorCode = "SVICI_REF",
                    ErrorDescription = ex.Message
                }
            };

            response.KBZRefNo = LogID;
            response.Error = ErrorCodeModel.ClientRespError;
            response.Error.Details = errorDetail;

            return response;
        }

        public (int statusCode, GetCardResponseDto GetCardResponse) GetCardResponse(string xmlresp)
        {
            GetCardResponseDto response = new() {Data = new CardData() { } };
            response.Data = new();
            XmlDocument doc = new XmlDocument();
            int statusCode = 0;

            doc.LoadXml(xmlresp);
            var json = JsonConvert.SerializeXmlNode(doc);
            var card = JsonConvert.DeserializeObject<SVGetCardList>(json);

            var soapBody = card!.soapEnvelope!.soapBody;
            var getCardList = card.soapEnvelope.soapBody!.getCardsListResponse;
            var allcards = getCardList?.card;
            List<Card> allCardsList = new();

            if (soapBody!.soapFault is not null && getCardList?.card is null)
            {
                statusCode = StatusCodes.Status500InternalServerError;
                response.Error = ErrorCodeModel.ClientRespError;
                response.Error.Details.Add(new BaseRespErrorDetail()
                {
                    ErrorCode = soapBody.soapFault.faultcode,
                    ErrorDescription = soapBody.soapFault.faultstring
                });
            }
            else if (allcards is not null && getCardList.ns2status == "0" && getCardList.ns2errorCode == "SUCCESS")
            {
                if (allcards.GetType() == typeof(JObject))
                {
                    allCardsList!.Add(allcards.ToObject<Card>());
                }
                else if (allcards.GetType() == typeof(JArray))
                {
                    foreach (JObject jObject in allcards)
                    {
                        Card? dt = jObject.ToObject<Card>();
                        allCardsList.Add(dt ?? new());
                    }
                }
                statusCode = StatusCodes.Status200OK;
                var i = allCardsList.OrderByDescending(x => x.memNumber).FirstOrDefault();
                    GetCardResponse data = new()
                    {
                        CardId = i.cardId,
                        PlasticId = i.plasticId,
                        CardNumber = i.cardNumber,
                        MemNumber = i.memNumber,
                        ExpirationDate = i.expirationDate,
                        CardMask = i.cardMask,
                        NetworkId = i.networkId,
                        NetworkDesc = i.networkDesc,
                        CardStatus = i.cardStatus,
                        CardStatusDesc = i.cardStatusDesc,
                        CardType = i.cardType,
                        EmbossName = i.embossName,
                        ProductId = i.productId,
                        ProductName = i.productName,
                        PrimAtmAcct = i.primAtmAcct,
                        PrimPosAcct = i.primPosAcct,
                        IssueDate = i.issueDate
                    };
                response.Data.Card.Add(data);
            }
            else
            {
                statusCode = StatusCodes.Status400BadRequest;
                response.Error = ErrorCodeModel.ClientRespError;
                response.Error.Details.Add(new BaseRespErrorDetail()
                {
                    ErrorCode = soapBody.getCardsListResponse!.ns2errorCode,
                    ErrorDescription = soapBody.getCardsListResponse.ns2errorDesc
                });
            }
            return (statusCode, response);
        }
    }
}
