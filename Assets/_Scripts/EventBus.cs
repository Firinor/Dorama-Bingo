using System;
using UnityEngine.Events;

public static class EventBus
{
    public static UnityEvent<string> GenerateCardEvent = new();
}
