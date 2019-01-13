/**
 * classe CombatModule
 * Maxime Giguere
 * 
 * 
 * Code relié au combat (e.g. points de vies, zones de dommage)
 * Ce module active des zones de dégats selon l'animation, et dicte quand l'acteur doit mourir
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatModule : MonoBehaviour {

	//si le module peut réagir (e.g. déasctiver pendant l'anim de mort)
	private bool _isInteractable;
    //points de vie initiaux / maximum
	public int _maxHitPoints = 2;
    //points de vie actuels
    private int _hitPoints;
    //puissance d'attaque
    public int _puissance = 1;
    //animations à écouter pour attaquer
	public Animator _animator;
	public string _animationClipAttaqueName = "attaque";
	public string _animationClipMortName = "mort";
	public string _triggerParamMortName = "t_mort";
    //materiels à recolorier quand l'acteur reçoit du dommage
    List<Material> _materialsToFlash;
	//son à jouer à l'attaque
	private AudioSource _attackSoundSource;
	private bool _hasSoundPlayed;

    //initialise points de vie
    protected virtual void Start () {
		_attackSoundSource = GetComponent<AudioSource>();
        _hitPoints = _maxHitPoints;
		_isInteractable = true;

        //materiels à recolorier quand l'acteur reçoit du dommage
        _materialsToFlash = new List<Material>();
        foreach (SkinnedMeshRenderer r in gameObject.transform.parent.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material m in r.materials)
            {
                _materialsToFlash.Add(m);
            }
        }
        foreach (MeshRenderer r in gameObject.transform.parent.gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material m in r.materials)
            {
                _materialsToFlash.Add(m);
            }
        }
    }
	

	void Update () {
        //si l'animation d'attaque joue, active le collider de détection de dommage
		if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationClipAttaqueName)) {
			gameObject.GetComponent<BoxCollider> ().enabled = true;
			if (!_hasSoundPlayed) {
				_attackSoundSource.Play ();
				_hasSoundPlayed = true;
			}
		} else {
			gameObject.GetComponent<BoxCollider> ().enabled = false;
			_hasSoundPlayed = false;
		}
	}

	void OnTriggerEnter(Collider coll){
        //prévient les CombatModule d'interagir les uns avec les autres; doivent seulement détecter les colliders des acteurs
		if (coll.gameObject.GetComponentInChildren<CombatModule> () != null && coll.gameObject.name != "CombatModule") {
            //applique les points de dommage au module de l'adversaire
			coll.gameObject.GetComponentInChildren<CombatModule> ().Damage (_puissance, gameObject);
		}
	}

	public void Damage(int power, GameObject source = null){
		if (!_isInteractable)
			return;
        //réduit points de vie
        _hitPoints -= power;
        //feedback visuel (flash rouge)
		StartCoroutine (OnDamageFlash ());
        Debug.Log(gameObject.transform.parent.gameObject.name + " took " + power + " damage point(s)" + (source == null ? "." : " from " + (source.name == "CombatModule" ? source.transform.parent.gameObject.name : source.name)));
        OnDamage();
        //séquence de mort
        if (_hitPoints <= 0) {
			StartCoroutine(WaitForDeathDelete ());
			Debug.Log (gameObject.transform.parent.gameObject.name + " died.");
		}
	}

    public void Heal(int power, GameObject source = null)
    {
		if (!_isInteractable)
			return;
        //augmente points de vie
        _hitPoints += power;
        //mais ne peuvent pas dépasser points maximum
        if (_hitPoints > _maxHitPoints) _hitPoints = _maxHitPoints;
        Debug.Log(gameObject.transform.parent.gameObject.name + " healed " + power + " damage point(s) from " + source.name);
        OnHeal();
    }

    public int GetCurrentHP()
    {
        return _hitPoints;
    }

    public int GetMaxHP()
    {
        return _maxHitPoints;
    }

    public void SetCurrentHP(int val)
    {
        _hitPoints = val;
        if (_hitPoints > _maxHitPoints) _hitPoints = _maxHitPoints;
    }

    public void SetMaxHP(int val)
    {
        _maxHitPoints = val;
        if (_hitPoints > _maxHitPoints) _hitPoints = _maxHitPoints;
    }

    public float GetHPRatio()
    {
        return (float)_hitPoints / (float)_maxHitPoints;
    }

    IEnumerator OnDamageFlash(){

        foreach(Material m in _materialsToFlash) { m.color = Color.red; }
		yield return new WaitForSeconds(0.1f);
        foreach (Material m in _materialsToFlash) { m.color = Color.white; }
        yield return new WaitForSeconds(0.1f);
        foreach (Material m in _materialsToFlash) { m.color = Color.red; }
        yield return new WaitForSeconds(0.1f);
        foreach (Material m in _materialsToFlash) { m.color = Color.white; }
	}

    //joue l'animation de mort et détruit l'acteur quand elle se termine
	IEnumerator WaitForDeathDelete(){
		_isInteractable = false;
		_animator.SetTrigger (_triggerParamMortName);
		do {
			yield return new WaitForSeconds(0.2f);
		} while (_animator.GetCurrentAnimatorStateInfo (0).IsName (_animationClipMortName) || _animator.GetNextAnimatorStateInfo(0).IsName(_animationClipMortName));
		Destroy (gameObject.transform.parent.gameObject);
		OnDeath ();
	}

    protected virtual void OnDamage()
    {

    }

    protected virtual void OnHeal()
    {

    }

	protected virtual void OnDeath()
	{
	
	}
}
