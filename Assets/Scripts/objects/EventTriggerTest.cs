//test/debug

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerTest : InteractableObject
{


    protected override void Start()
    {
        base.Start();
        this._requirePrompt = false;
    }

    protected override void ExecuteObjectAction()
    {
        if (PlayerData.Get("hasTriggeredEvent") == "true") return;
        TextBoxManager.EnqueueFile("testbox2");
        PlayerData.Set("hasTriggeredEvent", "true");
    }
}
