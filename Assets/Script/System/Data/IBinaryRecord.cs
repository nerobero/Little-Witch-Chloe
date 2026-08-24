using System.IO;

public interface IBinaryRecord
{
    void Serialize(BinaryWriter writer);
}

/// <summary>
/// A binary record that can also be looked up by a string key
/// (e.g. system/UI text keyed by an ID string).
/// </summary>
public interface IKeyedBinaryRecord : IBinaryRecord
{
    string Key { get; }
}
