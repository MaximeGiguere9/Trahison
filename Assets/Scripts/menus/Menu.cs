/**
 * classe Menu
 * Maxime Giguère
 * 
 * cette classe de base gère la fonctionnalité de base de menus interactifs (i.e. gestion d'une liste d'options et actions y étant reliées)
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public abstract class Menu : MonoBehaviour {

    //références aux options du menu pour la manipulation de l'affichage
    public GameObject[] _optionContainers;
    //l'élément visuel qui indique l'option sélectionnée
    public GameObject _selector;
    //index de l'option sélectionnée dans la liste
    private int _selectedIndex;
    //références aux options du menu pour la manipulation de l'affichage
    private List<GameObject> _options;
    //actions à exécuter quand une option est choisie (le nom du GameObject à associer à une Action)
    protected Dictionary<string, System.Action> _actions;
    //action par défaut qui s'applique à toutes les options à leur création
    private System.Action _defaultAction;
	//
	private bool _isMouseSelectEnabled;
	private bool _isOptionSelectUpdateEnabled;


    private void Start()
    {
		EnableMouseSelect (true);
		EnableUpdate (true);
        ExecuteBeforeInit();
        SetSelector(_selector);
        InitialiseOptions(_optionContainers);
        InitialiseActions();
    }

    //code à exécuter avant d'initialiser l'interaction (i.e. création des éléments interactifs du menu par prog)
    protected virtual void ExecuteBeforeInit()
    {

    }

    //définit l'object qui sert d'indication visuelle de l'option sélectionnée
    protected void SetSelector(GameObject s)
    {
        _selector = s;
    }

	//
	public void EnableMouseSelect(bool enabled){
		_isMouseSelectEnabled = enabled;
	}

	public void EnableUpdate(bool enabled){
		_isOptionSelectUpdateEnabled = enabled;
	}



    protected void InitialiseOptions(GameObject[] containers){
        if(containers.Length == 0)
        {
            Debug.LogError("No options containers given.");
        }
        //
        _options = new List<GameObject>();
        //
        foreach (GameObject container in containers)
        {
            //extrait toutes les options du conteneur
            for (int i = 0; i < container.transform.childCount; i++)
            {
                //les rajoute dans la liste des options qu'on peut selectionner
                _options.Add(container.transform.GetChild(i).gameObject);
                //interactivité additionnelle avec la souris
                container.transform.GetChild(i).gameObject.AddComponent<OptionMouseOver>().SetController(this);
            }
        }
        //alerte si il n'y a pas d'eventsystem par défaut. Si il n'y a aucun EventSystem dans la scène, il n'y aura pas d'erreur, mais les contrôles souris ne fonctionneront pas.
        if(GameObject.Find("EventSystem") == null)
        {
            Debug.LogWarning("EventSystem not found. If there is no StandaloneInputModule in the scene, mouse controls will not work.");
        }
        //la première option est sélectionnée automatiquement
        try
        {
            UpdateSelector(0);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            Debug.LogError("Option list is empty.");
        }
		
	}

    //forme le dictionnaire des actions pour pouvoir les associer aux options
    private void InitialiseActions()
    {
        _actions = new Dictionary<string, System.Action>();
        //fabrique le donctionnaire à partir de la liste des options déjà créée
        foreach(GameObject go in _options)
        {
            try
            {
                System.Action a = (_defaultAction == null ? delegate () { Debug.LogWarning("No action was defined for option " + go.name); } : _defaultAction);
                _actions.Add(go.name, a);
            }
            catch (System.ArgumentException)
            {
                Debug.LogError("Option with name " + go.name + " already exists. Multiple options with the same name will create conflicts.");
            }      
        }
        //associe les options personnalisées
        SetActions();
    }

    protected void SetDefaultAction(System.Action action)
    {
        _defaultAction = action;
    }

    //les classes héritant de Menu initialisent les paramètres des actions l'intérieur de SetActions()
    protected abstract void SetActions();

    //définit une action à exécuter quand la sélection de l'option spécifiée est confirmée
    protected void SetAction(string optionName, System.Action action)
    {
        if (_actions.ContainsKey(optionName)) _actions[optionName] = action;
        else Debug.LogWarning("No menu option with name " + optionName + " was initialised. Action definition will be ignored.");
    }

    //obtient une action définie, utile pour faire en sorte que OnCancel ait le même comportement qu'une des options
    protected System.Action GetAction(string optionName)
    {
        if (_actions.ContainsKey(optionName)) return _actions[optionName];
        else return delegate () { Debug.LogWarning("No menu option with name " + optionName + " was initialised."); };
    }

    //actualise la position du curseur vis-à-vis l'option sélectionnée
    public void UpdateSelector(int n)
    {
		if (!_isOptionSelectUpdateEnabled) {
			//Debug.Log ("Option selection update is currently disabled.");
			return;
		}
			
        try
        {
            //position doit rester à l'intérieur de la liste
            _selectedIndex = Mathf.Clamp(n, 0, _options.Count - 1);
            //change la hauteur du sélecteur pour correspondre à l'option sélectionnée
            _selector.transform.position = new Vector2(_selector.transform.position.x, _options[_selectedIndex].transform.position.y);
        }
        //si le sélecteur est null
        catch (System.Exception e)
        {
            if(e.GetType() == typeof(System.NullReferenceException))
            {
                Debug.LogError("No selector defined.");
            }
            else if(e.GetType() == typeof(System.ArgumentOutOfRangeException))
            {
                Debug.LogError("Cannot select ID " + n + " when there are " + _options.Count + " options.");
            }

            
        }
    }


    protected virtual void Update () {
		//confirmation
		if(InputManager.GetCommand(InputManager.UIControl.proceed)){
			Select ();
		}
        //annulation
        if (InputManager.GetCommand(InputManager.UIControl.cancel))
        {
            OnCancel();
        }
        //change de sélection et actualise la position du curseur
        if (InputManager.GetCommand (InputManager.UIControl.up)) {
            if (_selectedIndex > 0)
				UpdateSelector(_selectedIndex - 1);
			else
				UpdateSelector(_options.Count -1);
		} else if (InputManager.GetCommand (InputManager.UIControl.down)) {
			if (_selectedIndex < _options.Count - 1)
				UpdateSelector(_selectedIndex + 1);
			else
				UpdateSelector(0);
		}
	}

    //obtient l'index du GameObject spécifié dans la liste des options s'il y est présent, sinon retourne null
	public int? GetOptionIDFromObject(GameObject go){
		if (_options.Contains (go))
			return _options.IndexOf (go);
		else
			return null;
	}

    //obtient l'objet sélectionné
    public GameObject GetSelectedOption()
    {
        return _options[_selectedIndex];
    }

    //exécute l'action définie pour l'option sélectionnée
	private void Select(){
        _actions[_options[_selectedIndex].name]();
	}

	//
	public void MouseSelect(){
		if(_isMouseSelectEnabled)
			Select();
		else {
			//Debug.Log ("Mouse selection is currently disabled.");
		}
	}

	public void MouseUpdate(int n){
		if(_isMouseSelectEnabled)
			UpdateSelector(n);
	}

    //action à définir qui s'exécute quand l'utilisateur appuie sur la touche annuler à la base du menu (la plupart du temps sera charger le menu précédent)
    protected virtual void OnCancel()
    {
        Debug.LogWarning("No cancel action defined.");
    }

}
