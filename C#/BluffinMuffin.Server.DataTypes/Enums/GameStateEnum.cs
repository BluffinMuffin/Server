namespace BluffinMuffin.Server.DataTypes.Enums
{
    public enum GameStateEnum
    {
        Init,
        WaitForPlayers,
        WaitForBlinds,
        Playing,
        Showdown,
        DecideWinners,
        DistributeMoney,
        End
    }
}
