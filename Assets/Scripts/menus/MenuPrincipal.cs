/**
 * script MenuPrincipal
 * Maxime Giguere
 * 
 * script qui gère l'interactivité du menu principal. Permet au joueur de sélectionner les options du menu, et de charger la scène du jeu.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class MenuPrincipal : Menu {

    protected override void SetActions()
    {
        SetAction("Jouer", delegate () {
            SceneManager.LoadScene("menuCreationProfil");
        });
        SetAction("Charger", delegate () {
            SceneManager.LoadScene("menuChargement");
        });
        SetAction("Options", delegate () {
            SceneManager.LoadScene("menuOptions");
        });
        SetAction("Quitter", delegate () {
            Application.Quit();
            Debug.Log("Application.Quit is ignored in the editor.");
        });
    }

}
