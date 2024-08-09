using System.Collections.Generic;

public class GameEnvironment
{
    public List<Pawn> OwnUnits { get; set; }
    public List<Pawn> EnemyUnits { get; set; }
    public int TurnCounter { get; set; }
    public List<Spawner> HomeBases { get; internal set; }
    public List<Spawner> EnemyBases { get; internal set; }
}