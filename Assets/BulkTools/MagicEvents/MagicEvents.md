# Magic Events Module

## About
Magic Events is built to provide a convenient way to connect multiple bits of logic together without any direct connection or inspector configuration, using a globally accesssable event system.

## Platforms
All

## How to Use
Magic events do not require the event to be defined before being subscribed to. As long as the listener is subscribed to the matching event name before an event is invoked the listener will be called. Magic events also make sure the entire event is complete before starting another due to the internal event queue. (In the case of triggering an event inside a callback)

### Subscribing to an event
```csharp
private void Awake()
{
    MagicEvent.AddListener("Event Name", EventCallbackName);
}

private void EventCallbackName(MagicEventContext context) { }
```

### Invoking an event
```csharp
new MagicEvent("Event Name")
    .AddData("Data Name A", true)
    .AddData("Data Name B", new Vector3(0, 1, 2))
    .Invoke();
```

### Reading data from event context
```csharp
private void EventCallbackName(MagicEventContext context) 
{
    bool dataValueA = context.GetData<bool>("Data Name A");
    Vector3 dataValueB = context.GetData<Vector3>("Data Name B");
}
```