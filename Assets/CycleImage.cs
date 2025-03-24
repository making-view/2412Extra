
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CycleImage : MonoBehaviour
{
    [Header("Standard timing system")]
    [SerializeField] private float timeBetweenLoops = 1.5f;
    [SerializeField] private float timeBetweenFrames = 0.4f;
    [SerializeField] private float loopPause = 0.4f;

    [Space]
    [Header("Alternate timing system (0 = use default)")]
    [SerializeField] private float alternateExactLoopTime = 0f;

    [Space]
    [Header("Images")]
    private Image _myImage;
    [SerializeField] private Sprite[] _sprites;

    int currentIndex = 0;
    private bool _moveForward = true;

    private Coroutine _currentLoop;

    private void OnEnable()
    {
        StopAllCoroutines();
        _myImage = this.GetComponent<Image>();


        if (alternateExactLoopTime == 0f)
        {
            _currentLoop = StartCoroutine(StartNextLoop());
        }
        else
        {
            _currentLoop = StartCoroutine(ExactLoopTime());
        }
    }

    private IEnumerator ExactLoopTime()
    {
        float timePrSprite = alternateExactLoopTime / _sprites.Length;
        float time = 0f;

        while(true)
        {
            float timeInLoop = time % alternateExactLoopTime;
            int spriteNum =  Mathf.FloorToInt(timeInLoop / timePrSprite);
            _myImage.overrideSprite = _sprites[spriteNum];

            yield return null;
            time += Time.deltaTime;
        }
    }

    private IEnumerator StartNextLoop()
    {
        yield return new WaitForSeconds(timeBetweenLoops);
        StartCoroutine(AnimateSpriteArray());
    }

    private IEnumerator AnimateSpriteArray()
    {
        currentIndex = 0;

        while (currentIndex < _sprites.Length)
        {
            yield return new WaitForSeconds(timeBetweenFrames);
            _myImage.overrideSprite = _sprites[currentIndex];
            currentIndex += 1;
        }

        yield return new WaitForSeconds(loopPause);

        currentIndex -= 1;

        while (currentIndex > -1)
        {
            yield return new WaitForSeconds(timeBetweenFrames);
            _myImage.overrideSprite = _sprites[currentIndex];
            currentIndex -= 1;
        }

        _moveForward = !_moveForward;

        StartCoroutine(StartNextLoop());
    }
}

