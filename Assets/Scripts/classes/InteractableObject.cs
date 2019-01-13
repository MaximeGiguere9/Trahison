/**
 * classe InteractableObject
 * Maxime Giguere
 * 
 * Classe de base qui contient la fonctionnalité nécessaire pour permettre au joueur d'interagir avec des objets du monde.
 * Détecte que le joueur est proche et permet d'exécuter du code soit automatiquement, ou quand le joueur appuie sur la touche d'interaction.
 * Chaque objet interactif et trigger possède son propre script qui hérite et override cette classe.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour {

    //références au joueur et à l'interface nécessaires pour le fonctionnement
    protected GameObject _playerReference;
    protected GameObject _HUDReference;
    //savoir si le joueur est à proximité de l'objet
    protected bool _isPlayerNearby;
	//définit si l'objet doit attendre une action du joueur ou exécuter son script dès qu'il est à proximité
	protected bool _requirePrompt = false;
    //objet de la scène qui montre au joueur l'objet qui attent son action
    protected GameObject _interactPrompt;
    //texte à afficher dans le GameObject précédent
    protected string _interactActionText = "";  
    
	protected virtual void Start () {
		//initialisation des variables qui ne sont pas 
		_isPlayerNearby = false;
		_interactPrompt = null;
        //trouve le joueur et le HUD
		_playerReference = GameObject.FindWithTag("Player");
		_HUDReference = GameObject.Find("HUD");
        
	}


    protected virtual void Update () {
        //si besoin d'une action et joueur à proximité
		if (_requirePrompt && _isPlayerNearby)
        {
			//actualiser le texte d'interaction
            UpdateTextPosition();
			//exécuter le script de l'objet quand le joueur appuie sur la touche
            if (InputManager.GetCommand(InputManager.AvatarControl.interact)) ExecuteObjectAction();
        }       
	}

    protected virtual void OnTriggerEnter(Collider col)
    {
		//quand le joueur s'approche
		if (col.gameObject == _playerReference)
        {
			//montre le texte d'intéraction si manuel
			if (_requirePrompt)
            {
                ShowInteractPrompt();
            }
			//ou exécute le script si automatique
            else
            {
                ExecuteObjectAction();
            }
			_isPlayerNearby = true;
        }
    }

    protected virtual void OnTriggerExit(Collider col)
    {
		//cache le texte d'intéraction quand le joueur s'éloigne trop
		if (col.gameObject == _playerReference) 
		{
			HideInteractPrompt ();
			_isPlayerNearby = false;
		}
    }

	//affiche le texte d'intéraction (e.g. "intéragir" ou "ouvrir")
    protected virtual void ShowInteractPrompt()
    {
        //place texte dans GameObject
		_interactPrompt = new GameObject();
		Text interactTxt = _interactPrompt.AddComponent<Text>();
       
		try {
			//paramètres généraux d'identification et de position
			_interactPrompt.transform.SetParent(_HUDReference.transform);
			_interactPrompt.transform.SetAsFirstSibling ();
			_interactPrompt.name = "InteractPrompt";
			_interactPrompt.transform.position = gameObject.transform.position;
			//paramètres du texte
			interactTxt.rectTransform.sizeDelta = new Vector2(200, 20);
			interactTxt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			interactTxt.color = Color.white;
			interactTxt.text = _interactActionText;
			interactTxt.alignment = TextAnchor.UpperCenter;
		} catch(System.NullReferenceException){
			Debug.LogError ("Unable du find HUD reference. Please make sure the scene contains a canvas named \"HUD\" or override protected member _HUDReference.");
		}

    }

	//cache le texte d'intéraction
    protected virtual void HideInteractPrompt()
    {
		Destroy(_interactPrompt);
    }

	//actualise la position du texte d'intéraction sur l'écran pour le garder au dessus de l'objet peut importe la position de la caméra
    protected virtual void UpdateTextPosition()
    {
		_interactPrompt.GetComponent<Text>().transform.position = Camera.main.WorldToScreenPoint(
            new Vector3(
					gameObject.transform.position.x,
                    (float)(gameObject.transform.position.y + gameObject.GetComponentInChildren<Renderer>().bounds.size.y + 0.5),
                    gameObject.transform.position.z
            )
        );
    }

	//le code à exécuter quand le joueur interagit avec l'objet est définit à l'intérieur de cette fonction
	//les scripts des objets doivent donc contenir une définition d'override de cette fonction pour faire n'importe quoi
    protected virtual void ExecuteObjectAction()
    {
        Debug.LogWarning("No action has been defined. Please make sure all interactable objects use a derived class which overrides InteractableObject.ExecuteObjectAction().");
    }

	//nettoie les coroutines créées par un objet utilisant cette classe quand il est détruit pour prévenir des erreurs
	protected virtual void OnDestroy(){
		HideInteractPrompt ();
		StopAllCoroutines ();
	}

}
