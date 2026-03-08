using Godot;
using Godot.Collections;
using System;

public partial class SpawnSystem : Node2D
{
	[Export]
	public Array<PackedScene> zombiePrefabs;
	[Export]
	public float globalSpawnCooldownSeconds = 5f;
	private float currentSpawnCooldownSeconds;
	Array<Node> spawnPoints;

    public float CurrentSpawnCooldownSeconds { get => currentSpawnCooldownSeconds; set => currentSpawnCooldownSeconds = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		spawnPoints = GetChildren();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		int howManyCanSpawn = 0;
		foreach (Node spawnPoint in spawnPoints)
		{
			SpawnPoint sPoint = spawnPoint as SpawnPoint;
			sPoint.ZombiePrefab = zombiePrefabs[GD.RandRange(0,zombiePrefabs.Count - 1)];
			if(sPoint.canSpawn)
			{
				howManyCanSpawn++;
			}
		}
		CurrentSpawnCooldownSeconds = (float)howManyCanSpawn/(float)spawnPoints.Count * globalSpawnCooldownSeconds;
		
	}
}
