using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    // Start is called before the first frame update
    public static SceneTransitioner instance;
    [SerializeField] [Range(0.0f, 4.0f)] float _fadeTime = 1.0f;

    public UnityEvent onLoadScene = new UnityEvent();
    private bool _initialized = false;


    private void Start()
    {
        MovePlayerToStartingPosition();
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        _initialized = true;
    }

    private void OnEnable()
    {
        Initialize();
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
            PlayerManager.instance._screenFader.alpha = timer / halfFade;
            timer += Time.deltaTime;
            yield return null;
        }
        PlayerManager.instance._screenFader.alpha = 1.0f;

        //load new scene and fire events
        onLoadScene.Invoke();
        onLoadScene.RemoveAllListeners();
        SceneManager.LoadScene(sceneName);

        MovePlayerToStartingPosition();

        timer = 0.0f;
        while (timer < halfFade)
        {
            PlayerManager.instance._screenFader.alpha = 1.0f - (timer / halfFade);
            timer += Time.deltaTime;
            yield return null;
        }
        PlayerManager.instance._screenFader.alpha = 0.0f;


        yield return new WaitForSeconds(_fadeTime / 2.0f);
    }

    public void MovePlayerToStartingPosition()
    {
        var startingPosition = GameObject.FindGameObjectWithTag("StartingPosition").transform;

        if(startingPosition == null)
        {
            Debug.LogError("No starting position found");
            return;
        }
        
        var playerBody = PlayerManager.instance.GetComponentInChildren<AutoHandPlayer>();
        Vector3 targetPos = startingPosition.position;
        Quaternion targetRot = startingPosition.rotation;

        playerBody.SetPosition(targetPos, targetRot);
    }
}
