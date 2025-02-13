using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    private bool _gameRunning = false;
    private bool _gameFinished = false;

    private float _timer = 0;
    [SerializeField] private float _timeLimit = 120f;
    
    //TODO make into list with timers?
    [SerializeField] private TextMeshProUGUI _txtTimer;


    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;


        //should set up interactions for starting stopping, UI, score etc.
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameRunning)
            return;

        _timer += Time.deltaTime;
        _txtTimer.text = _timer.ToString("00:00:00");

        //if (_timer > _timeLimit)
        //    GameOver();
    }

    private void GameOver()
    {
        Debug.Log(this + " No time left. Fininshing game");

        //subtract points for missing elements
        ShowScore();
    }

    //called from start button
    public void StartGame()
    {
        if (_gameRunning || _gameFinished)
            return;

        _timer = 0;
        _gameRunning = true;

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
        _gameFinished = true;
        _gameRunning = false;
        //should take gun out of bubble mode and tp player to leaderboard
        //show UI for quitting or restarting
    }
}
