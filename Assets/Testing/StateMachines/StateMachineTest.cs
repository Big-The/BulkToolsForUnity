using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTools.StateMachines;

public class StateMachineTest : MonoBehaviour
{
    private SimpleStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = new SimpleStateMachine("StateXSin", StateXSin)
            .AddState("StateYSin", StateYSin)
            .AddState("StateZSin", StateZSin)
            .AddTransition("StateXSin", "StateYSin", CycleStateTransitionTest)
            .AddTransition("StateYSin", "StateZSin", CycleStateTransitionTest)
            .AddTransition("StateZSin", "StateXSin", CycleStateTransitionTest);
    }

    private void Update()
    {
        stateMachine.Run();
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
