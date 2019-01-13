/**
 * classe PlayerCharacterController
 * Maxime Giguere
 * 
 * PlayerCharacterController gère les actions reliées à l'avatar du joueur (e.g. déplacement, collision, combat)
 */

using UnityEngine;
using System.Collections;

public class PlayerCharacterController : MonoBehaviour {


    //vitesses de déplacement
	public float walkSpeed = 3;
	public float runSpeed = 9;
    //longueur maximale du raycast en dessous de l'avatar (pour qu'il soit considéré sur le sol)
    public float groundCheckRaycastMaxLength = 0.6f;

	

	//composantes additionnelles
	Animator animator;
	Rigidbody body;
	//différents états de l'avatar
	public bool isGrounded;
	public bool isAirborne;
	private bool isAirborneOld;
	public bool isWallGrabbing;
	public bool isSliding;
    //référence au GameObject du mur auquel le joueur est accroché (pour l'orienter)
	public GameObject currentWallObjRef;
	//variables reliées au contact avec le sol
	public GameObject currentGroundObjRef;
	RaycastHit groundContactHit;
    ContactPoint groundContactPoint;
    //conserver la vitesse du joueur quand il saute pour un déplacement dans les airs plus naturel
	public float maxAirborneVelocityX;
    //variales reliées au saut 
    public string jumpType;
    public float jumpVar_force;
	public float jumpVar_height;
	public float jumpVar_targetY;
    //velocité totale à appliquer au GameObject de l'avatar à la fin des calculations
	public Vector3 totalTargetVelocity;

    //layer utilisé pour les raycasts, la collision avec le décor statique (sol, murs, plafonds, etc.)
	private int terrainLayer;

    //au lieu d'appeler InputManager.GetInputDirectionX plusieurs fois
    private float inputDirectionX;
	private float inputDirectionY;


	//soit 90degress => +1 (est face à la droite de l'écran) ou -90degrees => -1 (face à la gauche de l'écran)
	float avatarFacingDirection;

	void Start () {
		


		body = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator> ();

        //180degrees, fait face à l'écran
		avatarFacingDirection = 2;

		jumpVar_height = 11;

		terrainLayer = LayerMask.NameToLayer("Terrain");



		//place Alex a la bonne place après transition
		if (PlayerData.Get ("sceneTransitionSource") == "ciel") {
			Vector3 pos = GameObject.Find ("SQ3_EntreeCiel").gameObject.transform.position;
			gameObject.transform.position = new Vector3(pos.x, pos.y, 0);
		} else if (PlayerData.Get ("sceneTransitionSource") == "egouts") {
			Vector3 pos = GameObject.Find ("SQ1_EntreeEgouts").gameObject.transform.position;
			gameObject.transform.position = new Vector3(pos.x, pos.y, 0);
		}

        
        


    }
	
