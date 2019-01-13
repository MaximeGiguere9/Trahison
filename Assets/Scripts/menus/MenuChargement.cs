/**
 * script MenuChargement
 * Maxime Giguere
 * 
 * Comme son nom l'indique, ce script contrôle l'interactivité du menu du même nom.
 * En plus, il cherche tous les fichiers dans le dossier de sauvegarde afin de pouvoir afficher les détails des différents profils
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuChargement : Menu {

    //données importantes pour l'affichage de données de sauvegarde
	private struct SaveFileDetails{
		public string name;
		public string date;
		public bool hasKeySQ1;
		public bool hasKeySQ2;
		public bool hasKeySQ3;
	}
    //liste des fichiers et leurs données
	private List<SaveFileDetails> _saveFileDetails;

    protected override void ExecuteBeforeInit()
    {
        try
        {
            _saveFileDetails = new List<SaveFileDetails>();
            //cherche tous les fichiers de sauvegarde sur le disque
            string[] fileNames = Directory.GetFiles(PlayerData._saveFolderPath);
            //Debug.Log("Found " + fileNames.Length + " file(s) in save folder.");
            SaveFileDetails fd = new SaveFileDetails();
            int fileNum = 0;
            foreach (string file in fileNames)
            {
                //Debug.Log(file);
                //charge chaque profil pour extraire les données de sauvegarde
                PlayerData.LoadFile(file);
                fd.name = PlayerData.Get("_ProfileName") == null ? "-ERREUR-" : PlayerData.Get("_ProfileName");
                fd.date = File.GetLastWriteTime(file).ToString();
                fd.hasKeySQ1 = PlayerData.Get("hasKeySQ1") == "true";
                fd.hasKeySQ1 = PlayerData.Get("hasKeySQ2") == "true";
                fd.hasKeySQ1 = PlayerData.Get("hasKeySQ3") == "true";
                _saveFileDetails.Add(fd);
                //puis instancie la ligne du profil dans le menu
                GameObject ligne = Instantiate(Resources.Load("Prefabs/UI/Fichier") as GameObject, GameObject.Find("MenuChargement/Options").transform);
                ligne.name = "FichierSauvegarde" + fileNum;
                ligne.transform.localPosition = new Vector3(0, ligne.transform.localPosition.y - 20 * fileNum, 0);
                ligne.transform.Find("Nom").GetComponent<UnityEngine.UI.Text>().text = fd.name;
                ligne.transform.Find("Date").GetComponent<UnityEngine.UI.Text>().text = fd.date;
                fileNum++;
            }
            //placer l'option de retour a la fin de la liste
            GameObject.Find("MenuChargement/Options").transform.GetChild(0).SetSiblingIndex(_saveFileDetails.Count);
        }
        //saute le chargement de fichiers si le dossier n'existe pas
        catch (System.IO.DirectoryNotFoundException)
        {
            Debug.LogError("Save folder " + PlayerData._saveFolderPath + " does not exist.");
        }
    }


    protected override void SetActions()
    {
        SetAction("Retour", delegate () {
            SceneManager.LoadScene("menuPrincipal");
        });
        for(int i = 0; i < _saveFileDetails.Count; i++)
        {
            SetAction("FichierSauvegarde"+i, delegate () {
                PlayerData.LoadProfile(GetSelectedOption().transform.Find("Nom").GetComponent<UnityEngine.UI.Text>().text);
                SceneManager.LoadScene("menuDetailsFichier");
            });
        }
        
    }

    protected override void OnCancel()
    {
        SceneManager.LoadScene("menuPrincipal");
    }
}
