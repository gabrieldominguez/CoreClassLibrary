using System.Linq.Expressions;

namespace CoreClassLibrary.Objects.Predicates;

public class LinqPredicate<T>(Expression<Func<T, bool>> expression) : FuncPredicate<T>(expression.Compile());
