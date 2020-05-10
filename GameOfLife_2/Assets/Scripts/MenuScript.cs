using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject MenuButtons;
    public GameObject GameInfo;
    public GameObject BackButton;

    public void PlayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    public void QuitGame() => Application.Quit();

    public void ShowInfo()
    {
        MenuButtons.SetActive(false);
        GameInfo.SetActive(true);
        BackButton.SetActive(true);
    }

    public void CloseInfo()
    {
        MenuButtons.SetActive(true);
        GameInfo.SetActive(false);
        BackButton.SetActive(false);
    }
}
