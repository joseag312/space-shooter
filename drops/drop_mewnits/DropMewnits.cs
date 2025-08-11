#nullable enable
using Godot;
using System;

[GlobalClass]
public partial class DropMewnits : DropBase, IDropAmount
{
    [Export] public int Amount = 10;

    public void SetAmount(int amount, DropContext? context = null)
    {
        float multiplier = 1f;

        if (context != null)
        {
            multiplier *= 1f + (context.LevelMultiplier * 0.05f);
            multiplier *= 1f + (context.Karma * 0.01f);
            switch (context.SourceType)
            {
                case DropSourceType.Rare:
                    multiplier *= 1.5f;
                    break;
                case DropSourceType.Epic:
                    multiplier *= 2.0f;
                    break;
                case DropSourceType.Legend:
                    multiplier *= 3.0f;
                    break;
                case DropSourceType.Boss:
                    multiplier *= 5.0f;
                    break;
                case DropSourceType.Common:
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
        G.GS.Mewnits += Amount;

        if (DropTextScene != null)
        {
            var dropTextInstance = DropTextScene.Instantiate<DropTextWhite>();
            dropTextInstance.Initialize(Amount);
            dropTextInstance.GlobalPosition = GlobalPosition;

            if (EffectContainer != null)
            {
                EffectContainer.AddChild(dropTextInstance);
            }
            else
            {
                GD.PrintErr("ERROR: DropMewnits - EffectContainer not assigned, using CurrentScene");
                GetTree().CurrentScene.AddChild(dropTextInstance);
            }
        }
        else
        {
            GD.PrintErr("ERROR: DropMewnits - DropTextScene not assigned");
        }
    }
}
