using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultMenuManager : MonoBehaviour
{

    public TMP_Text winLoseText;
    public Button backToMapButton;
    public Button restartButton;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        backToMapButton.onClick.AddListener(BackToMap);
        restartButton.onClick.AddListener(ReloadLevel);
    }

    public void PlayerWon()
    {
        winLoseText.text = "You Won!";
        restartButton.gameObject.SetActive(false);
        backToMapButton.gameObject.SetActive(true);

        gameObject.SetActive(true);
    }

    public void PlayerLost()
    {
        winLoseText.text = "You Lost!";
        restartButton.gameObject.SetActive(true);
        backToMapButton.gameObject.SetActive(true);

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



}
