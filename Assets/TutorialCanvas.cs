using Autohand;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialCanvas : MonoBehaviour
{
    private enum TutorialType
    {
        none,
        teleportButton,
        useGun
    }

    //Type decides when to hide the tutorial. E.g teleport tutorial is hidden after teleport
    [SerializeField] private TutorialType _type;

    List<CanvasGroup> _instructionCanvases = new List<CanvasGroup>();
    private Transform _cameraTrans;
    [SerializeField] private float _fadeDuration = 1.0f;

    [SerializeField] private bool _followPlayerRotation = false;
    [SerializeField] private bool _followPlayerPosition = false;
    private bool _completed;
    private bool _initialized;

    // Start is called before the first frame update
    void Start()
    {
        if (_cameraTrans == null)
            _cameraTrans = PlayerManager.instance.GetComponentInChildren<Camera>().transform;

        _instructionCanvases = GetComponentsInChildren<CanvasGroup>().ToList();

        Initialize();
    }

    private void Update()
    {
        if(_followPlayerPosition)
        {
            transform.position = _cameraTrans.position;
        }

        if(_followPlayerRotation)
        {
            Vector3 playerFacing = _cameraTrans.forward;
            playerFacing.y = 0;
            Quaternion wantedRotation = Quaternion.LookRotation(playerFacing);
            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * 2);
        }
    }

    private void OnEnable()
    {
        if (PlayerManager.instance == null)
            return;

        Initialize();
    }

    private void Initialize()
    {
        if (_initialized)
            return;
        
        _completed = false;

        switch (_type)
        {
            case TutorialType.none:
                break;
            case TutorialType.teleportButton:
                foreach (Teleporter teleporter in PlayerManager.instance.GetComponentsInChildren<Teleporter>())
                    teleporter.OnTeleport.AddListener(() => CompleteTutorial());
                break;
            case TutorialType.useGun:
                break;
        }

        _initialized = true;
    }

    private void OnDisable()
    {
        if (PlayerManager.instance == null)
            return;

        switch (_type)
        {
            case TutorialType.none:
                break;
            case TutorialType.teleportButton:
                foreach (Teleporter teleporter in PlayerManager.instance.GetComponentsInChildren<Teleporter>())
                    teleporter.OnTeleport.AddListener(() => CompleteTutorial());
                break;
            case TutorialType.useGun:
                break;
        }
    }

    private void CompleteTutorial()
    {
        if(_completed) return;

        _completed = true;
        StartCoroutine(FadeAway());
    }

    private void SetOpacity(float opacity)
    {
        foreach (CanvasGroup group in _instructionCanvases)
        {
            group.alpha = opacity;
        }
    }

    private IEnumerator FadeAway()
    {
        while (_fadeDuration > 0)
        {
            float opacity = Mathf.Clamp(_fadeDuration, 0f, 1f);
            SetOpacity(opacity);
            yield return null;
            _fadeDuration -= Time.deltaTime;
        }

        SetOpacity(0f);
        //stop videoplayer 
        var mediaplayer = GetComponentInChildren<MediaPlayer>();
        if (mediaplayer != null)
            mediaplayer.Stop();

        gameObject.SetActive(false);
    }
}
