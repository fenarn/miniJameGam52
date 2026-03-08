using Godot;
using System;

public partial class HealthSpawn : Node2D
{

	[Export] PackedScene spawnItem;
	[Export] float spawnRate = 30f;
	[Export] Rect2 spawnZone = new Rect2(-470,-220,940,440);
	Timer spawnTimer;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Time until testing for LeapAttack
		spawnTimer = new Timer();
		spawnTimer.Timeout += SpawnHealthCrate;
		spawnTimer.WaitTime = spawnRate;
		spawnTimer.Autostart = true;
		AddChild(spawnTimer); 
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	void SpawnHealthCrate()
	{
		GD.Print("healthSpawn");
		float xPos = (GD.Randf() * spawnZone.Size.X) + spawnZone.Position.X;
		float yPos = (GD.Randf() * spawnZone.Size.Y) + spawnZone.Position.Y;

		var obj = spawnItem.Instantiate() as Node2D;
		obj.GlobalPosition = new Vector2(xPos, yPos);
		GetTree().CurrentScene.AddChild(obj);	
	}
}
