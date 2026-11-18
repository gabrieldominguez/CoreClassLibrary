namespace CoreClassLibrary.Extensions.Paging;

/// <summary>
/// Parametros para paginar una consulta.
/// </summary>
/// <param name="PageNumber"> Número de página (empieza en 0). </param>
/// <param name="PageSize"> Cantidad de items por página. </param>
public readonly record struct PageRequest(int PageNumber, int PageSize);
