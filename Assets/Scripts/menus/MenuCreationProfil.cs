/**
 * script MenuCréationProfil
 * Maxime Giguere
 * 
 * Comme son nom l'indique, ce script contrôle l'interactivité du menu du même nom.
 * En plus, il cherche tous les fichiers dans le dossier de sauvegarde afin de pouvoir afficher les détails des différents profils
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCreationProfil : Menu {

    //message d'erreur si nom vide
	public GameObject _emptyNameAlert;
    //message de chargement
    public GameObject _loadNotification;

	private bool _isEnteringTextInput = false;
	private string _profileName = "";

    protected override void SetActions()
    {
        SetAction("Confirmer", delegate() {
            if (!_isEnteringTextInput)
            {
                if (_profileName == "")
                {
                    //le nom ne peut pas être vide
                    _emptyNameAlert.SetActive(true);
                }
                else
                {
					LoadGame();
                }
			} else {
				RecieveTextString();
				LoadGame();
			}
        });
        SetAction("InputField", delegate () {
            if (!_isEnteringTextInput)
            {
                //focus sur le champ de texte nécessaire car l'interface est contrôlée par WASD
                GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.StandaloneInputModule>().ActivateModule();
                GetSelectedOption().GetComponent<UnityEngine.UI.InputField>().ActivateInputField();
            }
        });
        SetAction("Retour", delegate () {
            if (!_isEnteringTextInput)
            {
                SceneManager.LoadScene("menuPrincipal");
            }
        });
    }

    protected override void OnCancel()
    {
        if (_isEnteringTextInput)
        {
            //nécessaire pour empêcher que le champ se focus
            GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.StandaloneInputModule>().DeactivateModule();
            //enlève le focus
            GetSelectedOption().GetComponent<UnityEngine.UI.InputField>().DeactivateInputField();
        }
        else
        {
            SceneManager.LoadScene("menuPrincipal");
        }
    }

	protected override void Update ()
	{
		base.Update ();

        if (GetSelectedOption().GetComponent<UnityEngine.UI.InputField>() == null)
        {
            //enlève le focus
            _optionContainers[0].GetComponentInChildren<UnityEngine.UI.InputField>().DeactivateInputField();
        }

        _isEnteringTextInput = _optionContainers[0].GetComponentInChildren<UnityEngine.UI.InputField> ().isFocused;
		EnableMouseSelect(!_isEnteringTextInput);
		EnableUpdate(!_isEnteringTextInput);

	}

    //méthode appelée par le champ de texte, extrait la chaîne de caractères entrée
    public void RecieveTextString(){
		_profileName = _optionContainers[0].GetComponentInChildren<UnityEngine.UI.InputField> ().textComponent.text;
		if (_profileName != "" && _emptyNameAlert.activeSelf)
			_emptyNameAlert.SetActive (false);

	}

	private void LoadGame(){
		//cacher le champ de texte dû à un comportement indésirable qui apparait dans certains cas. La fonctionnalité n'est pas affectée
		this._optionContainers[0].transform.GetChild(0).gameObject.transform.Translate(new Vector3(10000, 0, 0));
		//message de chargement
		_loadNotification.SetActive(true);
		//créer le profil et charger la première scène
		PlayerData.NewProfile(_profileName);
		PlayerData.Set("currentScene", "sceneTuto");
		//sauvegarde car currentScene est essentiel pour le chargement
		PlayerData.SaveToFile();
		SceneManager.LoadScene("cinematique");
	}

	public void DeselectInputField(){
		OnCancel ();
	}
}
