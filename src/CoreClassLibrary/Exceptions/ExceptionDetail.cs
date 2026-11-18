
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;

namespace CoreClassLibrary.Exceptions;

/// <summary>
/// Representa la información de los detalles del error.
/// </summary>
public class ExceptionDetail: ExceptionMessageDetail
    {
    public ExceptionDetail? InnerException { get; set; }
    public IDictionary<object, object?>? Data { get; set; }
    
    public string HelpLink { get; set; } = "";
    public string StackTrace { get; set; } = "";
    public string Type { get; set; } = "";
    
    public ExceptionDetail() { }

    /// <summary>
    /// Use: Exception.CreateDetail()
    /// </summary>
    /// <param name="exception"></param>
    internal ExceptionDetail(Exception exception): base(exception)
        {
        if (exception.InnerException != null)
            {
            this.InnerException = new ExceptionDetail(exception.InnerException);
            }

        this.Data = exception.Data.ToDictionary();

        this.HelpLink = exception.HelpLink ?? "";
        this.StackTrace = exception.StackTrace ?? "";
        this.Type = exception.GetType().ToString();
        }

    public override string ToString()
        => PooledStringBuilder.ToString(this, (detail, builder) => builder.AppendExceptionDetail(detail));
    }
