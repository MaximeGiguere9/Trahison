/**
 * script Void
 * Maxime Giguère
 * 
 * Ce script définit la zone où Alex est considéré comme dans le vide, hors de la carte, et le tue.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : InteractableObject {

	protected override void Start () {
        base.Start();
        this._requirePrompt = false;
        this._interactActionText = "";
	}

    protected override void ExecuteObjectAction()
    {
        //tue le joueur quand il rentre en collision avec le trigger du vide
        this._playerReference.GetComponentInChildren<PlayerCombatModule>().Damage(1000000, gameObject);
    }
}
