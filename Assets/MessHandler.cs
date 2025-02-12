using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MessHandler : MonoBehaviour
{
    [Serializable]
    public class MessList
    {
        public string name;
        public int numToClean = 3;
        public List<Mess> allMess = new List<Mess>();
        [HideInInspector] public List<Mess> remainingMess = new List<Mess>();
    }

    [SerializeField] private List<MessList> _messCategories = new List<MessList>();
    public static MessHandler instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;


        //activate a random subset of messes for each category
    }

    public void MessCleaned(Mess mess)
    {
        //remove mess from its list
        //spawn particle effect with correct counter UI on finished mess
       
        
        //check if no mess left = call for game handler to complete
        int messLeft = 0;
        foreach(MessList messList in _messCategories)
            messLeft += messList.remainingMess.Count;

        if (messLeft == 0)
            GameHandler.instance.FinishGame();
    }
}
