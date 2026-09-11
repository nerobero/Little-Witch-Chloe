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
                // Accepts the enum name ("Happy", case-insensitive) or its numeric value; blank -> Unspecified.
                emotion          = Enum.TryParse(cols[2], true, out EEmotion emo) ? emo : EEmotion.Unspecified,
                dialogueText     = cols[3],
                nextridx         = uint.TryParse(cols[4], out uint next)  ? next  : 0,
                hasDialogueEnded = bool.TryParse(cols[5], out bool ended) ? ended : false,
            }),

            ["CommissionData"] = csv => CsvToBinaryConverter.Convert(csv, cols => new CollectableData
            {
                levelType       = ParseLevelType(cols[0]),
                collectableType = ParseCollectable(cols[1]),
                collectedCount  = int.TryParse(cols[2], out int amount) ? amount : 0,
            }),

            ["LovePotionData"] = csv => CsvToBinaryConverter.Convert(csv, cols => new LovePotionIngredientData
            {
                levelType       = ParseLevelType(cols[0]),
                collectableType = ParseCollectable(cols[1]),
                requiredCount   = int.TryParse(cols[2], out int amount) ? amount : 0,
            }),

            ["MessageBoxData"] = csv => CsvToBinaryConverter.Convert(csv, cols => new SystemTextRow
            {
                key       = cols[0],
                text      = cols[1],
                spriteKey = cols.Length > 2 ? cols[2] : "",
            }),
        };

    /// <summary>
    /// Auto-parses and exports bytes whenever saved/re-imported.
    /// </summary>
    /// <param name="importedAssets"></param>
    /// <param name="deletedAssets"></param>
    /// <param name="movedAssets"></param>
    /// <param name="movedFromAssetPaths"></param>
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

            string result = ProcessEntry(filename, csv, convert);
            Debug.Log($"[CsvBinaryImporter] {result}");
        }
    }

    public static List<string> ConvertAll()
    {
        var results = new List<string>();

        foreach (var kvp in Registry)
        {
            string csvPath = FindCsvPath(kvp.Key);
            if (csvPath == null)
            {
                results.Add($"FAIL  {kvp.Key} — CSV not found in project");
                continue;
            }

            // Force Unity to re-read the file from disk first: an external edit
            // (e.g. adding quotes around a field) may not have been reimported yet,
            // in which case LoadAssetAtPath would hand us stale text.
            AssetDatabase.ImportAsset(csvPath, ImportAssetOptions.ForceUpdate);

            TextAsset csv = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
            results.Add(ProcessEntry(kvp.Key, csv, kvp.Value));
        }

        return results;
    }

    private static string ProcessEntry(string filename, TextAsset csv, Func<TextAsset, byte[]> convert)
    {
        try
        {
            byte[] bytes = convert(csv);

            if (!Directory.Exists(OutputFolder))
                Directory.CreateDirectory(OutputFolder);

            string outputPath = $"{OutputFolder}/{filename}.bytes";
            File.WriteAllBytes(outputPath, bytes);
            AssetDatabase.ImportAsset(outputPath);

            return $"OK    {filename} — {CountRecords(bytes)} records written to {outputPath}";
        }
        catch (Exception e)
        {
            return $"FAIL  {filename} — {e.Message}";
        }
    }

    private static string FindCsvPath(string filename)
    {
        string[] guids = AssetDatabase.FindAssets($"t:TextAsset {filename}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) &&
                Path.GetFileNameWithoutExtension(path).Equals(filename, StringComparison.OrdinalIgnoreCase))
                return path;
        }
        return null;
    }

    private static int CountRecords(byte[] bytes)
    {
        if (bytes.Length < 4) return 0;
        return BitConverter.ToInt32(bytes, 0);
    }

    /// <summary>Accepts the enum name ("BogLevel", case-insensitive) or its numeric value; blank/invalid -> 0.</summary>
    private static ELevelType ParseLevelType(string col) =>
        Enum.TryParse(col, true, out ELevelType level) ? level :
        (ELevelType)(uint.TryParse(col, out uint lidx) ? lidx : 0);

    /// <summary>Accepts the enum name ("CommDigestHerb", case-insensitive) or its numeric value; blank/invalid -> 0.</summary>
    private static ECollectable ParseCollectable(string col) =>
        Enum.TryParse(col, true, out ECollectable type) ? type :
        (ECollectable)(uint.TryParse(col, out uint tidx) ? tidx : 0);
}
