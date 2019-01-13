/**
 * script SQ1_CleEgouts
 * Maxime Giguere
 * 
 * le script contrôle le message qui s'affiche quand le joueur va sur les toits pour la première fois
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQ3_NotificationDebut : InteractableObject {

	protected override void Start () {
		base.Start();
		this._requirePrompt = false;
	}


	protected override void ExecuteObjectAction()
	{
		if (PlayerData.Get ("hasStartedSQ3") != "true") {
			TextBoxManager.EnqueueFile("SQ3_notificationDebut");
			PlayerData.Set ("hasStartedSQ3", "true");
		}

	}

}
