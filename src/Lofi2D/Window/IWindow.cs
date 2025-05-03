using Lofi2D.Core.Comp;

namespace Lofi2D.Window;

public interface IWindow
{
    void Draw();

    void BindTo(INodeInit self);
}