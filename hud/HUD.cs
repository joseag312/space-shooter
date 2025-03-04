using Godot;

public partial class HUD : Control
{
	[Export] public TextureProgressBar healthBar;
	[Export] public Label healthValue;
	private StatsComponent shipStats;

	public override void _Ready()
	{
		Node2D ship = GetTree().CurrentScene.GetNodeOrNull<Node2D>("Ship");
		if (ship != null)
		{
			shipStats = ship.GetNodeOrNull<StatsComponent>("StatsComponent");
		}

		if (shipStats != null)
		{
			healthBar.MaxValue = shipStats.Health;
			healthBar.Value = shipStats.Health;
			healthValue.Text = shipStats.Health.ToString();
		}

		healthBar.Size = new Vector2(200, 200);
		Position = new Vector2(20, 10);
	}

	public override void _Process(double delta)
	{
		if (shipStats != null)
		{
			healthBar.Value = shipStats.Health;
			healthValue.Text = shipStats.Health.ToString();
		}
	}
}
