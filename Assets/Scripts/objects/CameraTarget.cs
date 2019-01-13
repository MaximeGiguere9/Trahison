/**
 * script CameraTarget
 * Maxime Giguere
 * 
 * petit script qui fait en sorte que la caméra suive un GameObject spécifié 
 */

using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour {

    //gameobject à suivre
	public GameObject _target;

    //déplacement de la caméra WIP
    /*
    Vector3 distance;
    Rigidbody body;
    Rigidbody playerBody;

    void Start () {
      body = GetComponent<Rigidbody> ();
      playerBody = player.GetComponent<Rigidbody> ();
    }

    void FixedUpdate(){
        if (Mathf.Abs (playerBody.velocity.x) >= 3) {
            gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, player.transform.position + playerBody.velocity, 0.05f);  
        } else {
            body.velocity = Vector3.zero;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, player.transform.position, 0.05f);
        }
    }*/

    //déplacement fixe
    void FixedUpdate()
    {
        gameObject.transform.position = _target.transform.position;
    }


}
