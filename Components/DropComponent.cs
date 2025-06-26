using Godot;
using System;

[GlobalClass]
public partial class DropComponent : Node
{
    [Export] public DestroyedComponent DestroyedComponent;
    [Export] public WeightedDropTable DropTable;
    [Export] public DropSourceType Type = DropSourceType.Common;

    public override void _Ready()
    {
        DestroyedComponent.Destroyed += TrySpawnDrop;
    }

    public void TrySpawnDrop(Vector2 position)
    {
        if (DropTable == null || DropTable.Entries.Count == 0)
            return;

        float totalChance = 0f;
        foreach (var entryObj in DropTable.Entries)
        {
            if (entryObj is WeightedDropEntry entry && entry.DropScene != null)
                totalChance += entry.DropChance;
        }

        float roll = GD.Randf() * totalChance;
        float cumulative = 0f;

        foreach (var entryObj in DropTable.Entries)
        {
            if (entryObj is not WeightedDropEntry entry || entry.DropScene == null)
                continue;

            cumulative += entry.DropChance;

            if (roll <= cumulative)
            {
                var drop = entry.DropScene.Instantiate() as Node2D;
                if (drop == null)
                {
                    GD.PrintErr($"ERROR: DropComponent - Failed to instantiate drop from {entry.DropScene.ResourcePath}");
                    break;
                }

                drop.GlobalPosition = position;

                if (drop is IDropAmount dropAmount)
                {
                    int amount = (int)(GD.Randi() % (entry.MaxAmount - entry.MinAmount + 1) + entry.MinAmount);

                    var context = new DropContext
                    {
                        Level = G.GS.CurrentLevel,
                        CurrencyMultiplier = G.GS.CurrencyMultiplier,
                        Karma = G.GS.Karma,
                        SourceType = Type
                    };

                    dropAmount.SetAmount(amount, context);
                }

                AddDropDeferred(drop);
                break;
            }
        }
    }

    private void AddDropDeferred(Node drop)
    {
        var root = GetParent().GetParent().GetParent();

        if (root == null)
        {
            GD.PrintErr("ERROR: DropComponent - GetParent() returned null. Drop will not be added.");
            return;
        }

        if (!IsInstanceValid(drop))
        {
            GD.PrintErr("ERROR: DropComponent - Drop instance is invalid or already freed.");
            return;
        }

        GD.Print($"DEBUG: DropComponent - Deferring drop '{drop.Name}' into '{root.Name}'");
        root.CallDeferred("add_child", drop);
    }

}
