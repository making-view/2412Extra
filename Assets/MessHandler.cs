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

    [SerializeField] private List<MessList> messCategories = new List<MessList>();

    // Start is called before the first frame update
    void Start()
    {
        //activate a random subset of messes for each category
    }

    public void MessCleaned(Mess mess)
    {
        //remove mess from its list
        //spawn particle effect with counter on mess
        //check if no mess left = call for game handler to complete
    }
}
