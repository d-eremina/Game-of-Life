using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject TemperatureTogge;

    // Switch 2D to 3D and back
    public GameObject SwitchGameModeButton;
    public Text SwitchText;

    public GameObject TemperaturePatternPanel;
    public GameObject Scrollbar;

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

    // Checking which mode is currently playing
    protected bool gameIs2D;

    public bool temperatureModeOn = true;

    public static GameOfLifeManager instance;

    // Before the beginning all the fields and objects should be set correctly
    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        game = game2D;
        gameIs2D = true;
        game2D.gameObject.SetActive(true);
        game3D.gameObject.SetActive(false);
        game.TemperatureModeOn();

        instance.updateInterval = TimeSlider.value;
        updateIntervalText.text = "Update Iterval: " + Mathf.Round(instance.updateInterval * 1000.0f) + "ms";
    }

    public void NextStep() => game.NextStep();

    public void ResetCells() => game.ResetCells();

    public void StopSim() => game.StopSim();

    public void StartSim() => game.StartSim();

    /// <summary>
    /// Method for changing game mode
    /// </summary>
    public void ChangeGameMode()
    {
        StopSim();
        gameIs2D = !gameIs2D;
        game.gameObject.SetActive(false);
        if (gameIs2D)
        {
            game = game2D;
            TemperatureTogge.SetActive(true);
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
                Scrollbar.SetActive(true);
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
            TemperatureTogge.SetActive(false);
            TemperaturePatternPanel.SetActive(false);
            Scrollbar.SetActive(false);
            SwitchText.text = "switch to 2D";
        }
        game.gameObject.SetActive(true);
    }

    /// <summary>
    /// Changes settings depending on temperature mode
    /// </summary>
    public void ChangeTemperatureMode()
    {
        temperatureModeOn = !temperatureModeOn;
        game.ResetCells();
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
            Scrollbar.SetActive(true);
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
            // ! 
            Scrollbar.SetActive(false);
            game.TemperatureModeOff();
        }
    }

    /// <summary>
    /// Method for changing update time interval if slider's value was changed
    /// </summary>
    /// <param name="slider">Using slider</param>
    public void ChangeUpdateInterval(Slider slider)
    {
        instance.updateInterval = slider.value;
        updateIntervalText.text = "Update Iterval: " + Mathf.Round(instance.updateInterval * 1000.0f) + "ms";
    }

    // TODO: показывать в окошке текущий цвет или подсвечивать как-то кнопку
    public void HotButtonClick() => game.SwitchToHot();
    public void ColdButtonClick() => game.SwitchToCold();
    public void WarmButtonClick() => game.SwitchToWarm();

    public void Pattern1ButtonClick() => game.Pattern1Click();
    public void Pattern2ButtonClick() => game.Pattern2Click();
    public void Pattern3ButtonClick() => game.Pattern3Click();
}
