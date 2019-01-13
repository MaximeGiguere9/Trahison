/**
 * script Platform
 * Maxime Giguère
 * 
 * petit script simple qui parente l'avatar à une plateforme qui se déplace quand il est positionné dessus (lui permet de rester sur la plateforme et suivre son mouvement)
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	void OnCollisionEnter(Collision c){
		if (c.gameObject.tag == "Player") {
			c.gameObject.transform.parent = gameObject.transform;
		}
	}

	void OnCollisionExit(Collision c){
		if (c.gameObject.tag == "Player" && c.gameObject.transform.parent == gameObject.transform) {
			c.gameObject.transform.parent = null;
		}
	}
}
