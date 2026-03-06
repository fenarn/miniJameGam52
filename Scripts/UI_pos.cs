using Godot;
using System;

public partial class UI_pos : Control
{

	[Export] float speed = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetPos(Vector2 pos)
	{
		Position = Position.MoveToward(pos, speed * Math.Abs((pos - Position).Length()));
		//GD.Print(pos);
	}
}
