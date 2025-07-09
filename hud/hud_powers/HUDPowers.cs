using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class HUDPowers : Control
{
    [Export] public TextureRect BigLaserButton;
    [Export] public TextureRect BigLaserOverlay;
    [Export] public TextureRect PowerUp1Container;
    [Export] public TextureRect PowerUp1Icon;
    [Export] public TextureRect PowerUp1Overlay;
    [Export] public Label PowerUp1Amount;
    [Export] public TextureRect PowerUp2Container;
    [Export] public TextureRect PowerUp2Icon;
    [Export] public TextureRect PowerUp2Overlay;
    [Export] public Label PowerUp2Amount;
    [Export] public TextureRect PowerUp3Container;
    [Export] public TextureRect PowerUp3Icon;
    [Export] public TextureRect PowerUp3Overlay;
    [Export] public Label PowerUp3Amount;
    [Export] public TextureRect PowerUp4Container;
    [Export] public TextureRect PowerUp4Icon;
    [Export] public TextureRect PowerUp4Overlay;
    [Export] public Label PowerUp4Amount;
    [Export] public float opacity = 150;

    private Color _baseColor;
    private Color _bigOverlayColor;
    private Color _overlayColor;
    private Color _flashColor;

    public override void _Ready()
    {
        _baseColor = new Color(220f / 255f, 220f / 255f, 220f / 255f, opacity / 255f);
        _flashColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, opacity / 255f);
        _overlayColor = new Color(150f / 255f, 0f / 255f, 155f / 255f, opacity / 255f);

        BigLaserButton.Modulate = _baseColor;
        BigLaserOverlay.Modulate = _overlayColor;

        PowerUp1Container.Modulate = _baseColor;
        PowerUp1Icon.Modulate = _baseColor;
        PowerUp1Overlay.Modulate = _overlayColor;
        PowerUp1Amount.Modulate = _baseColor;

        PowerUp2Container.Modulate = _baseColor;
        PowerUp2Icon.Modulate = _baseColor;
        PowerUp2Overlay.Modulate = _overlayColor;
        PowerUp2Amount.Modulate = _baseColor;

        PowerUp3Container.Modulate = _baseColor;
        PowerUp3Icon.Modulate = _baseColor;
        PowerUp3Overlay.Modulate = _overlayColor;
        PowerUp3Amount.Modulate = _baseColor;

        PowerUp4Container.Modulate = _baseColor;
        PowerUp4Icon.Modulate = _baseColor;
        PowerUp4Overlay.Modulate = _overlayColor;
        PowerUp4Amount.Modulate = _baseColor;

        BigLaserOverlay.Visible = false;
        PowerUp1Overlay.Visible = false;
        PowerUp2Overlay.Visible = false;
        PowerUp3Overlay.Visible = false;
        PowerUp4Overlay.Visible = false;

        SetWeaponsDisplay();
    }

    public override void _Process(double delta)
    {

    }

    private void OnWeaponFired(int weaponSlot, WeaponStateComponent weapon)
    {
        var slotType = weapon.BaseData.SlotType;

        switch (weaponSlot)
        {
            case 1:
                _ = StartCooldown(weapon.EffectiveCooldown, BigLaserButton, BigLaserOverlay);
                break;

            case 2:
                weapon.CurrentAmount -= 1;
                _ = StartCooldown(weapon.EffectiveCooldown, PowerUp1Icon, PowerUp1Overlay);
                UpdateAmount(weapon, PowerUp1Amount);
                break;
            case 3:
                weapon.CurrentAmount -= 1;
                _ = StartCooldown(weapon.EffectiveCooldown, PowerUp2Icon, PowerUp2Overlay);
                UpdateAmount(weapon, PowerUp2Amount);
                break;
            case 4:
                weapon.CurrentAmount -= 1;
                _ = StartCooldown(weapon.EffectiveCooldown, PowerUp3Icon, PowerUp3Overlay);
                UpdateAmount(weapon, PowerUp3Amount);
                break;
            case 5:
                weapon.CurrentAmount -= 1;
                _ = StartCooldown(weapon.EffectiveCooldown, PowerUp4Icon, PowerUp4Overlay);
                UpdateAmount(weapon, PowerUp4Amount);
                break;
            default:
                GD.PrintErr($"ERROR: HUDPowers - Unknown slot type {slotType} for weapon {weapon.BaseData.Key}");
                break;
        }
    }

    private void SetWeaponsDisplay()
    {
        var inv = G.WI;

        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special1), PowerUp1Icon, PowerUp1Amount, PowerUp1Overlay);
        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special2), PowerUp2Icon, PowerUp2Amount, PowerUp2Overlay);
        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special3), PowerUp3Icon, PowerUp3Amount, PowerUp3Overlay);
        SetSlotDisplay(inv.GetEquippedWeaponState(WeaponSlotNames.Special4), PowerUp4Icon, PowerUp4Amount, PowerUp4Overlay);
    }

    private void SetSlotDisplay(WeaponStateComponent state, TextureRect icon, Label amountLabel, TextureRect overlay)
    {
        if (state?.BaseData == null)
        {
            icon.Texture = null;
            amountLabel.Text = "";
            overlay.Modulate = new Color(1, 1, 1, 0);
            overlay.Visible = false;
            return;
        }

        if (state?.BaseData?.IconPath is { Length: > 0 } iconPath)
            icon.Texture = GD.Load<Texture2D>(iconPath);
        else
            icon.Texture = null;

        if (state.BaseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot)
            amountLabel.Text = state.CurrentAmount.ToString();
        else
            amountLabel.Text = "";

        overlay.Modulate = new Color(1, 1, 1, 0);
        overlay.Visible = false;
    }

    public void SetWeaponManager(WeaponManagerComponent weaponManager)
    {
        weaponManager.Connect(WeaponManagerComponent.SignalName.WeaponFired, new Callable(this, nameof(OnWeaponFired)));
    }

    private void UpdateAmount(WeaponStateComponent state, Label amountLabel)
    {
        if (state != null && state.BaseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot)
            amountLabel.Text = state.CurrentAmount.ToString();
        else
            amountLabel.Text = "";
    }

    private void UpdateAllAmounts()
    {
        var inv = G.WI;

        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special1), PowerUp1Amount);
        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special2), PowerUp2Amount);
        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special3), PowerUp3Amount);
        UpdateAmount(inv.GetEquippedWeaponState(WeaponSlotNames.Special4), PowerUp4Amount);
    }

    private async Task StartCooldown(float duration, TextureRect icon, TextureRect overlay)
    {
        icon.Modulate = _baseColor;

        Tween tween1 = GetTree().CreateTween();
        tween1.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        tween1.Chain().TweenProperty(icon, "modulate", _flashColor, 0.1);
        tween1.Chain().TweenProperty(icon, "modulate", _baseColor, 0.1);
        await Task.Delay(200);

        Tween tween2 = GetTree().CreateTween();
        tween2.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        overlay.Visible = true;
        overlay.Modulate = _overlayColor;
        tween2.Chain().TweenProperty(overlay, "modulate:a", 0.1, duration - 0.4);
        tween2.Chain().TweenProperty(icon, "modulate", _flashColor, 0.1);
        tween2.Chain().TweenProperty(icon, "modulate", _baseColor, 0.1);
        tween2.Chain().TweenCallback(Callable.From(() =>
        {
            overlay.Visible = false;
            overlay.Modulate = _baseColor;
        }));
    }
}
