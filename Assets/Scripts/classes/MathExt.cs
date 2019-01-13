/**
 * classe MathExt
 * Maxime Giguère
 * 
 * Vérifier qu'un nombre est entre deux autres nombres est une opération très simple et plutôt utile, mais ne fait pas partie de la classe Math. 
 * Voici donc MathExt, pour les fois où on se demande pourquoi des trucs simples ne possèdent pas déjà de fonctions.
 */ 


using UnityEngine;
using System.Collections;

public static class MathExt {
	
	//vérifie si un nombre est entre deux valeurs
	public static bool RangeContains(float number, float min, float max){
		return (number >= min && number <= max);
	}

    //fait un raycast et vérifie s'il touche un objet spécifié (plutôt que e.g. tous les objets d'un layer)
	public static bool RaycastOnSingleObject(Vector3 origin, Vector3 direction, float maxDistance, GameObject target){
		RaycastHit[] hitList = Physics.RaycastAll (origin, direction, maxDistance);
		foreach (RaycastHit hit in hitList) {
			if(hit.collider.gameObject == target){
				return true;
			}
		}
		return false;
	}
}
