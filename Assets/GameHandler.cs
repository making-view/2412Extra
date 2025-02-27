using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    private bool _gameRunning = false;
    private bool _gameFinished = false;

    public UnityEvent onGameReady = new UnityEvent();
    public UnityEvent onGameStarted = new UnityEvent();

    private float _timer = 0;
    [SerializeField] private float _timeLimit = 10f;
    private Coroutine _helpPlayer = null;
    
    //TODO make into list with timers?
    [SerializeField] private TextMeshProUGUI _txtTimer;
    private AudioSource _backgroundMusic;

    [Space]
    [SerializeField] private AudioSource _rampenAudio;
    [SerializeField] private AudioClip _rampenGamePrepare;
    [SerializeField] private AudioClip _lightsOn;

    [Space]
    [Header("Stuff to enable and disable when starting game")]
    [SerializeField] private List<GameObject> _enableOnGameReady = new List<GameObject>();
    [SerializeField] private List<GameObject> _disableOnGameReady = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        _backgroundMusic = GetComponent<AudioSource>();


        foreach (GameObject go in _disableOnGameReady)
            go.SetActive(true);

        foreach (GameObject go in _enableOnGameReady)
            go.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameRunning)
            return;

        _timer += Time.deltaTime;
        TimeSpan ts = TimeSpan.FromSeconds(_timer);
        _txtTimer.text = String.Format(@"{0:mm\:ss\.ff}", ts);

        if(_timer > _timeLimit && _helpPlayer == null)
        {
            _helpPlayer = StartCoroutine(HelpPlayerFindMess());
        }
    }

    private IEnumerator HelpPlayerFindMess()
    {
        MessHighlighter highlighter = FindObjectOfType<MessHighlighter>(true);
        highlighter.gameObject.SetActive(true );
        Transform highlightedMess = MessHandler.instance.GetClosestMess();
        highlighter.StartMovingTowardsTarget(highlightedMess);

        while (_gameRunning)
        {
            Transform closestMess = MessHandler.instance.GetClosestMess();
            if(closestMess != highlightedMess)
            {
                highlightedMess = closestMess;
                highlighter.StartMovingTowardsTarget(closestMess);
            }

            yield return new WaitForSeconds(1.0f);
        }

        highlighter.gameObject.SetActive(false );
    }

    public void StartPreparingGame()
    {
        StartCoroutine(PrepareGame());
    }

    //Todo make this coroutine with sfx and visuals
    private IEnumerator PrepareGame()
    {
        if (_gameRunning)
            _backgroundMusic.Stop();

        _gameFinished = false;
        _gameRunning = false;
        _timer = 0;

        //turn off lights and play audio, use fade or actual lights
        PlayerManager.instance._screenFader.alpha = 1.0f;
        _rampenAudio.PlayOneShot(_rampenGamePrepare);

        //open gates and remove blocking collider
        foreach (GameObject go in _disableOnGameReady)
            go.SetActive(false);

        foreach (GameObject go in _enableOnGameReady)
            go.SetActive(true);

        MessHandler.instance.ReadyMess();

        yield return new WaitForSeconds(2.0f);
        _rampenAudio.PlayOneShot(_lightsOn);
        PlayerManager.instance._screenFader.alpha = 0.0f;

        onGameReady.Invoke();
    }

    //called from start button
    public void StartGame()
    {
        if (_gameRunning || _gameFinished)
            return;

        _timer = 0;
        _gameRunning = true;

        var playerCamera = PlayerManager.instance.GetComponentInChildren<Camera>();
        var confetti = playerCamera.GetComponentInChildren<ParticleSystem>(true).gameObject;

        confetti.SetActive(false);
        _backgroundMusic.Play();

        onGameStarted.Invoke();

        //should make messes interactable and put gun in bubble-mode

        //Transition idé: Du hører lyset slår seg av og det blir mørkt, latter fra ekstra-jævelen, du hører lyset slå seg på igjen og det blir lyst. Det har nå spawnet diverse rot som du må rydde 
    }

    //called from mess handler when all mess is cleaned up
    public void GameWin()
    {
        Debug.Log(this + " No mess left. Fininshing game");

        var playerCamera = PlayerManager.instance.GetComponentInChildren<Camera>();
        var confetti = playerCamera.GetComponentInChildren<ParticleSystem>(true).gameObject;

        confetti.SetActive(true);

        //give bonus points for remaining time
        ShowScore();
    }

    private void ShowScore()
    {
        _backgroundMusic.Stop();

        _gameFinished = true;
        _gameRunning = false;

        HighScoreHandler.instance.AddNewScore(_txtTimer.text);

        //should take gun out of bubble mode and tp player to leaderboard
        //show UI for quitting or restarting
    }
}
