using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Autohand;
using RenderHeads.Media.AVProVideo;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuMachine : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    [SerializeField] MeshRenderer _hologramRenderer;


    [SerializeField] Scrollbar _scrollBarHorizontal;
    [SerializeField] RectTransform _contentPanel;
    [SerializeField] PhysicsGadgetButton _btnStart;
    private Color _btnStartColor;
    [SerializeField] PhysicsGadgetButton _btnNext;
    [SerializeField] PhysicsGadgetButton _btnPrev;
    private Color _btnOtherColor;
    [SerializeField] TextMeshProUGUI _txtDebug;


    private List<RectTransform> _elements = new List<RectTransform>();
    private int _currentElement = 0;
    private bool _locked = false;

    [Space]
    [SerializeField] private float _blinkingSpeed = 4.0f;
    [SerializeField] [Range(0f, 1f)] private float _blinkingWhiteness = 1.0f;

    void Start()
    {
        _btnNext.OnPressed.AddListener(() => NextElement());
        _btnPrev.OnPressed.AddListener(() => PreviousElement());
        _btnStart.OnPressed.AddListener(() => ActivateElement());

        _btnStartColor = _btnStart.GetComponentInChildren<MeshRenderer>().material.color;
        _btnOtherColor = _btnNext.GetComponentInChildren<MeshRenderer>().material.color;

        mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
        _hologramRenderer.enabled = false;


        for (int i = 0; i < _contentPanel.childCount; i++)
        {
            var element = _contentPanel.GetChild(i).GetComponent<RectTransform>();
            if (element != null)
            {
                _elements.Add(element);
            }
        }

        SnapTo(_elements[_currentElement]);
    }


    private void ActivateElement()
    {
        if (_locked)
            return; //todo make locked grey out buttons

        Debug.Log("Activating element");
        _txtDebug.text = "Activating element";

        _elements[_currentElement].GetComponent<ModuleElement>()?.Activate();
        Lock(true);
    }

    public void Lock(bool locked)
    {
        _locked = locked;

        _btnStart.GetComponentInChildren<MeshRenderer>().material.color = !locked ? _btnStartColor : Color.Lerp(_btnStartColor, Color.grey, 0.5f);
        _btnNext.GetComponentInChildren<MeshRenderer>().material.color = !locked ? _btnOtherColor : Color.Lerp(_btnOtherColor, Color.grey, 0.5f);
        _btnPrev.GetComponentInChildren<MeshRenderer>().material.color = !locked ? _btnOtherColor : Color.Lerp(_btnOtherColor, Color.grey, 0.5f);
    }

    private void Update()
    {
        if(!_locked)
        {
            float factor = _blinkingWhiteness * (Mathf.Cos(Time.timeSinceLevelLoad * _blinkingSpeed) + 1f) / 2f;
            Color MainButtonColor = Color.Lerp(_btnStartColor, Color.grey, factor);
            Color OtherButtonColor = Color.Lerp(_btnOtherColor, Color.grey, factor);

            _btnStart.GetComponentInChildren<MeshRenderer>().material.color = MainButtonColor;
            _btnNext.GetComponentInChildren<MeshRenderer>().material.color = OtherButtonColor;
            _btnPrev.GetComponentInChildren<MeshRenderer>().material.color = OtherButtonColor;
        }
    }

    private void PreviousElement()
    {
        if (_locked)
            return;

        Debug.Log("Previous element");
        _txtDebug.text = "Previous element";

        _currentElement = Mathf.Clamp(_currentElement - 1, 0, _elements.Count - 1);
        SnapTo(_elements[_currentElement]);
    }

    private void NextElement()
    {
        if (_locked)
            return;

        Debug.Log("Next element");
        _txtDebug.text = "Next element";

        _currentElement = Mathf.Clamp(_currentElement + 1, 0, _elements.Count - 1);
        SnapTo(_elements[_currentElement]);
    }

    public void SnapTo(RectTransform target)
    {
        float scrollbarTarget = (float)_currentElement / (_elements.Count - 1);
        StartCoroutine(SnapToSmooth(target, scrollbarTarget));


        foreach(var element in _elements)
        {
            if (element == target)
            {
                element.GetComponent<ModuleElement>()?.Show();
            }
            else
            {
                element.GetComponent<ModuleElement>()?.Hide();
            }
        }
    }

    private IEnumerator SnapToSmooth(RectTransform elementTarget, float scrollbarTarget)
    {
        Lock(true);
        Canvas.ForceUpdateCanvases();
        float scrollbarStartpos = _scrollBarHorizontal.value;
        Vector2 startPos = _contentPanel.anchoredPosition;
        Vector2 targetPos = (Vector2)_scrollBarHorizontal.transform.InverseTransformPoint(_contentPanel.position)
                            - (Vector2)_scrollBarHorizontal.transform.InverseTransformPoint(elementTarget.position);
        float time = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            float smoothTransition = Mathf.SmoothStep(0, 1, elapsedTime / time);
            _contentPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, smoothTransition);
            _scrollBarHorizontal.value = Mathf.Lerp(scrollbarStartpos, scrollbarTarget, smoothTransition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _contentPanel.anchoredPosition = targetPos;
        _scrollBarHorizontal.value = scrollbarTarget;
        Lock(false);
    }


    private void OnMediaPlayerEvent(MediaPlayer arg0, MediaPlayerEvent.EventType arg1, ErrorCode arg2)
    {
        switch (arg1)
        {
            case MediaPlayerEvent.EventType.Started:
                _hologramRenderer.enabled = true;
                arg0.GetComponentInParent<CanvasGroup>().alpha = 0.9f;
                break;
           
            case MediaPlayerEvent.EventType.Closing:
                _hologramRenderer.enabled = false;
                arg0.GetComponentInParent<CanvasGroup>().alpha = 0f;
                break;
        }
    }
}
