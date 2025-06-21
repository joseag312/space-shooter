using Godot;
using System;

[GlobalClass]
public partial class AutoMusicTriggerComponent : Node
{
    public void PlayMusic()
    {
        G.MS.Play();
    }

    public void StopMusic()
    {
        G.MS.Stop();
    }

    public void FadeInMusic(float duration = 1.0f, float targetDb = -6)
    {
        G.MS.FadeIn(duration, targetDb);
    }

    public void FadeOutMusic(float duration = 1.0f)
    {
        G.MS.FadeOut(duration);
    }

    public void SetVolume(float db)
    {
        G.MS.SetVolumeDb(db);
    }
}
