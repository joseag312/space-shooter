using Godot;
using System;

[GlobalClass]
public partial class DropHealth : DropBase
{
    [Export] public int HealAmount = 10;

    public override void HandlePickup(HitboxComponent hitboxComponent)
    {
        if (hitboxComponent.Owner is Ship ship)
        {
            ship.Heal(HealAmount);
        }
    }
}

