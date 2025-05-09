using Lofi2D.Math;

namespace Lofi2D.Asset.Format;

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
