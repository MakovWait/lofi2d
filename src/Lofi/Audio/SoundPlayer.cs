using Lofi.Core.Comp;
using Lofi.Audio.Components;

namespace Lofi.Audio;

public class SoundPlayer(AudioBus bus)
{
    public Decibels Volume { get; set; } = Decibels.Default;
    public int MaxPolyphony { get; set; } = 1;
    
    private readonly List<SoundPlayback> _activePlaybacks = [];
    
    public void Play(Sound sound)
    {
        sound.SetVolume(bus.FinalVolume + Volume);
        
        var playback = sound.Playback;
        _activePlaybacks.Remove(playback);
        _activePlaybacks.Add(playback);
        playback.Play();
        
        EnsurePlaybackLimit();
    }

    public void Resume()
    {
        _activePlaybacks.ForEach(x => x.Resume());
    }

    public void Pause()
    {
        _activePlaybacks.ForEach(x => x.Pause());
    }

    public void Stop()
    {
        _activePlaybacks.ForEach(x => x.Stop());
        _activePlaybacks.Clear();
    }
    
    private void EnsurePlaybackLimit()
    {
        TrimPlaybacks();
        if (_activePlaybacks.Count > MaxPolyphony)
        {
            var playback = _activePlaybacks[0];
            playback.Stop();
            _activePlaybacks.RemoveAt(0);
        }
    }
    
    private void TrimPlaybacks()
    {
        _activePlaybacks.RemoveAll(x => !x.Active);
    }
}

public static class SoundPlayerEx
{
    public static SoundPlayer UseSoundPlayer(this INodeInit self, string busName)
    {
        var bus = self.UseAudioBus(busName);
        var player = new SoundPlayer(bus);
        self.OnLateCleanup(() => player.Stop());
        return player;
    }
}