using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using TecWrapperApi.Exceptions;
using TecWrapperApi.Types;

namespace TecWrapperApi.Helpers;

internal static partial class StudentStatusParser
{
    [GeneratedRegex(@"^(\d+)\s*\(([\d.]+)%\)$")]
    private static partial Regex CreditosRegex();

    public static StudentStatus Parse(string html)
    {
        var document = new HtmlParser().ParseDocument(html);
        var paragraphs = document.QuerySelectorAll("div.siia--inicio p");

        var fields = new Dictionary<string, string>();
        foreach (var p in paragraphs)
        {
            string text = p.TextContent.Trim();
            int separatorIndex = text.IndexOf(": ", StringComparison.Ordinal);
            if (separatorIndex < 0) continue;
            fields[text[..separatorIndex].Trim()] = text[(separatorIndex + 2)..].Trim();
        }

        if (fields.Count == 0)
            throw new TecApiParsingException(
                "Couldn't find student status fields on the page. (Tec changed the UI layout?)"
            );

        string GetOrThrow(string key)
        {
            if (!fields.TryGetValue(key, out var value))
                throw new TecApiParsingException(
                    $"Missing expected field '{key}' on the student status page. (Tec changed the UI layout?)"
                );
            return value;
        }

        string creditosRaw = GetOrThrow("Creditos acumulados");
        var creditosMatch = CreditosRegex().Match(creditosRaw);
        if (!creditosMatch.Success)
            throw new TecApiParsingException($"Couldn't parse 'Creditos acumulados' value: '{creditosRaw}'");

        string inscrito = GetOrThrow("Inscrito");

        return new StudentStatus
        {
            Control = GetOrThrow("Control"),
            Nombre = GetOrThrow("Nombre"),
            Carrera = GetOrThrow("Carrera"),
            Semestre = int.Parse(GetOrThrow("Semestre"), CultureInfo.InvariantCulture),
            Estatus = GetOrThrow("Estatus"),
            Inscrito = inscrito.Equals("Si", StringComparison.OrdinalIgnoreCase)
                || inscrito.Equals("Sí", StringComparison.OrdinalIgnoreCase),
            Ingreso = GetOrThrow("Ingreso"),
            CreditosAcumulados = int.Parse(creditosMatch.Groups[1].Value, CultureInfo.InvariantCulture),
            CreditosAcumuladosPorcentaje = double.Parse(
                creditosMatch.Groups[2].Value,
                CultureInfo.InvariantCulture
            ),
            Promedio = double.Parse(GetOrThrow("Promedio"), CultureInfo.InvariantCulture),
            PromedioSemestreAnterior = double.Parse(
                GetOrThrow("Promedio semestre anterior"),
                CultureInfo.InvariantCulture
            ),
            FechaReinscripcion = fields.GetValueOrDefault("Fecha reinscripción"),
            PagoReinscripcion = fields.GetValueOrDefault("Pago reinscripcion"),
        };
    }
}
