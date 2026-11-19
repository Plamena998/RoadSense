using System.Globalization;
using System.Text;

namespace RoadSense.Infrastructure.Import;

/// <summary>Един ред от CSV файл - достъп до стойностите по име на колоната.</summary>
public class CsvRow(Dictionary<string, string> values)
{
    public string Str(string column) => values[column];
    public int Int(string column) => int.Parse(values[column], CultureInfo.InvariantCulture);
    public double Dbl(string column) => double.Parse(values[column], CultureInfo.InvariantCulture);
}

/// <summary>Минимален четец на CSV (UTF-8, запетая, полета в кавички) за файловете от tools/prepare_data.py.</summary>
public static class CsvFile
{
    public static List<T> Read<T>(string path, Func<CsvRow, T> map)
    {
        var lines = Parse(File.ReadAllText(path, Encoding.UTF8));
        var header = lines[0];
        return lines.Skip(1)
            .Where(l => l.Count == header.Count)
            .Select(l => map(new CsvRow(header.Zip(l).ToDictionary(p => p.First, p => p.Second))))
            .ToList();
    }

    private static List<List<string>> Parse(string text)
    {
        if (text.Length > 0 && text[0] == '﻿') text = text[1..];
        var rows = new List<List<string>>();
        var row = new List<string>();
        var cell = new StringBuilder();
        var quoted = false;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < text.Length && text[i + 1] == '"') { cell.Append('"'); i++; }
                else if (c == '"') quoted = false;
                else cell.Append(c);
            }
            else if (c == '"') quoted = true;
            else if (c == ',') { row.Add(cell.ToString()); cell.Clear(); }
            else if (c == '\n' || c == '\r')
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                row.Add(cell.ToString()); cell.Clear();
                if (row.Count > 1 || row[0].Length > 0) rows.Add(row);
                row = new List<string>();
            }
            else cell.Append(c);
        }

        if (cell.Length > 0 || row.Count > 0) { row.Add(cell.ToString()); rows.Add(row); }
        return rows;
    }
}
