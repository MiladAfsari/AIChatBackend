namespace Application.Service.Common
{
    public class CommandResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string Data { get; }

        private CommandResult(bool isSuccess, string message, string data = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public static CommandResult Success(string data) => new CommandResult(true, "Success", data);
        public static CommandResult Failure(string message) => new CommandResult(false, message);
    }
}
