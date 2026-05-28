using System.Text.RegularExpressions;

namespace HLSLDebugger.Utils;

public sealed record DiagnosticMarker(int Line, int StartColumn, int EndColumn, bool IsWarning, string Message);

public static class DiagnosticMarkers
{
    private static readonly Regex OurDiagnostic = new(
        @"(Error|Warning) at [^(\n]*\((\d+),\s*(\d+)(?:-(\d+))?\):\s*([^\n]*)");

    private static readonly Regex SlangDiagnostic = new(
        @"(error|warning)(?:\[\w+\])?:[ \t]*([^\r\n]*)\r?\n[ \t]*-->[ \t]*[\w./\\-]+:(\d+):(\d+)",
        RegexOptions.IgnoreCase);

    public static IReadOnlyList<DiagnosticMarker> Parse(string message)
    {
        var markers = new List<DiagnosticMarker>();
        if (string.IsNullOrEmpty(message)) return markers;

        foreach (Match m in OurDiagnostic.Matches(message))
        {
            int endColumn = m.Groups[4].Success ? int.Parse(m.Groups[4].Value) : 0;
            markers.Add(new DiagnosticMarker(
                int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), endColumn,
                m.Groups[1].Value == "Warning", m.Groups[5].Value.Trim()));
        }

        foreach (Match m in SlangDiagnostic.Matches(message))
        {
            markers.Add(new DiagnosticMarker(
                int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value), 0,
                m.Groups[1].Value.Equals("warning", StringComparison.OrdinalIgnoreCase),
                m.Groups[2].Value.Trim()));
        }

        return markers;
    }
}
