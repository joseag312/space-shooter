using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class HUDCurrency : Control
{
    [Export] public Label MewnitsLabel { get; set; }
    [Export] public Label PawllarsLabel { get; set; }

    private bool _isConnected;
    private int _lastMewnits;
    private int _lastPawllars;

    private const int ScrambleSteps = 6;
    private const float StepDuration = 0.06f;
    private readonly Color _gainColor = new Color(0.55f, 1f, 0.55f);
    private readonly Color _spendColor = new Color(1f, 0.55f, 0.55f);
    private readonly Color _normalColor = new Color(1f, 1f, 1f);

    private readonly HashSet<Label> _animating = new HashSet<Label>();

    public override void _Ready()
    {
        G.GS.Mewnits += 1000;
        G.GS.Pawllars += 100;
        _lastMewnits = G.GS.Mewnits;
        _lastPawllars = G.GS.Pawllars;
        SetLabel(MewnitsLabel, _lastMewnits);
        SetLabel(PawllarsLabel, _lastPawllars);

        if (!_isConnected)
        {
            G.GS.CurrencyChanged += OnCurrencyChanged;
            _isConnected = true;
        }
    }

    public override void _ExitTree()
    {
        if (_isConnected)
        {
            G.GS.CurrencyChanged -= OnCurrencyChanged;
            _isConnected = false;
        }
    }

    private void OnCurrencyChanged()
    {
        if (_lastMewnits != G.GS.Mewnits)
        {
            _ = AnimateNumberChange(MewnitsLabel, _lastMewnits, G.GS.Mewnits);
            _lastMewnits = G.GS.Mewnits;
        }

        if (_lastPawllars != G.GS.Pawllars)
        {
            _ = AnimateNumberChange(PawllarsLabel, _lastPawllars, G.GS.Pawllars);
            _lastPawllars = G.GS.Pawllars;
        }
    }

    private void SetLabel(Label label, int value)
    {
        label.Text = $"{value}";
        label.Modulate = _normalColor;
    }

    private async Task AnimateNumberChange(Label label, int oldVal, int newVal)
    {
        if (_animating.Contains(label)) return;
        _animating.Add(label);
        try
        {
            bool gained = newVal > oldVal;
            var targetColor = gained ? _gainColor : _spendColor;
            Punch(label, targetColor);

            int digits = Math.Max(1, Math.Max(oldVal, newVal).ToString().Length);
            int min = digits == 1 ? 0 : (int)Mathf.Pow(10, digits - 1);
            int max = (int)Mathf.Pow(10, digits) - 1;

            var rng = new Random();

            for (int i = 0; i < ScrambleSteps - 1; i++)
            {
                int fake = rng.Next(min, max + 1);
                label.Text = fake.ToString().PadLeft(digits, '0'); // optional zero-pad
                await ToSignal(GetTree().CreateTimer(StepDuration), "timeout");
            }

            label.Text = $"{newVal}";
            await ToSignal(GetTree().CreateTimer(StepDuration), "timeout");

            var tween = GetTree().CreateTween();
            tween.TweenProperty(label, "modulate", _normalColor, 0.15f);
            await ToSignal(tween, "finished");
        }
        finally
        {
            _animating.Remove(label);
        }
    }

    private void Punch(Label label, Color flashColor)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(label, "modulate", flashColor, 0.05f);

        var baseScale = label.Scale;
        tween.Parallel().TweenProperty(label, "scale", baseScale * 1.15f, 0.08f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(label, "scale", baseScale, 0.12f)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
    }
}
