using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject textGo;


    [HideInInspector] public BoolReactiveProperty isGameOver = new BoolReactiveProperty(false);
    private bool isOneClick;
    private float count = 0f;
    private IDisposable _subject;

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _subject = isGameOver.TakeUntilDestroy(this).Subscribe(textGo.SetActive);
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
        if (_subject != null)
            _subject.Dispose();
    }

    public void GameOverEvent(bool isGameOver)
    {
        if (isGameOver)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
