using TecWrapperApi.Helpers;

namespace TecApiWrapperTesting.Helpers;

public class GradeHistoryParserTests
{
    [Fact]
    public void Parse_WithRealGradeHistoryPage_ParsesAllEightSemesters()
    {
        var semesters = GradeHistoryParser.Parse(MockPages.GradeHistoryPage);

        Assert.Equal(8, semesters.Count);
        Assert.Equal(Enumerable.Range(1, 8), semesters.Select(s => s.Numero));
    }

    [Fact]
    public void Parse_FirstSemester_HasCorrectHeaderAndAverage()
    {
        var semesters = GradeHistoryParser.Parse(MockPages.GradeHistoryPage);
        var first = semesters[0];

        Assert.Equal(1, first.Numero);
        Assert.Equal("AGO-DIC 2022", first.Periodo);
        Assert.Equal(86, first.PromedioSemestre);
        Assert.Equal(9, first.Materias.Count);
    }

    [Fact]
    public void Parse_NormalSubjectRow_HasAllSixColumns()
    {
        var semesters = GradeHistoryParser.Parse(MockPages.GradeHistoryPage);
        var subject = semesters[0].Materias[0];

        Assert.Equal("FUNDAMENTOS DE PROGRAMACION", subject.Materia);
        Assert.Equal(5, subject.Creditos);
        Assert.Equal("A", subject.Grupo);
        Assert.Equal("MARIA EJEMPLO DEMO", subject.Maestro);
        Assert.Equal("80", subject.Calificacion);
        Assert.Equal("NORMAL", subject.Oportunidad);
    }

    [Fact]
    public void Parse_ActivityRowWithoutOportunidad_LeavesOportunidadNull()
    {
        var semesters = GradeHistoryParser.Parse(MockPages.GradeHistoryPage);
        var tutoria = semesters[0].Materias.Single(m => m.Materia == "TUTORIA");

        Assert.Equal("APROBADA", tutoria.Calificacion);
        Assert.Null(tutoria.Oportunidad);
    }

    [Fact]
    public void Parse_LastSemester_HasCorrectHeaderAndAverage()
    {
        var semesters = GradeHistoryParser.Parse(MockPages.GradeHistoryPage);
        var last = semesters[^1];

        Assert.Equal(8, last.Numero);
        Assert.Equal("ENE-JUN 2026", last.Periodo);
        Assert.Equal(86, last.PromedioSemestre);
        Assert.Equal(7, last.Materias.Count);
    }
}
