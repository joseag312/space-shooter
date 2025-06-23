using Godot;
using System;

[GlobalClass]
public partial class DropMewnits : DropBase
{
    [Export] public int Amount = 10;

    public override void HandlePickup(HitboxComponent hitboxComponent)
    {
        G.GS.Mewnits += Amount;
    }
}
