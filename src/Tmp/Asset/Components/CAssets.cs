using Tmp.Asset.BuiltIn.Texture;
using Tmp.Asset.Format.Text;
using Tmp.Core.Comp;
using Tmp.Render;

namespace Tmp.Asset.Components;

public sealed class CAssets(IAssets assets) : ComponentFunc((self, children) =>
{
    self.CreateContext(assets);

    var textLoader = new AssetLoaderText();
    textLoader.AddDeserializer(new TextureRegion2D.Deserializer());
    textLoader.AddDeserializer(new Texture2D.Deserializer());
    textLoader.AddDeserializer(new Shader.Deserializer());

    assets.AddLoader(textLoader);

    self.OnLateCleanup(assets.Dispose);

    return children;
});