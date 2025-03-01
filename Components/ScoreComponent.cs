using Godot;

[GlobalClass]
public partial class ScoreComponent : Node
{
    [Export] public GameStats GameStats { get; set; }
    [Export] public int AdjustAmount { get; set; } = 5;

    public void AdjustScore(int amount = -1)
    {
        if (amount == -1)
            amount = AdjustAmount;

        GameStats.Score += amount;
    }
}
