using Godot;

public class DropContext
{
    public int Level;
    public int Karma;
    public float CurrencyMultiplier = 1f;
    public DropSourceType SourceType = DropSourceType.Common;
}

public enum DropSourceType
{
    Common,
    Rare,
    Epic,
    Legend,
    Boss
}