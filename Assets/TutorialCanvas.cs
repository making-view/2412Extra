using Autohand;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialCanvas : MonoBehaviour
{
    public enum TutorialType
    {
        none,
        teleportButton,
        grabGun,
        useGun
    }

    public UnityEvent onTutorialCompleted = new UnityEvent();

    //Type decides when to hide the tutorial. E.g teleport tutorial is hidden after teleport
    [SerializeField] private TutorialType _type;

    List<CanvasGroup> _instructionCanvases = new List<CanvasGroup>();
    private Transform _cameraTrans;
    [SerializeField] private float _fadeDuration = 1.0f;
    [SerializeField] private float _invokeDelay = 0.0f;

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

        //TODO de-initialize from teleporter etc when scene loads?
    }

    public TutorialType GetTutorialType()
    { return _type; }

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
        //check playerprefs if tutorial was completed without device pausing

        switch (_type)
        {
            case TutorialType.none:
                break;
            case TutorialType.teleportButton:
                if (PlayerPrefs.GetInt("TeleportTutorialDone", 0) == 1) //don't activate self if tp tutorial already done
                {
                    gameObject.SetActive(false);
                    return;
                }
                break;
            case TutorialType.grabGun:
                if (PlayerPrefs.GetInt("GunTutorialsDone", 0) == 1) //don't activate self if gun tutorial already done
                {
                    gameObject.SetActive(false);
                    return;
                }
                break;
            case TutorialType.useGun:
                if (PlayerPrefs.GetInt("GunTutorialsDone", 0) == 1) //don't activate self if gun tutorial already done
                {
                    gameObject.SetActive(false);
                    return;
                }
                break;
        }

        SetOpacity(1.0f);
        _completed = false;

        if (_initialized) //make sure we don't initialize bindings twice
            return;


                            //TODO, initialize it so thate the done value is set regardless of this gameobject being active
        switch (_type)
        {
            case TutorialType.none:
                break;
            case TutorialType.teleportButton:
                foreach (Teleporter teleporter in PlayerManager.instance.GetComponentsInChildren<Teleporter>())
                {
                    teleporter.OnTeleport.AddListener(() => PlayerPrefs.SetInt("TeleportTutorialDone", 1));
                    teleporter.OnTeleport.AddListener(() => CompleteTutorial());
                }
                break;
            case TutorialType.useGun:
                GameHandler.instance.onGameStarted.AddListener(() => PlayerPrefs.SetInt("GunTutorialsDone", 1));
                GameHandler.instance.onGameStarted.AddListener(() => CompleteTutorial());
                break;
            case TutorialType.grabGun:
                foreach (GrabbableExtraEvents extraEvents in FindObjectOfType<Watergun>().GetComponentsInChildren<GrabbableExtraEvents>())
                    extraEvents.OnFirstGrab.AddListener((hand, grabbable) => CompleteTutorial());
                break;
        }


        _initialized = true;
    }

    private void OnDisable()
    {
        //TODO, this is not needed. Remove this and make sure we're only initialized once


        //switch (_type)
        //{
        //    case TutorialType.none:
        //        break;
        //    case TutorialType.teleportButton:
        //        foreach (Teleporter teleporter in PlayerManager.instance.GetComponentsInChildren<Teleporter>())
        //            teleporter.OnTeleport.RemoveListener(() => CompleteTutorial());
        //        break;
        //    case TutorialType.useGun:
        //        GameHandler.instance.onGameStarted.RemoveListener(() => CompleteTutorial());
        //        break;
        //    case TutorialType.grabGun:
        //        foreach (GrabbableExtraEvents extraEvents in FindObjectOfType<Watergun>().GetComponentsInChildren<GrabbableExtraEvents>())
        //            extraEvents.OnFirstGrab.RemoveListener((hand, grabbable) => CompleteTutorial());
        //        break;
        //}

        //_initialized = false;
    }

    private void OnDestroy()
    {
        switch (_type)
        {
            case TutorialType.none:
                break;
            case TutorialType.teleportButton:
                foreach (Teleporter teleporter in PlayerManager.instance.GetComponentsInChildren<Teleporter>())
                {
                    teleporter.OnTeleport.RemoveListener(() => PlayerPrefs.SetInt("TeleportTutorialDone", 1));
                    teleporter.OnTeleport.RemoveListener(() => CompleteTutorial());
                }
                break;
            case TutorialType.useGun:
                GameHandler.instance.onGameStarted.RemoveListener(() => PlayerPrefs.SetInt("GunTutorialsDone", 1));
                GameHandler.instance.onGameStarted.RemoveListener(() => CompleteTutorial());
                break;
            case TutorialType.grabGun:
                foreach (GrabbableExtraEvents extraEvents in FindObjectOfType<Watergun>().GetComponentsInChildren<GrabbableExtraEvents>())
                    extraEvents.OnFirstGrab.RemoveListener((hand, grabbable) => CompleteTutorial());
                break;
        }
    }

    private void CompleteTutorial()
    {
        if (_completed) return;

        _completed = true;
        StartCoroutine(FadeAway());

        //moved this to the listener so that it can be completed without the tutorial being active
        ////mark tutorial as done so it won't be reloaded until application has been paused
        //switch (_type)
        //{
        //    case TutorialType.none:
        //        break;
        //    case TutorialType.teleportButton:
        //        PlayerPrefs.SetInt("TeleportTutorialDone", 1);
        //        break;
        //    case TutorialType.useGun:
        //        PlayerPrefs.SetInt("GunTutorialsDone", 1);
        //        break;
        //}
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

        yield return new WaitForSeconds(_invokeDelay);

        onTutorialCompleted.Invoke();
        gameObject.SetActive(false);
    }
}