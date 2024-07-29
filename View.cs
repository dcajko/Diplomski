using Diplomski;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class View : Camera3D
{
    private bool buttonHeld = false;
    private bool isDragging = false;
    private Vector3 firstLocation;
    private Vector3 lastLocation;
    private bool isShiftHeld;
    private float distanceMoved;
    private MouseEvents mouseEvents = MouseEvents.None;
    private InputEventMouseButton lastMouseInputEvent;
    private FormationBox formationBox;

    private float newDirection;

    private MeshInstance3D firstLocationPointMarker;
    private MeshInstance3D lastLocationPointMarker;

    [Signal]
    public delegate void MouseLeftDownEventHandler(Vector3 location);

    [Signal]
    public delegate void MouseLeftUpEventHandler(Vector3 location);

    [Signal]
    public delegate void MouseDragStartEventHandler(Vector3 location);

    [Signal]
    public delegate void MouseDragEndEventHandler(Vector3 location);

    [Signal]
    public delegate void MouseDragResizeEventHandler(Vector3 location);

    [Signal]
    public delegate void MouseRightDownEventHandler(Vector3 location);

    [Signal]
    public delegate void MouseRightUpEventHandler(Vector3 location);

    public List<Pawn> pawns { get; private set; }

    private void setFirstPoint(Vector3 location)
    {
        firstLocationPointMarker.GlobalPosition = location;
        newDirection = firstLocation.AngleTo(location);
        firstLocation = location;
    }

    private void setLastPoint(Vector3 location)
    {
        lastLocationPointMarker.GlobalPosition = location;
        lastLocation = location;
    }

    public override void _Ready()
    {
        pawns = GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().ToList();
        MouseRightUp += GoTo;
        firstLocationPointMarker = new MeshInstance3D();
        firstLocationPointMarker.Mesh = new CylinderMesh();

        lastLocationPointMarker = new MeshInstance3D();
        lastLocationPointMarker.Mesh = new BoxMesh();

        Helper.AddNode(this, firstLocationPointMarker, "FirstMarker");
        Helper.AddNode(this, lastLocationPointMarker, "LastMarker");
        //this.GetViewport().DebugDraw = Viewport.DebugDrawEnum.Overdraw;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // TODO: Optimize; don't fire each tick
        if (buttonHeld)
        {
            setLastPoint(ShootRay().Item1);
            if (firstLocation.DistanceSquaredTo(lastLocation) > 1)
            {
                if (!isDragging)
                {
                    isDragging = true;
                    EmitSignal(SignalName.MouseDragStart, lastLocation);
                }

                EmitSignal(SignalName.MouseDragResize, lastLocation);
            }
        }
    }
    private void ProcessMouseInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.Shift && keyEvent.Pressed)
            {
                isShiftHeld = true;
            }
            else if (keyEvent.Keycode == Key.Shift && !keyEvent.Pressed)
            {
                isShiftHeld = false;
            }
        }
        if (@event is InputEventMouseButton e)
        {
            lastMouseInputEvent = e;
            var clickWorldLocation = ShootRay();

            if (mouseEvents == MouseEvents.None)
            {
                setFirstPoint(clickWorldLocation.Item1);
                setLastPoint(clickWorldLocation.Item1);
            }

            if (e.ButtonIndex == MouseButton.Left)
            {
                buttonHeld = e.Pressed;
                if (e.Pressed)
                {
                    EmitSignal(SignalName.MouseLeftDown, firstLocation);
                }
                else
                {
                    if (isDragging)
                    {
                        if (clickWorldLocation.Item2 != null)
                        {
                            clickWorldLocation.Item2.Select();
                        }

                        isDragging = false;
                        EmitSignal(SignalName.MouseDragEnd, clickWorldLocation.Item1);
                    } 
                    else
                    {
                        EmitSignal(SignalName.MouseLeftUp, clickWorldLocation.Item1);
                    }
                    //EmitSignal(SignalName.SelectionEnded, lastLocation);
                }
            }
            else if (e.ButtonIndex == MouseButton.Right)
            {
                if (e.Pressed)
                {
                    EmitSignal(SignalName.MouseRightDown, clickWorldLocation.Item1);
                }
                else
                {
                    if (isDragging)
                    {
                        isDragging = false;
                        EmitSignal(SignalName.MouseDragEnd, clickWorldLocation.Item1);
                    }
                    else
                    {
                        EmitSignal(SignalName.MouseRightUp, clickWorldLocation.Item1);
                    }
                }
            }

            setLastPoint(clickWorldLocation.Item1);
        }
    }
    
    public void GoTo(Vector3 targetLocation)
    {
        if (formationBox != null)
        {
            var directionTo = targetLocation.DirectionTo(formationBox.GlobalPosition);
            newDirection = (float)Math.Atan2(directionTo.X, directionTo.Z);
            formationBox.QueueFree();
        }
        formationBox = new FormationBox();
        formationBox.MinSize = 2.1f;
        formationBox.Position = targetLocation;
        formationBox.Rotation = new Vector3(0,newDirection,0);
        int selectedPawnsCount = pawns.Where(x => x.Selected).Count();
        formationBox.Prepare(selectedPawnsCount);
        Helper.AddNode(GetTree().Root, formationBox, "FormationBox");

        var points = formationBox.PreparePoints();
        if (points.Count > 0)
        {
            SelectedToLocation(targetLocation, points);
        }
        setLastPoint(targetLocation);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        ProcessMouseInput(@event);
    }

    private void SelectedToLocation(Vector3 loaction, List<Vector3> points)
    {
        int i = 0;
        foreach (var p in pawns)
        {
            if (p.Selected)
            {
                p.SetGoToLocation(points[i++]);
            }

        }
    }

    public (Vector3, Pawn) ShootRay()
    {
        var mousePosition = this.GetViewport().GetMousePosition();
        int rayLength = 1000;
        var from = ProjectRayOrigin(mousePosition);
        var to = from + ProjectRayNormal(mousePosition) * rayLength;
        var space = GetWorld3D().DirectSpaceState;
        var rayQuery = new PhysicsRayQueryParameters3D();
        rayQuery.From = from;
        rayQuery.To = to;
        var raycastResult = space.IntersectRay(rayQuery);
        if (raycastResult != null)
        {
            if (raycastResult.TryGetValue("position", out var position))
            {

                if (raycastResult.TryGetValue("collider", out var collider) && collider.AsGodotObject() is Pawn pawn)
                {
                    return ((Vector3)position, pawn);
                }
                else
                {
                    return ((Vector3)position, null);
                }
            }
        }
        rayQuery.Dispose();
        return (Vector3.Zero, null);
    }
}
