using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level1Script : BaseLevelScript
{
	protected override int GetLevelId() => 1;
	protected override String GetLevelMusic() => Music.AMAZING;

	protected override async Task RunLevel(CancellationToken token)
	{
		try
		{
			G.GS.CurrentLevel = 1;
			StartDialog();
			await Task.Delay(300, token);
			await HUD.FirstMessage(Char.OIIA, Mood.OIIA.Default, "Ah there he is! The cat I've been hearing so much about!");
			// await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "Now... what was that word....");
			// await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.Default, "Your favorite word in the whole galaxy?");
			// await HUD.Message(Char.OIIA, Mood.OIIA.Default, "I knew it. Glad to have you on board rookie! Now listen up");
			// await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "This whole galaxy isn't like others...   Our main goal is to survive");
			// await HUD.Message(Char.OIIA, Mood.OIIA.Glasses, "Your ship isn't almighty, only I am..");
			// await HUD.Message(Char.OIIA, Mood.OIIA.GlassesZoom, "Now check this out");
			// G.SFX.Play("oiia_fast");
			// await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.SpinNormal, "See how beautiful this spin is?");
			// await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "I knew you were a cat with taste! Now.. Commander, show this new fur around!");
			// await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Yes sir, your spinness!");
			// string instructions = "";
			// do
			// {
			// 	await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Listen up rookie! Very simple:");
			// 	await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "You move with: ↑ ↓ ← →");
			// 	await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "You use cannon with: space");
			// 	await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "You use powers with: ① ② ③ ④");
			// 	instructions = await HUD.MessageWithPrompt(Char.OIIA, Mood.OIIA.Default, "You get all that?", "Yes sir!", "Huh?");
			// 	if (instructions.Equals("Pessimist"))
			// 	{
			// 		await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "You were busy watching this spinning greatness huh!");
			// 	}
			// }
			// while (!(instructions.Equals("Sure") || instructions.Equals("Optimist")));
			// await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Smart cat!");
			// if (instructions.Equals("Sure"))
			// {
			// 	await HUD.Message(Char.OIIA, Mood.OIIA.Default, "s u r ely ha!");
			// 	await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Annoyed, "Funny as always, sir");
			// }
			// await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "We're closing in, rookie heads up!");
			// await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Today we're scouting earth's defenses");
			// await HUD.Message(Char.OIIA, Mood.OIIA.Inverse, "These humans think they're the absolute best... Only I am.");
			// await HUD.Message(Char.COMMANDER, Mood.COMMANDER.Default, "Gather some intel on what their defenses look like, then head back immediately.");
			await HUD.LastMessage(Char.ROOKIE, Mood.ROOKIE.Default, "Good luck out there!");
			StopDialog();

			await Task.Delay(1000, token);
			_ = HUD.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "Survive!!!");

			LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 10, 15);
			LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy1Spawner, 300);

			await Task.Delay(20000, token);
			LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

			await Task.Delay(3000, token);
			LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 10, 30);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Humans...");
			await Task.Delay(3000, token);

			LevelFlowComponent.SpawnerRecurrent.StartSpawner1(Enemy1Spawner, 300);
			_ = HUD.PopUpMessage(Char.COMMANDER, Mood.COMMANDER.Default, "Return at once, cat!");
			await Task.Delay(20000, token);
			LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
			await Task.Delay(500, token);

			await Task.Delay(1500, token);
			_ = HUD.PopUpMessage(Char.ROOKIE, Mood.ROOKIE.Default, "They're blocking our return!");
			await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy2Spawner, 10, 30);
			await Task.Delay(3000, token);
			await HandleLevelClear();
		}
		catch (TaskCanceledException)
		{
			GD.Print("DEBUG: Level1Script - Script canceled");
		}
	}
}
