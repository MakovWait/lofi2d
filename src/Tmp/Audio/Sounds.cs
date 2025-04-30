namespace Tmp.Audio;

/// <summary>
/// Takes ownership over provided sounds.
/// </summary>
public class Sounds(List<Sound> sounds) : IDisposable
{
    public Sounds(Sound sound) : this([sound])
    {
        
    }
    
    public Sound this[int idx] => sounds[idx];

    public int Count => sounds.Count;
    
    public Sound Random => this[System.Random.Shared.Next(sounds.Count)];
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        sounds.ForEach(x => x.Dispose());
    }
}