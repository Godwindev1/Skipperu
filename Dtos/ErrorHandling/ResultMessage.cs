namespace Skipperu.Dtos.ErrorHandling
{
    public enum MessageTypes
    {
        ERROR = 1,
        INFO = 2,
        SUCCESFUL = 3,
        NOTFOUND = 4
    }
    public class ResultMessage
    {

        public MessageTypes type { get; set;  }
        public string Message { get; set;  }
    }
}
