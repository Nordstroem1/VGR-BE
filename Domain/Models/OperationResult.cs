namespace Domain.Models
{
    public class OperationResult<T> where T : class
    {
        public T? Data { get; set; }
        public bool IsFailure { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static OperationResult<T> SuccessResult(T data)
        {
            return new OperationResult<T>
            {
                IsFailure = false,
                Data = data
            };
        }
        public static OperationResult<T> FailureResult(string errorMessage)
        {
            return new OperationResult<T>
            {
                IsFailure = true,
                ErrorMessage = errorMessage
            };
        }
    }
}
