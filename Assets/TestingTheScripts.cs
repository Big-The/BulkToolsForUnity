using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTools.BTags;
using BTools.Singletons;
using BTools.StateMachines;
using BTools.StaticTools;
using BTools.UtilPack;
using BTools.MoreAttributes;
using BTools.MagicEvents; 

public class TestingTheScripts : MonoBehaviour
{    
    private void Awake()
    {
        MagicEvent.AddListener("", OnEvent);
        MagicEvent.AddListener("RandomNumber", OnRandomNumber);

        new MagicEvent("RandomNumber")
            .AddData("number", Random.Range(0f, 1f))
            .Invoke();
    }

    private void OnEvent(MagicEventContext context) 
    {
        Debug.Log(context.eventName);
    }

    private void OnRandomNumber(MagicEventContext context) 
    {
        Debug.Log(context.GetData<float>("number"));
    }
}
