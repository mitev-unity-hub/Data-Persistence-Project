using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
//TODO: Transfer the User name with the base score to the main scene 
public class MenuUIHandler : MonoBehaviour
{
    public int score = 0;

    private InputField userInput;
    private TextMeshProUGUI scoreText;

    private void Start()
    {
        userInput = GameObject.Find("UserName").GetComponent<InputField>();

        userInput.onEndEdit.AddListener(GetUserInput);

        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
    }
    public void StartNew()
    {
        SceneManager.LoadScene(1);
    }

    private void GetUserInput(string input)
    {
        scoreText.text ="Best Score: " + input + ": " + score;
        MainMenuManager.Instance.PlayerName = input;
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }
}
