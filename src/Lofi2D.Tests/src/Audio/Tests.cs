using Lofi2D.Audio;

namespace Lofi2D.Tests.Audio;

public class AudioBusLayoutTests
{
    [Test]
    public void AudioBusesAreSealedOnLayoutInit()
    {
        var layout = new AudioBusLayout([
            new AudioBus("Sfx"), 
            new AudioBus("Music")
            {
                Volume = Decibels.Mute,
                Children = []
            }, 
        ]);

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            layout.Master.Add(new AudioBus("bus"));
        });
        Assert.That(exception.Message, Is.EqualTo("Cannot add bus to sealed AudioBus 'Master'"));
    }
    
    [Test]
    public void AudioBusNamesAreUnique()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = new AudioBusLayout([
                new AudioBus("Sfx"), 
                new AudioBus("Sfx"),
            ]);
        });
        Assert.That(exception.Message, Is.EqualTo("AudioBus with name 'Sfx' already exists in the registry"));
    }
    
    [Test]
    public void AudioBusNamesAreUniqueRecursive()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = new AudioBusLayout([
                new AudioBus("Sfx")
                {
                    new AudioBus("Sfx")
                }
            ]);
        });
        Assert.That(exception.Message, Is.EqualTo("AudioBus with name 'Sfx' already exists in the registry"));
    }
    
    [Test]
    public void GetBusByName()
    {
        var indoorSfx = new AudioBus("IndoorSfx");
        var musicBus = new AudioBus("Music")
        {
            new AudioBus("BackgroundMusic"),
        };
        var masterBus = new MasterBus()
        {
            musicBus,
            new AudioBus("Sfx")
            {
                indoorSfx
            }
        };
        var layout = new AudioBusLayout(masterBus);
        Assert.Multiple(() =>
        {
            Assert.That(layout.GetBus("Master") == masterBus);
            Assert.That(layout.GetBus("Music") == musicBus);
            Assert.That(layout.GetBus("IndoorSfx") == indoorSfx);
        });
    }
}
