using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class PersistenceComponent : Node2D
{
	[Export] MoveComponent moveComponent;
	[Export] int PersistenceMiliseconds;
	private int margin;
	private int leftBorder;
	private int rightBorder;


	public override void _Ready()
	{
		leftBorder = 0;
		rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width");
		persist();
	}

	public async void persist()
	{
		float speedY = moveComponent.Velocity.Y;
		float distanceToLeft = GlobalPosition.X - leftBorder;
		float distanceToRight = rightBorder - GlobalPosition.X;
		float targetSpeedX = (distanceToLeft < distanceToRight) ? -speedY : speedY;

		await Task.Delay(3000);
		if (!IsInstanceValid(this)) return;
		moveComponent.Velocity = new Vector2(0, 0);

		await Task.Delay(PersistenceMiliseconds);
		if (!IsInstanceValid(this)) return;
		moveComponent.Velocity = new Vector2(targetSpeedX, 0);
	}
}
