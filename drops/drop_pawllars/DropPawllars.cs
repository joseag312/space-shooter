#nullable enable
using Godot;
using System;

[GlobalClass]
public partial class DropPawllars : DropBase, IDropAmount
{
    [Export] public int Amount = 10;

    public void SetAmount(int amount, DropContext? context = null)
    {
        float multiplier = 1f;

        if (context != null)
        {
            multiplier *= 1f + (context.Level * 0.05f);
            multiplier *= 1f + (context.Karma * 0.01f);
            switch (context.SourceType)
            {
                case DropSourceType.Rare:
                    multiplier *= 1f;
                    break;
                case DropSourceType.Epic:
                    multiplier *= 1f;
                    break;
                case DropSourceType.Legend:
                    multiplier *= 1.5f;
                    break;
                case DropSourceType.Boss:
                    multiplier *= 2.5f;
                    break;
                case DropSourceType.Common:
                    multiplier *= 0f;
                    break;
                default:
                    break;
            }
            multiplier *= context.CurrencyMultiplier;
        }
        Amount = Mathf.RoundToInt(amount * multiplier);
        if (Amount < 1) Amount = 1;
    }

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
