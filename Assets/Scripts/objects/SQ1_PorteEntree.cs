/**
 * script SQ1_PorteEntree
 * Maxime Giguere
 * 
 * le script associé à l'entrée des égouts. Permet au joueur de commencer ou d'abandonner la quête des égouts, et gère tout le code y étant relié
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SQ1_PorteEntree : InteractableObject {



	protected override void Start () {
		base.Start();
		this._interactActionText = "Interagir";
		this._requirePrompt = true;
		//assure le fonctionnement du dialogue entre sessions de jeu
		if(SceneManager.GetActiveScene().name == "ville") PlayerData.Set("hasStartedSQ1", "false");
	}

	protected override void ExecuteObjectAction()
	{
        //message si le joueur a déjà terminé la quête des égouts et essaie d'y retourner
		if (PlayerData.Get ("hasFinishedSQ1") == "true") {
			TextBoxManager.EnqueueFile("SQ1_postQuete");
			return;
		}

        //message de succès de la quête quand le joueur retourne à la porte avec la clé
		if (PlayerData.Get ("hasKeySQ1") == "true" && PlayerData.Get("hasFinishedSQ1") != "true") {
            //suppression du décompte
			if (TimerManager.DoesTimerExist ("SQ1")) {
				TimerManager.GetTimer ("SQ1").Stop ();
                TimerManager.RemoveTimer("SQ1");
            }
            //le joueur a terminé la quête
            PlayerData.Set ("hasFinishedSQ1", "true");
            //changer de scène quand le joueur termine la discussion
            System.Func<bool> TransitionFinSQ1 = () => {
				PlayerData.Set("sceneTransitionSource", "egouts");
				PlayerData.Set("currentScene", "ville");
                SceneManager.LoadScene("ville");
                return true;
            };
            //message de fin de quête
            TextBoxManager.EnqueueFile("SQ1_fin");
            TextBoxManager.SetOnCompleteAction(TransitionFinSQ1);

			
			return;
		}


        
        //si la quête est en cours
		if (PlayerData.Get("hasStartedSQ1") == "true") {
            //code à exécuter si le joueur confirme l'abandon
			System.Func<bool> AbandonSQ1 = () => {
                //arrête le timer
				if (TimerManager.DoesTimerExist ("SQ1")) {
					TimerManager.GetTimer ("SQ1").Stop ();
					//TimerManager.RemoveTimer("SQ1");
				}
                //le joueur a abandonné la quête
                PlayerData.Set("hasStartedSQ1", "false");
				PlayerData.Set("sceneTransitionSource", "egouts");
				PlayerData.Set("currentScene", "ville");
				SceneManager.LoadScene("ville");
				return true;
			};
            //message d'abandon de la quête quand le joueur interagit avec la porte sans la clé
            TextBoxManager.EnqueueFile("SQ1_abandon"); 
            TextBoxManager.SetOnReceiveDataAction("abandon", AbandonSQ1); //confirmation
            return;
		} 

        //code à exécuter si le joueur confirme le début de la quête
        System.Func<bool> TextBoxDataAction = () =>
        {
            //code à exécuter si le timer se termine (arrive à 0)
            System.Func<bool> TimerAction = () =>
            {
                GameObject.Find("Alex").GetComponentInChildren<PlayerCombatModule>().Damage(1000000);
                return true;
            };
            //crée le timer ou le redémarre s'il existe déjà
            if (TimerManager.DoesTimerExist("SQ1"))TimerManager.GetTimer("SQ1").ResetCountDown();
            else new Timer("SQ1", 240, TimerAction);
            //le joueur a commencé la quête
            PlayerData.Set("hasStartedSQ1", "true");
			PlayerData.Set("sceneTransitionSource", "ville");
			PlayerData.Set("currentScene", "egouts");
			SceneManager.LoadScene("egouts");
            return true;
        };


		//si le timer existe, ce n'est pas la premiere fois de la session de jeu qu'on entre dans les égouts, alors on donne un texte allégé
		if (TimerManager.DoesTimerExist ("SQ1")) {
			TextBoxManager.EnqueueFile ("SQ1_debutRetour");
		} else {
			TextBoxManager.EnqueueFile("SQ1_debut"); 
		}
		TextBoxManager.SetOnReceiveDataAction("SQ1_debut", TextBoxDataAction); //confirmation	


    }

}
