//test/debug

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoxProp : InteractableObject {

	
	protected override void Start () {
        base.Start();
        this._interactActionText = "Interact";
        this._requirePrompt = true;
	}

    protected override void ExecuteObjectAction()
    {
        TextBoxManager.EnqueueFile("testbox2");
        //on complete
        System.Func<bool> onCompleteAction = () =>
        {
            Debug.Log("On complete!!!!!");
            TextBoxManager.ClearOnReceiveDataAction("data test");
            TextBoxManager.ClearOnReceiveDataAction("gattai");
            return true;
        };
        TextBoxManager.SetOnCompleteAction(onCompleteAction);
        //on data
        System.Func<bool> onDataAction = () =>
        {
            Debug.Log("On Data!!!!!");
            return true;
        };
        TextBoxManager.SetOnReceiveDataAction("data test", onDataAction);
        //on complete defined by an on data function
        System.Func<bool> onDataAction2 = () =>
        {
            System.Func<bool> onCompleteAction2 = () =>
            {
                Debug.Log("OnComplete defined by OnData!!!!!");
                TextBoxManager.ClearOnReceiveDataAction("data test");
                TextBoxManager.ClearOnReceiveDataAction("gattai");
                return true;
            };
            TextBoxManager.SetOnCompleteAction(onCompleteAction2);

            return true;
        };
        TextBoxManager.SetOnReceiveDataAction("gattai", onDataAction2);

        PlayerData.Set("hasInteractedWithCube", "true");
    }
}
