/**
 * script MenuPause
 * Maxime Giguere
 * 
 * Comme son nom l'indique, ce script contrôle l'interactivité du menu du même nom.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : Menu { 

	protected override void SetActions ()
	{
		SetAction ("Reprendre", delegate() {
			TimerManager.ResumeAll ();//pas sur OnDisable car reprendrait aussi quand retour aux menus
			gameObject.SetActive (false);
		});
		SetAction ("Sauvegarder", delegate() {
			PlayerData.SaveToFile ();
		});
		SetAction ("Quitter", delegate() {
			if(SceneManager.GetActiveScene().name != "ville") PlayerData.Set("sceneTransitionSource", SceneManager.GetActiveScene().name);
			SceneManager.LoadScene ("menuPrincipal");
		});
	}

	protected override void OnCancel ()
	{
        GetAction("Reprendre")();
	}

	void OnEnable(){
		EnableMouseSelect (true);
		InputManager.SetAvatarInputLock (true);
		TimerManager.StopAll ();
	}

	void OnDisable(){
		InputManager.SetAvatarInputLock (false);
	}



}
