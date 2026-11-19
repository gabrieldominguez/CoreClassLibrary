namespace CoreClassLibrary.Tests.Reflection;

public record MessageEntry(DateTime DateTime, string Message);
public record LogEntry(DateTimeOffset DateTime, int Level, int EventId, string Name, string State, string Message, string Exception);

public class Poco
    {
    public static string StaticProperty { get; set; } = "";
    private const string staticField = "";

    private string field = Poco.staticField;
    public string PublicField = Poco.staticField;

    public string Property => this.field;

    public Poco() { }

    public void Method() { this.PrivateMethod(); }
    public string Function() => this.PrivateFunction();

    private void PrivateMethod() { this.field = ""; }
    private string PrivateFunction() => this.field;
    }

public class CuentaDTO
    {
    public string CuentaSucursal { get; set; } = "";
    public string CuentaNumero { get; set; } = "";
    public string CuentaDV { get; set; } = "";
    public string CuentaPBF { get; set; } = "";
    public string CuentaTipo { get; set; } = "";
    public string Producto { get; set; } = "";
    public bool EsCuentaSueldo { get; set; }
    public string Relacion { get; set; } = "";
    public string TipoFirma { get; set; } = "";
    public string CBU { get; set; } = "";
    public string Moneda { get; set; } = "";
    public decimal Saldo { get; set; }

    #pragma warning disable CA1822 // Mark members as static
    public DateTime FechaActualizacionSaldo => DateTime.Now;
    #pragma warning restore CA1822 // Mark members as static

    public string Estado { get; set; } = "";
    }

public class PrestamoModel
    {
    public int IdPrestamo { get; set; }
    public string CUITBanco { get; set; } = "";
    public string CUILSolicitante { get; set; } = "";
    public DateTime? FechaHoraSolicitud { get; set; }
    public decimal? MontoSolicitado { get; set; }
    public string IdentificadorCanal { get; set; } = "";
    public string Cuotas { get; set; } = "";
    public decimal? CambioDatos { get; set; }
    public decimal? NroCelular { get; set; }
    public decimal? IdPrestamoBanco { get; set; }
    public string DireccionElectronica { get; set; } = "";
    public DateTime? FechaRegistroBanco { get; set; }
    public string Canal { get; set; } = "";
    public string CuentaAcreditacion { get; set; } = "";
    public DateTime? FechaExportacion { get; set; }
    public DateTime? FechaHoraAceptadoBanco { get; set; }
    public decimal? MontoCreditoLiquidado { get; set; }
    public DateTime? FechaHoraRechazoBanco { get; set; }
    public DateTime? FechaHoraNotificaciónCliente { get; set; }
    public DateTime? FechaBaja { get; set; }
    public string MotivoBaja { get; set; } = "";
    public string CanalBaja { get; set; } = "";
    public DateTime? FechaPedidoLiquidacion { get; set; }
    public string CanalPedidoLiquidacion { get; set; } = "";    
    public string ResultadoValidacion { get; set; } = "";
    }

public class PRHOTSALBF
	{
	public long CUIL { get; set; }
	public string NROLISTA { get; set; } = "";

	public DateTime FECHAAPL { get; set; }
	public DateTime FECHADES { get; set; }
	public DateTime FECHAHAS { get; set; }
	public int TIPOFECHA { get; set; }
	public int TIPOBONIF { get; set; }
	public decimal BONIFICAC { get; set; }
	public decimal DIFTASA { get; set; }
	public decimal IND01 { get; set; }
	public string FILLER1 { get; set; } = "";
	}
