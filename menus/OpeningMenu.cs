using Godot;
using System;

public partial class OpeningMenu : Control
{
    [Export] AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        Opening();
    }

    public void Opening()
    {
        animationPlayer.Play("OpeningMenu/Opening");
    }

    public void MainMenuLoad()
    {
        GetTree().ChangeSceneToFile("res://menus/main_menu.tscn");
    }
}
