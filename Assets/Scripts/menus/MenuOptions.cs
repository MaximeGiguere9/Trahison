/**
 * script MenuOptions
 * Maxime Giguere
 * 
 * Comme son nom l'indique, ce script contrôle l'interactivité du menu du même nom.
 * En plus, il permet de faire le pont entre l'utilisateur et InputManager, affichant toutes les touches et permettant à l'utilisateur de les configurer comme il veut
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : Menu {



	//références aux options du menu pour la manipulation de l'affichage
	public GameObject _avatarKeyBindContainer;
	public GameObject _uiKeyBindContainer;
    //si le script attend une touche pour l'enregistrer
    private bool _isListeningToNewInput;
	//objet indiquant que le script attend une touche pour l'enregistrer
	public GameObject _prompt;

    protected override void ExecuteBeforeInit()
    {
        _isListeningToNewInput = false;
        UpdateKeyBindDisplay();
        SetDefaultAction(delegate() {
            if (!_isListeningToNewInput)
            {
                _isListeningToNewInput = true;
                Debug.Log("Listening to new key for alias " + GetSelectedOption().name);
            }
        });
    }

    protected override void SetActions()
    {
        SetAction("Restorer", delegate() {
            if (!_isListeningToNewInput)
            {
                InputManager.SetDefaultKeyBinds();
                UpdateKeyBindDisplay();
            }
        });
        SetAction("Retour", delegate () {
            if (!_isListeningToNewInput)
            {
                SceneManager.LoadScene("menuPrincipal");
            }
        });
    }

	protected override void OnCancel ()
	{
		if(!_isListeningToNewInput)
			SceneManager.LoadScene("menuPrincipal");
	}



    protected override void Update () {
        base.Update();

		if (_isListeningToNewInput) {
            if(_prompt.activeSelf == false)
            {
                _prompt.SetActive(true);
            }
            else
            {
                //quand on cherche une nouvelle touche
                if (Input.anyKeyDown)
                {
                    //si l'option sélectionnée correspond à un alias de touche de l'avatar
                    if (GetSelectedOption().transform.parent.name == "nomsControlesAvatar")
                    {
                        for (int i = 0; i < System.Enum.GetNames(typeof(InputManager.AvatarControl)).Length; i++)
                        {
                            //si nom du gameobject du titre correspond a l'entree dans l'enum
                            if (System.Enum.GetName(typeof(InputManager.AvatarControl), i) == GetSelectedOption().name)
                            {
                                InputManager.KeyBind kb = InputManager.GetKeyBind((InputManager.AvatarControl)i);
                                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                                {
                                    if (Input.GetKeyDown(key))
                                    {
                                        kb.keyCode = key;
                                        InputManager.SetCommand((InputManager.AvatarControl)i, kb);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //si l'option sélectionnée correspond à un alias de touche de l'interface
                    else if (GetSelectedOption().transform.parent.name == "nomsControlesUI")

                    {
                        for (int i = 0; i < System.Enum.GetNames(typeof(InputManager.UIControl)).Length; i++)
                        {

                            //si nom du gameobject du titre correspond a l'entree dans l'enum
                            if ("UI-" + System.Enum.GetName(typeof(InputManager.UIControl), i) == GetSelectedOption().name)
                            {
                                InputManager.KeyBind kb = InputManager.GetKeyBind((InputManager.UIControl)i);
                                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                                {
                                    if (Input.GetKeyDown(key))
                                    {
                                        kb.keyCode = key;
                                        InputManager.SetCommand((InputManager.UIControl)i, kb);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                    _isListeningToNewInput = false;
					_prompt.SetActive(false);
                    UpdateKeyBindDisplay();

                }
            }



            
		}







	}

	private void UpdateKeyBindDisplay(){
        //affichage des touches de l'avatar
		for (int i = 0; i < _avatarKeyBindContainer.transform.childCount; i++) {
			for (int j = 0; j < System.Enum.GetNames (typeof(InputManager.AvatarControl)).Length; j++) 
			{
				//si nom du gameobject du titre correspond a l'entree dans l'enum
				if(System.Enum.GetName (typeof(InputManager.AvatarControl), j) == _avatarKeyBindContainer.transform.GetChild(i).name)
				{
					_avatarKeyBindContainer.transform.GetChild (i).GetComponent<UnityEngine.UI.Text> ().text = TranslateKeyCode(InputManager.GetKeyBind ((InputManager.AvatarControl)j).keyCode.ToString());
				}
			}
		}
        //affichage des touches de l'interface
        for (int i = 0; i < _uiKeyBindContainer.transform.childCount; i++) {
			for (int j = 0; j < System.Enum.GetNames (typeof(InputManager.UIControl)).Length; j++) 
			{
				//si nom du gameobject du titre correspond a l'entree dans l'enum
				if(System.Enum.GetName (typeof(InputManager.UIControl), j) == _uiKeyBindContainer.transform.GetChild(i).name)
				{
					_uiKeyBindContainer.transform.GetChild (i).GetComponent<UnityEngine.UI.Text> ().text = TranslateKeyCode(InputManager.GetKeyBind ((InputManager.UIControl)j).keyCode.ToString());
				}
			}
		}
	}

    //traduit les key codes en français lisible par l'utilisateur
    //par soucis de temps, un vrai module de traduction (e.g. xml externe) ne sera pas implémenté
    private string TranslateKeyCode(string keyCode)
    {
        switch (keyCode)
        {
            case "Backspace":
                return "Retour";
            case "Delete":
                return "Supprimer";
            case "Return":
                return "Entrée";
            case "Escape":
                return "Échappe";
            case "Space":
                return "Espace";
            case "Keypad0":
                return "num.0";
            case "Keypad1":
                return "num.1";
            case "Keypad2":
                return "num.2";
            case "Keypad3":
                return "num.3";
            case "Keypad4":
                return "num.4";
            case "Keypad5":
                return "num.5";
            case "Keypad6":
                return "num.6";
            case "Keypad7":
                return "num.7";
            case "Keypad8":
                return "num.8";
            case "Keypad9":
                return "num.9";
            case "KeypadDivide":
                return "num. /";
            case "KeypadPeriod":
                return "num. .";
            case "KeypadMultiply":
                return "num. *";
            case "KeypadMinus":
                return "num. -";
            case "KeypadPlus":
                return "num. +";
            case "KeypadEnter":
                return "num. Entrée";
            case "KeypadEquals":
                return "num. =";
            case "UpArrow":
                return "Flèche haut";
            case "DownArrow":
                return "Flèche bas";
            case "RightArrow":
                return "Flèche droite";
            case "LeftArrow":
                return "Flèche gauche";
            case "Insert":
                return "Insérer";
            case "Home":
                return "Début";
            case "End":
                return "Fin";
            case "PageUp":
                return "Page Arrière";
            case "PageDown":
                return "Page Avant";
            case "Alpha0":
                return "0";
            case "Alpha1":
                return "1";
            case "Alpha2":
                return "2";
            case "Alpha3":
                return "3";
            case "Alpha4":
                return "4";
            case "Alpha5":
                return "5";
            case "Alpha6":
                return "6";
            case "Alpha7":
                return "7";
            case "Alpha8":
                return "8";
            case "Alpha9":
                return "9";
            case "Hash":
                return "#";
            case "Quote":
                return "'";
            case "BackQuote":
                return "`";
            case "Comma":
                return ",";
            case "Period":
                return ".";
            case "Slash":
                return "/";
            case "Colon":
                return ":";
            case "Semicolon":
                return ";";
            case "Equals":
                return "=";
            case "LeftBracket":
                return "[";
            case "RightBracket":
                return "]";
            case "Backslash":
                return "\\";
            case "Numlock":
                return "Verr. num.";
            case "CapsLock":
                return "Verr. maj.";
            case "ScrollLock":
                return "Verr. défil.";
            case "RightShift":
                return "Shift droit";
            case "LeftShift":
                return "Shift gauche";
            case "RightControl":
                return "Contrôle droit";
            case "LeftControl":
                return "Contrôle gauche";
            case "RightAlt":
                return "Alt. droit";
            case "LeftAlt":
                return "Alt. gauche";
            case "LeftCommand":
                return "Commande gauche";
            case "LeftApple":
                return "Commande gauche";
            case "LeftWindows":
                return "Windows gauche";
            case "RightCommand":
                return "Commande droite";
            case "RightApple":
                return "Commande droite";
            case "RightWindows":
                return "Windows droit";
            case "SysReq":
                return "Système";
            case "Mouse0":
                return "Clic gauche";
            case "Mouse1":
                return "Clic droit";
            case "Mouse2":
                return "Clic milieu";
            case "None":
                return "(Aucune)";
            //todo: joystick?
            default:
                return keyCode;
        }
    }
}
