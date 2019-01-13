/**
 * classe PlayerCombatModule
 * Maxime Giguere
 * 
 * Fait l'interface entre les données du joueur et le CombatModule, qui est plus générique et peut s'appliquer à n'importe qui
 * Ce code permet l'accès aux données de vie du joueur à partir  d'autres classes
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatModule : CombatModule {

	protected override void Start () {
        base.Start();
        //actualise la barre de vie (conserve le nombre de points de vie entre scenes, et entre sessions de jeu)
        if (PlayerData.Get("playerHP") != null) SetCurrentHP(System.Int32.Parse(PlayerData.Get("playerHP")));
        if (PlayerData.Get("playerMaxHP") != null) SetMaxHP(System.Int32.Parse(PlayerData.Get("playerMaxHP")));

        PlayerData.Set("playerHP", GetCurrentHP().ToString());
        PlayerData.Set("playerMaxHP", GetMaxHP().ToString());

    }


    //sauvegarde les points de vie
    protected override void OnHeal()
    {
        PlayerData.Set("playerHP", GetCurrentHP().ToString());
        PlayerData.Set("playerMaxHP", GetMaxHP().ToString());
    }

    protected override void OnDamage()
    {
        PlayerData.Set("playerHP", GetCurrentHP().ToString());
        PlayerData.Set("playerMaxHP", GetMaxHP().ToString());
    }

	//quand le joueur meurt
	protected override void OnDeath()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("menuGameOver");
	}

}
