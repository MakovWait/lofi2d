using Lofi.Math;

namespace Lofi.Asset.Format;

public interface IDeserialize<out T>
{
    T From(ISerializeInput input);
}

public interface ISerializeInput
{
    Rect2 ReadRect2(string name);
    
    string ReadString(string name);

    IAsset<T> ReadSubRes<T>(string name);
}
