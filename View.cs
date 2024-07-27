using Diplomski;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class View : FreeLookCameraBase
{
    private bool casting = false;
    private Vector3 firstLocation;
    private Vector3 lastLocation;
    private bool isShiftHeld;

    [Signal]
    public delegate void SelectionClickEventHandler(Vector3 location);
    [Signal]
    public delegate void SelectionStartedEventHandler(Vector3 location);
    [Signal]
    public delegate void SelectionResizeEventHandler(Vector3 location);
    [Signal]
    public delegate void SelectionEndedEventHandler(Vector3 location);
    public List<Pawn> pawns { get; private set; }

    public override void _Ready()
    {
        pawns = GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().ToList();
        //this.GetViewport().DebugDraw = Viewport.DebugDrawEnum.Overdraw;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

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

        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed)
                {
                    if (isShiftHeld)
                    {
                        casting = true;
                        firstLocation = ShootRay();
                        EmitSignal(SignalName.SelectionStarted, firstLocation);
                    }
                }
                else // Mouse button is released
                {
                    if (casting)
                    {
                        casting = false;
                        EmitSignal(SignalName.SelectionEnded, lastLocation);
                    }
                    else
                    {
                        EmitSignal(SignalName.SelectionClick);
                        var targetLocation = ShootRay();
                        var formationBox = new FormationBox();
                        formationBox.MinSize = 2;
                        formationBox.Position = targetLocation;
                        int selectedPawnsCount = pawns.Where(x => x.Selected).Count();
                        formationBox.Prepare(selectedPawnsCount);
                        var points = formationBox.PreparePoints();
                        Helper.AddNode(GetTree().Root, formationBox, "FormationBox");
                        
                        if(points.Count > 0)
                        {
                            SelectedToLocation(targetLocation, points);
                        }
                        lastLocation = targetLocation;
                    }
                }
            }
        }
    }
    private void SelectedToLocation(Vector3 loaction, List<Vector3> points)
    {
        int i = 0;
        foreach (var p in pawns)
        {
            if (p.Selected)
            {
                p.SetGoToLocation(loaction + points[i++]);
            }

        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // TODO: Optimize; don't fire each tick
        if (casting)
        {
            lastLocation = ShootRay();
            if (firstLocation.DistanceSquaredTo(lastLocation) > 1)
            {
                EmitSignal(SignalName.SelectionResize, lastLocation);
            }
        }
    }

    public Vector3 ShootRay()
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
        //if (raycastResult != null) 
        //{
        //    var collider = raycastResult["collider"].AsGodotObject();
        //    if (collider is Pawn pawn)
        //    {
        //        pawn.Selected = true;
        //    }
        //}
        return (Vector3)raycastResult["position"];
    }
}
