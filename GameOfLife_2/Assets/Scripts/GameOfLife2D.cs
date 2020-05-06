using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife2D : MonoBehaviour
{
    public Transform gameBoard;
    public Camera gameCamera;

    [SerializeField]
    private GameObject[,] grid;

    // For counting neighbours
    private int[,] values;

    private int width = 100;
    private int height = 100;

    protected int generation = 0;
    protected bool isPlaying = false;

    protected float counter = 0f;

    // For temperature mode
    [SerializeField]
    private GameObject hotCellPrefab;

    [SerializeField]
    private GameObject warmCellPrefab;

    [SerializeField]
    private GameObject coldCellPrefab;

    [SerializeField]
    private GameObject aliveCellPrefab;

    // All for cells working
    public GameObject cellPrefab;

    // Different values for different colors
    // Dead is 0
    // Black is 1
    // Cold is 2
    // Warm is 3
    // Hot is 4
    private int currentValue = 3;

    private void Awake()
    {
        grid = new GameObject[width, height];
        values = new int[width, height];
        // Default: cells are dead
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
                values[i, j] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            counter += Time.deltaTime;
            if (counter >= GameOfLifeManager.instance.TimeSlider.value)
            {
                counter = 0.0f;
                UpdateCells();
            }
        }
        else
        {
            bool mouseClicked = Input.GetMouseButtonDown(0);
            if (mouseClicked)
            {
                Vector2 pos = gameCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int boardPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                UpdateMaterial(boardPos);
            }
        }
    }

    protected virtual void UpdateCells()
    {
        generation++;
        if (GameOfLifeManager.instance.temperatureModeOn)
        {
            List<Vector2Int> toBeHot = new List<Vector2Int>();
            List<Vector2Int> toBeWarm = new List<Vector2Int>();
            List<Vector2Int> toBeCold = new List<Vector2Int>();
            List<Vector2Int> toBeDead = new List<Vector2Int>();
                
            for(int i = 0; i < width; ++i)
                for(int j = 0; j < height; ++j)
                {
                    int numNeighbours = GetNeighbours(i, j);

                    switch(values[i, j])
                    {
                        // Cel was cold
                        case 2:
                            // Gets warm if has enough neighbours
                            if (numNeighbours > 3)
                                toBeWarm.Add(new Vector2Int(i, j));
                            // Dies if it is too cold
                            if (numNeighbours < 2)
                                toBeDead.Add(new Vector2Int(i, j));
                            break;

                        // Cel was warm
                        case 3:
                            // Gets hot if has enough neighbours
                            if (numNeighbours > 3)
                                toBeHot.Add(new Vector2Int(i, j));
                            // Gets colder if it is not enough neighbours
                            if (numNeighbours < 2)
                                toBeCold.Add(new Vector2Int(i, j));
                            break;

                        // Cel was hot
                        case 4:
                            // Cell dies if it is too hot
                            if (numNeighbours > 3)
                                toBeDead.Add(new Vector2Int(i, j));
                            // Gets colder if it is not enough neighbours
                            if (numNeighbours < 2)
                                toBeWarm.Add(new Vector2Int(i, j));
                            break;

                        // Cell was dead
                        case 0:
                            if (numNeighbours == 3)
                                toBeCold.Add(new Vector2Int(i, j));
                            break;

                        default: throw new ArgumentException("Something went wrong: cell has incorrect value");
                    }
                }

            // To save user's choice
            int lastpos = currentValue;

            // Updating

            foreach (var cell in toBeDead)
                DestroyCell(cell);

            SwitchToCold();
            foreach (var cell in toBeCold)
                CreateCell(cell);

            SwitchToWarm();
            foreach (var cell in toBeWarm)
                CreateCell(cell);

            SwitchToHot();
            foreach (var cell in toBeHot)
                CreateCell(cell);



            if (lastpos == 2)
                SwitchToCold();
            if (lastpos == 3)
                SwitchToWarm();
            if (lastpos == 4)
                SwitchToHot();
        }
        else
        {
            List<Vector2Int> toBeAlive = new List<Vector2Int>();
            List<Vector2Int> toBeDead = new List<Vector2Int>();

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    bool cellIsAlive = grid[i, j] != null;
                    int numNeighbours = GetNeighbours(i, j);
                    if (cellIsAlive)
                    {
                        if (numNeighbours < 2 || numNeighbours > 3)
                            toBeDead.Add(new Vector2Int(i, j));
                    }
                    else
                    {
                        if (numNeighbours == 3)
                            toBeAlive.Add(new Vector2Int(i, j));
                    }
                }

            foreach (Vector2Int cell in toBeAlive)
                CreateCell(cell);

            foreach (Vector2Int cell in toBeDead)
                DestroyCell(cell);
        }
        

        GameOfLifeManager.instance.genText.text = $"Generation: {generation}";
    }

    /// <summary>
    /// Counts number of alive neighbours of cell
    /// </summary>
    /// <param name="x">x-coordinate of cell</param>
    /// <param name="y">y-coordinate of cell</param>
    /// <returns>Number of alive nighbours</returns>
    protected int GetNeighbours(int x, int y)
    {
        int neighbours = 0;

        int minXRange = x > 0 ? -1 : 0;
        int maxXRange = x < width - 1 ? 1 : 0;
        int minYRange = y > 0 ? -1 : 0;
        int maxYRange = y < height - 1 ? 1 : 0;

        for (int i = minXRange; i <= maxXRange; i++)
        {
            for (int j = minYRange; j <= maxYRange; j++)
            {
                if (i == 0 && j == 0) // Don't count ourselves
                    continue;
                // From warmer to colder
                if (values[x + i, y + j] >= values[x, y] && values[x + i, y + j] != 0)
                    neighbours++;
            }
        }

        return neighbours;
    }

    // Method for one-step updating
    public virtual void NextStep() => UpdateCells();

    /// <summary>
    /// Resets 2D board
    /// </summary>
    public virtual void ResetCells()
    {
        generation = 0;
        StopSim();

        foreach (GameObject cell in grid)
            if (cell != null)
                Destroy(cell);

        Array.Clear(grid, 0, grid.Length);
        Array.Clear(values, 0, values.Length);
        GameOfLifeManager.instance.genText.text = $"Generation: {generation}";
    }

    /// <summary>
    /// Starts game process
    /// </summary>
    public virtual void StartSim()
    {
        isPlaying = true;

        GameOfLifeManager.instance.StopButton.SetActive(true);
        GameOfLifeManager.instance.PlayButton.SetActive(false);
        GameOfLifeManager.instance.StepButton.SetActive(false);
    }

    /// <summary>
    /// Stops game process
    /// </summary>
    public virtual void StopSim()
    {
        isPlaying = false;
        counter = 0f;

        GameOfLifeManager.instance.StopButton.SetActive(false);
        GameOfLifeManager.instance.PlayButton.SetActive(true);
        GameOfLifeManager.instance.StepButton.SetActive(true);
    }

    private void UpdateMaterial(Vector2Int cellPosition)
    {
        try
        {
            GameObject cell = grid[cellPosition.x, cellPosition.y];
            if (cell == null)
                CreateCell(cellPosition);
            else
                DestroyCell(cellPosition);
        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// Method for creating a cell in 2D scene
    /// </summary>
    /// <param name="cellPosition">Cell's position</param>
    private void CreateCell(Vector2Int cellPosition)
    {
        if (grid[cellPosition.x, cellPosition.y] != null)
            DestroyCell(cellPosition);
        GameObject newCell = Instantiate(cellPrefab);
        newCell.transform.SetParent(gameBoard);
        newCell.transform.position = cellPosition + new Vector2(0.5f, 0.5f);
        grid[cellPosition.x, cellPosition.y] = newCell;
        values[cellPosition.x, cellPosition.y] = currentValue;
    }

    /// <summary>
    /// Method for destroying a cell in 2D scene
    /// </summary>
    /// <param name="cellPosition">Cell's position</param>
    private void DestroyCell(Vector2Int cellPosition)
    {
        GameObject deadCell = grid[cellPosition.x, cellPosition.y];
        if (deadCell != null)
            Destroy(deadCell);
        grid[cellPosition.x, cellPosition.y] = null;
        values[cellPosition.x, cellPosition.y] = 0;
    }

    public void TemperatureModeOff() {
        cellPrefab = aliveCellPrefab;
        currentValue = 1;
    }

    public void TemperatureModeOn() {
        cellPrefab = hotCellPrefab;
        currentValue = 4;
    }

    public void SwitchToCold() {
        cellPrefab = coldCellPrefab;
        currentValue = 2;
    }

    public void SwitchToWarm()
    {
        cellPrefab = warmCellPrefab;
        currentValue = 3;
    }

    public void SwitchToHot() {
        cellPrefab = hotCellPrefab;
        currentValue = 4;
    }
}
