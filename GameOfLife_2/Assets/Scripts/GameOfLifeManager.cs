using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOfLifeManager : MonoBehaviour
{
    // Buttons 
    public GameObject PlayButton;
    public GameObject StopButton;
    public GameObject StepButton;

    // Everything for temperature
    public GameObject ColdColorButton;
    public GameObject ColdText;
    public GameObject WarmColorButton;
    public GameObject WarmText;
    public GameObject HotColorButton;
    public GameObject HotText;
    public GameObject TemperatureText;
    public Toggle TemperatureToggle;

    // Switch 2D to 3D and back
    public GameObject SwitchGameModeButton;
    public Text SwitchText;

    public GameObject PatternPanel;
    public GameObject TemperaturePatternPanel;
    public GameObject TemperatureScrollbar;
    public GameObject BWScrollbar;

    // Update time
    public Slider TimeSlider;
    public Text updateIntervalText;
    protected float updateInterval;

    // Current generation
    public Text genText;

    // For different game modes
    public GameOfLife2D game2D;
    public GameOfLife3D game3D;

    // Current game mode
    protected GameOfLife2D game;

    // For checking which mode is currently playing
    protected bool gameIs2D;

    // For cheching if temperature mode is activated
    public bool temperatureModeOn;

    // Singleton
    public static GameOfLifeManager instance;

    // Before the beginning all the fields and objects should be set correctly
    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // By default game is 2D and has temperature mode off
        game = game2D;
        gameIs2D = true;
        game2D.gameObject.SetActive(true);
        game3D.gameObject.SetActive(false);

        // Remove checkmark from toggle
        TemperatureToggle.isOn = false;
        temperatureModeOn = true;
        ChangeTemperatureMode();

        // Set time value
        instance.updateInterval = TimeSlider.value;
        updateIntervalText.text = "Update Iterval: " + Mathf.Round(instance.updateInterval * 1000.0f) + "ms";
    }

    #region Group of methods for interaction with respective ones in current game
    public void NextStep() => game.NextStep();

    public void ResetCells() => game.ResetCells();

    public void StopSim() => game.StopSim();

    public void StartSim() => game.StartSim();

    // Interacts with temperature mode settings
    public void HotButtonClick() => game.SwitchToHot();
    public void ColdButtonClick() => game.SwitchToCold();
    public void WarmButtonClick() => game.SwitchToWarm();
    #endregion

    /// <summary>
    /// Changes game mode between 2D and 3D
    /// </summary>
    public void ChangeGameMode()
    {
        // Game should be stopped, cells should be removed
        ResetCells();

        // Switches flag of game mode
        gameIs2D = !gameIs2D;

        // Makes current game's game objects inactive
        game.gameObject.SetActive(false);

        // Depending on new game mode all the buttons and texts should be set correctly
        if (gameIs2D)
        {
            game = game2D;
            TemperatureToggle.gameObject.SetActive(true);
            if (temperatureModeOn)
            {
                ColdColorButton.SetActive(true);
                WarmColorButton.SetActive(true);
                HotColorButton.SetActive(true);
                HotText.SetActive(true);
                ColdText.SetActive(true);
                WarmText.SetActive(true);
                TemperatureText.SetActive(true);
                TemperaturePatternPanel.SetActive(true);
                TemperatureScrollbar.SetActive(true);
                PatternPanel.SetActive(false);
                BWScrollbar.SetActive(false);
                game.TemperatureModeOn();
            }
            else
            {
                ColdColorButton.SetActive(false);
                WarmColorButton.SetActive(false);
                HotColorButton.SetActive(false);
                HotText.SetActive(false);
                ColdText.SetActive(false);
                WarmText.SetActive(false);
                PatternPanel.SetActive(true);
                BWScrollbar.SetActive(true);
                TemperaturePatternPanel.SetActive(false);
                TemperatureScrollbar.SetActive(false);
                TemperatureText.SetActive(false);
                game.TemperatureModeOff();
            }
            SwitchText.text = "switch to 3D";
        }
        else
        {
            game = game3D;
            ColdColorButton.SetActive(false);
            WarmColorButton.SetActive(false);
            HotColorButton.SetActive(false);
            HotText.SetActive(false);
            ColdText.SetActive(false);
            WarmText.SetActive(false);
            TemperatureText.SetActive(false);
            TemperatureToggle.gameObject.SetActive(false);
            PatternPanel.SetActive(false);
            TemperaturePatternPanel.SetActive(false);
            PatternPanel.SetActive(false);
            TemperatureScrollbar.SetActive(false);
            BWScrollbar.SetActive(false);
            SwitchText.text = "switch to 2D";
        }

        // New game's game object should be active then
        game.gameObject.SetActive(true);
    }

    /// <summary>
    /// Changes temperature mode
    /// </summary>
    public void ChangeTemperatureMode()
    {
        temperatureModeOn = !temperatureModeOn;

        // Game should be stopped and cells should be removed
        ResetCells();

        // Depending on mode buttons and texts should be set correctly
        if (temperatureModeOn)
        {
            ColdColorButton.SetActive(true);
            WarmColorButton.SetActive(true);
            HotColorButton.SetActive(true);
            HotText.SetActive(true);
            ColdText.SetActive(true);
            WarmText.SetActive(true);
            TemperatureText.SetActive(true);
            PatternPanel.SetActive(false);
            BWScrollbar.SetActive(false);
            TemperaturePatternPanel.SetActive(true);
            TemperatureScrollbar.SetActive(true);
            game.TemperatureModeOn();
        }
        else
        {
            ColdColorButton.SetActive(false);
            WarmColorButton.SetActive(false);
            HotColorButton.SetActive(false);
            HotText.SetActive(false);
            ColdText.SetActive(false);
            WarmText.SetActive(false);
            TemperatureText.SetActive(false);
            TemperaturePatternPanel.SetActive(false);
            TemperatureScrollbar.SetActive(false);
            PatternPanel.SetActive(true);
            BWScrollbar.SetActive(true);
            game.TemperatureModeOff();
        }
    }

    /// <summary>
    /// Changes update time interval if slider's value was changed
    /// </summary>
    /// <param name="slider">Slider to get value from</param>
    public void ChangeUpdateInterval(Slider slider)
    {
        instance.updateInterval = slider.value;
        updateIntervalText.text = "Update Iterval: " + Mathf.Round(instance.updateInterval * 1000.0f) + "ms";
    }

    /// <summary>
    /// Loads Main Menu Scene
    /// </summary>
    public void LoadMenu() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    #region Methods for interacting with cell's patterns in different modes
    public void Pattern1ButtonClick() => game.Pattern1Click();
    public void Pattern2ButtonClick() => game.Pattern2Click();
    public void Pattern3ButtonClick() => game.Pattern3Click();

    public void BWPattern1ButtonClick() => game.BWPattern1Click();
    public void BWPattern2ButtonClick() => game.BWPattern2Click();
    public void BWPattern3ButtonClick() => game.BWPattern3Click();
    public void BWPattern4ButtonClick() => game.BWPattern4Click();
    public void BWPattern5ButtonClick() => game.BWPattern5Click();
    public void BWPattern6ButtonClick() => game.BWPattern6Click();
    public void BWPattern7ButtonClick() => game.BWPattern7Click();
    #endregion
}
