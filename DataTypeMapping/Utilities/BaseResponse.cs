namespace DataTypeMapping.Utilities
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();
        public bool IsSuccess { get; set; }

        public static BaseResponse<T> Success(T data)
        { 
            return new BaseResponse<T>
            {
                Data = data,
                IsSuccess = true,
            };
        }
        public static BaseResponse<T> Failure(List<string> errorMessages, string statusCode = "")
        {
            var baseResponse = new BaseResponse<T>();

            var errorDetail = errorMessages.Select(x => new ErrorDetail { Code = statusCode, Message = x }) ;
            baseResponse.Errors.AddRange(errorDetail);
            baseResponse.IsSuccess = false;
            return baseResponse;
        }
    }

    public class ErrorDetail
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
