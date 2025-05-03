using Lofi.Asset.Format.Text;
using Lofi.Audio;
using Lofi.Core.Comp;
using Lofi.Render;
using Lofi.Render.Text;
using Lofi.Render.Texture;

namespace Lofi.Asset.Components;

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