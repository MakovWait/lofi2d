using Lofi2D.Asset.Util;

namespace Lofi2D.Asset;

public interface IAssetLoader
{
    bool MatchPath(AssetPath path);

    T Load<T>(AssetPath path, IAssetsSource subAssets, IResultMapper<T> target);
    
    T Load<T>(AssetPath path, IAssetsSource subAssets)
    {
        return Load(path, subAssets, ResultMapper<T>.AsIs);
    }
}
