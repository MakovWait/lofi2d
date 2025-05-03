using Lofi.Asset;
using Lofi.Asset.Util;
using Lofi.Core.Comp;
using Lofi.IO;
using Raylib_cs;
using Lofi.Asset.Components;

namespace Lofi.Audio;

public interface ISoundSource
{
    Sound SpawnSound();
    
    Sounds SpawnSounds(int amount);
}

public class SoundSourceFile(FilePath path) : ISoundSource
{
    public Sound SpawnSound()
    {
        return new Sound(Raylib.LoadSound(path));
    }

    public Sounds SpawnSounds(int amount)
    {
        var sounds = new List<Sound>();
        var wave = Raylib.LoadWave(path);
        try
        {
            for (var i = 0; i < amount; i++)
            {
                sounds.Add(new Sound(Raylib.LoadSoundFromWave(wave)));
            }
        }
        finally
        {
            Raylib.UnloadWave(wave);
        }
        return new Sounds(sounds);
    }
}

public class SoundSourceLoader : IAssetLoader
{
    private static readonly string[] SupportedExtensions = ["wav", "qoa", "ogg", "mp3", "flac", "xm", "mod"];

    public bool MatchPath(AssetPath path)
    {
        return SupportedExtensions.Contains(path.Extension);
    }

    public T Load<T>(AssetPath path, IAssetsSource subAssets, IResultMapper<T> target)
    {
        return target.Map(new SoundSourceFile(path));
    }
}

public static class SoundSourceExtensions
{
    public static Sound UseSound(this INodeInit self, AssetPath path)
    {
        var src = self.UseAsset<ISoundSource>(path);
        var sound = src.Value.SpawnSound();
        return sound;
    }
    
    public static Sounds UseSounds(this INodeInit self, AssetPath path, int amount)
    {
        var src = self.UseAsset<ISoundSource>(path);
        return src.Value.SpawnSounds(amount);
    }
}