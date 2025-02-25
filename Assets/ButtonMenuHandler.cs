using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenuHandler : MonoBehaviour
{
    [SerializeField] PhysicsGadgetButton _btnPlayGame;
    [SerializeField] PhysicsGadgetButton _btnPlayVideoRampen;
    [SerializeField] PhysicsGadgetButton _btnPlayVideoEngelen;


    // Start is called before the first frame update
    void Start()
    {
        if(_btnPlayVideoEngelen == null || _btnPlayGame == null || _btnPlayVideoRampen == null)
        {
            Debug.LogError(this + " is missing buttons");
            return;
        }

        _btnPlayGame.OnPressed.AddListener(() => StartGame()); //lock other buttons?
        _btnPlayVideoRampen.OnPressed.AddListener(() => StartVideoRampen()); //lock other buttons?
        _btnPlayVideoEngelen.OnPressed.AddListener(() => StartVideoEngelen()); //lock other buttons?
    }


    private void StartIntro()
    {
        Debug.Log("Starting intro");
        //SceneTransitioner.instance.StartTransitionToScene("EXTRA_Tutorial");
    }

    private void StartGame()
    {
        Debug.Log("Starting game");
        //SceneTransitioner.instance.StartTransitionToScene("EXTRA_Interior");
        GameHandler.instance.StartPreparingGame();
    }

    private void StartVideoRampen()
    {
        Debug.Log("Starting video rampen");
        SceneTransitioner.instance.StartTransitionToScene("EXTRA_Videoplayer_Rampen");
    }

    private void StartVideoEngelen()
    {
        Debug.Log("Starting video engelen");
        SceneTransitioner.instance.StartTransitionToScene("EXTRA_Videoplayer_Engelen");
    }
}
