using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

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
        //should count down time and update 
    }

    //called from start button
    private void StartGame()
    {
        //should make messes interactable and put gun in bubble-mode
    }

    //called from mess handler when all mess is cleaned up
    public void FinishGame()
    {
        //should take gun out of bubble mode and tp player to leaderboard
        //show UI for quitting or restarting
    }
}
