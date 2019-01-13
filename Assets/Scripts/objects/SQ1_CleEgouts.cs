/**
 * script SQ1_CleEgouts
 * Maxime Giguere
 * 
 * le script contrôle l'apparition de la clé des égouts et son obtention par le joueur
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQ1_CleEgouts : InteractableObject {

	//source de sons
	public AudioClip _keyGetSound;


	protected override void Start () {
		base.Start();
		this._requirePrompt = false;
		StartCoroutine (DisplayKey());
	}

    //la clé n'apparaît que si le joueur a commencé la quête des égouts (et ne l'a pas abandonnée)
	private IEnumerator DisplayKey(){
		while (PlayerData.Get ("hasKeySQ1") != "true") {
			if(PlayerData.Get ("hasStartedSQ1") != "true")GetComponent<MeshRenderer> ().enabled = false;
			else GetComponent<MeshRenderer> ().enabled = true;
			yield return true;
		}
	}

    //message d'obtention de la clé et suppression de l'objet dans la scène
	protected override void ExecuteObjectAction()
	{
		if (PlayerData.Get ("hasStartedSQ1") == "true") {
			TextBoxManager.EnqueueFile("SQ1_cleAcquise");
			PlayerData.Set ("hasKeySQ1", "true");


			//si les trois clés sont obtenues
			if (PlayerData.Get ("hasKeySQ1") == "true" && PlayerData.Get ("hasKeySQ2") == "true" && PlayerData.Get ("hasKeySQ3") == "true") {
				//message de fin de quête
				TextBoxManager.EnqueueFile("MQ_Notice3ClesCollectees");
			}
			GameObject.Find ("Musique").GetComponent<AudioSource> ().PlayOneShot (_keyGetSound, 0.8f);
			Destroy (gameObject);
		}

	}

}
