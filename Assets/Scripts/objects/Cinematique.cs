using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cinematique : MonoBehaviour {

    private VideoPlayer _vp;
    private GameObject _canvas;


	void Start () {
        _vp = GetComponent<VideoPlayer>();
        _canvas = GameObject.Find("Canvas");

        _vp.loopPointReached += TransitionFromCutscene;
    }

    private void Update()
    {
        _canvas.SetActive(!_vp.isPlaying);
    }


    void TransitionFromCutscene(VideoPlayer v)
    {
        SceneManager.LoadScene("sceneTuto");
    }

}
