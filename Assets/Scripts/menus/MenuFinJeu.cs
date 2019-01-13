/**
 * script MenuFinJeu
 * Maxime Giguere
 * 
 * Ce script contrôle l'interactivité du menu affiché à la fin de la quête. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFinJeu : Menu {

	public UnityEngine.UI.Text _profileNameField;

	protected override void ExecuteBeforeInit ()
	{
		_profileNameField.text = PlayerData.Get ("_ProfileName");
	}

	protected override void SetActions ()
	{
		SetAction ("Retour", delegate() {
			SceneManager.LoadScene("menuPrincipal");
		});
	}

	protected override void OnCancel ()
	{
		GetAction ("Retour") ();
	}

}
