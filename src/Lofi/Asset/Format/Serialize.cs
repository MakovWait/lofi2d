namespace Lofi.Asset.Format;

public interface ISerializeOutput
{
    void Write<T>(string name, T data);
}

public interface ISerializable
{
    void WriteTo(ISerializeOutput output);
}
