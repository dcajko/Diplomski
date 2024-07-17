using Godot;
using System;
using System.Threading.Tasks;

[Tool]
public partial class Floor : StaticBody3D
{
    private bool temp = true;
    Random random = new Random();
    // FastNoiseLite noise = new();

    [Export]
    public Texture2D TreeTexture { get; set; }

    [Export]
    public int MeshSize { get; set; } = 64;
    [Export]
    public int MeshScale { get; set; } = 16;
    [Export]
    public bool GenerateMesh { get => temp; set => GenerateTerrain(); }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Console.WriteLine();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void GenerateTerrain()
    {
        // noise.Offset = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble()) * 1000;
        GD.Print("Generating terrain");
        var plane = new PlaneMesh();
        plane.Size = new Vector2(MeshSize, MeshSize);
        plane.SubdivideDepth = MeshScale - 1;
        plane.SubdivideWidth = MeshScale - 1;

        var surfaceTool = new SurfaceTool();
        surfaceTool.CreateFrom(plane, 0);
        surfaceTool.GenerateNormals();

        var TerrainMesh = GetNode<MeshInstance3D>("TerrainMesh");
        var TerrainCollisionMesh = GetNode<CollisionShape3D>("TerrainCollisionMesh");
        TerrainMesh.Mesh = surfaceTool.Commit();
        TerrainCollisionMesh.Shape = plane.CreateTrimeshShape();
    }
}
