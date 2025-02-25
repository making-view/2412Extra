using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizHandler : MonoBehaviour
{
    [Serializable]
    public class QuizQuestion
    {
        public string name;
        public List<string> questionAlternatives = new List<string>();
        public int correctAnswerIndex = 0;
    }

    [SerializeField] PhysicsGadgetButton _btnFirst;
    [SerializeField] PhysicsGadgetButton _btnSecond;
    [SerializeField] PhysicsGadgetButton _btnThird;

    [Space]
    [SerializeField] List<QuizQuestion> questions = new List<QuizQuestion>();

    void Start()
    {
        //setup quiz lists and hide quiz.
        //subscribe to video done event to spawn quiz
        //Add quiz ambient music 
    }

}
