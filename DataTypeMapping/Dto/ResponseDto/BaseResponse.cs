namespace AuthServer.Dto.ResponseDto
{
    public class BaseResponse<T>
    {
        public T Data { get; private set; }
        public List<ErrorDetail> Errors { get; private set; }
        public bool IsSuccess => Errors == null || Errors.Count == 0;

        public BaseResponse(T data, List<ErrorDetail> errors)
        {
            Data = data;
            Errors = errors;
        }

        public static BaseResponse<T> Success(T data) =>
            new BaseResponse<T>(data, null);

        // Single error
        public static BaseResponse<T> Failure(string code, string description)
        {
            var errorDetail = new List<ErrorDetail>
        {
            new ErrorDetail { Code = code, Message = description },
        };
            return new BaseResponse<T>(default, errorDetail);
        }

        // Multiple errors
        public static BaseResponse<T> Failure(List<ErrorDetail> errors)
        {
            return new BaseResponse<T>(default, errors);
        }
    }
    public class ErrorDetail
    {
        public string Code { get; set; }
        public string Message { get; set; }

    }
}
