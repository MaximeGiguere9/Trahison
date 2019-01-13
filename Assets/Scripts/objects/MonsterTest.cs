//test/debug

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTest : InteractableObject {

	protected override void Start () {
		base.Start();
		this._requirePrompt = false;
	}
	
	protected override void ExecuteObjectAction()
	{
		//Destroy (gameObject);
	}
}
