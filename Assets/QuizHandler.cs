using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    List<ParticleSystem> _celebration = new List<ParticleSystem>();
    [SerializeField] GameObject _tablet = null;
    [SerializeField] Color _colorCorrect = Color.green;
    [SerializeField] Color _colorWrong = Color.red;
    [SerializeField] Color _colorInactive = Color.white;
    [SerializeField] Color _colorWaiting = Color.yellow;

    [Space]
    [SerializeField] PhysicsGadgetButton _btnFirst;
    private MeshRenderer _rendererButtonFirst;
    [SerializeField] PhysicsGadgetButton _btnSecond;
    private MeshRenderer _rendererButtonSecond;
    [SerializeField] PhysicsGadgetButton _btnThird;
    private MeshRenderer _rendererButtonThird;

    [Space]
    [SerializeField] private TextMeshProUGUI _txtQuestion;
    [SerializeField] private TextMeshProUGUI _txtFirst;
    [SerializeField] private TextMeshProUGUI _txtSecond;
    [SerializeField] private TextMeshProUGUI _txtThird;

    [Space]
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioCelebrate;
    [SerializeField] private AudioClip _audioWrong;

    [Space]
    [SerializeField] List<QuizQuestion> questions = new List<QuizQuestion>();
    [SerializeField] int _currentQuestion = 0;
    [SerializeField] bool _buttonsLocked = false;

    void Start()
    {
        _rendererButtonFirst = _btnFirst.GetComponentsInChildren<MeshRenderer>().Where((x) => x.gameObject.name.Contains("Button")).First();
        _rendererButtonSecond = _btnSecond.GetComponentsInChildren<MeshRenderer>().Where((x) => x.gameObject.name.Contains("Button")).First();
        _rendererButtonThird = _btnThird.GetComponentsInChildren<MeshRenderer>().Where((x) => x.gameObject.name.Contains("Button")).First();
        _audioSource = GetComponent<AudioSource>();
        _celebration = GetComponentsInChildren<ParticleSystem>(true).ToList();

        _btnFirst.OnPressed.AddListener(() => TryLockAnswer(1, _rendererButtonFirst));
        _btnSecond.OnPressed.AddListener(() => TryLockAnswer(2, _rendererButtonSecond));
        _btnThird.OnPressed.AddListener(() => TryLockAnswer(3, _rendererButtonThird));

        if (_tablet == null)
            _tablet = transform.GetChild(0).gameObject;

        _tablet.SetActive(false);
    }

    public void StartQuiz()
    {
        _tablet.SetActive(true);
        StartCoroutine(PrepareQuestion());
    }

    private IEnumerator PrepareQuestion()
    {
        QuizQuestion question = questions[_currentQuestion];

        _txtQuestion.text = question.name;
        _txtFirst.text = question.questionAlternatives[0];
        _txtSecond.text = question.questionAlternatives[1];
        _txtThird.text = question.questionAlternatives[2];

        //clear old UI
        //update UI gradually, question texts and header text

        yield return null;
        //unlock interactions, prepare buttons
        LockButtons(false);
    }

    private void TryLockAnswer(int answerIndex, MeshRenderer buttonRenderer)
    {
        if (_buttonsLocked)
            return;

        LockButtons(true);
        StartCoroutine(LockAnswer(answerIndex, buttonRenderer));
    }

    private void LockButtons(bool lockButtons)
    {
        _buttonsLocked = lockButtons;

        if(lockButtons)
        {
            _rendererButtonFirst.material.SetColor("_Color", _colorInactive);
            _rendererButtonSecond.material.SetColor("_Color", _colorInactive);
            _rendererButtonThird.material.SetColor("_Color", _colorInactive);
        }
        else
        {
            _rendererButtonFirst.material.SetColor("_Color", _colorWaiting);
            _rendererButtonSecond.material.SetColor("_Color", _colorWaiting);
            _rendererButtonThird.material.SetColor("_Color", _colorWaiting);
        }
    }

    private IEnumerator LockAnswer(int answerIndex, MeshRenderer buttonRenderer)
    {
        QuizQuestion question = questions[_currentQuestion];
        bool correct = answerIndex == question.correctAnswerIndex;
 
        yield return StartCoroutine(HighlightButton(buttonRenderer));

        if(correct)
        {
            buttonRenderer.material.SetColor("_Color", _colorCorrect);
            _audioSource.PlayOneShot(_audioCelebrate);

            foreach (ParticleSystem particle in _celebration)
                particle.Play();
        }
        else
        {
            buttonRenderer.material.SetColor("_Color", _colorWrong);
            _audioSource.PlayOneShot(_audioWrong);
        }

        yield return new WaitForSeconds(1);
        _currentQuestion++;

        //go to next question or finish quiz
        if (_currentQuestion < questions.Count)
            StartCoroutine(PrepareQuestion());
        else
            StartCoroutine(FinishQuiz());

    }

    private IEnumerator FinishQuiz()
    {
        _txtQuestion.text = "Quiz fullført";
        _txtFirst.text =  " ";
        _txtSecond.text = " ";
        _txtThird.text = " ";

        LockButtons(true);

        yield return new WaitForSeconds(5); //duration of end screen
        SceneTransitioner.instance.StartTransitionToScene("EXTRA_Interior");
    }

    private IEnumerator HighlightButton(MeshRenderer buttonRenderer)
    {
        float timer = 2.0f;
        Material material = buttonRenderer.material;

        Color startColor = _colorWaiting;
        Color toColor = _colorInactive;

        while (timer > 0.0f)
        {
            float lerp = (Mathf.Sin(timer * Mathf.PI * 4) + 1.0f) / 2.0f;

            material.SetColor("_Color", Color.Lerp(startColor, toColor, lerp));

            timer -= Time.deltaTime;
            yield return null;
        }

        material.SetColor("_Color", toColor);
    }
}
