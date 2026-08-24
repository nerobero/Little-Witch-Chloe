using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BinaryTable<T> where T : IBinaryRecord
{
    public T[] Records { get; private set; } = Array.Empty<T>();

    private Dictionary<string, T> _byKey;

    public void Load(TextAsset asset, Func<BinaryReader, T> deserialize)
    {
        using var stream = new MemoryStream(asset.bytes);
        using var reader = new BinaryReader(stream);

        int count = reader.ReadInt32();
        Records = new T[count];

        for (int i = 0; i < count; i++)
            Records[i] = deserialize(reader);

        if (typeof(IKeyedBinaryRecord).IsAssignableFrom(typeof(T)))
        {
            _byKey = new Dictionary<string, T>(count);
            foreach (T record in Records)
                _byKey[((IKeyedBinaryRecord)record).Key] = record;
        }
    }

    /// <summary>
    /// Looks up a record by its string key. Only valid for record types
    /// implementing IKeyedBinaryRecord.
    /// </summary>
    public T GetByKey(string key)
    {
        if (_byKey == null)
            throw new InvalidOperationException($"[BinaryTable] {typeof(T).Name} does not implement IKeyedBinaryRecord.");

        return _byKey.TryGetValue(key, out T record) ? record : default;
    }
}
