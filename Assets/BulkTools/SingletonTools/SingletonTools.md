# Singleton Tools Module

## About
Provides some base classes and tools for using statically accessible singletons in the scene.

## Platforms
All

## How to Use

### Forced Singleton Behaviour
The forced singleton behaviour is a MonoBehaviour based singleton system that forces one instance to exist at runtime and automatically marked as don't destroy on load.
```csharp
public class ExampleSingleton : ForcedSingletonBehaviour<ExampleSingleton>
{
    public float instanceVar = 0.1f;

    //Do not use the normal Unity Awake method, use this instead to know you are in the valid instance first
    public override void ValidInstanceAwake() 
    {
        instanceVar = 5.2f;
    }

    public static void LogInstanceVar()
    {   
        //A static Instance property exists to provide an easy way to access instance members from a static location.
        Debug.Log(Instance.instanceVar); //Will log 5.2
    }
}
```

### Simple Singleton Behaviour
The simple singleton behaviour is a MonoBehaviour based singleton system that only allows up to one instance to exist at runtime.
```csharp
public class ExampleSingleton : ForcedSingletonBehaviour<ExampleSingleton>
{
    public float instanceVar = 0.1f;

    //Do not use the normal Unity Awake method, use this instead to know you are in the valid instance first
    public override void ValidInstanceAwake() 
    {
        instanceVar = 5.2f;
    }

    public static void LogInstanceVar()
    {   
        //A static Instance property exists to provide an easy way to access instance members from a static location.
        Debug.Log(Instance.instanceVar); //Will log 5.2
    }
}
```