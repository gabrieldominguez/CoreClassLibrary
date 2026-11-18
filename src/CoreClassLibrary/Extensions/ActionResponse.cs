namespace CoreClassLibrary.Extensions;

public class FuncResponse<T>: ActionResponse
    {
    public T? Value { get; private set; }

    public FuncResponse(Exception exception, T? Value = default) : base(exception)
        {
        this.Value = Value;
        }

    public FuncResponse(T? Value = default, bool IsSuccess = true, string message = ""): base(IsSuccess, message)
        {
        this.Value = Value;
        }
    }

public class ActionResponse(bool IsSuccess = true, string message = "")
    {
    public static readonly ActionResponse Success = new ActionResponse();

    public bool IsSuccess { get; private set; } = IsSuccess;
    public string Message { get; private set; } = message;
    public Exception? Exception { get; private set; }

    public ActionResponse(Exception exception) : this(false, exception.Message)
        {
        this.Exception = exception;
        }
    }
