#pragma warning disable IDE0046 // Convert to conditional expression

using System.Text;
using CoreClassLibrary.Exceptions.Domain;
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Exceptions;

public static class DetailExtensions
    {
    public static ExceptionDetail CreateDetail(this Exception ex)
        {
        if (ex is DomainException domainException) 
            { 
            return new DomainExceptionDetail(domainException); 
            }

        if (ex is AggregateException aggregateException) 
            { 
            return new AggregateExceptionDetail(aggregateException); 
            }

        return new ExceptionDetail(ex); 
        }

    public static StringBuilder AppendDetail(this StringBuilder stringBuilder, ExceptionDetail exceptionDetail)
        { 
        if (exceptionDetail is DomainExceptionDetail domainExceptionDetail) 
            { 
            return stringBuilder.AppendDomainExceptionDetail(domainExceptionDetail); 
            }

        if (exceptionDetail is AggregateExceptionDetail aggregateExceptionDetail) 
            { 
            return stringBuilder.AppendAggregateExceptionDetail(aggregateExceptionDetail); 
            }              

        return stringBuilder.AppendExceptionDetail(exceptionDetail); 
        }

    public static StringBuilder AppendDomainExceptionDetail(this StringBuilder stringBuilder, DomainExceptionDetail domainExceptionDetail)
        => stringBuilder
            .AppendExceptionDetail(domainExceptionDetail)
            .Append("Code: ").Append(domainExceptionDetail.Code);

    public static StringBuilder AppendAggregateExceptionDetail(this StringBuilder stringBuilder, AggregateExceptionDetail aggregateExceptionDetail)
        {
        stringBuilder
                .AppendExceptionDetail(aggregateExceptionDetail)
                .AppendLine("InnerExceptions: ");

        foreach (ExceptionDetail exceptionDetail in aggregateExceptionDetail.InnerExceptions)
            {
            stringBuilder.AppendDetail(exceptionDetail);
            }

        return stringBuilder;
        }

    public static StringBuilder AppendExceptionDetail(this StringBuilder stringBuilder, ExceptionDetail exceptionDetail)
        { 
        stringBuilder.Append(exceptionDetail.Type).Append(": ").Append(exceptionDetail.Message);

        if (exceptionDetail.InnerException != null)
            {
            stringBuilder.Append(" ---> ").Append(exceptionDetail.InnerException);
            }
        else
            {
            stringBuilder.AppendLine();
            }

        stringBuilder.AppendLine(exceptionDetail.StackTrace);

        if (exceptionDetail.Data != null)
            { 
            stringBuilder.Append("Data: ").AppendKeyValueString(exceptionDetail.Data).AppendLine();
            }

        return stringBuilder;
        }
    }
