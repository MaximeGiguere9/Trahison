/**
 * Tuto_sortie
 * Benjamin Marinier
 * 
 * le script associé à la porte menant aux toits.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tuto_sortie : InteractableObject {



protected override void Start () {
	base.Start();
	this._interactActionText = "Interagir";
	this._requirePrompt = true;
}

protected override void ExecuteObjectAction()
{
		
		System.Func<bool> TextBoxDataAction = () =>
		{
			PlayerData.Set("sceneTransitionSource", "sceneTuto");
			PlayerData.Set("currentScene", "ville");
			SceneManager.LoadScene("ville");
			return true;
		};
		//boîte de texte
		TextBoxManager.EnqueueFile("Tuto_sortie");
		TextBoxManager.SetOnReceiveDataAction("Tuto_sortie", TextBoxDataAction); //confirmation	     


	}

}
