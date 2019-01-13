/**
 * script HealthPack
 * Maxime Giguère
 * 
 * action de l'objet interactif du pack de soins (i.e. soigne les points de vie d'Alex quand il interagit avec l'objet)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : InteractableObject
{
    public int _strength = 1;

    protected override void Start()
    {
        base.Start();
        this._requirePrompt = false;
        this._interactActionText = "Se soigner";
    }

    protected override void ExecuteObjectAction()
    {
        //soigne le joueur s'il n'a pas tous ses points de vie
        PlayerCombatModule p = this._playerReference.GetComponentInChildren<PlayerCombatModule>();
        if (p.GetHPRatio() < 1)
        {
            p.Heal(_strength, gameObject);
            Destroy(gameObject);
        }

    }

}
