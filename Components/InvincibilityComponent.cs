using Godot;

[GlobalClass]
public partial class InvincibilityComponent : Node
{
	private bool _isInvincible = false;
	private Timer invincibilityTimer;
	private Tween flashTween; // Store tween to stop it later

	[Export] public float InvincibilityDuration { get; set; } = 1.0f;
	[Export] public Node2D OwnerNode { get; set; }

	public override void _Ready()
	{
		if (OwnerNode == null)
			OwnerNode = GetParent<Node2D>();

		invincibilityTimer = new Timer
		{
			WaitTime = InvincibilityDuration,
			OneShot = true
		};

		AddChild(invincibilityTimer);
		invincibilityTimer.Timeout += OnInvincibilityEnd;
	}

	public bool IsInvincible
	{
		get => _isInvincible;
		private set
		{
			_isInvincible = value;

			if (OwnerNode != null)
			{
				if (_isInvincible)
					FlashEffect(true);
				else
					FlashEffect(false);
			}
		}
	}

	public void StartInvincibility()
	{
		if (!_isInvincible)
		{
			IsInvincible = true;
			invincibilityTimer.Start();
		}
	}

	private void OnInvincibilityEnd()
	{
		IsInvincible = false;
	}

	private void FlashEffect(bool enable)
	{
		if (OwnerNode == null) return;

		if (flashTween != null && flashTween.IsRunning())
			flashTween.Kill();

		if (enable)
		{
			flashTween = OwnerNode.GetTree().CreateTween();
			flashTween.SetLoops((int)(InvincibilityDuration / 0.2f));
			flashTween.TweenProperty(OwnerNode, "modulate", new Color(1.5f, 1.5f, 1.5f, 0.5f), 0.1f);
			flashTween.TweenProperty(OwnerNode, "modulate", new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.1f);
		}
		else
		{
			OwnerNode.Modulate = new Color(1, 1, 1, 1);
		}
	}

}
