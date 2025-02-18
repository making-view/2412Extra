using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreHandler : MonoBehaviour
{
    private List<string> months = new List<string>
    {
        "Januar",
        "Februar",
        "Mars",
        "April",
        "Mai",
        "Juni",
        "Juli",
        "August",
        "Oktober",
        "November",
        "Desember"
    };



    [SerializeField] GameObject _entryPrefab;
    List<HighScoreEntry> _entries = new List<HighScoreEntry>();
    [SerializeField] TextMeshProUGUI _headerText;
    [SerializeField] bool _clearOnPlay = false;

    public static HighScoreHandler instance;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        string headerText = "Månedens ansatte \n" + months[DateTime.Now.Month - 1] + " " + DateTime.Now.Year.ToString();
        if(_headerText != null)
            _headerText.text = headerText;

        if(_clearOnPlay && Application.isEditor)
            PlayerPrefs.DeleteAll();

        //clear playerprefs on new month
        int monthWhenLastPlayed = PlayerPrefs.GetInt("lastMonth", 0);
        if(monthWhenLastPlayed > 0 && monthWhenLastPlayed != DateTime.Now.Month)
        {
            Debug.LogWarning("we're in a new month, deleting old scores");
            //we're in a new month
            PlayerPrefs.DeleteAll();
        }
        PlayerPrefs.SetInt("lastMonth", DateTime.Now.Month);

        //make sure scores are saved on scene reload and exit to menu
        SceneTransitioner.instance.onLoadScene.AddListener(() => SaveScores());

        //load scores
        int index = 0;
        while (true)
        {
            string playerName = PlayerPrefs.GetString("playerName" + index);
            string playerScore = PlayerPrefs.GetString("playerScore" + index);

            if (playerName != null && playerName.Length > 0)
            {
                index++;
                LoadScore(playerName, playerScore);
            }
            else
                break;
        }

        if (index == 0)
        {
            LoadScore("Rampen", "00:34:42");
            LoadScore("Engelen", "00:23:97");
        }

        SortScores();
        StartCoroutine(HighlightEntry(_entries[0]));
    }

    private void SortScores()
    {
        _entries = _entries.OrderBy(x => x._scoreText.text).ToList();

        for(int i = 0; i < _entries.Count; i++) 
        {
            _entries[i].transform.SetSiblingIndex(i);
        }
    }


    public void LoadScore(string playerName, string playerScore)
    {
        GameObject go = GameObject.Instantiate(_entryPrefab, transform);
        HighScoreEntry entry = go.GetComponent<HighScoreEntry>();
        entry._nameText.text = playerName;
        entry._scoreText.text = playerScore;
        _entries.Add(entry);
    }

    public void AddNewScore(string playerScore)
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

    //TODO save scores on level load as well
    private void OnApplicationQuit()
    {
        //save scores on quit
        SaveScores();
    }

    private void SaveScores()
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            HighScoreEntry entry = _entries[i];
            PlayerPrefs.SetString("playerName" + i, entry._nameText.text);
            PlayerPrefs.SetString("playerScore" + i, entry._scoreText.text);
        }
    }

    private IEnumerator HighlightEntry(HighScoreEntry entry)
    {
        float timer = 2.0f;
        Image image = entry._bgImage;

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
