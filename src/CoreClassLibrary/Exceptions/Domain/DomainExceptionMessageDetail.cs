using CoreClassLibrary.Exceptions;

namespace CoreClassLibrary.Exceptions.Domain;

public class DomainExceptionMessageDetail: ExceptionMessageDetail
    {
    public string Code { get; set; } = "";

    public DomainExceptionMessageDetail() { }

    public DomainExceptionMessageDetail(DomainException domainException): base(domainException)
        {
        this.Code = domainException.Code;
        }
    }