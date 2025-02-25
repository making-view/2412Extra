using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MediaEventHandler : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    // Start is called before the first frame update

    public MediaReference videoToPlay;

    bool _firstVideoDone = false;

    public static MediaEventHandler instance;

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
                OnVideoFinished();
                break;
        }
    }

    private void OnVideoFinished()
    {
        //TODO start quiz
    }
}
