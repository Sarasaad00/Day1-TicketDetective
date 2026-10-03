namespace Day1_TicketDetective.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        protected Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success(string message = "") => new(true, message);
        public static Result Failure(string message) => new(false, message);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, string message) : base(isSuccess, message)
        {
            Value = value;
        }

        public static Result<T> Success(T value, string message = "") => new(true, value, message);
        public new static Result<T> Failure(string message) => new(false, default, message);
    }
}
