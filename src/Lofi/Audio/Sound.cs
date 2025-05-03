using System.Diagnostics;
using Raylib_cs;

namespace Lofi.Audio;

public class Sound(_Sound origin) : IDisposable
{
    private bool _disposed;
    private readonly List<Alias> _aliases = [];
    private readonly SoundPlayback _playback = new(origin);

    public SoundPlayback Playback => GetPlayback();

    public Alias CreateAlias()
    {
        var alias = new Alias(Raylib.LoadSoundAlias(origin));
        _aliases.Add(alias);
        return alias;
    }

    public void SetVolume(Decibels volume)
    {
        Raylib.SetSoundVolume(origin, volume.Linear);
    }

    /// <summary>Set pitch for a sound (1.0 is base level)</summary>
    public void SetPitch(float pitch)
    {
        Debug.Assert(!_disposed);
        Raylib.SetSoundPitch(origin, pitch);
    }

    /// <summary>Set pan for a sound (0.5 is center)</summary>
    public void SetPan(float pan)
    {
        Debug.Assert(!_disposed);
        Raylib.SetSoundPan(origin, pan);
    }

    public void Dispose()
    {
        Debug.Assert(!_disposed);
        _disposed = true;
        GC.SuppressFinalize(this);
        _aliases.ForEach(x => x.EnsureDisposed());
        Raylib.UnloadSound(origin);
    }

    private SoundPlayback GetPlayback()
    {
        Debug.Assert(!_disposed);
        return _playback;
    }

    public sealed class Alias(_Sound sound) : IDisposable
    {
        public SoundPlayback Playback => GetPlayback();

        private readonly SoundPlayback _playback = new(sound);
        private bool _disposed;

        public void Dispose()
        {
            Debug.Assert(!_disposed);
            Raylib.UnloadSoundAlias(sound);
            _disposed = true;
        }

        public void EnsureDisposed()
        {
            if (_disposed) return;
            Dispose();
        }

        private SoundPlayback GetPlayback()
        {
            Debug.Assert(!_disposed);
            return _playback;
        }
    }
}

public class SoundPlayback(_Sound sound)
{
    public bool Running => Raylib.IsSoundPlaying(sound);
    public bool Paused { get; private set; }

    public bool Active => Running || Paused;

    public void Play()
    {
        Raylib.PlaySound(sound);
    }

    public void Pause()
    {
        Paused = true;
        Raylib.PauseSound(sound);
    }

    public void Resume()
    {
        Paused = false;
        Raylib.ResumeSound(sound);
    }

    public void Stop()
    {
        Paused = false;
        Raylib.StopSound(sound);
    }
}
