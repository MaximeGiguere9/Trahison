/**
 * script SQ2_CleMonstre
 * Maxime Giguere
 * 
 * gère l'apparition et l'obtention de la clé des monstres
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQ2_CleMonstre : InteractableObject {

    //le GameObject parent au groupe de monstres à tuer pour obtenir la clé
	public GameObject _monsterContainer;
    //la position du dernier monstre à sa mort
	private Vector3 _lastChildPosition;
	//source de sons
	public AudioClip _keyAppearSound;
	public AudioClip _keyGetSound;

	protected override void Start () {
		base.Start();
		this._requirePrompt = false;
		//si la clé est déjà obtenue (e.g. en rechargeant la scène)
		if (PlayerData.Get ("hasKeySQ2") == "true") Destroy (gameObject);
		StartCoroutine (WatchMonsterCount());
	}

    //la clé n'apparait que quand tous les monstres sont vaincus
    private IEnumerator WatchMonsterCount(){
        //tant qu'il y a un monstre dans le groupe, sa position est gardée en mémoire et la clé est cachée
        while (_monsterContainer.GetComponent<Transform>().childCount > 0) {
			_lastChildPosition = _monsterContainer.GetComponent<Transform> ().GetChild (0).transform.position;
			GetComponent<MeshRenderer> ().enabled = false;
			yield return true;
		}
        //quand tous les monstres sont morts, la clé apparait à la dernière position connue (effectivement la position du dernier monstre au moment de sa mort)
		gameObject.transform.position = new Vector3(_lastChildPosition.x, _lastChildPosition.y +1, 0);
		GetComponent<MeshRenderer> ().enabled = true;
		GameObject.Find ("Musique").GetComponent<AudioSource> ().PlayOneShot (_keyAppearSound);

	}

    //message d'obtention de la clé et suppression de l'objet dans la scène
    protected override void ExecuteObjectAction()
	{
		if (_monsterContainer.GetComponent<Transform> ().childCount == 0) {
			TextBoxManager.EnqueueFile("SQ2_cleAcquise");
			PlayerData.Set ("hasKeySQ2", "true");

			//si les trois clés sont obtenues
			if (PlayerData.Get ("hasKeySQ1") == "true" && PlayerData.Get ("hasKeySQ2") == "true" && PlayerData.Get ("hasKeySQ3") == "true") {
				//message de fin de quête
				TextBoxManager.EnqueueFile("MQ_Notice3ClesCollectees");
			}
			GameObject.Find ("Musique").GetComponent<AudioSource> ().PlayOneShot (_keyGetSound);
			Destroy (gameObject);
		}

	}

}
