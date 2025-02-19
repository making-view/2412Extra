using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenuHandler : MonoBehaviour
{
    [SerializeField] PhysicsGadgetButton _btnPlayIntro;
    [SerializeField] PhysicsGadgetButton _btnPlayGame;
    [SerializeField] PhysicsGadgetButton _btnPlayVideo;


    // Start is called before the first frame update
    void Start()
    {
        if(_btnPlayIntro == null || _btnPlayGame == null || _btnPlayVideo == null)
        {
            Debug.LogError(this + " is missing buttons");
            return;
        }

        _btnPlayIntro.OnPressed.AddListener(() => StartIntro()); //lock other buttons?
        _btnPlayGame.OnPressed.AddListener(() => StartGame()); //lock other buttons?
        _btnPlayVideo.OnPressed.AddListener(() => StartVideo()); //lock other buttons?
    }


    private void StartIntro()
    {
        Debug.Log("Starting intro");
    }

    private void StartGame()
    {
        Debug.Log("Starting game");
        SceneTransitioner.instance.StartTransitionToScene("EXTRA_Interior");
    }

    private void StartVideo()
    {
        Debug.Log("Starting video");
    }
}
