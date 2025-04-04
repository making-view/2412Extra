using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static MessHandler;
using Random = System.Random;


public class MessHandler : MonoBehaviour
{
    [Serializable]
    public class MessList
    {
        public string tag;
        public int numToClean = 3;
        public List<Mess> allMess = new List<Mess>();
        public List<Mess> remainingMess = new List<Mess>();
        [SerializeField] public TextMeshProUGUI infoTxt;
    }

    public UnityEvent onMessCleaned = new UnityEvent();

    [SerializeField] GameObject _messCleanedNotification;

    [SerializeField] private List<MessList> _messCategories = new List<MessList>();
    public static MessHandler instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        HideMessOnStart();
        //ReadyMess();
    }

    private void HideMessOnStart()
    {
        foreach (MessList messList in _messCategories)
        {
            List<Mess> messes = FindObjectsOfType<Mess>(true).ToList().Where((x) => x.tag.Equals(messList.tag)).ToList();
            foreach ( Mess  mess in messes)
                mess.EnableMess(false);
        }
    }

    //Readies a random subset off mess and opens the gate
    public void ReadyMess()
    {
        foreach (MessList messList in _messCategories)
        {
            //add all messes in scene that matches tag of messList
            messList.allMess.Clear();
            messList.remainingMess.Clear();

            messList.allMess.AddRange(FindObjectsOfType<Mess>(true).ToList().Where((x) => x.tag.Equals(messList.tag)));

            if (messList.numToClean > messList.allMess.Count)
            {
                Debug.LogError(this + " Not enough mess with tag " + messList.tag + " to clean " + messList.numToClean + " items");
                return;
            }

            Debug.Log(this + " Shuffling messes with tag " + messList.tag + " and enabling " + messList.numToClean + " items");
            messList.allMess = ShuffleMessList(messList.allMess);

            for (int i = 0; i < messList.allMess.Count; i++)
            {
                Mess mess = messList.allMess[i];

                bool enableMesss = i < messList.numToClean;
                mess.EnableMess(enableMesss);

                if (enableMesss)
                    messList.remainingMess.Add(mess);
            }

            string numberText = (messList.numToClean - messList.remainingMess.Count) + " / " + messList.numToClean;

            if (messList.infoTxt != null)
                messList.infoTxt.text = numberText;
        }
    }


    public void MessCleaned(Mess mess)
    {
        foreach (MessList messList in _messCategories)
        {
            if(messList.remainingMess.Contains(mess))
            {
                messList.remainingMess.Remove(mess);

                //spawn particle effect with correct counter UI on finished mess
                GameObject notification = Instantiate(_messCleanedNotification, this.transform);
                
                Vector3 cameraPosition = PlayerManager.instance.GetComponentInChildren<Camera>().transform.position;
                Vector3 popupPosition = mess._popupPosition.transform.position;

                //move popup slightly towards camera
                popupPosition = Vector3.MoveTowards(popupPosition, cameraPosition, 0.2f);
                

                notification.transform.position = popupPosition;
                string numberText = (messList.numToClean - messList.remainingMess.Count) + " / " + messList.numToClean;
                notification.GetComponent<CleanedPopup>().SetValues(messList.tag, numberText);

                Debug.Log(this + " removing mess " + mess + " from " + messList.tag + " -list");
                Debug.Log(this + " with notification " + numberText);

                if(messList.infoTxt != null)
                    messList.infoTxt.text = numberText;

                break;
            }
        }


            //check if no mess left = call for game handler to complete
        int messLeft = 0;

        foreach(MessList messList in _messCategories)
            messLeft += messList.remainingMess.Count;

        if (messLeft == 0)
            GameHandler.instance.GameWin();
        else
            onMessCleaned.Invoke();
    }

    public List<Mess> ShuffleMessList(List<Mess> list)
    {
        Random random = new Random();
        return list.OrderBy(x => random.Next()).ToList();
    }

    internal Transform GetClosestMess()
    {
        Transform closestMess = null;
        float closestDistance = float.MaxValue;
        Vector3 playerPosition = PlayerManager.instance._screenFader.transform.position;

        foreach (MessList messList in _messCategories)
        {
            foreach(Mess mess in messList.remainingMess)
            {
                float distance;
                
                if (mess._popupPosition != null)
                    distance = Vector3.Distance(mess._popupPosition.position, playerPosition);
                else
                    distance = Vector3.Distance(mess._messTransform.position, playerPosition);

                if (distance < closestDistance)
                {
                    closestDistance = distance;

                    if (mess._popupPosition != null)
                        closestMess = mess._popupPosition;
                    else
                        closestMess = mess._messTransform;

                }
            }
        }

        return closestMess;
    }
}
