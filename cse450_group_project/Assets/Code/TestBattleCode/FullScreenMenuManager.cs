using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class FullScreenMenuManager : MonoBehaviour
{

    public TMP_Text winLoseText;
    public Button backToMapButton;
    public Button restartButton;
    public Button resumeLevel;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        backToMapButton.onClick.AddListener(BackToMap);
        restartButton.onClick.AddListener(ReloadLevel);
        resumeLevel.onClick.AddListener(Hide);

    }

    public void SetWinLoseText(string text) {
        winLoseText.text = text;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    void BackToMap()
    {
        SceneManager.LoadScene("MapTesting");
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

}
