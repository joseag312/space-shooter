using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level1Script : Node
{
	[Export] public Ship Ship { get; set; }
	[Export] public LevelFlowComponent LevelFlowComponent { get; set; }
	[Export] public SpawnerComponent Enemy1Spawner { get; set; }
	[Export] public SpawnerComponent Enemy2Spawner { get; set; }
	[Export] public SpawnerComponent Enemy3Spawner { get; set; }
	[Export] public CanvasLayer HUDMain { get; set; }

	public HUDDialogSystem Dialog;

	public override void _Ready()
	{
		G.CR.Run("LevelScript", RunLevelScript);
		Dialog = HUDMain.GetNode<HUDDialogSystem>("HUDDialogSystem");
		Ship.StatsComponent.Connect("NoHealth", new Callable(this, nameof(OnShipDeath)));
	}

	private async Task RunLevelScript(CancellationToken token)
	{

		try
		{
			// StartDialog();
			// await Task.Delay(300, token);
			// await Dialog.FirstMessage(Char.OIIA, Mood.OIIA.Default, "Ah there he is! The cat I've been hearing so much about!");
			// await Dialog.Message(Char.OIIA, Mood.OIIA.Inverse, "Now... what was that word....");
			// await Dialog.MessageWithPrompt(Char.OIIA, Mood.OIIA.Default, "Your favorite word in the whole galaxy?");
			// await Dialog.Message(Char.OIIA, Mood.OIIA.Default, "I knew it. Glad to have you on board rookie! Now listen up");
			// await Dialog.Message(Char.OIIA, Mood.OIIA.Inverse, "This whole galaxy isn't like others...   Our main goal is to survive");
			// await Dialog.Message(Char.OIIA, Mood.OIIA.Glasses, "Your ship isn't almighty, only I am..");
			// await Dialog.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "Now check this out");
			// G.SFX.Play("oiia_fast");
			// await Dialog.MessageWithPrompt(Char.OIIA, Mood.OIIA.SpinNormal, "See how beautiful this spin is?");
			// await Dialog.Message(Char.OIIA, Mood.OIIA.Inverse, "I knew you were a cat with taste! Now.. Commander, show this new fur around!");
			// await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Yes sir, your spinness!");
			// string instructions = "";
			// do
			// {
			// 	await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Listen up rookie! Very simple:");
			// 	await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "You move with: ↑ ↓ ← →");
			// 	await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "You use cannon with: space");
			// 	await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "You use powers with: ① ② ③ ④");
			// 	instructions = await Dialog.MessageWithPrompt(Char.OIIA, Mood.OIIA.Default, "You get all that?", "Yes sir!", "Huh?");
			// 	if (instructions.Equals("Pessimist"))
			// 	{
			// 		await Dialog.Message(Char.OIIA, Mood.OIIA.Inverse, "You were busy watching this spinning greatness huh!");
			// 	}
			// }
			// while (!(instructions.Equals("Sure") || instructions.Equals("Optimist")));
			// await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Smart cat!");
			// if (instructions.Equals("Sure"))
			// {
			// 	await Dialog.Message(Char.OIIA, Mood.OIIA.Default, "s u r ely ha!");
			// 	await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Funny as always, sir");
			// }
			// await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "We're closing in, rookie heads up!");
			// await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Today we're scouting earth's defenses");
			// await Dialog.Message(Char.OIIA, Mood.OIIA.Inverse, "These humans think they're the absolute best... Only I am.");
			// await Dialog.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Gather some intel on what their defenses look like, then head back immediately.");
			// await Dialog.LastMessage(Char.ROOKIE, Mood.ROOKIE.Default, "Good luck out there!");
			// StopDialog();

			await Task.Delay(30000, token);
			// _ = Dialog.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "Survive!!!");

			// LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 10, 15);
			// LevelFlowComponent.SpawnerRecurrent.StartSpawner1(300);
			// await Task.Delay(20000, token);
			// LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

			// await Task.Delay(3000, token);
			// LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 10, 30);
			// _ = Dialog.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Humans...");
			// await Task.Delay(3000, token);

			// LevelFlowComponent.SpawnerRecurrent.StartSpawner1(300);
			// _ = Dialog.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Return at once, cat!");
			// await Task.Delay(20000, token);
			// LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
			// await Task.Delay(500, token);

			// await Task.Delay(1500, token);
			// _ = Dialog.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "They're blocking our return!");
			// await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy2Spawner, 10, 15);
			// await Task.Delay(1500, token);
			await HandleLevelClear();
		}
		catch (TaskCanceledException)
		{
			GD.Print("DEBUG: Level1Script - Script canceled");
		}
	}

	private async Task HandleLevelClear()
	{
		G.BG.BlockInput();
		G.GF.BlockInput();

		await LevelFlowComponent.LevelCleanup.FadeOutAll();

		Ship.StopFiring();
		Ship.PositionClampComponent.Enabled = false;
		Ship.MoveComponent.Velocity = new Vector2(0, -250);

		await Task.Delay(3000);
		G.MS.FadeOut();

		await G.BG.FadeInBlack(0.3f);
		G.GF.UnblockInput();

		await G.GF.FadeToSceneWithBG(G.GF.LevelClearScene);
	}

	private void OnShipDeath()
	{
		G.MS.FadeOut();
		G.SFX.Play("oiia_death", -15);
		G.CR.Stop("LevelScript");
	}

	private void StartDialog()
	{
		G.GF.StartDialog();
		Ship.StopFiring();
	}

	private void StopDialog()
	{
		G.GF.StopDialog();
		Ship.StartFiring();
	}
}
