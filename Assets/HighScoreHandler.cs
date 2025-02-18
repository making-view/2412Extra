using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreHandler : MonoBehaviour
{
    [SerializeField] GameObject _entryPrefab;
    List<HighScoreEntry> _entries = new List<HighScoreEntry>();

    public static HighScoreHandler instance;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //load scores
        int index = 0;
        while (true)
        {
            string playerName = PlayerPrefs.GetString("playerName" + index);
            string playerScore = PlayerPrefs.GetString("playerScore" + index);

            if (playerName != null && playerName.Length > 0)
            {
                index++;
                AddEntry(playerName, playerScore);
            }
            else
                break;
        }

        SortScores();
    }

    private void SortScores()
    {
        _entries = _entries.OrderBy(x => x._scoreText.text).ToList();

        for(int i = 0; i < _entries.Count; i++) 
        {
            _entries[i].transform.SetSiblingIndex(i);
        }
    }


    public void AddEntry(string playerName, string playerScore)
    {
        GameObject go = GameObject.Instantiate(_entryPrefab, transform);
        HighScoreEntry entry = go.GetComponent<HighScoreEntry>();
        entry._nameText.text = playerName;
        entry._scoreText.text = playerScore;
        _entries.Add(entry);

        SortScores();
        StartCoroutine(HighlightEntry(entry));
    }

    public void CreateEntry(string playerScore)
    {
        GameObject go = GameObject.Instantiate(_entryPrefab, transform);
        HighScoreEntry entry = go.GetComponent<HighScoreEntry>();
        
        int employeeNumber = _entries.Count + 1;
        
        entry._nameText.text = "Ansatt #" + employeeNumber;
        entry._scoreText.text = playerScore;
        _entries.Add(entry);

        SortScores();
        StartCoroutine(HighlightEntry(entry));
    }

    private void OnApplicationQuit()
    {
        //save scores
        for(int i = 0; i < _entries.Count; i++)
        {
            HighScoreEntry entry = _entries[i];
            PlayerPrefs.SetString("playerName" + i, entry._nameText.text);
            PlayerPrefs.SetString("playerScore" + i, entry._scoreText.text);
        }
    }

    private IEnumerator HighlightEntry(HighScoreEntry entry)
    {
        float timer = 2.0f;
        Image image = entry.GetComponentInChildren<Image>();

        Color startColor = image.color;
        Color toColor = Color.Lerp(startColor, Color.white, 0.5f);

        while (timer > 0.0f)
        {
            float lerp = (Mathf.Sin(timer * Mathf.PI * 4) + 1.0f) / 2.0f;
            image.color = Color.Lerp(startColor, toColor, lerp);

            timer -= Time.deltaTime;
            yield return null;
        }

        image.color = startColor;
    }
}
