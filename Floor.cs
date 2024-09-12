using Godot;
using System;
using System.Threading.Tasks;

[Tool]
public partial class Floor : StaticBody3D
{
    private bool temp = true;

    [Export]
    public Texture2D TreeTexture { get; set; }

    [Export]
    public int MeshSize { get; set; } = 64;
    [Export]
    public int MeshScale { get; set; } = 16;
    [Export]
    public bool GenerateMesh { get => temp; set => GenerateTerrain(); }

    public void GenerateTerrain()
    {
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