	void FixedUpdate () {

		////////////////////////////////////////
		//gestion des variables de déplacement//
		////////////////////////////////////////

		//sauve un peu de ressources en n'appeleant ces fonctions qu'une seule fois par boucle
		inputDirectionX = InputManager.GetInputDirectionX ();
		inputDirectionY = InputManager.GetInputDirectionY ();

		/////--- états ---/////

		//état sur le sol défini par un raycast en desous de l'avatar
		isGrounded = Physics.Raycast (gameObject.transform.position + Vector3.up * 0.5f, Vector3.down * 0.5f, out groundContactHit, groundCheckRaycastMaxLength, ~terrainLayer);
		Debug.DrawRay (gameObject.transform.position + Vector3.up * 0.5f, Vector3.down * groundCheckRaycastMaxLength, Color.red, 0.1f);


		//état en glissement défini par une collision avec le sol sans être sur le sol
		isSliding = !isGrounded && currentGroundObjRef != null
		&& MathExt.RangeContains (GetSurfaceAngle () % 90, 1, 61)//et l'objet à un angle approprié
		&& body.velocity.y < 0.1; // et la gravité pousse l'avatar vers le sol
		//état d'accrochage sur un mur défini en interagissant avec un mur
		isWallGrabbing = (isWallGrabbing || isAirborne) && InputManager.GetCommand (InputManager.AvatarControl.run) && currentWallObjRef != null;

		//état dans les airs si aucune interaction avec le terrain
		isAirborne = !isGrounded && !isWallGrabbing && !isSliding;

		/////--- fin des états ---/////

		



		//la vélocité maximale de l'avatar dans les airs est limitée à sa vitesse au moment du saut
		if (isAirborne && !isAirborneOld) {
			maxAirborneVelocityX = Mathf.Clamp (Mathf.Abs (body.velocity.x), 3, 9);
		}
		isAirborneOld = isAirborne;

		//orientation de l'avatar
		if (isWallGrabbing) {
			//dos au mur
			avatarFacingDirection = (currentWallObjRef.transform.position.x < gameObject.transform.position.x) ? 1 : -1;
		} else if (isSliding) {
			//dos au sol
			avatarFacingDirection = (groundContactPoint.point.x < gameObject.transform.position.x) ? 1 : -1;
		} else {
			if (inputDirectionX != 0)
				//face la direction des touches de l'axe x
				avatarFacingDirection = Mathf.Clamp (inputDirectionX, -1, 1);
		}


		//////////////////////////
		//vélocité du GameObject//
		//////////////////////////

		//orientation de l'avatar dépend du déplacement x
		gameObject.transform.rotation = Quaternion.Euler (0, 90 * avatarFacingDirection, 0);
		//+ override pour faire face au mur car l'animation la requiert
		if(isWallGrabbing)
			gameObject.transform.rotation = Quaternion.Euler (0, 90*-avatarFacingDirection, 0);

		//vélocité
		if ((isGrounded || isSliding) && currentGroundObjRef != null) { //si l'avatr est sur le sol
			totalTargetVelocity =  this.SetAvatarVelocityGrounded ();
		} else if (isAirborne || (currentGroundObjRef == null && currentWallObjRef == null)) { //si il est dans les airs
			totalTargetVelocity = this.SetAvatarVelocityAirborne ();
		} else { //aucun état valide
			totalTargetVelocity = new Vector3 (0, 0, 0);
		}
		Debug.DrawRay (gameObject.transform.position, totalTargetVelocity, Color.red, 0.1f);
		//sauter modifie la vélocité
		if (InputManager.GetCommand (InputManager.AvatarControl.jump) && (!isAirborne || isSliding)) {
			totalTargetVelocity = this.SetJumpVelocity ();
		}


        
		////////////
		//animator//
		////////////
		if (InputManager.GetCommand (InputManager.AvatarControl.attack)) {
			animator.SetTrigger("attack");
		}
		if(animator.GetCurrentAnimatorStateInfo (0).IsTag ("attaque") || animator.GetCurrentAnimatorStateInfo(0).IsTag("mort"))
        {
			totalTargetVelocity.x = 0;
            if(totalTargetVelocity.y >= 0) totalTargetVelocity.y = 0;
            //orientation de l'avatar dépend de la position de la souris
            //gameObject.transform.rotation = Quaternion.Euler (0, 90 * (Mathf.Clamp01((Camera.main.WorldToScreenPoint(gameObject.transform.position) - Input.mousePosition).x) == 1 ? -1 : 1), 0);
        }
		animator.SetFloat ("velocityX_Perso", Mathf.Lerp(animator.GetFloat("velocityX_Perso"), Mathf.Abs(body.velocity.x), 0.2f));
		//animator.SetFloat ("velocityY_Perso", Mathf.Lerp(animator.GetFloat("velocityY_Perso"), body.velocity.y, 0.2f));
		animator.SetBool ("jump", isAirborne);
		animator.SetBool ("grimper", isWallGrabbing);


        //applique la vélocité totale calculée au GameObject de l'avatar
		body.velocity = totalTargetVelocity;

		//annule la vélocité quand Alex interagit avec une boîte de texte
		if(InputManager.IsAvatarInputLocked()) body.velocity = new Vector3(0, body.velocity.y - Time.deltaTime*18, 0);


	}


    //retourne un vector3 parallèle à la surface du terrain, en direction +x
    Vector3 GetSurfaceAngleVector()
    {
        return (Quaternion.AngleAxis(-90, Vector3.forward) * groundContactPoint.normal);
    }

