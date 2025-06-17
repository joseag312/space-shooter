using Godot;

[GlobalClass]
public partial class PortraitEntry : Resource
{
    [Export] public string Name { get; set; }
    [Export] public Texture2D Portrait { get; set; }
}
