using Godot;
using Godot.NativeInterop;
using System;

public partial class GameOver : Control
{
	ColorRect 	goColor;
	Label		goLabel;
	Label		zombiesKilledLabel;
	Label		timeSurvivedLabel;

	Color color;
	Color labelColor;
	Color zombiesKilledColor;
	Color timeSurvivedColor;

	bool isPlayerDead = false;
	int zombiesKilled = 0;
	float timeSurvived;

    public bool IsPlayerDead { get => isPlayerDead; set => isPlayerDead = value; }
    public int ZombiesKilled { get => zombiesKilled; set => zombiesKilled = value; }
    public float TimeSurvived { get => timeSurvived; set => timeSurvived = value; }

    // Called when the node enters the scene tree for the first time.


    public override void _Ready()
	{
		goColor = GetNode("GOColor") as ColorRect;
		goLabel = GetNode("GOLabel") as Label;
		zombiesKilledLabel = GetNode("ZombiesKilled") as Label;
		timeSurvivedLabel = GetNode("TimeSurvived") as Label;

		Visible = false;

		color = goColor.Color;
		labelColor = goLabel.LabelSettings.FontColor;
		zombiesKilledColor = zombiesKilledLabel.LabelSettings.FontColor;
		timeSurvivedColor = timeSurvivedLabel.LabelSettings.FontColor;

		color.A8 = 0;
		labelColor.A8 = 0;
		timeSurvivedColor.A8 = 0;
		zombiesKilledColor.A8 = 0;

		goColor.Color = color;
		goLabel.LabelSettings.FontColor = labelColor;
		zombiesKilledLabel.LabelSettings.FontColor = zombiesKilledColor;
		timeSurvivedLabel.LabelSettings.FontColor = timeSurvivedColor;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(IsPlayerDead)
		{
			color.A8 += 1;
			labelColor.A8 += 1;
			timeSurvivedColor.A8 += 1;
			zombiesKilledColor.A8 += 1;

			goColor.Color = color;
			goLabel.LabelSettings.FontColor = labelColor;
			zombiesKilledLabel.LabelSettings.FontColor = zombiesKilledColor;
			timeSurvivedLabel.LabelSettings.FontColor = timeSurvivedColor;

			zombiesKilledLabel.Text = "Killed " + zombiesKilled + " zombies";

			TimeSpan timeSpan = TimeSpan.FromSeconds(timeSurvived);

			timeSurvivedLabel.Text = timeSpan.ToString(@"hh\:mm\:ss");
		}
	}

}
