/**
 * script MenuGameOver
 * Maxime Giguere
 * 
 * Comme son nom l'indique, ce script contrôle l'interactivité du menu du même nom.
 * En plus, il sauvegarde et recharge les données du profil, et compile de nombre de morts du profil
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameOver : Menu {

	void SaveOnDeath ()
	{
		//regénère les points de vie pour ne pas qu'Alex meurt immédiatement après avoir rechargé la partie
		PlayerData.Set("playerHP", PlayerData.Get("playerMaxHP"));
		//garde en mémoire le nombre de morts
		if(PlayerData.Get("nbMort") == null) PlayerData.Set("nbMort", "0");
		PlayerData.Set("nbMort", (System.Int32.Parse(PlayerData.Get("nbMort")) + 1).ToString());
		//sauvegarde
		PlayerData.SaveToFile();
	}

	protected override void SetActions ()
	{
		SetAction ("Charger", delegate() {
			//sauvegarde
			SaveOnDeath();
			//charge la bonne scène
			switch (PlayerData.Get("currentScene")) 
			{
				case "ville":
					SceneManager.LoadScene("ville");
					break;
				case "egouts": 
					PlayerData.Set("sceneTransitionsource", "egouts");
					SceneManager.LoadScene("ville");
					break;
				case "ciel" : 
					PlayerData.Set("sceneTransitionsource", "ville");
					SceneManager.LoadScene("ciel");
					break;
				case "sceneTuto" : 
					SceneManager.LoadScene("sceneTuto");
					break;
				default:
					Debug.LogError("Unable to parse scene name " + PlayerData.Get("currentScene"));
					break;
			}
		});
		SetAction ("Sauvegarder", delegate() {
			//sauvegarde
			SaveOnDeath();
			//charge le menu
			SceneManager.LoadScene("menuPrincipal");
		});
	}

}
