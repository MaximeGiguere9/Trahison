/**
 * script MQ_Generateur
 * Maxime Giguere
 * 
 * le script associé au générateur. Gère aussi l'état de la quête principale. 
 * La quête démarre quand le joueur interagit avec le générateur pour la première fois et ne s'active que si les trois clés sont obtenues.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MQ_Generateur : InteractableObject {

	public GameObject _cles;
	public GameObject _generateur;
	public Image _rideau;

	protected override void Start () {
		base.Start();
		this._interactActionText = "Interagir";
		this._requirePrompt = true;
	}


	protected override void ExecuteObjectAction()
	{
        //démarre la quête
		if (PlayerData.Get ("hasStartedMQ") != "true") {
			PlayerData.Set ("hasStartedMQ", "true");
		    TextBoxManager.EnqueueFile("MQ_Debut");
			return;
		}
		if (PlayerData.Get ("hasActivatedGenerator") != "true") {
			//si le joueur a toutes les clés, affiche le texte de confirmation qui lui permet d'activer le générateur, sinon lui indique qu'il lui manque des clés
			if (PlayerData.Get ("hasKeySQ1") == "true" && PlayerData.Get ("hasKeySQ2") == "true" && PlayerData.Get ("hasKeySQ3") == "true") {
				System.Func<bool> ActiverGenerateur = () => {
					PlayerData.Set ("hasActivatedGenerator", "true");
					_cles.SetActive (true);
					_cles.gameObject.GetComponent<Animator> ().SetBool ("actif", true);
					_generateur.GetComponent<Animator> ().SetBool ("actif", true);
					StartCoroutine (CineFin ());
					_isPlayerNearby = false;
					HideInteractPrompt ();
					return true;
				};
				TextBoxManager.EnqueueFile ("MQ_ActivationGenerateur");
				TextBoxManager.SetOnReceiveDataAction ("MQ_ConfirmationGenerateurActif", ActiverGenerateur);
			} else {
				TextBoxManager.EnqueueFile ("MQ_NoticeCleManquante");
			}
		}

	}


	IEnumerator CineFin(){
		_rideau.gameObject.SetActive (true);
		//laisse place à l'animation
		yield return new WaitForSeconds (3f);
		//augmente l'opacité pour faire un fondu au noir
		Color color = _rideau.color;
		while (_rideau.color.a < 1) {
			color.a += 0.01f;
			_rideau.color = color;
			yield return true;
		}
		//écran de fin
		SceneManager.LoadScene ("menuFinJeu");
	}

}
