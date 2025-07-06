using Godot;
using System;

public partial class HUDCurrency : Control
{
    [Export] public Label MewnitsLabel { get; set; }
    [Export] public Label PawllarsLabel { get; set; }

    private bool _isConnected;

    public override void _Ready()
    {
        UpdateValues();

        if (!_isConnected)
        {
            G.GS.CurrencyChanged += UpdateValues;
            _isConnected = true;
        }
    }

    private void UpdateValues()
    {
        MewnitsLabel.Text = $"{G.GS.Mewnits}";
        PawllarsLabel.Text = $"{G.GS.Pawllars}";
    }

    public override void _ExitTree()
    {
        if (_isConnected)
        {
            G.GS.CurrencyChanged -= UpdateValues;
            _isConnected = false;
        }
    }
}
