using Godot;

public partial class WeaponData : Node
{
	// Define constants for weapon spawn locations
	public const int LeftMuzzle = 0;
	public const int RightMuzzle = 1;
	public const int BothMuzzles = 2;
	public const int CenterCannon = 3;

	// Export the integer with a dropdown in the Godot Editor
	[Export(PropertyHint.Enum, "Left Muzzle,Right Muzzle,Both Muzzles,Center Cannon")]
	public int spawnLocation = LeftMuzzle; // Default to LeftMuzzle
}

