using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UniRx;
using TMPro;
using System.IO;

public class PlayerInfo
{
    public string name;
    public int bestRecord;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject textGo;

    public TMP_Text bestScoreTxt, currentScoreTxt;

    [HideInInspector] public BoolReactiveProperty isGameOver = new BoolReactiveProperty(false);
    [HideInInspector] public IntReactiveProperty currentScore = new IntReactiveProperty(0);
    [HideInInspector] public IntReactiveProperty bestScore = new IntReactiveProperty(0);

    private bool isOneClick;
    private float count = 0f;
    private string dataPath;
    private PlayerInfo playerInfo = new PlayerInfo();
    private List<IDisposable> _subject = new List<IDisposable>();

    public static GameManager instance;
    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "saveData.json");
        if (instance == null)
        {
            instance = this;
        }
        _subject.Add(isGameOver.TakeUntilDestroy(this).Subscribe(textGo.SetActive));
        _subject.Add(currentScore.TakeUntilDestroy(this).Subscribe(ScoreInit));
        _subject.Add(bestScore.TakeUntilDestroy(this).Subscribe(BestScoreInit));
    }

    private void Start()
    {
        if (File.Exists(dataPath))
        {
            var loadData = File.ReadAllText(dataPath);
            playerInfo = JsonUtility.FromJson<PlayerInfo>(loadData);
            bestScore.Value = playerInfo.bestRecord;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGameOver.Value)
            {
                if (!isOneClick)
                {
                    isOneClick = true;
                }
                else if (isOneClick && count < 0.3f)
                {
                    isOneClick = false;
#if !UNITY_EDITOR
                    Application.Quit();
#else
                    EditorApplication.isPlaying = false;
#endif
                }
            }
            else
            {
                isGameOver.Value = false;
                GameOverEvent(false);
            }
        }
        if (isOneClick)
        {
            count += Time.deltaTime;
            if (count >= 0.3f)
            {
                count = 0f;
                isOneClick = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (_subject.Count >= 0)
        {
            foreach(IDisposable sub in _subject)
            {
                sub.Dispose();
            }
        }
    }

    public void GameOverEvent(bool isGameOver)
    {
        if (isGameOver)
        {
            Time.timeScale = 0f;
            playerInfo.bestRecord = bestScore.Value;
            var saveData = JsonUtility.ToJson(playerInfo, true);
            File.WriteAllText(dataPath, saveData);
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            var loadData = File.ReadAllText(dataPath);
            playerInfo = JsonUtility.FromJson<PlayerInfo>(loadData);
        }
    }

    private void ScoreInit(int value)
    {
        currentScoreTxt.text = string.Format("{0:#,0}", value);
        if(value >= bestScore.Value)
        {
            bestScore.Value = value;
        }
    }

    private void BestScoreInit(int value)
    {
        bestScoreTxt.text = string.Format("Best Record : {0:#,0}", value);
    }
}
