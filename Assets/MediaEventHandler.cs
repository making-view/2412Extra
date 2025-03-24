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
    public static MediaEventHandler instance;

    [Space]
    [SerializeField] private bool _skipToQuizInEditor = true;
    [SerializeField] private float _endOfVideoOffset = 10f;

    private bool _shouldExitOnLeaveArea = false;

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

        var exitVolume = FindObjectOfType<DoOnPlayerEnterVolume>();
        exitVolume.onPlayerExit.AddListener(() => PlayerExitedArea(exitVolume));
        //add area exitet listener

        var exitUI = FindObjectOfType<VideoInstructionHandler>();
        if (exitUI != null)
            exitUI.SetInstructionText("Teleporter ut for å gå rett til quiz");
    }

    private void PlayerExitedArea(DoOnPlayerEnterVolume area)
    {
        if (_shouldExitOnLeaveArea)
        {
            SceneTransitioner.instance.StartTransitionToScene("EXTRA_Interior");
            return;
        }
        
        _shouldExitOnLeaveArea= true;

        var exitUI = FindObjectOfType<VideoInstructionHandler>();
        if(exitUI != null)
            exitUI.SetInstructionText("Teleporter ut for å avslutte video");

        SkipToEndOfVideo(mediaPlayer);
        SceneTransitioner.instance.StartMovePlayerWithFade(false);
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
        _shouldExitOnLeaveArea = true;

        var exitUI = FindObjectOfType<VideoInstructionHandler>();
        if (exitUI != null)
            exitUI.SetInstructionText("Teleporter ut for å avslutte video");

        yield return new WaitForSeconds(SceneTransitioner.instance._fadeTime / 2);
        quizHandler.StartQuiz();
    }

    private void OnVideoStarted(MediaPlayer mediaPlayer)
    {
        //make sure this isn't triggered in release builds
        if (_skipToQuizInEditor && (Debug.isDebugBuild || Application.isEditor))
        {
            SkipToEndOfVideo(mediaPlayer);
        }
    }

    //TODO call this on exiting area + reposition if we're not already in quiz mode
    private void SkipToEndOfVideo(MediaPlayer mediaPlayer)
    {
        double duration = mediaPlayer.Info.GetDuration();
        double goToTime = duration - _endOfVideoOffset;
        // Seek to nearest keyframe at end of video
        mediaPlayer.Control.Seek(goToTime);
    }
}
