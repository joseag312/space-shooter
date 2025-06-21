using Godot;
using System;

[GlobalClass]
public partial class AutoSFXTriggerComponent : Node
{
    public void Play(string soundName)
    {
        G.SFX.Play(soundName);
    }

    public void OIIAFast()
    {
        G.SFX.Play("oiia_fast");
    }

    public void OIIASlow()
    {
        G.SFX.Play("oiia_slow");
    }

    public void OIIADeath()
    {
        G.SFX.Play("oiia_death");
    }

    public void Meow()
    {
        G.SFX.Play("meow");
    }

    public void SetVolume(float db)
    {
        G.SFX.SetVolume(db);
    }
}
