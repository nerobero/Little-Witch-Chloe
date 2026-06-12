using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class DataTableRegistry
{
    private static readonly Dictionary<Type, object> _tables = new Dictionary<Type, object>();

    /// <summary>
    /// Loads a .bytes asset from Resources/Data/Binary/ and registers it
    /// under type T. Call this once at startup for each data type.
    /// </summary>
    public static void Register<T>(string assetName, Func<BinaryReader, T> deserialize)
        where T : IBinaryRecord
    {
        TextAsset asset = Resources.Load<TextAsset>($"Data/Binary/{assetName}");

        if (asset == null)
        {
            Debug.LogError($"[DataTableRegistry] Could not load Data/Binary/{assetName}.bytes — has the CSV been imported yet?");
            return;
        }

        var table = new BinaryTable<T>();
        table.Load(asset, deserialize);
        _tables[typeof(T)] = table;
    }

    /// <summary>
    /// Returns the loaded BinaryTable for type T.
    /// </summary>
    public static BinaryTable<T> Get<T>() where T : IBinaryRecord
    {
        if (_tables.TryGetValue(typeof(T), out object table))
            return (BinaryTable<T>)table;

        throw new InvalidOperationException($"[DataTableRegistry] No table registered for {typeof(T).Name}. Did you call Register in DataInitializer?");
    }
}
