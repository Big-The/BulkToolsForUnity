# Static Tools Module

## About
Static Tools provides tools for static classes such as an Update callback and coroutines.

## Platforms
All

## How To Use

### Static Update
Static update provides a simple way to add into Unity's player loop under the standard update section.
```csharp
private static void InitCustomUpdate() 
{
    StaticUpdate.AddUpdate(CustomUpdate);
}

private static void CustomUpdate() 
{
    //Do custom update things...
}
```

### Static Coroutines
Static coroutines provide a way to build coroutine style logic without an owner.
```csharp
private StaticCoroutine coroutineRef;

private static void StartCoroutine() 
{
    coroutineRef = StaticCoroutines.StartCoroutine(ExampleRoutine());
}

private static void StopCoroutine() 
{
    StaticCoroutines.StopCoroutine(coroutineRef); //StopCoroutine can handle coroutineRef being null
}

private static IEnumerator ExampleRoutine()
{
    yield return new WaitForSeconds(1); //Static coroutines supports most but not all unity provided yield options. 
}
```
