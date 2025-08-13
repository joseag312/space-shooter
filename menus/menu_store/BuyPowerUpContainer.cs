using Godot;
using System;

public partial class BuyPowerUpContainer : Control
{
    [Export] public Key Key;
    [Export] public Button Add1Button;
    [Export] public Button Minus1Button;
    [Export] public Button Add10Button;
    [Export] public Button Minus10Button;
    [Export] public Label CurrentLabel;
    [Export] public Label AmountLabel;
    [Export] public Label PriceLabel;
    [Export] public Button ConfirmButton;

    private WeaponStateComponent _state;
    private int PurchaseAmount;

    public override void _Ready()
    {
        Add1Button.Pressed += OnAdd1Pressed;
        Minus1Button.Pressed += OnMinus1Pressed;
        Add10Button.Pressed += OnAdd10Pressed;
        Minus10Button.Pressed += OnMinus10Pressed;
    }

    public void LoadContainer(string key)
    {
        _state = G.WI.GetWeaponState(key);
        CurrentLabel.Text = _state.CurrentAmount.ToString();
        AmountLabel.Text = "";
        PriceLabel.Text = "0";
    }

    public void OnAdd1Pressed()
    {
        if (_state == null) return;
        if (PurchaseAmount + 1 > _state.EffectiveMaxAmount - _state.CurrentAmount)
        {
            PurchaseAmount = _state.EffectiveMaxAmount - _state.CurrentAmount;
        }
        else
        {
            PurchaseAmount = PurchaseAmount + 1;
        }
        AmountLabel.Text = "+" + PurchaseAmount;
    }

    public void OnMinus1Pressed()
    {
        if (_state == null) return;
        PurchaseAmount = PurchaseAmount - 1 < 0 ? 0 : PurchaseAmount - 1;
        if (PurchaseAmount > 0)
        {
            AmountLabel.Text = "+" + PurchaseAmount;
        }
        else
        {
            AmountLabel.Text = "";
        }
    }

    public void OnAdd10Pressed()
    {
        if (_state == null) return;
        if (PurchaseAmount + 10 > _state.EffectiveMaxAmount - _state.CurrentAmount)
        {
            PurchaseAmount = _state.EffectiveMaxAmount - _state.CurrentAmount;
        }
        else
        {
            PurchaseAmount = PurchaseAmount + 10;
        }
        AmountLabel.Text = "+" + PurchaseAmount;
    }

    public void OnMinus10Pressed()
    {
        if (_state == null) return;
        PurchaseAmount = PurchaseAmount - 10 < 0 ? 0 : PurchaseAmount - 10;
        if (PurchaseAmount > 0)
        {
            AmountLabel.Text = "+" + PurchaseAmount;
        }
        else
        {
            AmountLabel.Text = "";
        }
    }

}
