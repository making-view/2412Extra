using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    // Start is called before the first frame update
    static SceneTransitioner instance;
    [SerializeField] CanvasGroup _screenFader;
    [SerializeField] [Range(0.0f, 4.0f)] float _fadeTime = 1.0f;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        _screenFader = PlayerManager.instance._screenFader;
    }

    public void StartTransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        Debug.Log(this + ": going to scene " + sceneName);

        //TODO make function for fading in and out. Call this on start to fade in
        float timer = 0.0f;
        float halfFade = _fadeTime / 2.0f;

        while(timer < halfFade)
        {
            _screenFader.alpha = timer / halfFade;
            timer += Time.deltaTime;
            yield return null;
        }
        _screenFader.alpha = 1.0f;

        SceneManager.LoadScene(sceneName);

        //TODO set player rig at desired location

        timer = 0.0f;
        while (timer < halfFade)
        {
            _screenFader.alpha = 1.0f - (timer / halfFade);
            timer += Time.deltaTime;
            yield return null;
        }
        _screenFader.alpha = 0.0f;


        yield return new WaitForSeconds(_fadeTime / 2.0f);
    }
}
