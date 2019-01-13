/**
 * script HUD
 * Maxime Giguere
 * 
 * contrôle l'affichage des éléments du HUD en allant chercher les valeurs dans PlayerData
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDAlex : MonoBehaviour {

	//éléments de l'interface à contrôler et modifier par prog
	public GameObject _barreVieJoueur;
	public GameObject _barreMana;
	public GameObject _groupeBoss;
	public GameObject _barreVieBoss;
	public GameObject _groupeTimer;
    public UnityEngine.UI.Text _timer;
    public GameObject _cleCiel;
	public GameObject _cleEgouts;
	public GameObject _cleMonstre;
	public GameObject _menuPause;


	void start(){
		_menuPause.gameObject.SetActive (false);
	}

	void Update () {
		//affiche les clés sur l'interface quand le joueur les obtient
		_cleCiel.SetActive (PlayerData.Get ("hasKeySQ3") == "true" ? true : false);
		_cleEgouts.SetActive (PlayerData.Get ("hasKeySQ1") == "true" ? true : false);
		_cleMonstre.SetActive (PlayerData.Get ("hasKeySQ2") == "true" ? true : false);
        //actualise l'affichage du timer
		_groupeTimer.SetActive(TimerManager.DoesTimerExist("SQ1") && (PlayerData.Get("hasStartedSQ1") == "true"));
        _timer.text = (TimerManager.DoesTimerExist("SQ1") ? TimerManager.GetTimer("SQ1").GetFormattedRemainingTime() : " ");
        //actualise la barre de vie
        if(PlayerData.Get("playerHP") != null && PlayerData.Get("playerMaxHP") != null)
        {
            _barreVieJoueur.GetComponent<UnityEngine.UI.Image>().fillAmount = (float)System.Int32.Parse(PlayerData.Get("playerHP")) / (float)System.Int32.Parse(PlayerData.Get("playerMaxHP"));
        }
        
		//affichage du menu de pause
		if (!InputManager.IsAvatarInputLocked() && InputManager.GetCommand (InputManager.UIControl.menu) && _menuPause.activeSelf == false) {
			_menuPause.gameObject.SetActive (true);
		}
	}
}
