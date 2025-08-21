namespace DataTypeMapping.Utilities
{
    public class BaseResponse<T>
    {
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsSuccess => Errors.Count == 0;

        public static BaseResponse<T> Success(T data, string message = "Operation completed successfully.")
        {
            return new BaseResponse<T>
            {
                Data = data,
                Message = message,
                Errors = new List<string>()
            };
        }
        public static BaseResponse<T> Failure(List<string> errorMessages, string message = "Operation failed.")
        {
            return new BaseResponse<T>
            {
                Message = message,
                Errors = errorMessages,
                Data = default
            };
        }
    }
}
