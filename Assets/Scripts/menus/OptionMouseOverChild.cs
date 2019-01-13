/**
 * classe OptionMouseOverChild
 * Maxime Giguère
 * 
 * considérant que l'objet sur lequel la souris pointe est toujours l'enfant le plus profond et jamais un groupe, 
 * cette classe permet la propagation des événements souris sur tous les éléments d'un même groupe
 * (i.e. aller chercher l'élément plus profond pour renvoyer l'événement souris à la base du groupe)
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionMouseOverChild : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    //écouteur à la surface du groupe
    private OptionMouseOver _parent;
    //renvoie les événements à la surface
    public void OnPointerEnter(PointerEventData p){ _parent.OnPointerEnter(p); }
    public void OnPointerClick(PointerEventData p){ _parent.OnPointerClick(p); }
    //fait le lien entre les enfants plus profonds de la hiérarchie et la surface du groupe
    public void SetMasterScript(OptionMouseOver p)
    {
        _parent = p;
        _parent.PropagateToChildren(gameObject);
    }
}
