using CoreClassLibrary.Exceptions;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Collections.ObjectModel;

namespace CoreClassLibrary.Exceptions;

public class AggregateExceptionDetail(AggregateException aggregateException) : ExceptionDetail(aggregateException)
    {
    public ReadOnlyCollection<ExceptionDetail> InnerExceptions { get; } = aggregateException.InnerExceptions.Select(DetailExtensions.CreateDetail).ToReadOnlyCollection();

    public override string ToString()
        => PooledStringBuilder.ToString(this, (detail, builder) => builder.AppendAggregateExceptionDetail(detail));
    }
