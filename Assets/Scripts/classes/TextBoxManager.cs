/**
 * classe TextBoxManager
 * Maxime Giguère
 * 
 * TextBoxManager permet de charger des fichiers XML pour afficher des boîtes de texte et données de dialogue
 * Cette classe s'occupe de garder en mémoire et de fournir les données de boîtes chargées ainsi que les actions y étant reliées
 */

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class TextBoxManager {
    //fonction à exécuter quand la séquence de boîte se temrine
    private static System.Func<bool> _onComplete;
    //état du module (est-ce qu'une boîte est encore chargée, en attente d'être affichée)
	private static bool _isDataAwaitingDisplay;
    private static bool IsDataAwaitingDisplay
    {
        get
        {
            return _isDataAwaitingDisplay;
        }
        set
        {
            _isDataAwaitingDisplay = value;
            //bloquer les contrôles de l'avatar pour focuser sur le dialogue
            InputManager.SetAvatarInputLock(value);
            if (value == false) OnComplete();
        }
    }
    //contient les conditions reliées aux données additionnelles transmises par les boîte et les fonctions reliées
    private static Dictionary<string, System.Func<bool>> _additionalActions;
    private static Dictionary<string, System.Func<bool>> AdditionalActions
    {
        get
        {
            if (_additionalActions == null) _additionalActions = new Dictionary<string, System.Func<bool>>();
            return _additionalActions;
        }
    }
    //données d'une section de la boîte
    public struct TextField
	{
		public string value;
		public Dictionary<string, string> args;
        public TextField(string value)
        {
            this.value = value;
            this.args = new Dictionary<string, string>();
        }
	}
    //données d'une boîte complète
	public struct TextBoxFields
	{
		public string prefabID;
		public TextField text;
		public TextField name; 
		public TextField avatar;
		public List<TextField> prompts; 
        public TextBoxFields(string prefabID)
        {
            this.prefabID = prefabID;
            this.text = new TextField(null);
            this.name = new TextField(null);
            this.avatar = new TextField(null);
            this.prompts = new List<TextField>();
        }
	}
    //données de toutes les boîtes chargées, en séquence
	private static List<TextBoxFields> _textBoxList = new List<TextBoxFields>();


    //ajoute le contenu d'un fichier à la liste de boîtes
	public static bool EnqueueFile(string fileName) 
	{
		//manipulation et extraction des données du fichier
		string fileData;
		TextBoxFields textBoxData = new TextBoxFields(null);
		string prefabID = "null";
		XmlReader xmlReader;

		try
		{
			//charger le fichier
			fileData = (Resources.Load("text/" + fileName) as TextAsset).text;
		}
		catch (System.NullReferenceException)
		{
			//abandonner si fichier introuvable
			Debug.LogError("Unable to find " + fileName + " in Resources/text.");
			return false;
		}

		//démarrer la lecture du XML
		xmlReader = XmlReader.Create(new StringReader(fileData));
		while (xmlReader.Read())
		{
			//pour chaque balise d'ouverture...
			if (xmlReader.NodeType == XmlNodeType.Element)
			{
				switch (xmlReader.Name)
				{
				//conteneur de séquence de dialogue
				case "dialog":
					//extraire le type de boîte de texte et charger le prefab correspondant
					try
					{
						prefabID = xmlReader.GetAttribute(0);
					}
					//si aucune définition de type
					catch (System.ArgumentOutOfRangeException)
					{
						Debug.LogError("No text box type defined for file " + fileName);
						prefabID = "null";
					}       
					break;
					//conteneur de boîte de texte
				case "box":
					//initialise données de boîte
					textBoxData = new TextBoxFields (null);
					textBoxData.prefabID = prefabID;
					break;
					//nom de l'acteur à qui la réplique est attribuée
				case "name":
					//extraire les attributs du choix (lien vers un autre fichier, données additionnelles)
					while (xmlReader.MoveToNextAttribute()) {
						textBoxData.name.args.Add (xmlReader.Name, xmlReader.Value);
					}
					//lire jusq'au contenu de la balise pour extraire les données
					xmlReader.Read();
					textBoxData.name.value = xmlReader.Value;
					break;
					//texte de la boîte
				case "text":
					//extraire les attributs du choix (lien vers un autre fichier, données additionnelles)
					while (xmlReader.MoveToNextAttribute()) {
						textBoxData.text.args.Add (xmlReader.Name, xmlReader.Value);
					}
					//
					xmlReader.Read();
					textBoxData.text.value = xmlReader.Value;
					break;
					//image d'avatar à charger pour la boîte
				case "avatar":
					//extraire les attributs du choix (lien vers un autre fichier, données additionnelles)
					while (xmlReader.MoveToNextAttribute()) {
						textBoxData.avatar.args.Add (xmlReader.Name, xmlReader.Value);
					}
					//
					xmlReader.Read();
					textBoxData.avatar.value = xmlReader.Value;
					break;
					//choix
				case "prompt":
					//initialise données de choix
					TextField p = new TextField(null);
					//extraire les attributs du choix (lien vers un autre fichier, données additionnelles)
					while (xmlReader.MoveToNextAttribute()) {
						p.args.Add (xmlReader.Name, xmlReader.Value);
					}
					//lire jusq'au contenu de la balise pour extraire le texte du choix
					xmlReader.Read();
					p.value = xmlReader.Value;
					//ajouter à la liste de choix
					textBoxData.prompts.Add(p);
					break;
				}

			}
			//pour chaque fermeture de balise de boîte de texte...
			else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "box")
			{
				//ajouter la boîte complétée à la liste
				_textBoxList.Add(textBoxData);
			}
		}
        //Debug.Log("Succesfully loaded text file " + fileName);
        //débute l'affichage de la séquence si ce n'Est pas déjà en cours (la séquence continue jusqu'à ce qu'il n'y ait plus de boîtes chargées)
        if (IsDataAwaitingDisplay == false) 
		{
            IsDataAwaitingDisplay = true;
			TextBoxFactory.LoadNextBox ();
		}
		return true;
	}

    //envoie la prochaine boîte à être affichée
	public static TextBoxFields RequestNextTextBoxData()
	{
		try
		{
            //la prochaine boîte est celle à l'index 0, et elle est enlevée pour laisser la place à la prochaine, etc. jusqu'à ce qu'il n'y en ait plus...
			TextBoxFields textBoxContents = _textBoxList [0];
			_textBoxList.RemoveAt (0);
			return textBoxContents;
		} 
		catch (System.ArgumentOutOfRangeException) 
		{
            //.. dans quel cas plus aucune boîte n'attend d'être affichée
            IsDataAwaitingDisplay = false;
			//Debug.LogWarning ("A text box was requested, but there was no data awaiting display.");
            TextBoxFields textBoxContents = new TextBoxFields();
            textBoxContents.prefabID = "null";
			return textBoxContents;
		}
	}

    //définit une action à exécuter quand une option contenant la chaîne spécifiée est sélectionnée
    public static bool SetOnReceiveDataAction(string data, System.Func<bool> action)
    {
        if (AdditionalActions.ContainsKey(data))
        {
            AdditionalActions[data] = action;
            Debug.LogWarning("The additional text box data \"" + data + "\" was already associated to an action, which has now been overwritten. Was this expected?");
        }
        else
        {
            AdditionalActions.Add(data, action);
        }
        return true;
    }

    //supprime l'écoute d'une chaîne spécifiée
    public static bool ClearOnReceiveDataAction(string data)
    {
        if (!AdditionalActions.ContainsKey(data))
        {
            Debug.LogWarning("Cannot clear action associated to additional text box data \"" + data + "\". No action was stored in the first place.");
        }
        AdditionalActions.Remove(data);
        return true;
    }

    //envoie l'action à exécuter pour une chaîne spécifiée
    public static System.Func<bool> GetOnReceiveDataAction(string data)
    {
        if (AdditionalActions.ContainsKey(data))
        {
            return AdditionalActions[data];
        }
        else
        {
            System.Func<bool> f = () =>
            {
                return true;
            };
            return f;
        }
    }

    //définit l'action à exécuter quand il ne reste plus de boîtes à afficher
    public static void SetOnCompleteAction(System.Func<bool> action)
    {
        _onComplete = action;
    }


    //s'exécute quand aucune boîte n'est en attente
    private static void OnComplete()
    {
        if (_onComplete == null)
        {
            //Debug.Log("No action defined for textbox completion.");
        }
        else
        {
            _onComplete();
            //nettoie la variable pour ne pas que l'action s'exécute pour chaque séquence de boîte jusqu'à ce qu'une autre action soit définit
            _onComplete = null;
        }
    }


}

