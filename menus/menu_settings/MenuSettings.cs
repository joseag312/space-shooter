using Godot;
using System;
using System.Threading.Tasks;

public partial class MenuSettings : Control
{
    [Export] public VBoxContainer StartOverContainerVBox;
    [Export] public VBoxContainer PreferencesContainerVBox;

    // Preferences Section
    [Export] public HSlider OpacitySlider;
    [Export] public Label OpacityLabel;

    [Export] public HSlider VolumeSlider;
    [Export] public Label VolumeLabel;

    [Export] public HSlider SfxSlider;
    [Export] public Label SfxLabel;

    [Export] public Button MuteToggleButton;
    [Export] public Button StartOverButton;

    // Start Over Section
    [Export] public Button ConfirmStartOverButton;
    [Export] public Button CancelStartOverButton;

    [Export] public MenuFadeComponent FadeComponent;
    [Export] public MenuLoadComponent LoadComponent;

    public override void _Ready()
    {
        StartOverButton.Pressed += OnStartOverPressed;
        ConfirmStartOverButton.Pressed += OnConfirmStartOverPressed;
        CancelStartOverButton.Pressed += OnCancelStartOverPressed;
    }

    private async void OnStartOverPressed()
    {
        await FadeComponent.FadeOutAsync();
        PreferencesContainerVBox.Visible = false;
        StartOverContainerVBox.Visible = true;
        LoadComponent._Ready(); // Manual trigger to fade in again
    }

    private async void OnCancelStartOverPressed()
    {
        await FadeComponent.FadeOutAsync();
        StartOverContainerVBox.Visible = false;
        PreferencesContainerVBox.Visible = true;
        LoadComponent._Ready(); // Manual trigger to fade in again
    }

    private void OnConfirmStartOverPressed()
    {
        // TODO: Add your reset logic here
        GD.Print("DEBUG: MenuSettings - Start Over Confirmed");
    }
}
