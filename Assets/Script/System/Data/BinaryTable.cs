using System;
using System.IO;
using UnityEngine;

public class BinaryTable<T> where T : IBinaryRecord
{
    public T[] Records { get; private set; } = Array.Empty<T>();

    public void Load(TextAsset asset, Func<BinaryReader, T> deserialize)
    {
        using var stream = new MemoryStream(asset.bytes);
        using var reader = new BinaryReader(stream);

        int count = reader.ReadInt32();
        Records = new T[count];

        for (int i = 0; i < count; i++)
            Records[i] = deserialize(reader);
    }
}
