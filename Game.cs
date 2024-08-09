using Diplomski;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node3D
{
    private int movesToExecute;
    private Timer MoveExecutionTimer;
    private bool TimedOut;

    [Signal]
    public delegate void TurnChangeEventHandler(int player);
    [Signal]
    public delegate void ExecutionStartedEventHandler();
    [Signal]
    public delegate void ExecutionEndedEventHandler();

    [Export]
    public int NumberOfPlayers { get; set; } = 2;
    [Export]
    public double TurnExectutionDuration { get; set; } = 1.5;

    public int PlayerTurn { get; set; } = 0;
    public int PlayerGold { get; set; } = 0;
    public List<Move> Moves { get; set; } = new List<Move>();
    public int TurnCounter { get; private set; } = 0;

    private View ViewNode { get; set; }

    public void SpawnPlayerPawn()
    {
        GetTree().CallGroup("PlayerSpawner", "Spawn");
    }

    public void SpawnEnemyPawn()
    {
        GetTree().CallGroup("EnemySpawner", "Spawn");
    }

    public override void _Ready()
    {
        ViewNode = GetNode<View>("CameraRoot/Camera");
        MoveExecutionTimer = new Timer();
        MoveExecutionTimer.OneShot = true;
        MoveExecutionTimer.ProcessCallback = Timer.TimerProcessCallback.Physics;
        MoveExecutionTimer.Timeout += TurnTimeout;
        Helper.AddNode(this, MoveExecutionTimer);
        this.NextTurn();
    }

    public void NextTurn()
    {
        PlayerTurn++;
        if (PlayerTurn > NumberOfPlayers)
        {
            TurnCounter++;
            this.TimedOut = false;
            PlayerTurn = 0;
            PlayMoves();
        }
        SetupTurn();
    }

    public void SetupTurn()
    {
        EmitSignal(SignalName.TurnChange, PlayerTurn);
        ViewNode.SetupPlayerView(PlayerTurn);
    }

    public void PlayMoves()
    {
        EmitSignal(SignalName.ExecutionStarted, PlayerTurn);
        movesToExecute = Moves.Count;
        this.MoveExecutionTimer.Start(TurnExectutionDuration);
        foreach (var move in Moves)
        {
            move.FormationBox.AllPawnsAtLocation += MoveDone;
            move.Execute();
        }
    }

    public void MoveDone()
    {
        if (!TimedOut)
        {
            movesToExecute--;
            if (movesToExecute <= 0)
            {
                //ExecutionFinished();
            }
        }
    }

    public void TurnTimeout()
    {
        GD.Print("TurnTimeout");
        this.TimedOut = true;
        GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().ToList().ForEach(x => x.ExitCombat());
        ExecutionFinished();
    }

    public void ExecutionFinished()
    {
        GD.Print("AllMovesFinished");
        //this.MoveExecutionTimer.Stop();
        foreach (var item in Moves)
        {
            item.Dispose();
        }
        Moves.Clear();
        GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().Where(x => x.Player == -1).ToList().ForEach(x => x.QueueFree());
        EmitSignal(SignalName.ExecutionEnded, PlayerTurn);
        this.NextTurn();
    }

    public GameEnvironment GetGameEnvironment(int Player)
    {
        GameEnvironment env = new GameEnvironment();
        env.OwnUnits = GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().Where(x => x.Player == Player).ToList();
        env.EnemyUnits = GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().Where(x => x.Player != Player).ToList();
        env.HomeBases = GetTree().GetNodesInGroup("Spawner").Cast<Spawner>().Where(x => x.Player == Player).ToList();
        env.EnemyBases = GetTree().GetNodesInGroup("Spawner").Cast<Spawner>().Where(x => x.Player != Player).ToList();
        env.TurnCounter = this.TurnCounter;
        return env;
    }
}
