using Lofi.Core.Comp;

namespace Lofi.Window;

public interface IWindow
{
    void Draw();

    void BindTo(INodeInit self);
}