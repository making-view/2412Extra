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
        StartCoroutine(FadeInOnStart());
    }

    IEnumerator FadeInOnStart()
    {
        PlayerManager.instance._screenFader.alpha = 1.0f;
        yield return null; //wait for tracking to be aquired before moving player
        MovePlayerToStartingPosition();
        yield return StartCoroutine(Fade(1.0f, 0.0f)); //fade in
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

        yield return StartCoroutine(Fade(0.0f, 1.0f));
        //load new scene and fire events
        onLoadScene.Invoke();
        yield return null;
        onLoadScene.RemoveAllListeners();
        SceneManager.LoadScene(sceneName);

        yield return null;
        MovePlayerToStartingPosition();

        yield return StartCoroutine(Fade(1.0f, 0.0f));
    }

    IEnumerator Fade(float from, float to)
    {
        float timer = 0.0f;
        float halfFade = _fadeTime / 2.0f;
        PlayerManager.instance._screenFader.alpha = from;

        while (timer < halfFade)
        {
            float progress = Mathf.SmoothStep(from, to, timer / halfFade);
            PlayerManager.instance._screenFader.alpha = progress;
            timer += Time.deltaTime;
            yield return null;
        }
        PlayerManager.instance._screenFader.alpha = to;
    }

    public void MovePlayerToStartingPosition()
    {
        var startingPosition = GameObject.FindGameObjectWithTag("StartingPosition").transform;

        if(startingPosition == null)
        {
            Debug.LogError(this + " No starting position found");
            return;
        }

        Debug.Log(this + " Found starting pos. Moving player");
        
        Vector3 targetPos = startingPosition.position;
        Quaternion targetRot = startingPosition.rotation;

        MovePlayerTo(targetPos, targetRot);
    }

    public void MovePlayerTo(Vector3 targetPos, Quaternion targetRot)
    {
        var trackingContainer = PlayerManager.instance.GetComponentInChildren<AutoHandPlayer>().trackingContainer;
        var cameraTrans = PlayerManager.instance.GetComponentInChildren<Camera>().transform;

        trackingContainer.transform.rotation = Quaternion.Euler(0, trackingContainer.transform.eulerAngles.y + targetRot.eulerAngles.y - cameraTrans.eulerAngles.y, 0);
        PlayerManager.instance.GetComponentInChildren<AutoHandPlayer>().SetPosition(targetPos);
    }
}
