using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ScoreObject;

    [SerializeField]
    private Text m_FinalScoreObject;

    [SerializeField]
    private Text m_ScoreText;

    public void ShowFinalScore()
    {
        m_ScoreObject.SetActive(false);
        m_FinalScoreObject.text = m_ScoreText.text;
    }

    public void OnBackToMainMenuButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
