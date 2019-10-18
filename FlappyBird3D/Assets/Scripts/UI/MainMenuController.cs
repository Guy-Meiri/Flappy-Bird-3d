using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    private GameObject m_MainMenuPanel;

    [SerializeField]
    private GameObject m_HowToPlayPanel;

    public void StartGameButtonPressed()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void HowToPlayButtonPressed()
    {
        m_HowToPlayPanel.SetActive(true);
        m_MainMenuPanel.SetActive(false);
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        m_HowToPlayPanel.SetActive(false);
        m_MainMenuPanel.SetActive(true);
    }
}
