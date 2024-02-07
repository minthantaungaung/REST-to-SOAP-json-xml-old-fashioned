namespace api.svici.sys.Utilities.Data.Dtos.ResponseModels
{
    public static class ReturnMessage
    {
        #region Properties

        public static Error AppUnauthorizedErr
        {
            get { return new Error { Code = "1020", Message = "Application Unauthorized." }; }
        }

        public static Error DatabaseErr
        {
            get { return new Error { Code = "1015", Message = "Database error!" }; }
        }

        public static Error DuplicateErr
        {
            get { return new Error { Code = "1011", Message = "Duplicate data!" }; }
        }

        public static Error InvalidDataErr
        {
            get { return new Error { Code = "1007", Message = "Invalid Data!" }; }
        }

        public static Error InvalidRequest
        {
            get { return new Error { Code = "1008", Message = "Invalid Request Body!" }; }
        }

        public static Error LatestVerErr
        {
            get { return new Error { Code = "1006", Message = "Latest version of the app is required to access the API." }; }
        }

        public static Error Maintenance
        {
            get { return new Error { Code = "1005", Message = "Maintenance mode." }; }
        }

        public static Error NoRecordsFondErr
        {
            get { return new Error { Code = "1013", Message = "No Records Found!" }; }
        }

        public static Error NoRowsAffectedErr
        {
            get { return new Error { Code = "1012", Message = "No Rows Affected!" }; }
        }

        public static Error Pending
        {
            get { return new Error { Code = "1003", Message = "Pending" }; }
        }

        public static Error TargetSystemErr
        {
            get { return new Error { Code = "1009", Message = "Target System Not Found!" }; }
        }

        public static Error ThirdPartyErr
        {
            get { return new Error { Code = "1014", Message = "Third Party Response Message." }; }
        }

        public static Error TimeoutErr
        {
            get { return new Error { Code = "1002", Message = "Timeout error!" }; }
        }

        public static Error UnknownErr
        {
            get { return new Error { Code = "1004", Message = "Unknown error!" }; }
        }

        public static Error TargetSystemResponseErr
        {
            get { return new Error { Code = "1016", Message = "Target System Response Error." }; }
        }

        public static Error CRUDErr
        {
            get { return new Error { Code = "CRUD 2", Message = "Generic CRUD 2 error!" }; }
        }

        public static Error UnknownException { get { return new Error { Code = "1004", Message = "Unknown error." }; } }

        public static ErrorResponse Unauthorized { get { return new ErrorResponse { Code = "1000", Message = "Unauthorized." }; } }

        public static ErrorResponse ValidationtError { get { return new ErrorResponse { Code = "1001", Message = "Validation error" }; } }

        public static ErrorResponse Unknown { get { return new ErrorResponse { Code = "1004", Message = "Unknown error" }; } }

        //Common
        public static Error ValidationErr
        {
            get { return new Error { Code = "1001", Message = "Validation error!" }; }
        }

        #endregion
    }
}
