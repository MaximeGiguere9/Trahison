/**
 * script MenuDetailsFichier
 * Maxime Giguere
 * 
 * Comme son nom l'indique, ce script contrôle l'interactivité du menu du même nom.
 * En plus, il affiche les données de partie du profil chargé
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuDetailsFichier : Menu {

    //composante où afficher le nom du profil
	public UnityEngine.UI.Text _titreProfil;


    protected override void ExecuteBeforeInit()
    {
        //affiche le nom du profil
        _titreProfil.text = PlayerData.Get("_ProfileName");
        //afficher les clés si elles ont été obtenues
        GameObject.Find("CleMonstre").SetActive(PlayerData.Get("hasKeySQ2") == "true");
        GameObject.Find("CleEgout").SetActive(PlayerData.Get("hasKeySQ1") == "true");
        GameObject.Find("CleSky").SetActive(PlayerData.Get("hasKeySQ3") == "true");
    }

    protected override void SetActions()
    {
        SetAction("Charger", delegate () {
			//charge la bonne scène
			switch (PlayerData.Get("currentScene")) {
			case "ville":
				SceneManager.LoadScene("ville");
				break;
			case "egouts": 
				PlayerData.Set("sceneTransitionSource", "egouts");
				SceneManager.LoadScene("ville");
				break;
			case "ciel" : 
				PlayerData.Set("sceneTransitionSource", "ville");
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
        SetAction("Supprimer", delegate () {
            System.IO.File.Delete(Application.persistentDataPath + "/saves/" + PlayerData.Get("_ProfileName") + ".xml");
            SceneManager.LoadScene("menuChargement");
        });
        SetAction("Retour", delegate () {
            SceneManager.LoadScene("menuChargement");
        });
    }

    protected override void OnCancel()
    {
        SceneManager.LoadScene("menuChargement");
    }

}
