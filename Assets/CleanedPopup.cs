using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CleanedPopup : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _txtTag;
    [SerializeField] private TextMeshProUGUI _txtNum;

    [Space]
    [SerializeField] private Sprite _imgRenhold;
    [SerializeField] private Sprite _imgKvalitet;
    [SerializeField] private Sprite _imgPrisOgVare;
    [SerializeField] private Sprite _imgExtrabonus;

    [Space]
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private float _lifeTime = 2.0f;
    private float _timer = 0f;
    private Camera _camera;
    private CanvasGroup _canvasGroup;

    public void SetValues(string tag, string num)
    {
        _camera = PlayerManager.instance.GetComponentInChildren<Camera>();
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _txtTag.text = tag;
        _txtNum.text = num;

        switch (tag.ToLower())
        {
            case "renhold":
                _icon.sprite = _imgRenhold;
                break;

            case "kvalitet":
                _icon.sprite = _imgKvalitet;
                break;

            case "prisogvare":
                _txtTag.text = "Pris og Vare"; //override tag with spacing
                _icon.sprite = _imgPrisOgVare;
                break;

            case "extrabonus":
                _icon.sprite = _imgExtrabonus;
                break;
        }
    }

    private void Start()
    {
        _camera = PlayerManager.instance.GetComponentInChildren<Camera>();
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        transform.localScale = Vector3.one * _animCurve.Evaluate(_timer);
        transform.LookAt(_camera.transform);
        _canvasGroup.alpha = 1 - (_timer / _lifeTime);
        //face the player
        //delete self at end of animation

        if (_timer > _lifeTime)
            Destroy(this);
    }
}
