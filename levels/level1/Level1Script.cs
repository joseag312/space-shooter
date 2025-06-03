using System;
using System.Threading.Tasks;
using Godot;

public partial class Level1Script : Node
{
	[Export] Ship ship;
	[Export] SpawnerComponent Enemy1Spawner;
	[Export] SpawnerComponent Enemy2Spawner;
	[Export] SpawnerComponent Enemy3Spawner;
	private double cleanupTimer = 0.0;
	private bool ShouldSpawn1;
	private bool ShouldSpawn2;
	private bool ShouldSpawn3;
	private int margin;
	private int leftBorder;
	private int rightBorder;
	private bool complete;

	public override void _Ready()
	{
		MenuBackground.Instance.UnblockInput();
		margin = 8;
		leftBorder = margin;
		rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width") - margin;
		levelScript();
	}

	public override void _Process(double delta)
	{
		cleanupTimer += delta;
		if (cleanupTimer >= 1.0)
		{
			cleanupTimer = 0;
			CleanupOffscreenEnemies();
		}
	}

	private async void levelScript()
	{
		await Task.Delay(3000);
		SpawnWave(Enemy2Spawner, 10, 15);
		await Task.Delay(3000);

		await Task.Delay(3000);
		ShouldSpawn1 = true;
		RecurrentSpawn1(Enemy1Spawner, 300);
		await Task.Delay(20000);
		ShouldSpawn1 = false;

		await Task.Delay(3000);
		SpawnWave(Enemy2Spawner, 10, 30);
		await Task.Delay(3000);

		await Task.Delay(500);
		ShouldSpawn1 = true;
		RecurrentSpawn1(Enemy1Spawner, 300);
		await Task.Delay(20000);
		ShouldSpawn1 = false;
		await Task.Delay(500);
	}

	public void SpawnWave(SpawnerComponent spawner, int count, int margin)
	{
		PackedScene enemyScene = spawner.Scene;
		if (enemyScene == null)
		{
			GD.PrintErr("ERROR: Level1Script - enemyScene is null.");
			return;
		}

		int spriteWidth = GetSpriteWidth(enemyScene);
		if (spriteWidth <= 0)
		{
			GD.PrintErr("ERROR: Level1Script - Could not retrieve sprite width.");
			return;
		}

		int screenCenter = (leftBorder + rightBorder) / 2;
		int totalSpacing = spriteWidth + margin;

		for (int i = 0; i < count; i++)
		{
			int offset = ((i - count / 2) * totalSpacing);
			Vector2 spawnPosition = new Vector2(screenCenter + offset, -120);
			Node2D enemy = enemyScene.Instantiate<Node2D>();
			enemy.GlobalPosition = spawnPosition;
			AddChild(enemy);
		}
	}


	public async void RecurrentSpawn1(SpawnerComponent spawner, int time)
	{
		while (ShouldSpawn1)
		{
			Vector2 position = new Vector2(GD.RandRange(leftBorder + margin, rightBorder - margin), -120);
			spawner.Spawn(position, GetParent());
			await Task.Delay(time);
		}
	}

	public async void RecurrentSpawn2(SpawnerComponent spawner, int time)
	{
		while (ShouldSpawn2)
		{
			Vector2 position = new Vector2(GD.RandRange(leftBorder + margin, rightBorder - margin), -120);
			spawner.Spawn(position, GetParent());
			await Task.Delay(time);
		}
	}

	public async void RecurrentSpawn3(SpawnerComponent spawner, int time)
	{
		while (ShouldSpawn3)
		{
			Vector2 position = new Vector2(GD.RandRange(leftBorder + margin, rightBorder - margin), -120);
			spawner.Spawn(position, GetParent());
			await Task.Delay(time);
		}
	}

	private int GetSpriteWidth(PackedScene enemyScene)
	{
		int spriteWidth = 0;
		Node2D tempEnemy = enemyScene.Instantiate<Node2D>();
		AnimatedSprite2D animatedSprite = tempEnemy.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		if (animatedSprite != null)
		{
			SpriteFrames frames = animatedSprite.SpriteFrames;
			if (frames != null && frames.HasAnimation("move"))
			{
				spriteWidth = (int)frames.GetFrameTexture("move", 0).GetWidth();
			}
		}

		tempEnemy.QueueFree();
		return spriteWidth;
	}

	private void CleanupOffscreenEnemies()
	{
		var viewportRect = GetViewport().GetVisibleRect();

		foreach (var child in GetTree().GetNodesInGroup("despawnable"))
		{
			if (child is Node2D node2D && !viewportRect.HasPoint(node2D.GlobalPosition))
			{
				node2D.QueueFree();
			}
		}
	}
}