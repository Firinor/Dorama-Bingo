using UnityEngine.Events;

public static class EventBus // Pattern for subscribers on events
{
    public static UnityEvent _loadBingoCardEvent = new UnityEvent(); // When we need to load the bingoCard
}
