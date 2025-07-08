using Godot;
using System;
using System.Collections.Generic;

public partial class HUDPowers : Control
{
    private List<PowerSlotUI> _powerSlots = new();

    public override void _Ready()
    {
        for (int i = 1; i <= 4; i++)
        {
            var slot = GetNode<Control>($"HBoxContainer/PowerUpContainer{i}");
            var ui = new PowerSlotUI(slot);
            _powerSlots.Add(ui);
        }

        UpdateWeaponsDisplay();
    }

    public void UpdateWeaponsDisplay()
    {
        var inv = G.WI;

        for (int i = 0; i < _powerSlots.Count; i++)
        {
            var state = inv.GetEquippedWeaponState(WeaponSlotNames.GetSpecialSlot(i + 1));
            _powerSlots[i].UpdateFromState(state);
        }
    }

    public override void _Process(double delta)
    {
        foreach (var slot in _powerSlots)
            slot.UpdateCooldownVisual();
    }

    private class PowerSlotUI
    {
        private readonly TextureRect _icon;
        private readonly Label _amountLabel;
        private readonly TextureRect _cooldownOverlay;
        private WeaponStateComponent _state;

        public PowerSlotUI(Control container)
        {
            _icon = container.GetNode<TextureRect>("PowerUpIconTextureRect");
            _amountLabel = container.GetNode<Label>("AmountLabel");
            _cooldownOverlay = container.GetNode<TextureRect>("CooldownOverlay");
        }

        public void UpdateFromState(WeaponStateComponent state)
        {
            _state = state;

            if (_state?.BaseData?.IconPath is { Length: > 0 } iconPath)
            {
                _icon.Texture = GD.Load<Texture2D>(iconPath);
            }
            else
            {
                _icon.Texture = null;
            }

            if (_state != null && _state.BaseData.SlotType == WeaponDataComponent.WeaponSlotType.Slot)
            {
                _amountLabel.Text = _state.CurrentAmount.ToString();
            }
            else
            {
                _amountLabel.Text = "";
            }

            UpdateCooldownVisual();
        }

        public void UpdateCooldownVisual()
        {
            if (_state?.EffectiveCooldown > 0f)
            {
                float ratio = Mathf.Clamp(_state.CooldownRemaining / _state.EffectiveCooldown, 0f, 1f);
                _cooldownOverlay.Modulate = new Color(1, 1, 1, ratio); // Fade in
            }
            else
            {
                _cooldownOverlay.Modulate = new Color(1, 1, 1, 0); // Hide
            }
        }
    }
}
