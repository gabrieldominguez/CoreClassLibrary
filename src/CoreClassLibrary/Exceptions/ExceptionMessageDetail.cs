namespace CoreClassLibrary.Exceptions;

public class ExceptionMessageDetail
    {
    public string Message { get; set; } = "";

    public ExceptionMessageDetail() { }

    public ExceptionMessageDetail(Exception exception)
        {
        this.Message = exception.Message;
        }
    }
