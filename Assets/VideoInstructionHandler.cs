using Autohand;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoInstructionHandler : MonoBehaviour
{
    [SerializeField] List<CanvasGroup> _instructionCanvases = new List<CanvasGroup>();
    private float _instructionDuration = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowInstructionsAndStartVideo());
    }

    private void OnEnable()
    {
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

        //start first video
        MediaEventHandler.instance.mediaPlayer.OpenMedia(MediaEventHandler.instance.firstVideo);
    }
}
