/**
 * classe TextBoxFactory
 * Maxime Giguère
 * 
 * Cette classe construit les boîtes de texte à afficher à l'écran, une à la fois, à partir des données fournies sur demande par TextBoxManager
 */
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class TextBoxFactory {
	//données de la boîte actuelle
	private static TextBoxManager.TextBoxFields _textBoxData;
    //prefabs de la boîte de texte
	private static string _prefabFolder;
	private static Dictionary<string, GameObject> _prefabs;
    //le script attaché à la boîte pour lui donner son interactivité
	private static TextBox _textBox;

    //fournit le prefab sur demande
	public static GameObject GetPrefab(string element)
	{
		if (_prefabs.ContainsKey (element))
			return _prefabs [element];
		else {
			Debug.LogError ("Invalid text box element query : " + element + ". Use either TextBoxElement, PromptElement or TextCharElement");
			return null;
		}

			
	}
    //fournit les données de boîtes sur demande (utile pour e.g. savoir le nom de l'acteur qui parle)
    public static TextBoxManager.TextBoxFields GetCurrentBoxData()
    {
        return _textBoxData;
    }


	//charge les données nécessaire à la construction de la boîte
	public static bool LoadNextBox()
	{
		_textBoxData = TextBoxManager.RequestNextTextBoxData();
        if (_textBoxData.prefabID == "null")
        {
            Object.Destroy(_textBox.gameObject);
            return false;
        }
        
		if (_prefabFolder != _textBoxData.prefabID) 
		{
			if (!LoadTextBoxPrefab ()) return false;
		}
		return BuildNextBox();
	}

    //charge le dossier contenant les prefab et s'assure que ceux-ci sont conformes
	private static bool LoadTextBoxPrefab()
	{
		_prefabFolder = _textBoxData.prefabID;
		//charge contenu du dossier
		try{
			Object[] prefabs = Resources.LoadAll ("Prefabs/textboxes/" + _prefabFolder);
            _prefabs = new Dictionary<string, GameObject>();
            for (int i = 0; i < prefabs.Length; i++)
            {
                _prefabs.Add(prefabs[i].name, prefabs[i] as GameObject);
            }
		}
		catch (System.NullReferenceException)
		{
			Debug.LogError ("Invalid load operation. Does the specified folder Prefabs/textboxes/" + _prefabFolder + " exist?");
			return false;
		}
		//vérifie que tous les prefabs sont présents
		if (!_prefabs.ContainsKey ("TextBoxElement")) Debug.LogError("Missing prefab. Prefabs/textboxes/" + _prefabFolder + " requires a TextBoxElement prefab.");
		if (!_prefabs.ContainsKey ("PromptElement")) Debug.LogError("Missing prefab. Prefabs/textboxes/" + _prefabFolder + " requires a PromptElement prefab.");
		if (!_prefabs.ContainsKey ("TextCharElement")) Debug.LogError("Missing prefab. Prefabs/textboxes/" + _prefabFolder + " requires a TextCharElement prefab.");
		if (!_prefabs.ContainsKey ("TextBoxElement") || !_prefabs.ContainsKey ("PromptElement") || !_prefabs.ContainsKey ("TextCharElement")) return false;
		return true;
	}

    //remplit les prefabs avec les données et attache le script TextBox
	private static bool BuildNextBox()
	{
		Transform hud = GameObject.Find ("HUD").transform;

        //instancie les prefabs s'il sont différents que ceux de la dernière boîte
		try
		{
			if(_textBox != null)
			{
				if (_textBox.GetPrefabFolderName () != _prefabFolder) 
				{
					Object.Destroy (_textBox.gameObject);
					_textBox = Object.Instantiate(_prefabs["TextBoxElement"], hud).AddComponent<TextBox>();
				}
			} 
			else 
			{
				_textBox = Object.Instantiate(_prefabs["TextBoxElement"], hud).AddComponent<TextBox>();
			}
		}
		catch(System.NullReferenceException) 
		{
            //pas certain que ça va s'exécuter sans briser quelque chose ailleurs, mais important à voir si jamais il y a un problème
            Debug.LogError ("Error while instanciating prefabs at TextBoxFactory.BuildNextBox()."); 
			return false;
		}

        //le script de la boîte met le texte dans les composantes
        _textBox.PopulateFieldsFromData(_textBoxData);

        //conteneur qui enveloppe les choix des boîtes
        GameObject promptContainer = null;
        try
        {
            promptContainer = _textBox.gameObject.transform.Find("PromptContainer").gameObject;
            //cacher le conteneur ici sert à empêcher l'ffichage des prompts entre le moment où ils sont créés et le moment où TextBox.Update s'exécute (~1frame)
            promptContainer.SetActive(false); 
        }
        catch (System.NullReferenceException)
        {
            //si le prefab est non conforme, message exprimant exactement ce qu'il manque
            Debug.LogError("Prefab Prefabs/textboxes/" + _textBoxData.prefabID + "/TextBoxElement.prefab is missing a GameObject named \"PromptContainer\".");
            return false;
        }

        
        //instancier les choix s'il y en a
        if (_textBoxData.prompts.Count > 0) 
		{
			
			//objet d'un choix
			GameObject prompt;
            
            //afficher tous les choix
            try
			{
				for (int i = 0; i < _textBoxData.prompts.Count; i++)
				{
					//instancier le prefab du choix
					prompt = Object.Instantiate(_prefabs["PromptElement"], promptContainer.transform);
					prompt.name = "Prompt_" + i;
					//les afficher un au dessus de l'autre
					prompt.transform.Translate(new Vector3(0, prompt.GetComponent<RectTransform>().rect.height * i, 0));
					//insérer le texte du choix
					prompt.transform.Find("textField").GetComponent<Text>().text = _textBoxData.prompts[i].value;
				}
			}
			catch (System.NullReferenceException) 
			{
                //si le prefab est non conforme, message exprimant exactement ce qu'il manque
                Debug.LogError ("Prefab Prefabs/textboxes/" + _prefabFolder + "/PromptElement.prefab is missing a GameObject named \"textField\", or this GameObject does not contain a UI.Text component.");
				return false;
			}
		}
        else
        {
            //vider le conteneur (e.g. une boîte n'a pas d'options, mais celle précédente en avait)
            for(int i = 0; i < promptContainer.transform.childCount; i++)
            {
                Object.Destroy(promptContainer.transform.GetChild(i).gameObject);
            }
            
        }
			
		return true;

	}

}



