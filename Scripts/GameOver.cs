using Godot;
using Godot.NativeInterop;
using System;

public partial class GameOver : Control
{
	ColorRect 	goColor;
	Label		goLabel;

	Color color;
	Color labelColor;

	bool isPlayerDead = false;

    public bool IsPlayerDead { get => isPlayerDead; set => isPlayerDead = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		goColor = GetNode("GOColor") as ColorRect;
		goLabel = GetNode("GOLabel") as Label;

		Visible = false;

		color = goColor.Color;
		labelColor = goLabel.LabelSettings.FontColor;
		color.A8 = 0;
		labelColor.A8 = 0;

		goColor.Color = color;
		goLabel.LabelSettings.FontColor = labelColor;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(IsPlayerDead)
		{
			color.A8 += 1;
			labelColor.A8 += 1;

			goColor.Color = color;
			goLabel.LabelSettings.FontColor = labelColor;
		}
	}

}
