using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModuleElement : MonoBehaviour
{
    [SerializeField] Image _thumbnail;
    [SerializeField] GameObject _graphicRoot;
    [SerializeField] MediaReference _previewVideo;

    public UnityEvent onActivated = new UnityEvent();


    public void OnHovered()
    {
        Debug.Log("Hovered on " + gameObject.name);
        _thumbnail.color = Color.white;
    }

    public void Hide()
    {
        Debug.Log("Hiding " + gameObject.name);
        StartCoroutine(ShowCoroutine(false));
    }

    public void Show()
    {
        Debug.Log("Showing " + gameObject.name);
        StartCoroutine(ShowCoroutine(true));
    }

    public void Activate()
    {
        Debug.Log("Activating " + gameObject.name);
        onActivated.Invoke();
    }

    private IEnumerator ShowCoroutine(bool show)
    {
        if (show)
            GetComponentInParent<MenuMachine>().mediaPlayer.CloseMedia();

        Vector3 startingScale = _graphicRoot.transform.localScale;
        Color startingColor = _thumbnail.color;

        Vector3 targetScale = Vector3.one;
        Color targetColor = Color.white;

        if (!show)
        {
            targetScale = Vector3.one * 0.8f;
            targetColor = Color.Lerp(Color.white, Color.grey, 0.5f);
        }

        float time = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            float smoothTransition = Mathf.SmoothStep(0, 1, elapsedTime / time);
            _graphicRoot.transform.localScale = Vector3.Lerp(startingScale, targetScale, smoothTransition);
            _thumbnail.color = Color.Lerp(startingColor, targetColor, smoothTransition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if(show)
            GetComponentInParent<MenuMachine>().mediaPlayer.OpenMedia(_previewVideo);

        _graphicRoot.transform.localScale = targetScale;
        _thumbnail.color = targetColor;
    }


}