    //retourne un nombre positif, entre 0 (sol plat) et 90 (mur)
    float GetSurfaceAngle()
    {
        return Vector3.Angle(Vector3.right, GetSurfaceAngleVector())%90;
    }


    //calcule le vecteur de vitesse du joueur sur le sol
    Vector3 SetAvatarVelocityGrounded()
    {
        float targetSpeed = 0;
        float inputDirX = InputManager.GetInputDirectionX();
        Vector3 surfaceAngle = GetSurfaceAngleVector();

        if (isGrounded)
        {
            //plus ou moins vite dépendamment de la commande de course vs marche, modifié selon l'orientation des commandes
            targetSpeed = (InputManager.GetCommand(InputManager.AvatarControl.run) ? runSpeed : walkSpeed)*inputDirX;
        }
        else if (isSliding)
        {
            //0 si contre la pente, 1 si aucune commande ou commande dans la même direction que la pente
            float slideInput = Mathf.Clamp01(inputDirX * avatarFacingDirection + 1);
            //accélère/décélère en 0.5 secondes
            targetSpeed = Mathf.Lerp(body.velocity.magnitude * avatarFacingDirection, runSpeed * slideInput * avatarFacingDirection, Time.deltaTime / 0.5f);
        }
        else
        {
            Debug.LogError("Error calculating grounded velocity : invalid state.");
        }

        //Debug.Log(targetSpeed);
        //Debug.DrawRay(gameObject.transform.position, surfaceAngle, Color.blue, 0.1f);
        //Debug.DrawRay(gameObject.transform.position, surfaceAngle * targetSpeed, Color.cyan, 0.1f);
        
        //place le vecteur de vitesse par rapport à la surface du sol
        return surfaceAngle * targetSpeed;
    }

    //calcule le vecteur de vitesse du joueur dans les airs
    Vector3 SetAvatarVelocityAirborne(){
		Vector3 targetVelocity = new Vector3(0,0,0);
        //chamgements graduels donne un déplacement aérien plus naturel
		targetVelocity.x = Mathf.Clamp (body.velocity.x+((float)inputDirectionX/5), -maxAirborneVelocityX, maxAirborneVelocityX);
		//arc de saut, réduit la vélocité verticale
		targetVelocity.y = body.velocity.y - Time.deltaTime*18;
		return targetVelocity;
	}

    //rajoute la force instantanée du saut
	Vector3 SetJumpVelocity(){
        //saut normal
        Vector3 jumpVelocity = new Vector3(body.velocity.x, jumpVar_height, 0);
        jumpType = "std";
           
        
        //vélocité modifiée par pente si en train de glisser
        if (isSliding)
        {
            jumpVelocity = Quaternion.Euler(0, 0, (GetSurfaceAngle()*0.6f) * -avatarFacingDirection) * jumpVelocity;
            jumpType = "slide";

        }
        //saut modifé si arroché à un mur
        else if (isWallGrabbing)
        {
            //écrase la vélocité x pour sauter dans la direction opposée au mur, mais laisse un peu de liberté au joueur
            jumpVelocity.x = 4f * avatarFacingDirection + 2f * inputDirectionX;
            //peut sauter plus haut ou plus bas en appuyant sur les touches de déplacement vertical
            jumpVelocity.y = (jumpVar_height / 2) + inputDirectionY * (jumpVar_height / 2);
            jumpType = "wall";
        }
		return jumpVelocity;
	}

   

	void OnCollisionEnter(Collision col){
        //conserve référence au mur
		if (col.gameObject.tag == "Wall") {
			currentWallObjRef = col.gameObject;
		}
	}

	void OnCollisionStay(Collision col){
        //conserve référence au sol
		if (col.gameObject.tag == "Ground") {
			currentGroundObjRef = col.gameObject;
            groundContactPoint = col.contacts[0];
            Debug.DrawRay(groundContactPoint.point, groundContactPoint.normal, Color.blue, 0.1f);
        }

       

    }

	void OnCollisionExit(Collision col){
        //supprime références aux objets quand le joueur s'en sépare
		if (col.gameObject.tag == "Wall") {
			if(currentWallObjRef == col.gameObject)currentWallObjRef = null;
		}
		if (col.gameObject.tag == "Ground" && col.gameObject == currentGroundObjRef) {
            currentGroundObjRef = null;
		}
	}


    




}
