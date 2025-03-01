# Singleton Tools Module

## About
Includes a simple state machine class that can be used in many simple state machine use cases.

## Platforms
All

## How to Use
```csharp
private SimpleStateMachine stateMachine;
private void Awake()
{
    //StateMachine state setup:
    stateMachine = new SimpleStateMachine("EvenState", EvenState);//The state passed into the constructor is the default state
    stateMachine.AddState("OddState", OddState);
    
    //Vars:
    stateMachine.SetFloat("time", Time.timeSinceLevelLoad);

    //Transitions:
    sateMachine.AddTransition(new SimpleStateMachine.Transition("EvenState", "OddState", EvenToOddTransition));
    sateMachine.AddTransition(new SimpleStateMachine.Transition("OddState", "EvenState", OddToEvenTransition));
}

private void Update()
{
    stateMachine.SetFloat("time", Time.timeSinceLevelLoad);//You can access and set variables from anywhere in or out of the stateMachine
    stateMachine.Run();//Call Run to run one iteration of a state and it's transitions.
}

//States:
private static void EvenState(SimpleStateMachine ownerMachine)
{
    Debug.Log("Time is even.");
}

private static void OddState(SimpleStateMachine ownerMachine)
{
    Debug.Log("Time is odd.");
}

//Transitions:
private static bool EvenToOddTransition(SimpleStateMachine ownerMachine)
{
    return ownerMachine.GetFloat("time") % 2 >= 1.0f;
}

private static bool OddToEvenTransition(SimpleStateMachine ownerMachine)
{
    return ownerMachine.GetFloat("time") % 2 < 1.0f;
}
```