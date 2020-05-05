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

    // Colors of 2D cells
    public GameObject ColdColorButton;
    public GameObject MediumColorButton;
    public GameObject HotColorButton;
    public Color currentColor = Color.black;

    // Switch 2D to 3D and back
    public GameObject SwitchGameModeButton;
    public Text SwitchText;

    // Changing between B&W and Temperature modes
    public GameObject TemperatureToggle;

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
            SwitchText.text = "switch to 3D";
        }
        else
        {
            game = game3D;
            SwitchText.text = "switch to 2D";
        }
        game.gameObject.SetActive(true);
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
}
