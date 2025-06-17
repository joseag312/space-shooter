using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class HUDDialogSystem : CanvasLayer
{
    [Export] public PortraitLibrary PortraitLibrary { get; set; }

    // Dialog Setup
    [Export] public Control DialogBox { get; set; }
    [Export] public TextureRect DialogPortrait { get; set; }
    [Export] public Label DialogName { get; set; }
    [Export] public Label DialogText { get; set; }
    [Export] public Button NextButton { get; set; }
    [Export] public CenterContainer NextButtonContainer { get; set; }

    // PopUp Setup
    [Export] public Control PopUpBox { get; set; }
    [Export] public TextureRect PopUpPortrait { get; set; }
    [Export] public Label PopUpText { get; set; }

    // Button Setup
    [Export] public Control PromptButtonBox { get; set; }
    [Export] public Button SureButton { get; set; }
    [Export] public Button Option1Button { get; set; }
    [Export] public Button Option2Button { get; set; }

    private bool _skipTyping = false;
    private TaskCompletionSource _nextPressedTcs;
    private TaskCompletionSource<string> _promptPressedTcs = new TaskCompletionSource<string>();

    public override void _Ready()
    {
        NextButton.Pressed += OnNextPressed;
        DialogBox.Visible = false;
    }

    public async Task MessageSequence()
    {

        await FirstMessage("SpinGod", "Wussup");
        await Message("SpinGod", "I spin, what do you do brah?");
        await HandleChoice(
            await MessageWithPrompt("SadMan", "BRISSKEEET", "Good job!", "Dumb Fuck"),
            "SpinGod",
            "Classic!",
            "Don't lie to him",
            "Woah settle down sailor"
        );
        await Message("SadMan", "BRISKET BRISKET");
        await MessageWithPrompt("BossMan", "Is he okay?", "He's great!", "Idk man");
        await LastMessage("SpinGod", "K moving on");
    }

    public async Task PopUpMessage(string speakerKey, string message, float duration = 3f)
    {
        PopUpText.Text = message;

        // Set the portrait
        if (PortraitLibrary != null)
        {
            var portrait = PortraitLibrary.GetPortrait(speakerKey);
            PopUpPortrait.Texture = portrait;
            PopUpPortrait.Visible = portrait != null;
        }
        else
        {
            GD.PrintErr("ERROR: HUDDialogSystem - PortraitLibrary is null");
            PopUpPortrait.Visible = false;
        }

        // Start fade-in
        PopUpBox.Modulate = new Color(1, 1, 1, 0);
        PopUpBox.Visible = true;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(PopUpBox, "modulate:a", 1.0f, 0.5f)
             .SetTrans(Tween.TransitionType.Sine);
        await ToSignal(tween, "finished");

        await ToSignal(GetTree().CreateTimer(duration), "timeout");

        // Fade-out
        var fadeOutTween = GetTree().CreateTween();
        fadeOutTween.TweenProperty(PopUpBox, "modulate:a", 0.0f, 0.5f)
                    .SetTrans(Tween.TransitionType.Sine);
        await ToSignal(fadeOutTween, "finished");

        PopUpBox.Visible = false;
    }

    public async Task FirstMessage(string speakerKey, string message)
    {
        await ShowMessage(speakerKey, message, fadeIn: true, fadeOut: false);
    }

    public async Task Message(string speakerKey, string message)
    {
        await ShowMessage(speakerKey, message, fadeIn: false, fadeOut: false);
    }

    public async Task LastMessage(string speakerKey, string message)
    {
        await ShowMessage(speakerKey, message, fadeIn: false, fadeOut: true);
    }

    private async Task ShowMessage(string speakerKey, string message, bool fadeIn, bool fadeOut)
    {
        DialogName.Text = speakerKey;
        DialogText.Text = "";

        var portrait = PortraitLibrary?.GetPortrait(speakerKey);
        DialogPortrait.Texture = portrait;
        DialogPortrait.Visible = portrait != null;
        NextButtonContainer.Visible = true;

        if (fadeIn)
        {
            DialogBox.Modulate = new Color(1, 1, 1, 0);
            DialogBox.Visible = true;

            var tween = GetTree().CreateTween();
            tween.TweenProperty(DialogBox, "modulate:a", 1.0f, 0.5f)
                 .SetTrans(Tween.TransitionType.Sine);
            await ToSignal(tween, "finished");
        }
        else
        {
            DialogBox.Visible = true;
        }
        await TypeText(message);
        _nextPressedTcs = new TaskCompletionSource();
        await _nextPressedTcs.Task;

        if (fadeOut)
        {
            var fadeOutTween = GetTree().CreateTween();
            fadeOutTween.TweenProperty(DialogBox, "modulate:a", 0.0f, 0.5f)
                        .SetTrans(Tween.TransitionType.Sine);
            await ToSignal(fadeOutTween, "finished");

            DialogBox.Visible = false;
        }
    }

    private void OnNextPressed()
    {
        _nextPressedTcs?.TrySetResult();
    }

    public async Task<string> MessageWithPrompt(string speakerKey, string message, string optimistic = "", string pessimistic = "")
    {
        DialogName.Text = speakerKey;

        var portrait = PortraitLibrary?.GetPortrait(speakerKey);
        DialogPortrait.Texture = portrait;
        DialogPortrait.Visible = portrait != null;

        // Hide next button, show prompt buttons
        NextButtonContainer.Visible = false;
        PromptButtonBox.Visible = true;

        // Set up buttons
        SureButton.Text = "Sure";
        Option1Button.Text = optimistic;
        Option2Button.Text = pessimistic;

        // Dynamic visibility
        Option1Button.Visible = !string.IsNullOrWhiteSpace(optimistic);
        Option2Button.Visible = !string.IsNullOrWhiteSpace(pessimistic);

        // Style the Sure button (e.g., bold color)
        SureButton.AddThemeColorOverride("font_color", new Color(0f / 255f, 105f / 255f, 92f / 255f));

        // Show dialog box
        DialogBox.Visible = true;

        // Typing effect
        await TypeText(message);

        // Wait for user input
        _promptPressedTcs = new TaskCompletionSource<string>();

        SureButton.Pressed += OnPrompt0Pressed;
        Option1Button.Pressed += OnPrompt1Pressed;
        Option2Button.Pressed += OnPrompt2Pressed;

        var result = await _promptPressedTcs.Task;

        // Cleanup
        SureButton.Pressed -= OnPrompt0Pressed;
        Option1Button.Pressed -= OnPrompt1Pressed;
        Option2Button.Pressed -= OnPrompt2Pressed;

        PromptButtonBox.Visible = false;
        DialogBox.Visible = false;

        return result;
    }

    private void OnPrompt0Pressed() => _promptPressedTcs?.TrySetResult(D.Sure);
    private void OnPrompt1Pressed() => _promptPressedTcs?.TrySetResult(D.Optimist);
    private void OnPrompt2Pressed() => _promptPressedTcs?.TrySetResult(D.Pessimist);

    public async Task HandleChoice(string choice, string speakerKey, string sureLine, string optimistLine, string pessimistLine)
    {
        switch (choice)
        {
            case D.Sure:
                await Message(speakerKey, sureLine);
                break;
            case D.Optimist:
                await Message(speakerKey, optimistLine);
                break;
            case D.Pessimist:
                await Message(speakerKey, pessimistLine);
                break;
            default:
                GD.PrintErr($"ERROR: Unexpected choice '{choice}' in HandleChoice");
                break;
        }
    }

    private async Task TypeText(string fullText, float delay = 0.02f)
    {
        DialogText.Text = "";
        _skipTyping = false;

        void OnSkipPressed()
        {
            _skipTyping = true;
        }

        // Connect the signal temporarily
        NextButton.Pressed += OnSkipPressed;

        foreach (char c in fullText)
        {
            if (_skipTyping)
            {
                DialogText.Text = fullText;
                break;
            }

            DialogText.Text += c;
            await ToSignal(GetTree().CreateTimer(delay), "timeout");
        }

        // Cleanup
        NextButton.Pressed -= OnSkipPressed;
    }
}
