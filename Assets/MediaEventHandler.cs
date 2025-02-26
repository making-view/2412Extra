using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class MediaEventHandler : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    // Start is called before the first frame update

    public MediaReference videoToPlay;

    bool _firstVideoDone = false;

    public static MediaEventHandler instance;

    [SerializeField] private bool _skipToQuizInEditor = true;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this);


        mediaPlayer = GetComponent<MediaPlayer>();
        mediaPlayer.Events.AddListener((mediaPlayer, eventType, errorCode) => HandledEvent(mediaPlayer, eventType, errorCode));
    }

    private void HandledEvent(MediaPlayer mediaPlayer, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
    {
        switch (eventType)
        {
            case MediaPlayerEvent.EventType.FinishedPlaying:
                StartCoroutine(OnVideoFinished());
                break;
            case MediaPlayerEvent.EventType.Started:
                OnVideoStarted(mediaPlayer);
                break;
        }
    }

    private IEnumerator OnVideoFinished()
    {
        QuizHandler quizHandler = FindObjectOfType<QuizHandler>(true);
        if(quizHandler == null)
            Debug.LogError(this + " could not find quiz handler");

        SceneTransitioner.instance.StartMovePlayerWithFade(true);
        yield return new WaitForSeconds(SceneTransitioner.instance._fadeTime / 2);
        quizHandler.StartQuiz();
    }

    private void OnVideoStarted(MediaPlayer mediaPlayer)
    {
        //make sure this isn't triggered in release builds
        if (_skipToQuizInEditor && (Debug.isDebugBuild || Application.isEditor))
        {
            double duration = mediaPlayer.Info.GetDuration();
            double goToTime = duration - 5.0;
            // Seek to nearest keyframe at end of video
            mediaPlayer.Control.SeekFast(goToTime);
        }
    }
}
