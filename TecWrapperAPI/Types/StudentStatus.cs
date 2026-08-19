namespace TecWrapperApi.Types;

public record StudentStatus
{
    public required string Control;
    public required string Nombre;
    public required string Carrera;
    public required int Semestre;
    public required string Estatus;
    public required bool Inscrito;
    public required string Ingreso;
    public required int CreditosAcumulados;
    public required double CreditosAcumuladosPorcentaje;
    public required double Promedio;
    public required double PromedioSemestreAnterior;
    public string? FechaReinscripcion;
    public string? PagoReinscripcion;
}
