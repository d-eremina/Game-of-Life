using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject MenuButtons;
    public GameObject GameInfo;
    public GameObject BackButton;

    /// <summary>
    /// Loads Main Scene
    /// </summary>
    public void PlayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    /// <summary>
    /// Quits game
    /// </summary>
    public void QuitGame() => Application.Quit();

    /// <summary>
    /// Shows Game info
    /// </summary>
    public void ShowInfo()
    {
        MenuButtons.SetActive(false);
        GameInfo.SetActive(true);
        BackButton.SetActive(true);
    }

    /// <summary>
    /// Closes Game info
    /// </summary>
    public void CloseInfo()
    {
        MenuButtons.SetActive(true);
        GameInfo.SetActive(false);
        BackButton.SetActive(false);
    }
}
