using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CsvToBinaryConverter
{
    /// <summary>
    /// Parses a CSV TextAsset into a list of T, then serializes the records
    /// to a byte array: a 4-byte record count followed by each record's binary data.
    /// </summary>
    public static byte[] Convert<T>(TextAsset csv, Func<string[], T> rowMapper)
        where T : IBinaryRecord
    {
        List<T> records = CSVParser.Parse(csv, rowMapper);

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write(records.Count);
        foreach (T record in records)
            record.Serialize(writer);

        return stream.ToArray();
    }
}
