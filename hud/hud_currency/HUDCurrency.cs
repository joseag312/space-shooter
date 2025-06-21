using Godot;
using System;

public partial class HUDCurrency : Control
{
    [Export] public Label MewnitsLabel { get; set; }
    [Export] public Label PawllarsLabel { get; set; }

    public override void _Ready()
    {
        UpdateValues();
        G.GS.CurrencyChanged += UpdateValues;
    }

    private void UpdateValues()
    {
        MewnitsLabel.Text = $"{G.GS.Mewnits}";
        PawllarsLabel.Text = $"{G.GS.Pawllars}";
    }
}
