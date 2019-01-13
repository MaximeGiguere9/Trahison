/**
 * script TutoAttaque
 * Olivier Doyle-Turcotte
 * 
 * Explique au joueur comment attaquer à partir d'une boite de texte qui est intéractive
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoAttaque : InteractableObject {

	public GameObject _Monstre;


	protected override void Start () {
		base.Start();
		this._interactActionText = "Interagir";
		this._requirePrompt = true;
	}

	protected override void ExecuteObjectAction()
	{
		TextBoxManager.EnqueueFile ("TutoAttaque");
	}

}
