/**
 * classe Timer et conteneur statique TimerManager
 * Maxime Giguère
 * 
 * Peut-être un peu overkill pour ce projet-ci, l'idée derrière cette classe est de créer/modifier/supprimer des décomptes et d'y avoir accès à partir d'ailleurs dans le code.
 * On peut ainsi gérer les événements sensibles au temps en appelant cette classe au lieu d'avoir des boucles un peu n'importe où n'importe comment.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {


	//temps restant au décompte
	public float _remainingTime;
	//temps initial
	public float _initialTime;
	//accéder au statut du décompte à l'extérieur de la classe
	public bool _timesUpFlag;
	private string _name;
	//le code à exécuter quand le décompte arrive à 0
	private System.Func<bool> _OnTimeUpAction;
	//la boucle du décompte
	private Coroutine _countDown;

	public Timer(string name, float time, System.Func<bool> timeUpAction){
		//quitte sans créer de timer si un autre timer du même nom existe déjà
		if (TimerManager.AddTimer (name, this) == false) {
			Debug.LogError("Timer " + name + " already exists.");
			return;
		}
		//initialisation
		_name = name;
		_timesUpFlag = false;
		_initialTime = time;
		_remainingTime = _initialTime;
		_OnTimeUpAction = timeUpAction;
		Debug.Log("Created " + _initialTime + "s Timer under ID " + this._name);
		Resume();
	}
		
	//actualise le décompte et l'affiche en format m:ss
	IEnumerator UpdateCountDown(){
		//actualise à chaque seconde tant qu'il n'est pas temriné
		while(_remainingTime > 0){
			_remainingTime--;
			//Debug.Log ("(" + this._name + ") " + Mathf.Floor(this.GetRemainingTime()/60) + ":" + (this.GetRemainingTime()%60 < 10 ? "0" : "") + (this.GetRemainingTime()%60) + " remaining.");
			yield return new WaitForSeconds(1);
		}
		//exécute l'action de la fin du décompte
		_timesUpFlag = true;
		this.OnTimeUp ();
	}

    //retourne le temps restant au décompte
	public float GetRemainingTime()
    {
		return _remainingTime;
	}
    //retourne le temps restant sous forme mm:ss
    public string GetFormattedRemainingTime()
    {
        return (Mathf.Floor(this.GetRemainingTime() / 60) + ":" + (this.GetRemainingTime() % 60 < 10 ? "0" : "") + (this.GetRemainingTime() % 60));
    }

    //le décompte s'est-il terminé
    public bool IsTimeUp(){
		return _timesUpFlag;
	}

	//exécute l'action de la fin du décompte et enlève le timer de la liste
	private void OnTimeUp(){
		_OnTimeUpAction ();
		TimerManager.RemoveTimer(this._name);
	}


	//renvoie le décompte à son état initial et le redémarre
	public void ResetCountDown(){
        Debug.Log("Resetting Timer " + this._name);
		TimerHelper.Instance.StopCoroutine(_countDown);
		_remainingTime = _initialTime;
		_countDown = TimerHelper.Instance.StartCoroutine(UpdateCountDown());
	}

	//arrête/pause le décompte
	public void Stop(){
		Debug.Log("Stopping Timer " + this._name);
		TimerHelper.Instance.StopCoroutine (_countDown);

	}

	//démarre le décompte
	public void Resume(){
		Debug.Log("Starting Timer " + this._name);
		_countDown = TimerHelper.Instance.StartCoroutine(UpdateCountDown());

	}



}

//donne accès aux coroutines, nécessaire car Timer n'est pas un MonoBehaviour
public class TimerHelper : MonoBehaviour
{
	private static TimerHelper _Instance;
	public static TimerHelper Instance
	{
		get
		{
			if (_Instance == null){ 
				GameObject go = new GameObject ("TimerHelper");
				DontDestroyOnLoad (go);
				_Instance = go.AddComponent<TimerHelper>(); 
			}
			return _Instance;
		}
	}
}

//gère la liste de tous les timers
public static class TimerManager
{
	//dictionnaire contenant la liste des timers (ID et objet)
	private static Dictionary<string, Timer> _timerList;
	private static Dictionary<string, Timer> TimerList
	{
		get
		{
            //initialisation
            if (_timerList == null) _timerList = new Dictionary<string, Timer>();
            return _timerList;
		}
	}
	//extrait un timer du dictionnaire
	public static Timer GetTimer(string source){
		if (TimerList.ContainsKey (source)) return TimerList [source];
		return null;
	}
	//si le nom spécifié est présent dans le dictionnaire
	public static bool DoesTimerExist(string source){
		if (TimerList.ContainsKey (source)) return true;
		return false;
	}
	//ajoute un timer dans le dictionnaire
	public static bool AddTimer(string source, Timer timer){
		if (TimerList.ContainsKey (source) == false) {
			TimerList.Add (source, timer);
			return true;
		}
		return false;
	}
	//enlève un timer du dictionnaire (effectivement la suppression de références au Timer)
	public static bool RemoveTimer(string source){
		if (TimerList.ContainsKey (source)) {
			TimerList.Remove (source);
			return true;
		}
		return false;
	}
	//arrête tous les timers (e.g. quand le menu de pause est actif)
	public static void StopAll(){
		foreach (KeyValuePair<string, Timer> t in _timerList) {
			t.Value.Stop ();
		}
	}
	//arrête tous les timers (e.g. quand le menu de pause est désactivé)
	public static void ResumeAll(){
		foreach (KeyValuePair<string, Timer> t in _timerList) {
			t.Value.Resume ();
		}
	}


}

