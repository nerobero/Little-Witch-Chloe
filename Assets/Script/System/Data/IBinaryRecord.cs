using System.IO;

public interface IBinaryRecord
{
    void Serialize(BinaryWriter writer);
}
