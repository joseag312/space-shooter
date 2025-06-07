using Godot;
using System;
using System.Threading.Tasks;

public partial class MenuOpening : Control
{
    [Export] public AnimationPlayer AnimationPlayer { get; set; }

    public override void _Ready()
    {
        AutoShipStats.Instance.Load();
        AutoGameStats.Instance.Load();
        PlayOpening();
    }

    public void PlayOpening()
    {
        AnimationPlayer.Play("MenuOpening/Opening");
    }

    public async Task MainMenuLoad()
    {
        await AutoGameFlow.Instance.FadeToSceneBasic(AutoGameFlow.Instance.MenuMainScene);
    }

    public void MainMenuLoadWrapper()
    {
        _ = MainMenuLoad();
    }
}
