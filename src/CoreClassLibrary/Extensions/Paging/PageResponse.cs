namespace CoreClassLibrary.Extensions.Paging;

/// <summary>
/// Resultado de paginar una consulta.
/// </summary>
/// <param name="TotalCount"></param>
/// <param name="Items"></param>
/// <typeparam name="T"></typeparam>
public readonly record struct PageResponse<T>(int TotalCount, IEnumerable<T> Items);
