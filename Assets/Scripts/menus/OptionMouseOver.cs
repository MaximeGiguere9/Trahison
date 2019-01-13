/**
 * classe OptionMouseOver
 * Maxime Giguère
 * 
 * gère l'interactivité des éléments d'un menu avec la souris (i.e. permet de reconnaître quand la souris est sur un élément et clic dessus)
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {

    //lien avec le script du menu pour pouvoir communiquer
    private Menu _controller;
    private int _depth;

    private void Start()
    {
        _depth = 0;
        PropagateToChildren(gameObject);
    }

    //ajoute un écouteur sur tous les enfants du gameObject si applicable (voir OptionMouseOverChild)
    public void PropagateToChildren(GameObject go)
    {
        if (go.transform.childCount > 0)
        {
            _depth++;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.AddComponent<OptionMouseOverChild>().SetMasterScript(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData p){
        //amène l'événement à la surface du groupe
        GameObject hoveredObject = p.pointerCurrentRaycast.gameObject;
        for(int i = 0; i < _depth; i++) { hoveredObject = hoveredObject.transform.parent.gameObject; }
        //trouve l'index du GameObject sur lequel la souris est placée dans la liste des options du menu
        int? index = _controller.GetOptionIDFromObject(hoveredObject);
        //si il y est présent, le sélectionne
        if (index != null) {
			_controller.MouseUpdate ((int)index);
		}
	}

	public void OnPointerClick(PointerEventData p){
        //confirme la sélection quand clic
        _controller.MouseSelect ();
	}

    //définit le script qui contrôle le menu
	public void SetController(Menu m){
        _controller = m;
	}


		
}
