/**
 * classe TextBox
 * Maxime Giguère
 * 
 * Cette classe s'attache à la boîte de texte afficher et gère l'interactivité (i.e. écoute les touches de l'utilisateur pour progresser ou sélectionner un choix)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
	//conteneur de choix
	private GameObject _promptContainer;
	private int _promptCount = 0;
	//aucun choix est sélectionné
	private int _selectedPromptID = -1;
    //garde en mémoire les données de boîte en code pour ne pas avoir à les extraire des GameObjects
    private TextBoxManager.TextBoxFields _textBoxData;
    
	public void Start()
	{
        _promptContainer = gameObject.transform.Find("PromptContainer").gameObject;
	}

	public void Update()
	{
        
        try
        {
            //attendre que l'animation de texte soit terminée avant d'afficher les choix
            _promptContainer.SetActive(!gameObject.transform.Find("box/textWrapper").GetComponent<TextBoxCharAnim>().IsWorking());
            //identifier le nombre de prompts
            _promptCount = _promptContainer.transform.childCount;
        }
        catch(System.NullReferenceException)
        {
            //enlève du spam d'erreurs quand les prompts ne sont pas instanciés correctement 
            //(i.e. permet de voir le message d'erreur pertinent dans la console au lieu de 1000x NullReferenceException)
        }
        
        //progression
        if (InputManager.GetCommand(InputManager.UIControl.proceed)){
			//si une animation de texte est en cours...
			bool isWorking = false;
			if(gameObject.transform.Find("box/textWrapper") != null)
			{
				isWorking = gameObject.transform.Find("box/textWrapper").GetComponentInChildren<TextBoxCharAnim>().IsWorking();
			}
			//...forcer l'animation à se terminer...
			if (isWorking) 
			{ 
				gameObject.transform.Find("box/textWrapper").GetComponentInChildren<TextBoxCharAnim>().ForceComplete(); 
			}
			//... et sélectionne le premier choix automatiquement si il y en a mais aucun n'est sélectionné...
			else if(_promptCount > 0 && _selectedPromptID == -1)
			{
				SelectPrompt(0);
			}
			//...sinon créer la prochaine boîte de texte
			else
			{

                //exécute les données de l'option sélectionnée si il y en avait une
                if (_promptCount > 0) {
                    //exécute les données de la sélection
                    string selectionLink = "";
                    //lien vers un autre fichier de texte
                    if (_textBoxData.prompts[_selectedPromptID].args.ContainsKey("link")) selectionLink = _textBoxData.prompts[_selectedPromptID].args["link"];
                    //données aditionnelles
                    if (_textBoxData.prompts[_selectedPromptID].args.ContainsKey("data"))
                    {
                        //va chercher l'action associée aux données additionnelles et l'exécute
                        TextBoxManager.GetOnReceiveDataAction(_textBoxData.prompts[_selectedPromptID].args["data"])();
                    }
                    
                    //charger le fichier spécifié dans le lien
                    if (selectionLink != "") TextBoxManager.EnqueueFile(selectionLink);
                }

				TextBoxFactory.LoadNextBox();
			}
		}
        //sélectionne un choix si disponible
		if(_promptCount > 0){
			//sélectionne un choix plus haut
			if (InputManager.GetCommand(InputManager.UIControl.up))
			{
				if (_selectedPromptID == -1) SelectPrompt(_promptCount - 1); //sélectionne le choix le plus haut si aucun n'a déjà été choisi
				else SelectPrompt(_selectedPromptID + 1);
			}
			//sélectionne un choix plus bas
			else if (InputManager.GetCommand(InputManager.UIControl.down))
			{
				if (_selectedPromptID == -1) SelectPrompt(0); //sélectionne le choix le plus bas si aucun n'a déjà été choisi
				else SelectPrompt(_selectedPromptID - 1);
			}
			
		}
	}

    //retourne le type de boîte
    public string GetPrefabFolderName()
    {
        return _textBoxData.prefabID;
    }


    public void PopulateFieldsFromData(TextBoxManager.TextBoxFields t)
    {
        //garde les données en mémoire
        _textBoxData = t;
        //mettre les données dans le prefab
        gameObject.transform.Find("box/nameField").GetComponent<Text>().text = _textBoxData.name.value;
        gameObject.transform.Find("box/textField").GetComponent<Text>().text = _textBoxData.text.value;
        //commencer l'animation du texte
        if (gameObject.transform.Find("box/textWrapper").GetComponent<TextBoxCharAnim>().SetCharPrefab(TextBoxFactory.GetPrefab("TextCharElement")))
            gameObject.transform.Find("box/textWrapper").GetComponent<TextBoxCharAnim>().StartAnim();
    }


    //actualise le choix sélectionné
    private void SelectPrompt(int num)
    {
        _selectedPromptID = Mathf.Clamp(num, 0, _promptCount - 1);
        //le choix sélectionné a une teinte
        if (_selectedPromptID != -1)
        {
            for (int i = 0; i < _promptCount; i++) { _promptContainer.transform.Find("Prompt_" + i + "/Selection").gameObject.SetActive(false); }
            _promptContainer.transform.Find("Prompt_" + _selectedPromptID + "/Selection").gameObject.SetActive(true);
        }

    }

	
}


