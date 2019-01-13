/**
 * script SQ1_CleEgouts
 * Maxime Giguere
 * 
 * le script contrôle le message qui s'affiche quand le joueur s'approche du groupe de monstre pour la première fois
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQ2_NotificationDebut : InteractableObject {

    protected override void Start()
    {
        base.Start();
        this._requirePrompt = false;
    }


    //message d'obtention de la clé et suppression de l'objet dans la scène
    protected override void ExecuteObjectAction()
    {
        if (PlayerData.Get("hasStartedSQ2") != "true")
        {
            TextBoxManager.EnqueueFile("SQ2_notificationDebut");
            PlayerData.Set("hasStartedSQ2", "true");
        }

    }

}
