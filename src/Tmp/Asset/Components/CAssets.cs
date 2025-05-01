using Tmp.Asset.Format.Text;
using Tmp.Audio;
using Tmp.Core.Comp;
using Tmp.Render;
using Tmp.Render.Texture;

namespace Tmp.Asset.Components;

public sealed class CAssets(IAssets assets) : CFunc((self, children) =>
{
    self.CreateContext(assets);

    var textLoader = new AssetLoaderText();
    textLoader.AddDeserializer(new TextureRegion2D.Deserializer());
    textLoader.AddDeserializer(new Texture2D.Deserializer());
    textLoader.AddDeserializer(new Shader.Deserializer());

    assets.AddLoader(textLoader);
    assets.AddLoader(new SoundSourceLoader());

    self.OnLateCleanup(assets.Dispose);

    return children;
});