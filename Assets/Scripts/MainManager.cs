using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public Text ScoreText;
    public Text ScoreTextTop;
    public GameObject GameOverText;

    private int prevScore;
    private string prevPlayerName;

    private string PlayerName;
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private void Awake()
    {
        LoadPlayerName();
    }

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        if (MainMenuManager.Instance != null)
        {
            PlayerName = MainMenuManager.Instance.PlayerName;
            ScoreText.text = $"{PlayerName} Score :";

            if (!string.IsNullOrEmpty(prevPlayerName))
            {
                ScoreTextTop.text = $"{prevPlayerName} Score : {prevScore}";
            }
            else
            {
                ScoreTextTop.text = $"{PlayerName} Score : {m_Points}";
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
             //   RestartScene();
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        if (MainMenuManager.Instance != null)
        {
            ScoreText.text = $"{PlayerName} Score : {m_Points}";
        } else
        {
            ScoreText.text = $"Score : {m_Points}";
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        if (MainMenuManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(prevPlayerName))
            {
                ScoreTextTop.text = $"{prevPlayerName} Score : {prevScore}";
            } else
            {
                ScoreTextTop.text = $"{PlayerName} Score : {m_Points}";
            }
    
            if (m_Points > prevScore)
            {
                SavePlayerName();
            }
        }
    }

    public void RestartScene() 
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }

    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int points;
    }

    public void SavePlayerName()
    {
        SaveData data = new SaveData();
        data.playerName = PlayerName;
        data.points = m_Points;
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadPlayerName()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            prevPlayerName = data.playerName;
            prevScore = data.points;
        }
    }
}
