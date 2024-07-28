using Diplomski;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class View : FreeLookCameraBase
{
    private bool buttonHeld = false;
    private Vector3 firstLocation;
    private Vector3 lastLocation;
    private bool isShiftHeld;
    private float distanceMoved;
    private MouseEvents mouseEvents = MouseEvents.None;
    private InputEventMouseButton lastMouseInputEvent;

    [Signal]
    public delegate void SelectionClickEventHandler(Vector3 location);
    [Signal]
    public delegate void SelectionStartedEventHandler(Vector3 location);
    [Signal]
    public delegate void SelectionResizeEventHandler(Vector3 location);
    [Signal]
    public delegate void SelectionEndedEventHandler(Vector3 location);
    [Signal]
    public delegate void MouseEventEventHandler(Vector3 location, MouseEvents mouseEvents);
    public List<Pawn> pawns { get; private set; }

    public override void _Ready()
    {
        pawns = GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().ToList();
        //this.GetViewport().DebugDraw = Viewport.DebugDrawEnum.Overdraw;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // TODO: Optimize; don't fire each tick
        if (buttonHeld)
        {
            lastLocation = ShootRay().Item1;
            if (firstLocation.DistanceSquaredTo(lastLocation) > 1)
            {
                EmitSignal(SignalName.SelectionResize, lastLocation);
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        ProcessMouseInput(@event);

        //if (@event is InputEventMouseButton mouseEvent)
        //{
        //    if (mouseEvent.ButtonIndex == MouseButton.Left)
        //    {
        //        if (mouseEvent.Pressed)
        //        {
        //            if (isShiftHeld)
        //            {
        //                casting = true;
        //                firstLocation = ShootRay().Item1;
        //                EmitSignal(SignalName.SelectionStarted, firstLocation);
        //            }
        //        }
        //        else // Mouse button is released
        //        {
        //            if (casting)
        //            {
        //                casting = false;
        //                EmitSignal(SignalName.SelectionEnded, lastLocation);
        //            }
        //            else
        //            {
        //                EmitSignal(SignalName.SelectionClick);
        //                var targetLocation = ShootRay();
        //                var formationBox = new FormationBox();
        //                formationBox.MinSize = 2;
        //                formationBox.Position = targetLocation.Item1;
        //                int selectedPawnsCount = pawns.Where(x => x.Selected).Count();
        //                formationBox.Prepare(selectedPawnsCount);
        //                var points = formationBox.PreparePoints();
        //                Helper.AddNode(GetTree().Root, formationBox, "FormationBox");

        //                if (points.Count > 0)
        //                {
        //                    SelectedToLocation(targetLocation.Item1, points);
        //                }
        //                lastLocation = targetLocation.Item1;
        //            }
        //        }
        //    }
        //}
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
            buttonHeld = e.Pressed;

            if (mouseEvents == MouseEvents.None)
            {
                lastLocation = clickWorldLocation.Item1;
                firstLocation = clickWorldLocation.Item1;
                EmitSignal(SignalName.SelectionStarted, firstLocation);
            }

            if (clickWorldLocation.Item1.DistanceSquaredTo(lastLocation) > 1)
            {
                if (e.ButtonIndex == MouseButton.Left)
                {
                    //GD.Print(e.Pressed ? "Left pressed" : "Left released");
                    mouseEvents = e.Pressed
                        ? mouseEvents | MouseEvents.LeftDragStart
                        : mouseEvents ^ MouseEvents.LeftDragStart;
                }
                else if (e.ButtonIndex == MouseButton.Right)
                {
                    //GD.Print(e.Pressed ? "Right pressed" : "Right released");
                    mouseEvents = e.Pressed
                        ? mouseEvents | MouseEvents.RightDragStart
                        : mouseEvents ^ MouseEvents.RightDragStart;
                }
            }
            else
            {
                if (e.ButtonIndex == MouseButton.Left)
                {
                    //GD.Print(e.Pressed ? "Left pressed" : "Left released");
                    mouseEvents = e.Pressed
                        ? mouseEvents | MouseEvents.LeftDown
                        : mouseEvents ^ MouseEvents.LeftDown;
                }
                else if (e.ButtonIndex == MouseButton.Right)
                {
                    //GD.Print(e.Pressed ? "Right pressed" : "Right released");
                    mouseEvents = e.Pressed
                        ? mouseEvents | MouseEvents.RightDown
                        : mouseEvents ^ MouseEvents.RightDown;
                }
            }
            if (mouseEvents == MouseEvents.None)
            {
                EmitSignal(SignalName.SelectionEnded, lastLocation);
            }
            GD.Print(mouseEvents);
            lastLocation = clickWorldLocation.Item1;
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
        return (Vector3.Zero, null);
    }
}
