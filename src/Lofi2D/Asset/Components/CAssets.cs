using Lofi2D.Asset.Format.Text;
using Lofi2D.Audio;
using Lofi2D.Core.Comp;
using Lofi2D.Render;
using Lofi2D.Render.Text;
using Lofi2D.Render.Texture;

namespace Lofi2D.Asset.Components;

public sealed class CAssets(IAssets assets) : CFunc((self, children) =>
{
    self.CreateContext(assets);

    var textLoader = new AssetLoaderText();
    textLoader.AddDeserializer(new TextureRegion2D.Deserializer());
    textLoader.AddDeserializer(new Texture2D.Deserializer());
    textLoader.AddDeserializer(new Shader.Deserializer());

    assets.AddLoader(textLoader);
    assets.AddLoader(new SoundSourceLoader());
    assets.AddLoader(new DefaultFontLoader());

    self.OnLateCleanup(assets.Dispose);

    return children;
});