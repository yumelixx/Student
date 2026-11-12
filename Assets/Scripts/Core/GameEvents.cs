using System;

public static class GameEvents
{
    public static event Action<GameItem> OnTargetDestroyed;

    public static void TargetDestroyed(GameItem gameItem)
    {
        OnTargetDestroyed?.Invoke(gameItem);
    }
}
