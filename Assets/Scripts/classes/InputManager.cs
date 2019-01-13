/**
 * classe InputManager
 * Maxime Giguere
 * 
 * InputManager permet l'utilisation d'aliases de touches pour prévenir l'utilisation constante de touches hardcodées via Input.GetKey un peu partout dans le code.
 * Incidentalement, cela signifie que les touches de contrôles peuvent être modifiées dynamiquement.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public static class InputManager {

	//listes de commandes à associer à des touches
	public enum AvatarControl{
		left,
		right,
		up,
		down,
		run,
		jump,
		attack,
        interact,
	}
    public enum UIControl
    {
        left,
        right,
        up,
        down,
        proceed,
        cancel,
        menu,
    }

	//détails des touches
	public struct KeyBind{
		public KeyCode keyCode; //touche clavier
		public string type; //définir soit "down" (pour appeler Input.GetKeyDown), "hold" (Input.GetKey), ou "up" (Input.GetKeyUp)
	}

    //dictionnaires contenant chacune des commandes et la touche qui lui est associée
    private static Dictionary<AvatarControl, KeyBind> _avatarControls;
    private static Dictionary<AvatarControl, KeyBind> AvatarControls
    {
        get
        {
			//initialisation du dictionnaire
            if(_avatarControls == null)
            {
                //crée un dictionary de la même taille qu'AvatarControl pour que chaque comamnde dans la liste ait son propre KeyBind
                _avatarControls = new Dictionary<AvatarControl, KeyBind>();
                foreach (AvatarControl command in Enum.GetValues(typeof(AvatarControl)))
                {
                    _avatarControls.Add(command, new KeyBind());
                }
				//cherche les KeyBinds déjà sauvegardés, ou associe les Keybinds par défaut
                try
                {
					if (!LoadCustomKeyBinds())
						SetDefaultKeyBinds();
                    Debug.Log("AvatarControl list initialised succesfully");
                }
				//si le chargement échoue pour quelque raison, associe les Keybinds par défaut
                catch (System.Exception e)
                {
                    Debug.LogError("AvatarControl loading failed : " + e.Message);
					SetDefaultKeyBinds();
                }
            }
            return _avatarControls;
        }
    }
	//les deux dictionnaires sont similaires, pour différencier les contrôles reliés à l'avatar de ceux reliés à l'interface. (plus de flexibilité dans la gestion des contrôles)
    private static Dictionary<UIControl, KeyBind> _uiControls;
    private static Dictionary<UIControl, KeyBind> UIControls
    {
        get
        {
            if(_uiControls == null)
            {
                _uiControls = new Dictionary<UIControl, KeyBind>();
                foreach (UIControl command in Enum.GetValues(typeof(UIControl)))
                {
                    _uiControls.Add(command, new KeyBind());
                }
                try
                {
					if (!LoadCustomKeyBinds())
						SetDefaultKeyBinds();
                    Debug.Log("UIControl list initialised succesfully");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("UIControl loading failed : " + e.Message);
					SetDefaultKeyBinds();
                }
            }
            return _uiControls;
        }
    }

	//empêche les commandes de l'avatar d'être reconnues (pour contrôler uniquement l'interface, e.g. boîtes de texte ou menu pause)
	private static bool _avatarInputLockState;



    //contrôler le blocage des commandes de l'avatar
    public static void SetAvatarInputLock(bool bLock)
    {
		if (_avatarInputLockState != bLock)
        {
			Debug.Log("Locked Avatar Input : " + bLock);
			_avatarInputLockState = bLock;
        }
        return;
    }

    //récupérer le statut du blocage
	public static bool IsAvatarInputLocked()
    {
		return _avatarInputLockState;
    }

	//rechercher les KeyBinds sauvegardés sur le disque
	private static bool LoadCustomKeyBinds(){
		//chemin du fichier
		string filePath = Application.persistentDataPath + "/keys/keys.xml";

		if (!File.Exists(filePath)) 
		{ 
			Debug.Log ("Saved KeyBinds not found");
			return false; 
		}
		//charge le contenu du ficher dans une string
		string fileData = File.ReadAllText(filePath);

		//
		KeyBind kb = new KeyBind();
		bool? isReadingAvatarKeyBinds = null;

		Debug.Log ("Loading custom KeyBinds");

		//analyse le contenu
		XmlReader xmlReader = XmlReader.Create(new StringReader(fileData));
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType == XmlNodeType.Element)
            {
				switch (xmlReader.Name)
                {
				    case "avatarkeybinds":
					    isReadingAvatarKeyBinds = true;
					    break;
				    case "uikeybinds": 
					    isReadingAvatarKeyBinds = false;
					    break;
				    case "key":
					    if (isReadingAvatarKeyBinds == null)
                        {
						    Debug.LogWarning("Key not defined within valid group identifier.");
						    break;
					    }
                        //selon la balise qui a été analysée plus tôt, on peut savoir si le keybind est associé à l'avatar ou à l'interface (isReadingAvatarKeyBinds)
                        //donc, on cherche d'abord le nom, dans le bon enum, correspondant à la valeur du premier attribut de la balise, ce qui requiert d'itérer sur les données de l'enum
                        for (int i = 0; i < System.Enum.GetNames ((bool)isReadingAvatarKeyBinds ? (typeof(InputManager.AvatarControl)) : (typeof(InputManager.UIControl))).Length; i++)
                        {
                            //si la valeur correspond à un nom dans le bon enum, on analyse le reste des données
                            if (System.Enum.GetName ((bool)isReadingAvatarKeyBinds ? (typeof(InputManager.AvatarControl)) : (typeof(InputManager.UIControl)), i) == xmlReader.GetAttribute(0))
                            {
                            
                                //les attributs subséquents contiennent les données de keybind, alors on les met dans l'objet nécessaire pour créer l'alias
					            kb.keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), xmlReader.GetAttribute (1));
						        kb.type = xmlReader.GetAttribute (2);
                                //enfin, on met l'objet de KeyBind dans le bon tableau, au bon index (l'index numérique de l'enum correspondant où se trouve nom de la commande)
						        if ((bool)isReadingAvatarKeyBinds)
							        AvatarControls [(AvatarControl)i] = kb;
						        else 
							        UIControls [(UIControl)i] = kb;
						        break;
					        }
					    }
					    break;
				    default:
					    if(xmlReader.Name != "keybinds")
					    Debug.LogWarning ("Unrecognized tag " + xmlReader.Name);
					    break;
				}
			}
		}

		return true;


	}

	public static bool SaveKeyBinds(){


		string fileName = "keys.xml";
		string folder = Application.persistentDataPath + "/keys";
		string filePath = folder + "/" + fileName;

		try
		{
			//créer un fichier l'ouvre mais ne le ferme pas automatiquement; besoin de controller le stream explicitement pour ne pas avoir d'erreur d'accès
			FileStream fs;
			//chemin complet
			Debug.Log("Writing keybind data to " + filePath);
			//crée le fichier s'il n'existe pas (et le ferme)
			if(!Directory.Exists(folder))Directory.CreateDirectory(folder);
			if (!File.Exists(filePath)) { fs = File.Create(filePath); fs.Close(); }
			//chaîne XML
			string fileData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<keybinds>\n";
			//écrit toutes les données sauvegardées

			fileData += "<avatarkeybinds>\n";
			foreach (KeyValuePair<AvatarControl, KeyBind> entry in AvatarControls)
			{
				fileData += ("\t<key alias=\"" + entry.Key + "\" code=\"" + entry.Value.keyCode + "\" type=\"" + entry.Value.type + "\"/>\n");
			}
			fileData += "</avatarkeybinds>\n";
			fileData += "<uikeybinds>\n";
			foreach (KeyValuePair<UIControl, KeyBind> entry in UIControls)
			{
				fileData += ("\t<key alias=\"" + entry.Key + "\" code=\"" + entry.Value.keyCode + "\" type=\"" + entry.Value.type + "\"/>\n");
			}
			fileData += "</uikeybinds>\n</keybinds>";
			//écrit le fichier
			File.WriteAllText(filePath, fileData);
			Debug.Log("KeyBind data was succesfully saved to " + filePath);
			return true;
		}
		//empêche la sauvegarde si erreur pour ne pas risquer de corrompre le ficher
		catch (System.Exception e)
		{
			Debug.LogError("Error trying to save KeyBind data. Save operation aborted.");
			Debug.LogError(e.Message);
			return false;
		} 
	}

    //associe les touches par défaut aux commandes dans les dictionnaires
    public static void SetDefaultKeyBinds(){
		//avatar
		Debug.Log ("Setting default Avatar KeyBinds");
		KeyBind keyBind = new KeyBind ();
		//gauche
		keyBind.keyCode = KeyCode.A;
		keyBind.type = "hold";
        AvatarControls[AvatarControl.left] = keyBind;
		//droite
		keyBind.keyCode = KeyCode.D;
		keyBind.type = "hold";
        AvatarControls[AvatarControl.right] = keyBind;
		//haut
		keyBind.keyCode = KeyCode.W;
		keyBind.type = "hold";
        AvatarControls[AvatarControl.up] = keyBind;
		//bas
		keyBind.keyCode = KeyCode.S;
		keyBind.type = "hold";
        AvatarControls[AvatarControl.down] = keyBind;
		//course
		keyBind.keyCode = KeyCode.LeftShift;
		keyBind.type = "hold";
        AvatarControls[AvatarControl.run] = keyBind;
		//saut
		keyBind.keyCode = KeyCode.Space;
		keyBind.type = "down";
        AvatarControls[AvatarControl.jump] = keyBind;
		//combat
		keyBind.keyCode = KeyCode.Mouse0;
		keyBind.type = "down";
        AvatarControls[AvatarControl.attack] = keyBind;
        //interaction
        keyBind.keyCode = KeyCode.E;
        keyBind.type = "down";
        AvatarControls[AvatarControl.interact] = keyBind;
		//interface
		Debug.Log("Setting default UI KeyBinds");
		//gauche
		keyBind.keyCode = KeyCode.A;
		keyBind.type = "down";
		UIControls[UIControl.left] = keyBind;
		//droite
		keyBind.keyCode = KeyCode.D;
		keyBind.type = "down";
		UIControls[UIControl.right] = keyBind;
		//haut
		keyBind.keyCode = KeyCode.W;
		keyBind.type = "down";
		UIControls[UIControl.up] = keyBind;
		//bas
		keyBind.keyCode = KeyCode.S;
		keyBind.type = "down";
		UIControls[UIControl.down] = keyBind;
		//continuer
		keyBind.keyCode = KeyCode.E;
		keyBind.type = "down";
		UIControls[UIControl.proceed] = keyBind;
		//annuler
		keyBind.keyCode = KeyCode.Tab;
		keyBind.type = "down";
		UIControls[UIControl.cancel] = keyBind;
        //pause
        keyBind.keyCode = KeyCode.Escape;
        keyBind.type = "down";
        UIControls[UIControl.menu] = keyBind;
        //sauvegarde les touches
        SaveKeyBinds();
    }

	public static KeyBind GetKeyBind(AvatarControl command)
	{
		if (!AvatarControls.ContainsKey (command)) 
		{
			Debug.LogError ("Command " + command + " could not be found");
		} 
		else 
		{
			return AvatarControls[command];
		}
		return new KeyBind();
	}

	public static KeyBind GetKeyBind(UIControl command)
	{
		if (!UIControls.ContainsKey (command)) 
		{
			Debug.LogError ("Command " + command + " could not be found");
		} 
		else 
		{
			return UIControls[command];
		}
		return new KeyBind();
	}

    //associer une touche à un alias
    public static bool SetCommand(AvatarControl command, KeyBind key)
	{
		//commande spécifiée absente du dictionnaire
		if (!AvatarControls.ContainsKey (command)) 
		{
			Debug.LogError ("Command " + command + " could not be found");
			return false;
		} 
		//touche spécifiée est déjà associée à une autre commande
		foreach(KeyValuePair<AvatarControl, KeyBind> currentBind in AvatarControls){
			if (currentBind.Key != command && currentBind.Value.keyCode == key.keyCode) {
				Debug.LogError ("Duplicate key " + key.keyCode);
				return false;
			}
		}
		//associer la commande à la touche
        AvatarControls[command] = key;
		Debug.Log("Command " + command + " bound to " + key.keyCode);
		SaveKeyBinds ();
		return true;

	}
    //overloading pour gérer le deuxième dictionnaire
    public static bool SetCommand(UIControl command, KeyBind key)
    {
        if (!UIControls.ContainsKey(command))
        {
			Debug.LogError("Command " + command + " could not be found");
			return false;
        }
		foreach(KeyValuePair<UIControl, KeyBind> currentBind in UIControls){
			if (currentBind.Key != command && currentBind.Value.keyCode == key.keyCode) {
				Debug.LogError ("Duplicate key " + key.keyCode);
				return false;
			}
		}
        UIControls[command] = key;
		Debug.Log("Command " + command + " bound to " + key.keyCode);
		SaveKeyBinds ();
        return true;
    }


	//vérifie si les condition du KeyBind d'une commande sont atteintes
    public static bool GetCommand(AvatarControl command)
    {
        //si les contrôles sont bloqués
		if (IsAvatarInputLocked())
        {
            return false;
        }
        //commande spécifiée est absente du dictionnaire
        if (!AvatarControls.ContainsKey(command))
        {
            Debug.LogError("Command " + command + " could not be found");
            return false;
        }
		//extrait le code de la touche
        KeyCode keyCode = AvatarControls[command].keyCode;
		//appele la fonction correspondant au type de la toucche de la commande
        switch (AvatarControls[command].type)
        {
            case "down":
                return Input.GetKeyDown(keyCode) || ControllerInputManager.GetControllerCommandStateOnce(command.ToString());
            case "hold":
                return Input.GetKey(keyCode) || ControllerInputManager.GetControllerCommandStateContinuous(command.ToString());
            case "up":
                return Input.GetKeyUp(keyCode);
            default:
                Debug.LogError("Invalid command type " + AvatarControls[command].type + " . Command type must be either \"down\", \"hold\" or \"up\".");
                return false;
        }
    }
    //overloading pour gérer le deuxième dictionnaire
    public static bool GetCommand(UIControl command)
    {
        if (!UIControls.ContainsKey(command))
        {
            Debug.LogError("Command " + command + " could not be found");
            return false;
        }
        KeyCode keyCode = UIControls[command].keyCode;
        switch (UIControls[command].type)
        {
            case "down":
                return Input.GetKeyDown(keyCode) || ControllerInputManager.GetControllerCommandStateOnce(command.ToString());
            case "hold":
                return Input.GetKey(keyCode) || ControllerInputManager.GetControllerCommandStateContinuous(command.ToString());
            case "up":
                return Input.GetKeyUp(keyCode);
            default:
                Debug.LogError("Invalid command type " + UIControls[command].type + " . Command type must be either \"down\", \"hold\" or \"up\".");
                return false;
        }
    }
		
	//fonctions pour remplacer Input.GetAxis, similaires mais utilisent les touches définies dans les dictionnaires
	public static float GetInputDirectionX(){
		if (GetCommand (InputManager.AvatarControl.right))
			return 1;
		else if(GetCommand (InputManager.AvatarControl.left))
			return -1;
		else
			return 0;
	}
	public static float GetInputDirectionY(){
		if (GetCommand (InputManager.AvatarControl.up))
			return 1;
		else if(GetCommand (InputManager.AvatarControl.down))
			return -1;
		else
			return 0;
	}



	


    
}
