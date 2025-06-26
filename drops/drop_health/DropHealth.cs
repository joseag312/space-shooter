#nullable enable
using Godot;
using System;

[GlobalClass]
public partial class DropHealth : DropBase, IDropAmount
{
    [Export] public int HealAmount = 10;

    public void SetAmount(int amount, DropContext? context = null)
    {
        HealAmount = amount;
    }

    public override void HandlePickup(HitboxComponent hitboxComponent)
    {
        if (hitboxComponent.Owner is Ship ship)
        {
            ship.Heal(HealAmount);
        }
    }
}

