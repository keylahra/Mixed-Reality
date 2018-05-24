using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public delegate void PlayerDied();
    public static event PlayerDied OnPlayerDeath;

    private bool playerOnFloe = true;
    private string sceneName = "SpatialMapping2";

    List<IceFloe> iceFloes;

    void Start () {
		
	}

    private void OnEnable()
    {
        //OnPlayerDeath += EndScene;
        OnPlayerDeath += PlayerDeathFeedback;
    }

    void Update () {

        CheckIfPlayerOnFloe();
	}

    void FloeEnter(int id)
    {
        print("FloeEnter");
    }

    void FloeExit(int id)
    {
        print("FloeExit");
    }

    private void CheckIfPlayerOnFloe()
    {
        //iceFloes = GetComponent<IceFloeManager>().floeList;
        //foreach()
    }

    private void PlayerDeathFeedback()
    {

    }

    private void EndScene()
    {
        // start game over UI

        StartCoroutine(WaitAndReloadScene());
    }

    private IEnumerator WaitAndReloadScene()
    {
        yield return new WaitForSecondsRealtime(5);
        // reload scene
        SceneManager.LoadScene(sceneName);
    }

    public void FirePlayerDeath()
    {
        OnPlayerDeath();
    }
}
