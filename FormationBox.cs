using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class FormationBox : Node3D
{
    private int x, y, r;
    private Node3D pointContainer;
    private int numPawnsReachedTarget;

    [Signal]
    public delegate void AllPawnsAtLocationEventHandler();

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
    public List<Pawn> Pawns { get; private set; }

    public void Prepare(List<Pawn> pawns)
    {
        this.Pawns = pawns.Where(x => x.Selected).ToList();
        numPawnsReachedTarget = this.Pawns.Count;
        foreach (var item in pawns)
        {
            item.Agent.NavigationFinished += ReachedTarget;
            item.InCombat += FormationInCombat;
        }
        Prepare(numPawnsReachedTarget);
    }

    private void FormationInCombat()
    {
    }

    public void StopUnits()
    {
        StopCombat();
    }

    public void StopCombat()
    {
        foreach (var pawn in Pawns)
        {
            pawn.ExitCombat();
        }
    }

    private void ReachedTarget()
    {
        numPawnsReachedTarget--;
        if (numPawnsReachedTarget <= 0)
        {
            EmitSignal(SignalName.AllPawnsAtLocation);
        }
    }

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
        Size = new Vector2((x-1) * MinSize, (y-1) * MinSize);
        // GD.Print($"{Num}: {sqrt}, {x}, {y}, {r}, {Size}");
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
            //// GD.Print($"Offset vars: x:{x}, r:{r}, minSize:{MinSize}");
            float offset = ((x-1) - (r - 1f))/2f;
            //// GD.Print($"offset: {offset}");   
            for (int i = 0; i< r; i++)
            {
                CreatePoint((i + offset)*MinSize, y * MinSize);
            }
        }
        return GetPoints();
    }

    public List<Vector3> GetPoints()
    {
        var nodes = this.GetChildren();
        foreach (MeshInstance3D node in nodes)
        {
            TargetPoints.Add(node.GlobalPosition);
        }
        return TargetPoints;
    }

    public void CreatePoint(float x, float y)
    {
        var newVector = new Vector3(x, 0, y) - new Vector3(Size.X, 0, Size.Y)*0.5f;
        //TargetPoints.Add(newVector);
        //// GD.Print($"Node Added: {x}, {y}");
        //var newNode = new Marker3D();
        var newNode = new MeshInstance3D();
        newNode.Mesh = new SphereMesh();
        this.AddChild(newNode);
        newNode.Owner = this;
        newNode.Position = newVector;
    }

    public void MoveUnits()
    {
        int i = 0;
        var points = this.GetPoints();
        foreach (var p in this.Pawns)
        {
            p.SetGoToLocation(points[i++]);
        }
    }
}
