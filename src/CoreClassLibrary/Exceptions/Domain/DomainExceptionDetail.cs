using CoreClassLibrary.Objects.Pooling;

namespace CoreClassLibrary.Exceptions.Domain;

public class DomainExceptionDetail: ExceptionDetail
    {
    public string Code { get; set; } = "";

    public DomainExceptionDetail() { }

    public DomainExceptionDetail(DomainException domainException) : base(domainException) 
        { 
        this.Code = domainException.Code; 
        }

    public override string ToString()
        => PooledStringBuilder.ToString(this, (detail, builder) => builder.AppendDomainExceptionDetail(detail));
    }