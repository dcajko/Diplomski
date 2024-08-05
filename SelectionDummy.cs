using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public partial class SelectionDummy : MeshInstance3D
{
    private Vector3 corner1;
    private Vector3 corner2;

    private BoxShape3D selectShape = new BoxShape3D();

    public List<Pawn> pawns { get; set; }

    public override void _Ready()
    {
        //pawns = GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().ToList();
        this.Visible = false;
    }

    public void OnSelectionStart(Vector3 location)
    {
        this.Visible = true;
        this.corner1 = location;
        foreach (var pawn in pawns)
        {
            pawn.Selected = false;
        }
    }
    public void OnSelectionChange(Vector3 location)
    {
        corner2 = location;
        Update();
        /*
        foreach (var pawn in pawns)
        {
            pawn.Selected = false;
        }
        */
    }

    public void OnSelectionEnd(Vector3 location)
    {
        this.Visible = false;
        OnSelectionChange(location);
        this.Visible = false;

        Vector3 minCorner = new Vector3(
            Mathf.Min(corner1.X, corner2.X),
            Mathf.Min(corner1.Y, corner2.Y),
            Mathf.Min(corner1.Z, corner2.Z)
        );

        Vector3 maxCorner = new Vector3(
            Mathf.Max(corner1.X, corner2.X),
            Mathf.Max(corner1.Y, corner2.Y),
            Mathf.Max(corner1.Z, corner2.Z)
        );

        minCorner.Y -= 10;
        maxCorner.Y += 10;

        var aabb = new Aabb(minCorner, maxCorner - minCorner);

        foreach (var p in pawns)
        {
            if (aabb.HasPoint(p.GlobalPosition))
            {
                p.Selected = true;
            }
        }

    }

    public void Update()
    {

        this.Position = GetPosition();
        this.Scale = GetScale();
        this.Visible = true;
    }

    public Vector3 GetPosition()
    {
        float centerX = (corner1.X + corner2.X) / 2.0f;
        float centerY = (corner1.Z + corner2.Z) / 2.0f;
        return new Vector3(centerX, 1f, centerY);
    }

    // Method to calculate and return the scale (size) of the selection rectangle
    public Vector3 GetScale()
    {
        float width = Math.Abs(corner1.X - corner2.X);
        float height = Math.Abs(corner1.Z - corner2.Z);
        return new Vector3(width, 1f, height);
    }

}
