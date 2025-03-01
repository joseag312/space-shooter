using Godot;

[GlobalClass]
public partial class HurtboxComponent : Area2D
{
    [Signal] public delegate void HurtEventHandler(HitboxComponent hitbox);

    private bool _isInvincible = false;

    public bool IsInvincible
    {
        get => _isInvincible;
        set
        {
            _isInvincible = value;
            foreach (var child in GetChildren())
            {
                if (child is not CollisionShape2D && child is not CollisionPolygon2D)
                    continue;

                child.SetDeferred("disabled", _isInvincible);
            }
        }
    }
}
