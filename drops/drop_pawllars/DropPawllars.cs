using Godot;
using System;

[GlobalClass]
public partial class DropPawllars : DropBase
{
    [Export] public int Amount = 10;

    public override void HandlePickup(HitboxComponent hitboxComponent)
    {
        G.GS.Pawllars += Amount;

        if (DropTextScene != null)
        {
            var dropTextInstance = DropTextScene.Instantiate<DropTextWhite>();
            dropTextInstance.Initialize(Amount);
            dropTextInstance.Position = GlobalPosition;

            GetTree().CurrentScene.AddChild(dropTextInstance);
        }
        else
        {
            GD.PrintErr("ERROR: DropMewnits - DropTextScene not assigned");
        }
    }
}
