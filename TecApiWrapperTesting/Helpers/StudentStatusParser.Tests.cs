using TecWrapperApi.Helpers;

namespace TecApiWrapperTesting.Helpers;

public class StudentStatusParserTests
{
    [Fact]
    public void Parse_WithRealStudentStatusPage_ParsesAllFields()
    {
        var status = StudentStatusParser.Parse(MockPages.StudentStatusPage);

        Assert.Equal("00000000", status.Control);
        Assert.Equal("JUAN PEREZ GOMEZ", status.Nombre);
        Assert.Equal("INGENIERÍA EN SISTEMAS COMPUTACIONALES", status.Carrera);
        Assert.Equal(9, status.Semestre);
        Assert.Equal("VIGENTE", status.Estatus);
        Assert.False(status.Inscrito);
        Assert.Equal("AGOSTO 2022", status.Ingreso);
        Assert.Equal(240, status.CreditosAcumulados);
        Assert.Equal(92.3, status.CreditosAcumuladosPorcentaje);
        Assert.Equal(94.56, status.Promedio);
        Assert.Equal(96, status.PromedioSemestreAnterior);
        Assert.Equal("21 de ago. del 2026, 08:09 hr", status.FechaReinscripcion);
        Assert.Equal("SI", status.PagoReinscripcion);
    }
}
