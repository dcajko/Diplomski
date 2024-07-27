using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class FormationBox : Node3D
{
    private int x, y, r;
    private Node3D pointContainer;

    [Export]
    public Vector2 Size { get; set; } = new Vector2(10, 10);
    [Export]
    public float MinSize { get; set; } = 1f;
    [Export]
    public int NumberOfSelected { get; set; } = 0;
    [Export]
    public bool Refresh
    {
        get => false; set => _Ready();
    }
    public List<Vector3> TargetPoints { get; private set; } = new List<Vector3>();

    public void Prepare(int Num)
    {
        var sqrt = Math.Sqrt(Num);
        x = (int)Math.Ceiling(sqrt);
        y = (int)Math.Floor(sqrt);
        r = Num - x * y;
        if (r<0)
        {
            y -= 1;
            r = x + r;
        }
        Size = new Vector2(x * MinSize, y * MinSize);
        GD.Print($"{Num}: {sqrt}, {x}, {y}, {r}, {Size}");
    }

    public override void _Ready()
    {
        //ClearChildren();
        //Prepare(NumberOfSelected);
        //PreparePoints();
    }

    private void ClearChildren()
    {
        TargetPoints.Clear();
        var children = this.GetChildren();
        foreach (var child in children)
        {
            child.QueueFree();
        }
    }

    private void CreatePointContainer()
    {
        // The name of the node to check/create
        string nodeName = "PointContainer";

        // Check if the node already exists as a child
        Node3D existingNode = (Node3D)GetNodeOrNull(nodeName);

        // If the node exists, remove it
        if (existingNode != null)
        {
            existingNode.QueueFree();
        }

        // Create a new instance of the node
        Node3D newNode = new Node3D();
        newNode.Name = nodeName;

        // Add the new node as a child of this node (or the desired parent node)
        AddChild(newNode);
        newNode.Owner = this;
        pointContainer = newNode;
    }

    public List<Vector3> PreparePoints()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                CreatePoint(i * MinSize, j * MinSize);
            }
        }
        if (r != 0)
        {
            GD.Print($"Offset vars: x:{x}, r:{r}, minSize:{MinSize}");
            float offset = ((x-1) - (r - 1f))/2f;
            GD.Print($"offset: {offset}");   
            for (int i = 0; i< r; i++)
            {
                CreatePoint((i + offset)*MinSize, y * MinSize);
            }
        }
        return TargetPoints;
    }

    public List<Vector3> GetPoints()
    {

        return TargetPoints;
    }

    public void CreatePoint(float x, float y)
    {
        var newVector = new Vector3(x, 0, y);
        TargetPoints.Add(newVector);
        //GD.Print($"Node Added: {x}, {y}");
        var newNode = new Marker3D();
        this.AddChild(newNode);
        newNode.Owner = this;
        newNode.Position = newVector;
    }
}
