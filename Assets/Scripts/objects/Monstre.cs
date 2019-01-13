/**
 * script Monstre
 * Maxime Giguère
 * 
 * contrôle le comportement des monstres par navmesh
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monstre : MonoBehaviour {

    //navmesh
    private NavMeshAgent _nv;
    //cible (i.e. Alex)
    public GameObject _target;
    //distance maximum de chasse
    public float _maxDistance;
    //distance minimum avant d'attaquer
    public float _minDistance;
    //si le monstre est en train d'attaquer
    private bool _isAttacking;
    //animator
    private Animator _animator;


	void Start () {
        _isAttacking = false;
        _nv = GetComponent<NavMeshAgent>();
        _nv.stoppingDistance = _minDistance;
        _animator = GetComponent<Animator>();
        _nv.destination = _target.transform.position;
    }
	

	void Update () {

        

        //arrête le déplacement si une interface est ouverte (e.g. menu pause, boîte de texte)
        if (InputManager.IsAvatarInputLocked())
        {
            _nv.isStopped = true;
        }
        else
        {
            //traque la position d'Alex et le chasse s'il s'approche trop
            _nv.destination = _target.transform.position;
			//s'arrète si Alex est trop loin ou si le monstre est mort
			_nv.isStopped = (_nv.remainingDistance > _maxDistance || _nv.remainingDistance < _minDistance || GetComponentInChildren<CombatModule>().GetCurrentHP() <= 0);
            //attaque Alex quand le monstre est assez près de lui
            //(_target.transform.position - gameObject.transform.position).magnitude est un check additionnel pour les égouts, où Alex est souvent au dessus de la destination du navmesh sans être à portée du monstre
            if (_nv.remainingDistance < _minDistance && !_isAttacking && (_target.transform.position - gameObject.transform.position).magnitude < _minDistance*2)
            {
                StartCoroutine(Attack());
            }
        }

        //animation de déplacement si le navmesh est en mouvement
        _animator.SetBool("enDeplacement", !_nv.isStopped);


    }


    //rythme d'attaque
    IEnumerator Attack()
    {
        _isAttacking = true;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
		//vérifie que le monstre n'a pas été tué entre-temps
		if(GetComponentInChildren<CombatModule>().GetCurrentHP() > 0)
        	GetComponent<Animator>().SetTrigger("attaque");
        yield return new WaitForSeconds(2f);
        _isAttacking = false;
        yield return true;
    }
}
