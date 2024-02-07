using api.svici.sys.Utilities.Data.Dtos.ResponseModels;
using api.svici.sys.Utilities.Data.Dtos.SVDtos;
using app.api.Controllers;
using app.api.Infrastructure.StoreService;
using System.Xml.Linq;

namespace api.svici.sys.Infrastructure.StoreService
{
    public class StoreResponseService
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

        public storeAppFinalRespModel GetErrorResp(Exception ex,string LogID)
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
    }
}
