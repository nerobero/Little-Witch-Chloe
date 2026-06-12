using System;
using System.Collections.Generic;
using System.IO;
using Data;
using Types;
using UnityEditor;
using UnityEngine;

public class CsvBinaryImporter : AssetPostprocessor
{
    private const string OutputFolder = "Assets/Resources/Data/Binary";

    // Maps CSV filename (no extension) to a function that produces the binary bytes.
    // Add a new entry here whenever a new CSV data type is introduced.
    private static readonly Dictionary<string, Func<TextAsset, byte[]>> Registry =
        new Dictionary<string, Func<TextAsset, byte[]>>
        {
            ["DialogueLinesData"] = csv => CsvToBinaryConverter.Convert(csv, cols => new DialogueRow
            {
                currentridx      = uint.TryParse(cols[0], out uint ridx)  ? ridx  : 0,
                speakerName      = cols[1],
                dialogueText     = cols[2],
                nextridx         = uint.TryParse(cols[3], out uint next)  ? next  : 0,
                hasDialogueEnded = bool.TryParse(cols[4], out bool ended) ? ended : false,
            }),

            ["CommissionData"] = csv => CsvToBinaryConverter.Convert(csv, cols => new CollectableData
            {
                levelType       = (ELevelType)(uint.TryParse(cols[0], out uint lidx) ? lidx : 0),
                collectableType = (ECollectable)(uint.TryParse(cols[1], out uint tidx) ? tidx : 0),
                collectedCount  = int.TryParse(cols[2], out int amount) ? amount : 0,
            }),
        };

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            if (!path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)) continue;

            string filename = Path.GetFileNameWithoutExtension(path);
            if (!Registry.TryGetValue(filename, out Func<TextAsset, byte[]> convert)) continue;

            TextAsset csv = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (csv == null) continue;

            byte[] bytes = convert(csv);

            if (!Directory.Exists(OutputFolder))
                Directory.CreateDirectory(OutputFolder);

            string outputPath = $"{OutputFolder}/{filename}.bytes";
            File.WriteAllBytes(outputPath, bytes);
            AssetDatabase.ImportAsset(outputPath);

            Debug.Log($"[CsvBinaryImporter] {outputPath} regenerated ({bytes.Length} bytes, {CountRecords(bytes)} records)");
        }
    }

    private static int CountRecords(byte[] bytes)
    {
        if (bytes.Length < 4) return 0;
        return BitConverter.ToInt32(bytes, 0);
    }
}
