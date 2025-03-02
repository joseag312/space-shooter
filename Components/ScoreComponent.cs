using Godot;

[GlobalClass]
public partial class ScoreComponent : Node
{
    [Export] public int AdjustAmount { get; set; } = 5;

    public void AdjustScore(int amount = -1)
    {
        if (amount == -1)
            amount = AdjustAmount;

        if (GameStats.Instance != null)
        {
            GameStats.Instance.Score += amount;
        }
        else
        {
            GD.PrintErr("GameStats singleton not initialized.");
        }
    }
}
