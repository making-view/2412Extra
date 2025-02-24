using Autohand;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoInstructionHandler : MonoBehaviour
{
    [SerializeField] List<CanvasGroup> _instructionCanvases = new List<CanvasGroup>();
    [SerializeField] private Transform _cameraTrans;
    [SerializeField] private float _instructionDuration = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowInstructionsAndStartVideo());

        if(_cameraTrans == null)
            _cameraTrans = PlayerManager.instance.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        Vector3 playerFacing = _cameraTrans.forward;
        playerFacing.y = 0;
        Quaternion wantedRotation = Quaternion.LookRotation(playerFacing);

        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * 2);
    }

    private void OnEnable()
    {
        if (PlayerManager.instance == null)
            return;

        //attach teleport on pressed til å vise boundary, og on release til å gjemme det
        foreach (Teleporter teleporter in PlayerManager.instance.GetComponentsInChildren<Teleporter>())
        {
            teleporter.OnStartTeleport.AddListener(() => ShowInstructions(1f));
            teleporter.OnStopTeleport.AddListener(() => ShowInstructions(0f));
            teleporter.OnTeleport.AddListener(() => ShowInstructions(0f));
        }
    }

    private void OnDisable()
    {
        if (PlayerManager.instance == null)
            return;

        Debug.Log(this + " trying to remove its listeners on disable");

        foreach (Teleporter teleporter in PlayerManager.instance.GetComponentsInChildren<Teleporter>())
        {
            teleporter.OnStartTeleport.RemoveListener(() => ShowInstructions(1f));
            teleporter.OnStopTeleport.RemoveListener(() => ShowInstructions(0f));
            teleporter.OnTeleport.RemoveListener(() => ShowInstructions(0f));
        }
    }

    private void ShowInstructions(float opacity)
    {
        foreach(CanvasGroup group in _instructionCanvases)
        {
            group.alpha = opacity;
        }
    }

    private IEnumerator ShowInstructionsAndStartVideo()
    {
        while(_instructionDuration > 0)
        {
            float opacity = Mathf.Clamp(_instructionDuration, 0f, 1f);
            ShowInstructions(opacity);
            yield return null;
            _instructionDuration -= Time.deltaTime;
        }

        ShowInstructions(0f);

        //start first video
        MediaEventHandler.instance.mediaPlayer.OpenMedia(MediaEventHandler.instance.firstVideo);
    }
}
