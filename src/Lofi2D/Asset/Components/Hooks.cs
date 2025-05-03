using Lofi2D.Core.Comp;

namespace Lofi2D.Asset.Components;

public static class Hooks
{
    public static IAsset<T> UseAsset<T>(this INodeInit self, AssetPath path)
    {
        var assets = self.UseContext<IAssets>();
        return assets.Load<T>(path);
    }
}