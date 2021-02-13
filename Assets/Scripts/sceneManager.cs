using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneManager : MonoBehaviour
{
    public Text LevelText;
    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;
    public Text lastScoreText;

    void Start()
    {
        if (LevelText != null)
        {
            LevelText.text = "0";
        }

        if (highScoreText2 != null)
        {
            highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();
        }
        if (highScoreText != null)
        {
            highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();
        }
        if (highScoreText3 != null)
        {
            highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();
        }

        if (lastScoreText != null)
        {
            lastScoreText.text = PlayerPrefs.GetInt("lastscore").ToString();
        }
        
    }

    public void goTo(string sahndeAdi)
    {
        SceneManager.LoadScene(sahndeAdi);
        
    }

    public void PlayGame()
    {
        if (Game.startingLevel == 0)
        {
            Game.startingLevelZero = true;
        }
        else
        {
            Game.startingLevelZero = false;
        }

        SceneManager.LoadScene("Level1");
        Game.isPaused = false;
        Time.timeScale = 1;
    }

    public void ExitGame()
    {

        Application.Quit();
    }

    public void ChangedValue(float value)
    {
        Game.startingLevel = (int) value;
        LevelText.text = value.ToString();
    }
}
