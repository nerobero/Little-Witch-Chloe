using System;
using System.Collections.Generic;
using UnityEngine;

public static class CSVParser
{
    public static List<T> Parse<T>(TextAsset csv, Func<string[], T> rowMapper)
    {
        var results = new List<T>();
        var rows = SplitRows(csv.text);

        for (int i = 1; i < rows.Count; i++) // row 0 is the header
        {
            var cols = ParseRow(rows[i]);
            if (cols.Count == 0) continue;
            results.Add(rowMapper(cols.ToArray()));
        }

        return results;
    }

    public static string[] ParseHeader(TextAsset csv)
    {
        var rows = SplitRows(csv.text);
        return rows.Count == 0 ? Array.Empty<string>() : ParseRow(rows[0]).ToArray();
    }

    private static List<string> SplitRows(string text)
    {
        var rows = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (c == '"')
            {
                // "" inside a quoted field → escaped literal quote
                if (inQuotes && i + 1 < text.Length && text[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                    i++; // treat \r\n as one newline

                if (current.Length > 0)
                    rows.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            rows.Add(current.ToString());

        return rows;
    }

    private static List<string> ParseRow(string row)
    {
        var fields = new List<string>();
        var field = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < row.Length; i++)
        {
            char c = row[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < row.Length && row[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        fields.Add(field.ToString());
        return fields;
    }
}
