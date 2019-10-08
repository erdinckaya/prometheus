namespace Client
{
    public enum GameMessageType
    {
        PlayerMove,
        Ball,
        Player,
        GameState,
        PlayerHit,

        #region Test

        ChangeBallSpeed,

        #endregion
        
        Reset,
        Pause,

        #region END

        Count,

        #endregion
        
    }
}