using Godot;
using System;

public partial class CameraRoot : Marker3D
{
    private Camera3D camera;
    private SelectionDummy selectionDummy;

    private Vector3 newCameraPosition;

    [Export]
    public float MoveSpeed { get; set; } = 1f;

    public override void _Ready()
    {
        base._Ready();
        camera = GetNode<Camera3D>("Camera");
        newCameraPosition = camera.Position;
        if (camera == null)
        {
            throw new Exception("Camera not found!");
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Zoom();
        Pan();
        ExitApp();
    }

    private void ExitApp()
    {
        if (Input.IsActionJustReleased("KillApp"))
        {
            GetTree().Quit();
        }
    }

    private void Zoom()
    {
        if (Input.IsActionJustPressed("CameraZoomIn"))
        {
            camera.Size -= 2;
            //newCameraPosition -= new Vector3(0, 10f, 0);
        }
        if (Input.IsActionJustPressed("CameraZoomOut"))
        {
            camera.Size += 2;
            //newCameraPosition += new Vector3(0, 10f, 0);
        }

        camera.Position = camera.Position.Lerp(newCameraPosition, 1);
    }

    private void Pan()
    {
        Vector3 moveDirection = Vector3.Zero;

        if (Input.IsActionPressed("CameraPanUp"))
        {
            moveDirection += Vector3.Forward;
        }
        if (Input.IsActionPressed("CameraPanDown"))
        {
            moveDirection += Vector3.Back;
        }
        if (Input.IsActionPressed("CameraPanLeft"))
        {
            moveDirection += Vector3.Left;
        }
        if (Input.IsActionPressed("CameraPanRight"))
        {
            moveDirection += Vector3.Right;
        }
        moveDirection = moveDirection.Normalized() * MoveSpeed;

        this.GlobalPosition += this.GlobalPosition.Lerp(moveDirection, 1);
    }
}
