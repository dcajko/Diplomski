using Diplomski;
using Godot;
using System;

public partial class Spawner : Node3D
{
    [Export]
    public PackedScene PawnScene { get; set; }

    [Export]
    public float SpawnDistance { get; set; } = 2f;

    [Export]
    public int Player { get; set; }

    [Export]
    public Color DebugPlayerColor { get; set; } = new Color(1, 1, 1, 1);

    private MeshInstance3D debugMesh;
    public StandardMaterial3D debugMaterial;

    public override void _Ready()
    {
        debugMaterial = GenerateDebugMaterial(DebugPlayerColor);

        debugMesh = GetNode<MeshInstance3D>("DebugMesh");
        debugMesh.SetSurfaceOverrideMaterial(0, debugMaterial);
    }

    public void Spawn()
    {
        var currentLocation = this.GlobalPosition;
        var randomVector = GenerateRandomVector(SpawnDistance);
        Pawn pawn = PawnScene.Instantiate<Pawn>();
        pawn.Player = this.Player;
        pawn.GlobalPosition = currentLocation + randomVector;
        Helper.AddNode(GetTree().Root, pawn);
    }
    public static Vector3 GenerateRandomVector(float distance)
    {
        Random random = new Random();
        double angle = random.NextDouble() * Math.PI * 2;
        float x = (float)Math.Cos(angle) * distance;
        float y = (float)Math.Sin(angle) * distance;
        return new Vector3(x, 1, y);
    }

    private StandardMaterial3D GenerateDebugMaterial(Color Color)
    {
        var material = new StandardMaterial3D();
        material.AlbedoColor = Color;
        return material;
    }
}
