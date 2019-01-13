/**
 * script SQ4_WallJump
 * Lilian Vinel
 * 
 * le script associé à la boite de dialogue qui explique comment faire un "walljump".
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SQ4_WallJump : InteractableObject {



	protected override void Start () {
		base.Start();
		this._interactActionText = "Interagir";
		this._requirePrompt = true;
	}

	protected override void ExecuteObjectAction()
	{
		//boîte de texte
		TextBoxManager.EnqueueFile("SQ4_InstructionsWallJump");
		//PlayerData.Set("SQ3_abandon", "true"); //confirmation

	}

}

