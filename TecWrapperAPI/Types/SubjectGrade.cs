namespace TecWrapperApi.Types;

public record SubjectGrade
{
    public required string Materia;
    public int? Creditos;
    public required string Grupo;
    public required string Maestro;
    public required string Calificacion;
    public string? Oportunidad;
}
