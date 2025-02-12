using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTools.MagicEvents;

public class EventsTesterSender : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            new MagicEvent("TestSpaceEvent")
                .AddData("Down", true)
                .AddData("Null", null)
                .Invoke();
        }
        if (Input.GetKeyUp(KeyCode.Space)) 
        {
            new MagicEvent("TestSpaceEvent")
                .AddData("Down", false)
                .Invoke();
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            new MagicEvent("TestEscEvent")
                .Invoke();
        }
    }
}

