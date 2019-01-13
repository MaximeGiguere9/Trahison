/**
 * script SQ2_ClePlateforme
 * Maxime Giguere
 * 
 * gère l'apparition et l'obtention de la clé des plateformes. Cete clé n'a pas de conditions particulières à son apparition, le joueur doit seulement réussir à l'atteindre.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQ3_ClePlateforme : InteractableObject {

	//source de sons
	public AudioClip _keyGetSound;

	protected override void Start () {
		base.Start();
		this._requirePrompt = false;
	}


    //message d'obtention de la clé et suppression de l'objet dans la scène
    protected override void ExecuteObjectAction()
	{
		TextBoxManager.EnqueueFile("SQ3_cleAcquise");
		PlayerData.Set ("hasKeySQ3", "true");

		//si les trois clés sont obtenues
		if (PlayerData.Get ("hasKeySQ1") == "true" && PlayerData.Get ("hasKeySQ2") == "true" && PlayerData.Get ("hasKeySQ3") == "true") {
			//message de fin de quête
			TextBoxManager.EnqueueFile("MQ_Notice3ClesCollectees");
		}
		GameObject.Find ("Musique").GetComponent<AudioSource> ().PlayOneShot (_keyGetSound);
		Destroy (gameObject);
	}

}
