using Godot;
using System;

public partial class HUDMain : CanvasLayer
{
    [Export] public HUDHealthBar HealthBar { get; set; }
    [Export] public HUDDialogSystem Dialog { get; set; }
    [Export] public HUDCurrency Currency { get; set; }

    public override void _Ready()
    {

    }

}

