using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using TecWrapperApi.Exceptions;
using TecWrapperApi.Types;

namespace TecWrapperApi.Helpers;

internal static partial class GradeHistoryParser
{
    [GeneratedRegex(@"Semestre:\s*(\d+)\s*Periodo:\s*(.+)")]
    private static partial Regex SemesterHeaderRegex();

    public static List<SemesterHistory> Parse(string html)
    {
        var document = new HtmlParser().ParseDocument(html);
        var table = document.QuerySelector("table.tabla");
        if (table is null)
            throw new TecApiParsingException(
                "Couldn't find the grades table on the page. (Tec changed the UI layout?)"
            );

        var semesters = new List<SemesterHistory>();
        (int Numero, string Periodo)? currentHeader = null;

        foreach (var section in table.Children)
        {
            if (section.TagName.Equals("thead", StringComparison.OrdinalIgnoreCase))
            {
                currentHeader = ParseSemesterHeader(section.Children[0].TextContent.Trim());
                continue;
            }

            if (!section.TagName.Equals("tbody", StringComparison.OrdinalIgnoreCase) || section.Children.Length == 0)
                continue;

            if (currentHeader is null)
                throw new TecApiParsingException(
                    "Found a grades block with no semester header before it. (Tec changed the UI layout?)"
                );

            var materias = new List<SubjectGrade>();
            double? promedioSemestre = null;

            foreach (var row in section.Children)
            {
                var cells = row.Children.Select(c => c.TextContent.Trim()).ToArray();

                if (cells.Length == 2 && cells[0].StartsWith("Promedio del semestre", StringComparison.OrdinalIgnoreCase))
                {
                    promedioSemestre = double.Parse(cells[1], CultureInfo.InvariantCulture);
                    continue;
                }

                if (cells.Length is not (5 or 6))
                    throw new TecApiParsingException(
                        $"Unexpected grade row with {cells.Length} cells. (Tec changed the UI layout?)"
                    );

                materias.Add(
                    new SubjectGrade
                    {
                        Materia = cells[0],
                        Creditos = int.TryParse(cells[1], out int creditos) ? creditos : null,
                        Grupo = cells[2],
                        Maestro = cells[3],
                        Calificacion = cells[4],
                        Oportunidad = cells.Length == 6 ? cells[5] : null,
                    }
                );
            }

            if (promedioSemestre is null)
                throw new TecApiParsingException(
                    $"Missing 'Promedio del semestre' row for Semestre {currentHeader.Value.Numero}. (Tec changed the UI layout?)"
                );

            semesters.Add(
                new SemesterHistory
                {
                    Numero = currentHeader.Value.Numero,
                    Periodo = currentHeader.Value.Periodo,
                    Materias = materias,
                    PromedioSemestre = promedioSemestre.Value,
                }
            );
            currentHeader = null;
        }

        return semesters;
    }

    private static (int Numero, string Periodo) ParseSemesterHeader(string headerText)
    {
        var match = SemesterHeaderRegex().Match(headerText);
        if (!match.Success)
            throw new TecApiParsingException(
                $"Couldn't parse semester header: '{headerText}'. (Tec changed the UI layout?)"
            );
        return (int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), match.Groups[2].Value.Trim());
    }
}
