namespace CoreClassLibrary.Extensions.Paging;

public static class PagingExtensions
    {
    /// <summary>
    /// Pagina la enumeración.
    /// </summary>
    /// <param name="source">Enumeracion.</param>
    /// <param name="pageNumber">Número de página (la primera es la 0).</param>
    /// <param name="pageSize">Cantidad de items por página.</param>
    /// <param name="pageSize">Cantidad de items por página (si es 0 no pagina).</param>
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        => pageSize == 0 ? source : source.Skip(pageNumber * pageSize).Take(pageSize);

    /// <summary>
    /// Pagina y ejecuta la consulta.
    /// </summary>
    /// <typeparam name="TSource">Tipo de datos que devuelve de la consulta.</typeparam>
    /// <param name="query">Consulta a paginar.</param>
    /// <param name="pageRequest">Número (empieza en 0) y tamaño de página. Si es nulo trae toda la consulta.</param>
    /// <returns>Resultado paginado.</returns>
    /// <remarks>
    /// Este método realiza dos round-trip a la fuente de datos, el primero para obtener la cantidad de registros totales,
    /// y el segundo para traer los registros correspondientes a la página.
    ///
    /// La cantidad de registros totales permite calcular la cantidad de páginas totales, lo que puede ser aprovechado
    /// en la interface de usuario.
    /// Si esto no es necesario, usar IQueryable<TSource> IQueryablePagingExtension.Paginate<TSource>(this IQueryable<TSource> query, int pageNumber, int pageSize)
    /// </remarks>
    public static PageResponse<TSource> Paginate<TSource>(this IQueryable<TSource> query, PageRequest? pageRequest = null)
        => new PageResponse<TSource>(query.Count(), pageRequest.HasValue? query.Paginate(pageRequest.Value.PageNumber, pageRequest.Value.PageSize).ToList(): [.. query]);
    }
