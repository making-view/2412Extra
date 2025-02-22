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

    public MediaReference firstVideo;
    public MediaReference secondVideo;

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
            case MediaPlayerEvent.EventType.MetaDataReady:
                break;
            case MediaPlayerEvent.EventType.ReadyToPlay:
                break;
            case MediaPlayerEvent.EventType.Started:
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                OnVideoFinished();
                break;
            case MediaPlayerEvent.EventType.Closing:
                break;
            case MediaPlayerEvent.EventType.Error:
                break;
            case MediaPlayerEvent.EventType.SubtitleChange:
                break;
            case MediaPlayerEvent.EventType.Stalled:
                break;
            case MediaPlayerEvent.EventType.Unstalled:
                break;
            case MediaPlayerEvent.EventType.ResolutionChanged:
                break;
            case MediaPlayerEvent.EventType.StartedSeeking:
                break;
            case MediaPlayerEvent.EventType.FinishedSeeking:
                break;
            case MediaPlayerEvent.EventType.StartedBuffering:
                break;
            case MediaPlayerEvent.EventType.FinishedBuffering:
                break;
            case MediaPlayerEvent.EventType.PropertiesChanged:
                break;
            case MediaPlayerEvent.EventType.PlaylistItemChanged:
                break;
            case MediaPlayerEvent.EventType.PlaylistFinished:
                break;
            case MediaPlayerEvent.EventType.TextTracksChanged:
                break;
            case MediaPlayerEvent.EventType.Paused:
                break;
            case MediaPlayerEvent.EventType.Unpaused:
                break;
        }
    }

    private void OnVideoFinished()
    {
        if (!_firstVideoDone)
        {
            _firstVideoDone = true;
            //Do quiz here?
            //Play other video
            mediaPlayer.OpenMedia(secondVideo.MediaPath);
        }
        else
        {
            //do second quiz
            //then go back to menu
            SceneTransitioner.instance.StartTransitionToScene("EXTRA_Interior");
        }

    }
}
