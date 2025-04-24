using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTools.StateMachines;

public class StateMachineTest : MonoBehaviour
{
    private SimpleStateMachine stateMachine;


    [SerializeField]
    private TestDataObject testDataObject = new TestDataObject();
    private StateMachine<TestDataObject> classBasedStateMachine;

    private void Awake()
    {
        stateMachine = new SimpleStateMachine("StateXSin", StateXSin)
            .AddState("StateYSin", StateYSin)
            .AddState("StateZSin", StateZSin)
            .AddTransition("StateXSin", "StateYSin", CycleStateTransitionTest)
            .AddTransition("StateYSin", "StateZSin", CycleStateTransitionTest)
            .AddTransition("StateZSin", "StateXSin", CycleStateTransitionTest);


        classBasedStateMachine = new StateMachine<TestDataObject>(testDataObject)
            .AddState("AState", new TestStateA()
                .AddTransition("BState", (data) => { return data.testData == "B"; })
                .AddTransition("CState", (data) => { return data.testData == "C"; })
                .AddTransition("DState", (data) => { return data.testData == "D"; }))
            .AddState("BState", new TestStateB()
                .AddTransition("AState", (data) => { return data.testData == "A"; })
                .AddTransition("CState", (data) => { return data.testData == "C"; })
                .AddTransition("DState", (data) => { return data.testData == "D"; }))
            .AddState("CState", new TestStateC()
                .AddTransition("AState", (data) => { return data.testData == "A"; })
                .AddTransition("BState", (data) => { return data.testData == "B"; })
                .AddTransition("DState", (data) => { return data.testData == "D"; }))
            .AddState("DState", new TestStateD()
                .AddTransition("AState", (data) => { return data.testData == "A"; })
                .AddTransition("BState", (data) => { return data.testData == "B"; })
                .AddTransition("CState", (data) => { return data.testData == "C"; }));

    }

    private void Update()
    {
        //stateMachine.Run();
        classBasedStateMachine.Tick();
    }

    private bool CycleStateTransitionTest(SimpleStateMachine stateMachine) 
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private void StateXSin(SimpleStateMachine stateMachine)
    {
        transform.position = new Vector3(Mathf.Sin(Time.timeSinceLevelLoad), transform.position.y, transform.position.z);
    }

    private void StateYSin(SimpleStateMachine stateMachine)
    {
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.timeSinceLevelLoad), transform.position.z);
    }

    private void StateZSin(SimpleStateMachine stateMachine)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Sin(Time.timeSinceLevelLoad));
    }
}

[System.Serializable]
public class TestDataObject
{
    public string testData = "A";
}

#region ClassStates

public class TestStateA : StateMachine<TestDataObject>.State
{
    protected override void Tick()
    {
        Debug.Log("StateA");
    }
}

public class TestStateB : StateMachine<TestDataObject>.State
{
    protected override void Tick()
    {
        Debug.Log("StateB");
    }
}

public class TestStateC : StateMachine<TestDataObject>.State
{
    protected override void Tick()
    {
        Debug.Log("StateC");
    }
}

public class TestStateD : StateMachine<TestDataObject>.State
{
    protected override void Tick()
    {
        Debug.Log("StateD");
    }
}


#endregion