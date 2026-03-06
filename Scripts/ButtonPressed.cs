using Godot;
using System;

public partial class ButtonPressed : Button
{

	[Export] public bool up = false;
	[Export] public bool down = false;
	[Export] public bool left = false;
	[Export] public bool right = false;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (up)
			{
				if (Input.IsActionPressed("forward"))
			{
				ButtonPressed = true;
			}
			if (Input.IsActionJustReleased("forward"))
			{
				ButtonPressed = false;
			}
		}
		else if (down)
			{
				if (Input.IsActionPressed("backward"))
			{
				ButtonPressed = true;
			}
			if (Input.IsActionJustReleased("backward"))
			{
				ButtonPressed = false;
			}
		}
		else if (left)
			{
				if (Input.IsActionPressed("left"))
			{
				ButtonPressed = true;
			}
			if (Input.IsActionJustReleased("left"))
			{
				ButtonPressed = false;
			}
		}
		else if (right)
			{
				if (Input.IsActionPressed("right"))
			{
				ButtonPressed = true;
			}
			if (Input.IsActionJustReleased("right"))
			{
				ButtonPressed = false;
			}
		}

	}


}
