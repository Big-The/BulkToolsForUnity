using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTools.MagicEvents;

public class EventsTesterListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MagicEvent.AddListener("TestSpaceEvent", OnSpace);
        MagicEvent.AddListener("TestEscEvent", OnEsc);
        MagicEvent.AddListener("TestEscEvent", BrokenOnEsc);
        MagicEvent.AddListener("SubEvent", SubEvent);
    }

    public void OnSpace(MagicEventContext context)
    {
        Debug.Log("Space state changed, Down:" + context.GetData<bool>("Down"));
        if (context.GetData<bool>("Down")) 
        {
            new MagicEvent("SubEvent")
                .Invoke();
        }
    }

    public void OnEsc(MagicEventContext context)
    {
        Debug.Log("Escape Down");
    }

    public void BrokenOnEsc(MagicEventContext context)
    {
        throw new System.Exception();
    }

    public void SubEvent(MagicEventContext context)
    {
        Debug.Log("Escape Down");
    }
}
