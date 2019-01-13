/**
 * script SQ3_PorteEntree
 * Maxime Giguere
 * 
 * le script associé à la porte menant aux toits.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SQ3_PorteEntree : InteractableObject {



	protected override void Start () {
		base.Start();
		this._interactActionText = "Interagir";
		this._requirePrompt = true;
	}

	protected override void ExecuteObjectAction()
	{
        //si le joueur est dans la scene du ciel
        if (SceneManager.GetActiveScene ().name == "ciel") {
            //code à exécuter si le joueur confirme la transition vers la ville
            System.Func<bool> TextBoxDataAction = () =>
            {
                PlayerData.Set("sceneTransitionSource", "ciel");
				PlayerData.Set("currentScene", "ville");
                SceneManager.LoadScene("ville");
                return true;
            };
            //boîte de texte
            TextBoxManager.EnqueueFile("SQ3_abandon");
            TextBoxManager.SetOnReceiveDataAction("SQ3_abandon", TextBoxDataAction); //confirmation	     
        //si le joueur est dans la scene de la ville
		} else {
            //code à exécuter si le joueur confirme la transition vers le ciel
            System.Func<bool> TextBoxDataAction = () =>
            {
                PlayerData.Set("sceneTransitionSource", "ville");
				PlayerData.Set("currentScene", "ciel");
                SceneManager.LoadScene("ciel");
                return true;
            };
            //boîte de texte
            TextBoxManager.EnqueueFile("SQ3_debut");
            TextBoxManager.SetOnReceiveDataAction("SQ3_debut", TextBoxDataAction); //confirmation	     
		}

	}

}
