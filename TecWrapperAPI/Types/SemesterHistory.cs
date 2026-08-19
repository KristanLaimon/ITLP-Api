namespace TecWrapperApi.Types;

public record SemesterHistory
{
    public required int Numero;
    public required string Periodo;
    public required List<SubjectGrade> Materias;
    public required double PromedioSemestre;
}
