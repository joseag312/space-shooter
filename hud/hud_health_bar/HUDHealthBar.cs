using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class HUDHealthBar : Control
{
	[Export] public TextureProgressBar HealthBar { get; set; }
	[Export] public TextureProgressBar BackgroundBar { get; set; }
	[Export] public Label HealthValue { get; set; }
	[Export] private float _lostHealthDuration = 0.5f;

	private StatsComponent _statsComponent;
	private Tween _tween;

	public override void _Ready()
	{
		_ = FadeIn(this);

		Node2D ship = GetTree().CurrentScene.GetNodeOrNull<Node2D>("Ship");
		if (ship != null)
		{
			_statsComponent = ship.GetNodeOrNull<StatsComponent>("StatsComponent");
			ship.Connect(nameof(Ship.HealthChanged), new Callable(this, nameof(OnHealthChanged)));
		}

		if (_statsComponent != null)
		{
			HealthBar.MaxValue = _statsComponent.MaxHealth;
			HealthBar.Value = _statsComponent.Health;
			HealthValue.Text = $"{_statsComponent.Health}/{_statsComponent.MaxHealth}";
		}

		HealthBar.Size = new Vector2(200, 200);
		Position = new Vector2(20, 40);
	}

	private void OnHealthChanged(int damage)
	{
		if (_statsComponent == null) return;

		HealthBar.Value = Mathf.Max(_statsComponent.Health, 0);
		HealthValue.Text = $"{HealthBar.Value}/{HealthBar.MaxValue}";

		var gradientUnder = new Gradient();
		var hurtColor = new Color(145f / 200f, 125f / 255f, 230f / 255f, 1f);
		gradientUnder.SetColor(0, hurtColor);
		gradientUnder.SetColor(1, hurtColor);

		var gradientTexture = new GradientTexture2D
		{
			Gradient = gradientUnder,
			Width = (int)(BackgroundBar.TextureUnder.GetWidth() * ((float)damage / HealthBar.MaxValue)),
			Height = BackgroundBar.TextureUnder.GetHeight()
		};

		var lostHealthBar = new TextureProgressBar
		{
			TextureProgress = gradientTexture,
			MinValue = HealthBar.MinValue,
			MaxValue = HealthBar.MaxValue,
			Value = HealthBar.MaxValue,
			TextureProgressOffset = new Vector2(BackgroundBar.TextureUnder.GetWidth() * (_statsComponent.Health / (float)HealthBar.MaxValue), 0f),
			ZIndex = 5
		};

		HealthBar.GetParent().AddChild(lostHealthBar);
		lostHealthBar.GlobalPosition = HealthBar.GlobalPosition;

		var tween = GetTree().CreateTween();
		if (_statsComponent.Health > 0)
		{
			float startOffsetX = lostHealthBar.TextureProgressOffset.X;
			float endOffsetX = startOffsetX - lostHealthBar.TextureProgress.GetWidth();
			tween.TweenProperty(lostHealthBar, "texture_progress_offset:x", endOffsetX, 0.4f)
				 .SetTrans(Tween.TransitionType.Linear)
				 .SetEase(Tween.EaseType.Out);
		}
		else
		{
			tween.TweenProperty(lostHealthBar, "modulate", new Color(1f, 1f, 2f, 1f), 0.1f)
				 .SetTrans(Tween.TransitionType.Cubic)
				 .SetEase(Tween.EaseType.Out);
			tween.TweenProperty(lostHealthBar, "modulate", new Color(2f, 2f, 2f, 1f), 0.1f)
				 .SetDelay(0.1f)
				 .SetTrans(Tween.TransitionType.Bounce)
				 .SetEase(Tween.EaseType.Out);
			tween.TweenProperty(lostHealthBar, "modulate", new Color(0.5f, 0.5f, 0.5f, 1f), 0.1f)
				 .SetDelay(0.2f)
				 .SetTrans(Tween.TransitionType.Cubic)
				 .SetEase(Tween.EaseType.Out);
			tween.TweenProperty(lostHealthBar, "modulate:a", 0f, 0.4f)
				 .SetDelay(0.3f)
				 .SetTrans(Tween.TransitionType.Linear);
		}

		tween.TweenCallback(Callable.From(() =>
		{
			if (IsInstanceValid(lostHealthBar)) lostHealthBar.QueueFree();
		})).SetDelay(0.6f);
	}

	public async Task FadeIn(CanvasItem root, float duration = 0.5f)
	{
		var canvasItems = new List<CanvasItem>();
		CollectCanvasItems(root, canvasItems);

		foreach (var item in canvasItems)
		{
			item.Visible = false;
			item.Modulate = new Color(item.Modulate.R, item.Modulate.G, item.Modulate.B, 0f);
		}

		await ToSignal(GetTree(), "process_frame");

		foreach (var item in canvasItems)
			item.Visible = true;

		int completed = 0;
		int total = canvasItems.Count;

		foreach (var item in canvasItems)
		{
			var tween = GetTree().CreateTween();
			tween.TweenProperty(item, "modulate:a", 1f, duration);
			tween.Finished += () => completed++;
		}

		while (completed < total)
			await ToSignal(GetTree(), "process_frame");
	}

	private void CollectCanvasItems(Node node, List<CanvasItem> list)
	{
		if (node is CanvasItem item)
			list.Add(item);

		foreach (var child in node.GetChildren())
			CollectCanvasItems(child, list);
	}
}
