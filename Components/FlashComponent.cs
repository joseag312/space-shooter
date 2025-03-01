using Godot;

[GlobalClass]
public partial class FlashComponent : Node
{
    private static readonly Material FlashMaterial = GD.Load<Material>("res://effects/white_flash_material.tres");

    [Export] public CanvasItem Sprite { get; set; }
    [Export] public float FlashDuration { get; set; } = 0.2f;

    private Material _originalSpriteMaterial;
    private Timer _timer = new Timer();

    public override void _Ready()
    {
        AddChild(_timer);
        _originalSpriteMaterial = Sprite.Material;
    }

    public async void Flash()
    {
        Sprite.Material = FlashMaterial;
        _timer.Start(FlashDuration);
        await ToSignal(_timer, "timeout");
        Sprite.Material = _originalSpriteMaterial;
    }
}
