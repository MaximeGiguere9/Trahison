/**
 * classe PlayerData
 * Maxime Giguere
 * 
 * Cette classe contient un dictionnaire accessible dans le reste du code, 
 * principalement utilisé pour sauvegarder des données de profil ou progression sous forme key/value.
 * Ce dictionnaire peut être écrit dans un fichier XML, et on peut charger ces fichiers pour récupérer les données, rendant possible la sauvegarde de la partie.
 */ 

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public static class PlayerData {

	//dossier qui contient les fichiers de sauvegarde
    public static string _saveFolderPath = Application.persistentDataPath + "/saves";
	//dictionnaire contenant les données de jeu
    private static SortedDictionary<string, string> _savedPlayerData;
    private static SortedDictionary<string, string> SavedPlayerData
    {
        get
        {
			//initialisation
            if(_savedPlayerData == null)
            {
                Debug.Log("Initialising player data");
                _savedPlayerData = new SortedDictionary<string, string>();
            }
            return _savedPlayerData;
        }
    }
		
	//ajoute une clé et sa valeur, ou modifie la valeur si la clé est déjà présente
    public static bool Set(string name, string data)
    {
        try
        {
			//clé déjà présente
            if (SavedPlayerData.ContainsKey(name)) {
                SavedPlayerData[name] = data;
				//les données précédentes de la clé sont écrasées sans confirmation, alors ce Warning aide au débugage
                //Debug.LogWarning(name + " was already present in PlayerData, thus its value was overwritten. Was this expected?");
            }
			//nouvelle clé
            else {
				//Debug.Log("Adding PlayerData key " + name);
                SavedPlayerData.Add(name, data);
            }
            return true;
        }
		//au cas ou
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
        
    }

	//extrait la valeur d'une clé si elle est présente, sinon null
    public static string Get(string name)
    {
        if (SavedPlayerData.ContainsKey(name)) return SavedPlayerData[name];
        else return null;
    }

	public static void NewProfile(string profileName){
		Debug.Log ("Creating new profile " + profileName);
		SavedPlayerData.Clear ();
		PlayerData.Set ("_ProfileName", profileName);
	}
		
	//lit un ficher XML spécifié contenu dans le dossier de sauvegarde, l'analyse, et place les données dans le dictionnaire
    public static bool LoadFile(string filePath)
    {
        try
        {
            //écrase les données existantes
            SavedPlayerData.Clear();
            //quitte si le ficher n'est pas trouvé
            if (!File.Exists(filePath)) 
			{ 
				Debug.LogError("Unable to find " + filePath);
				return false; 
			}
            //charge le contenu du ficher dans une string
            string fileData = File.ReadAllText(filePath);
            //analyse le contenu
			XmlReader xmlReader = XmlReader.Create(new StringReader(fileData));
            while (xmlReader.Read())
            {
                //trouver les lignes contenant les données pertinentes
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "entry")
                {
                    //ajoute les données dans le dictionnnaire
                    Set(xmlReader.GetAttribute(0), xmlReader.GetAttribute(1));
                }
            }
            return true;
		}
		//empêche le programme de planter s'il y a une erreur dans la lecture du fichier
        catch (System.Exception e)
        {
            //j'assume que l'erreur est causée par du XML invalide (e.g. mauvaise structure/formation de balises ou tenter de lire un fichier qui n'est pas du XML)
            if(e.GetType() == typeof(XmlException))
            {
                Debug.LogError("File " + filePath + " contains invalid XML.");
            }
            //mais au cas ou que ce soit quelque chose d'autre (e.g. AccessViolationException ou IOException)
            else
            {
                Debug.LogError(e.Message);
            }
            return false;
        }
    }


	public static bool LoadProfile(string profileName)
	{
        //quitte si le dossier n'existe pas (pas de fichiers à charger)
        if (!Directory.Exists(_saveFolderPath))
        {
            Debug.LogError("Save folder " + _saveFolderPath + "  does not exist.");
            return false;
        }
        //charge le fichier via le nom de profil
        return LoadFile(_saveFolderPath + "/" + profileName + ".xml");
	}
		
	//crée une chaîne XML contenant les données du dictionanires et l'écrit dans un fichier sur disque.
	public static bool SaveToFile(string fileName = null)
    {

		fileName = (fileName == null) ? PlayerData.Get ("_ProfileName") : fileName;

		if (fileName == null) {
			Debug.LogError("Error trying to save game data: must provide file name.");
			return false;
		}

        try
        {
			//créer un fichier l'ouvre mais ne le ferme pas automatiquement; besoin de controller le stream explicitement pour ne pas avoir d'erreur d'accès
            FileStream fs;
            //chemin complet
			string filePath = _saveFolderPath + "/" + fileName + ".xml";
            //crée le dossier s'il n'existe pas
            
			if (!Directory.Exists(_saveFolderPath)) 
			{
				Debug.Log("Creating save folder " + _saveFolderPath);
				Directory.CreateDirectory(_saveFolderPath);
			}
            //crée le fichier s'il n'existe pas (et le ferme)
            Debug.Log("Writing game data to " + filePath);
            if (!File.Exists(filePath)) { fs = File.Create(filePath); fs.Close(); }
            //chaîne XML
            string fileData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<SavedData>\n";
            //écrit toutes les données sauvegardées
            foreach (KeyValuePair<string, string> entry in SavedPlayerData)
            {
                fileData += ("\t<entry key=\"" + entry.Key + "\" value=\"" + entry.Value + "\"/>\n");
            }
            fileData += "</SavedData>";
            //écrit le fichier
            File.WriteAllText(filePath, fileData);
            Debug.Log("Game data was succesfully saved to " + filePath);
            return true;
        }
		//empêche la sauvegarde si erreur pour ne pas risquer de corrompre le ficher ou de faire planter le jeu
        catch (System.Exception e)
        {
            Debug.LogError("Error trying to save game data. Save operation aborted.");
            Debug.LogError(e.Message);
            return false;
        } 
    }
}
