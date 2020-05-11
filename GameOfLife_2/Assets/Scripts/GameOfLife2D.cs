using System;
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

    // Grid's size
    private int width = 100;
    private int height = 100;

    protected int generation = 0;
    protected bool isPlaying = false;

    // For counting elapsed time
    protected float counter = 0f;


    // Prefabs for different cell's states

    [SerializeField]
    private GameObject hotCellPrefab;

    [SerializeField]
    private GameObject warmCellPrefab;

    [SerializeField]
    private GameObject coldCellPrefab;

    [SerializeField]
    private GameObject aliveCellPrefab;

    // Current cell prefab
    public GameObject cellPrefab;

    // Saves value to count nighbours later
    private int currentValue = 3;

    private void Awake()
    {
        grid = new GameObject[width, height];
        values = new int[width, height];
        // By default cells are dead
        for (int i = 0; i < width; ++i)
            for (int j = 0; j < height; ++j)
                values[i, j] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // If game is not paused, elapsed time increases
        if (isPlaying)
        {
            counter += Time.deltaTime;
            // If elapsed time is more or equals update time, grid updates
            if (counter >= GameOfLifeManager.instance.TimeSlider.value)
            {
                counter = 0.0f;
                UpdateCells();
            }
        }
        // If game is paused, player can input new cell or remove it from grid
        else
        {
            bool mouseClicked = Input.GetMouseButtonDown(0);
            // If player clicked on grid, cell's material updates
            if (mouseClicked)
            {
                Vector2 pos = gameCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int boardPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                UpdateMaterial(boardPos);
            }
        }
    }

    /// <summary>
    /// Updates cell's grid
    /// </summary>
    protected virtual void UpdateCells()
    {
        generation++;

        // Updating depends on current game mode
        if (GameOfLifeManager.instance.temperatureModeOn)
        {
            // Saves coordinates of cells for update 
            List<Vector2Int> toBeHot = new List<Vector2Int>();
            List<Vector2Int> toBeWarm = new List<Vector2Int>();
            List<Vector2Int> toBeCold = new List<Vector2Int>();
            List<Vector2Int> toBeDead = new List<Vector2Int>();

            // Going through the grid
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                {
                    // Number of neighbours of current cell
                    int numNeighbours = GetNeighbours(i, j);

                    switch (values[i, j])
                    {
                        // If cell was cold
                        case 2:
                            // Gets warm if has enough neighbours
                            if (numNeighbours > 3)
                                toBeWarm.Add(new Vector2Int(i, j));
                            // Dies if it is too cold
                            if (numNeighbours < 2)
                                toBeDead.Add(new Vector2Int(i, j));
                            break;

                        // If cell was warm
                        case 3:
                            // Gets hot if has enough neighbours
                            if (numNeighbours > 3)
                                toBeHot.Add(new Vector2Int(i, j));
                            // Gets colder if it is not enough neighbours
                            if (numNeighbours < 2)
                                toBeCold.Add(new Vector2Int(i, j));
                            break;

                        // If cell was hot
                        case 4:
                            // Cell dies if it is too hot
                            if (numNeighbours > 3)
                                toBeDead.Add(new Vector2Int(i, j));
                            // Gets colder if it is not enough neighbours
                            if (numNeighbours < 2)
                                toBeWarm.Add(new Vector2Int(i, j));
                            break;

                        // If cell was dead
                        case 0:
                            if (numNeighbours == 3)
                                toBeCold.Add(new Vector2Int(i, j));
                            break;

                        default: throw new ArgumentException("Something went wrong: cell has incorrect value");
                    }
                }

            // To save user's last color choice
            int lastpos = currentValue;

            // Updating

            // Removes from grid all cells which must be dead
            foreach (var cell in toBeDead)
                DestroyCell(cell);

            // Creates all cold cells
            SwitchToCold();
            foreach (var cell in toBeCold)
                CreateCell(cell);

            // Creates all warm cells
            SwitchToWarm();
            foreach (var cell in toBeWarm)
                CreateCell(cell);

            // Creates all hot cells
            SwitchToHot();
            foreach (var cell in toBeHot)
                CreateCell(cell);

            // Returns back to player's choice
            if (lastpos == 2)
                SwitchToCold();
            if (lastpos == 3)
                SwitchToWarm();
            if (lastpos == 4)
                SwitchToHot();
        }
        else
        {
            // Saves coordinates of cells which must be updated 
            List<Vector2Int> toBeAlive = new List<Vector2Int>();
            List<Vector2Int> toBeDead = new List<Vector2Int>();

            // Going through the grid
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

            // Updates cells
            foreach (Vector2Int cell in toBeAlive)
                CreateCell(cell);

            foreach (Vector2Int cell in toBeDead)
                DestroyCell(cell);
        }

        GameOfLifeManager.instance.genText.text = $"Generation: {generation}";
    }

    /// <summary>
    /// Counts number of neighbours of cell
    /// </summary>
    /// <param name="x">x-coordinate of cell</param>
    /// <param name="y">y-coordinate of cell</param>
    /// <returns>Number of cell's neighbours</returns>
    protected int GetNeighbours(int x, int y)
    {
        int neighbours = 0;

        int minXRange = x > 0 ? -1 : 0;
        int maxXRange = x < width - 1 ? 1 : 0;
        int minYRange = y > 0 ? -1 : 0;
        int maxYRange = y < height - 1 ? 1 : 0;

        for (int i = minXRange; i <= maxXRange; i++)
            for (int j = minYRange; j <= maxYRange; j++)
            {
                // Don't count ourselves
                if (i == 0 && j == 0) 
                    continue;
                // From warmer to colder
                if (values[x + i, y + j] >= values[x, y] && values[x + i, y + j] != 0)
                    neighbours++;
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

        // Destroys all game objects on grid
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

    /// <summary>
    /// Updates cell's mateial
    /// </summary>
    /// <param name="cellPosition">Position of cell on grid</param>
    private void UpdateMaterial(Vector2Int cellPosition)
    {
        // If coodinates are out of range, nothing will happen
        try
        {
            GameObject cell = grid[cellPosition.x, cellPosition.y];
            if (cell == null)
                CreateCell(cellPosition);
            else
                DestroyCell(cellPosition);
        }
        catch (Exception)
        {
            return;
        }
    }

    /// <summary>
    /// Creates a cell on grid
    /// </summary>
    /// <param name="cellPosition">Cell's position</param>
    private void CreateCell(Vector2Int cellPosition)
    {
        // If game object already exists, destroys it first
        if (grid[cellPosition.x, cellPosition.y] != null)
            DestroyCell(cellPosition);

        GameObject newCell = Instantiate(cellPrefab);
        newCell.transform.SetParent(gameBoard);
        newCell.transform.position = cellPosition + new Vector2(0.5f, 0.5f);
        grid[cellPosition.x, cellPosition.y] = newCell;
        values[cellPosition.x, cellPosition.y] = currentValue;
    }

    /// <summary>
    /// Destroys a cell on grid
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

    /// <summary>
    /// Resets values
    /// </summary>
    public void TemperatureModeOff()
    {
        cellPrefab = aliveCellPrefab;
        currentValue = 1;
    }

    /// <summary>
    /// Sets values if temperature mode is on
    /// </summary>
    public void TemperatureModeOn()
    {
        cellPrefab = hotCellPrefab;
        currentValue = 4;
    }

    /// <summary>
    /// Sets values for cold cell
    /// </summary>
    public void SwitchToCold()
    {
        cellPrefab = coldCellPrefab;
        currentValue = 2;
    }

    /// <summary>
    /// Sets values for warm cell
    /// </summary>
    public void SwitchToWarm()
    {
        cellPrefab = warmCellPrefab;
        currentValue = 3;
    }

    /// <summary>
    /// Sets values for hot cell
    /// </summary>
    public void SwitchToHot()
    {
        cellPrefab = hotCellPrefab;
        currentValue = 4;
    }

    #region Temperature Patterns
    public void Pattern1Click()
    {
        ResetCells();
        SwitchToHot();

        int middle = 50;

        for (int i = -3; i <= 3; ++i)
        {
            CreateCell(new Vector2Int(middle, middle + i));
            CreateCell(new Vector2Int(middle + i, middle));
        }

        SwitchToWarm();

        CreateCell(new Vector2Int(middle - 1, middle + 1));
        CreateCell(new Vector2Int(middle - 1, middle + 2));
        CreateCell(new Vector2Int(middle - 2, middle + 1));

        CreateCell(new Vector2Int(middle + 1, middle + 1));
        CreateCell(new Vector2Int(middle + 1, middle + 2));
        CreateCell(new Vector2Int(middle + 2, middle + 1));

        CreateCell(new Vector2Int(middle - 1, middle - 1));
        CreateCell(new Vector2Int(middle - 1, middle - 2));
        CreateCell(new Vector2Int(middle - 2, middle - 1));

        CreateCell(new Vector2Int(middle + 1, middle - 1));
        CreateCell(new Vector2Int(middle + 1, middle - 2));
        CreateCell(new Vector2Int(middle + 2, middle - 1));
    }

    public void Pattern2Click()
    {
        ResetCells();

        int middle = 50;
        SwitchToCold();

        CreateCell(new Vector2Int(middle - 1, middle));
        CreateCell(new Vector2Int(middle - 1, middle - 1));
        CreateCell(new Vector2Int(middle, middle));
        CreateCell(new Vector2Int(middle, middle - 1));

        SwitchToWarm();
        for (int j = -2; j <= 1; ++j)
        {
            CreateCell(new Vector2Int(middle - 2, middle + j));
            CreateCell(new Vector2Int(middle + 1, middle + j));
        }
        for (int j = -2; j <= 1; ++j)
        {
            CreateCell(new Vector2Int(middle + j, middle - 2));
            CreateCell(new Vector2Int(middle + j, middle + 1));
        }

        SwitchToHot();
        for (int j = -3; j <= 2; ++j)
        {
            CreateCell(new Vector2Int(middle - 3, middle + j));
            CreateCell(new Vector2Int(middle + 2, middle + j));
        }
        for (int j = -3; j <= 2; ++j)
        {
            CreateCell(new Vector2Int(middle + j, middle - 3));
            CreateCell(new Vector2Int(middle + j, middle + 2));
        }
    }

    public void Pattern3Click()
    {
        ResetCells();
        SwitchToHot();

        int middle = 50;

        CreateCell(new Vector2Int(middle, middle - 1));
        CreateCell(new Vector2Int(middle, middle - 2));
        CreateCell(new Vector2Int(middle, middle - 3));
        CreateCell(new Vector2Int(middle + 1, middle - 3));
        CreateCell(new Vector2Int(middle + 2, middle - 3));
        CreateCell(new Vector2Int(middle + 2, middle - 2));
        CreateCell(new Vector2Int(middle + 2, middle - 1));
        CreateCell(new Vector2Int(middle + 1, middle - 1));

        CreateCell(new Vector2Int(middle - 1, middle));
        CreateCell(new Vector2Int(middle - 2, middle));
        CreateCell(new Vector2Int(middle - 3, middle));
        CreateCell(new Vector2Int(middle - 3, middle + 1));
        CreateCell(new Vector2Int(middle - 3, middle + 2));
        CreateCell(new Vector2Int(middle - 2, middle + 2));
        CreateCell(new Vector2Int(middle - 1, middle + 2));
        CreateCell(new Vector2Int(middle - 1, middle + 1));
    }
    #endregion

    #region BW Mode Patterns

    public void BWPattern1Click()
    {
        ResetCells();

        CreateCell(new Vector2Int(1, height - 4));
        CreateCell(new Vector2Int(2, height - 4));
        CreateCell(new Vector2Int(3, height - 4));
        CreateCell(new Vector2Int(3, height - 3));
        CreateCell(new Vector2Int(2, height - 2));
    }

    public void BWPattern2Click()
    {
        ResetCells();
        int middle = 50;

        for (int i = -6; i <= 5; ++i)
            CreateCell(new Vector2Int(middle + i, middle));
        for (int i = -4; i <= 3; ++i)
        {
            CreateCell(new Vector2Int(middle + i, middle + 2));
            CreateCell(new Vector2Int(middle + i, middle - 2));
        }
        for (int i = -2; i <= 1; ++i)
        {
            CreateCell(new Vector2Int(middle + i, middle + 4));
            CreateCell(new Vector2Int(middle + i, middle - 4));
        }
    }

    public void BWPattern3Click()
    {
        ResetCells();

        int middle = 50;

        CreateCell(new Vector2Int(1, middle - 1));
        CreateCell(new Vector2Int(1, middle + 1));

        CreateCell(new Vector2Int(3, middle - 2));
        CreateCell(new Vector2Int(4, middle - 2));

        CreateCell(new Vector2Int(6, middle - 1));

        CreateCell(new Vector2Int(7, middle));
        CreateCell(new Vector2Int(7, middle + 1));

        for (int i = 0; i <= 5; ++i)
            CreateCell(new Vector2Int(2 + i, middle + 2));
    }

    public void BWPattern4Click()
    {
        ResetCells();

        // for borders
        int leftStart = 5;
        int upper = 10;

        CreateCell(new Vector2Int(leftStart + 1, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 1, 92 - upper));
        CreateCell(new Vector2Int(leftStart + 2, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 2, 92 - upper));

        CreateCell(new Vector2Int(leftStart + 12, 92 - upper));
        CreateCell(new Vector2Int(leftStart + 12, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 12, 94 - upper));

        CreateCell(new Vector2Int(leftStart + 13, 95 - upper));
        CreateCell(new Vector2Int(leftStart + 14, 96 - upper));
        CreateCell(new Vector2Int(leftStart + 15, 95 - upper));

        CreateCell(new Vector2Int(leftStart + 16, 92 - upper));
        CreateCell(new Vector2Int(leftStart + 16, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 16, 94 - upper));
        CreateCell(new Vector2Int(leftStart + 17, 92 - upper));
        CreateCell(new Vector2Int(leftStart + 17, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 17, 94 - upper));

        CreateCell(new Vector2Int(leftStart + 13, 91 - upper));
        CreateCell(new Vector2Int(leftStart + 14, 90 - upper));
        CreateCell(new Vector2Int(leftStart + 15, 91 - upper));

        CreateCell(new Vector2Int(leftStart + 22, 94 - upper));
        CreateCell(new Vector2Int(leftStart + 22, 95 - upper));
        CreateCell(new Vector2Int(leftStart + 22, 96 - upper));

        CreateCell(new Vector2Int(leftStart + 23, 94 - upper));
        CreateCell(new Vector2Int(leftStart + 24, 94 - upper));
        CreateCell(new Vector2Int(leftStart + 25, 94 - upper));
        CreateCell(new Vector2Int(leftStart + 23, 96 - upper));
        CreateCell(new Vector2Int(leftStart + 24, 96 - upper));
        CreateCell(new Vector2Int(leftStart + 25, 96 - upper));

        CreateCell(new Vector2Int(leftStart + 25, 95 - upper));

        CreateCell(new Vector2Int(leftStart + 23, 97 - upper));
        CreateCell(new Vector2Int(leftStart + 24, 97 - upper));
        CreateCell(new Vector2Int(leftStart + 25, 97 - upper));
        CreateCell(new Vector2Int(leftStart + 26, 97 - upper));

        CreateCell(new Vector2Int(leftStart + 23, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 24, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 25, 93 - upper));
        CreateCell(new Vector2Int(leftStart + 26, 93 - upper));

        CreateCell(new Vector2Int(leftStart + 26, 98 - upper));
        CreateCell(new Vector2Int(leftStart + 26, 92 - upper));

        CreateCell(new Vector2Int(leftStart + 31, 97 - upper));
        CreateCell(new Vector2Int(leftStart + 31, 96 - upper));

        CreateCell(new Vector2Int(leftStart + 35, 95 - upper));
        CreateCell(new Vector2Int(leftStart + 36, 95 - upper));
        CreateCell(new Vector2Int(leftStart + 35, 94 - upper));
        CreateCell(new Vector2Int(leftStart + 36, 94 - upper));
    }

    public void BWPattern5Click()
    {
        ResetCells();

        int middle = 50;

        for (int i = -2; i <= 2; ++i)
        {
            CreateCell(new Vector2Int(middle - 2, middle + i));
            CreateCell(new Vector2Int(middle + 2, middle + i));
        }

        CreateCell(new Vector2Int(middle, middle - 2));
        CreateCell(new Vector2Int(middle, middle + 2));
    }

    public void BWPattern6Click()
    {
        ResetCells();

        int middle = 50;

        for (int i = -2; i <= 2; ++i)
        {
            CreateCell(new Vector2Int(middle - 1, middle + i));
            CreateCell(new Vector2Int(middle + 1, middle + i));
        }

        for (int i = 0; i <= 1; ++i)
        {
            CreateCell(new Vector2Int(middle - (int)Math.Pow(-1, i) * 2, middle + 2));
            CreateCell(new Vector2Int(middle - (int)Math.Pow(-1, i) * 2, middle + 1));
            CreateCell(new Vector2Int(middle - (int)Math.Pow(-1, i) * 2, middle - 3));
            CreateCell(new Vector2Int(middle - (int)Math.Pow(-1, i) * 3, middle - 3));
            CreateCell(new Vector2Int(middle - (int)Math.Pow(-1, i) * 3, middle - 2));
            CreateCell(new Vector2Int(middle - (int)Math.Pow(-1, i) * 3, middle - 1));
        }
    }

    public void BWPattern7Click()
    {
        ResetCells();

        int middle = 49;

        CreateCell(new Vector2Int(middle, middle));
        CreateCell(new Vector2Int(middle + 1, middle));
        CreateCell(new Vector2Int(middle + 1, middle + 1));
        CreateCell(new Vector2Int(middle + 2, middle + 2));
        CreateCell(new Vector2Int(middle + 3, middle + 1));
        CreateCell(new Vector2Int(middle + 3, middle));
        CreateCell(new Vector2Int(middle + 4, middle));

        CreateCell(new Vector2Int(middle + 7, middle));
        CreateCell(new Vector2Int(middle + 6, middle + 1));
        CreateCell(new Vector2Int(middle + 7, middle + 2));
        CreateCell(new Vector2Int(middle + 8, middle + 2));
        CreateCell(new Vector2Int(middle + 8, middle + 1));
        CreateCell(new Vector2Int(middle + 8, middle - 1));
        CreateCell(new Vector2Int(middle + 9, middle - 1));
        CreateCell(new Vector2Int(middle + 10, middle - 1));
        CreateCell(new Vector2Int(middle + 10, middle - 2));

        CreateCell(new Vector2Int(middle + 7, middle + 4));
        CreateCell(new Vector2Int(middle + 8, middle + 4));
        CreateCell(new Vector2Int(middle + 8, middle + 5));
        CreateCell(new Vector2Int(middle + 7, middle + 6));
        CreateCell(new Vector2Int(middle + 7, middle + 7));
        CreateCell(new Vector2Int(middle + 8, middle + 7));

        CreateCell(new Vector2Int(middle, middle + 6));
        CreateCell(new Vector2Int(middle + 1, middle + 7));
        CreateCell(new Vector2Int(middle + 1, middle + 8));
        CreateCell(new Vector2Int(middle + 1, middle + 9));
        CreateCell(new Vector2Int(middle + 2, middle + 9));
        CreateCell(new Vector2Int(middle - 1, middle + 5));
        CreateCell(new Vector2Int(middle - 2, middle + 6));
        CreateCell(new Vector2Int(middle - 2, middle + 7));
        CreateCell(new Vector2Int(middle - 1, middle + 7));
        CreateCell(new Vector2Int(middle - 4, middle + 7));
        CreateCell(new Vector2Int(middle - 5, middle + 7));
        CreateCell(new Vector2Int(middle - 4, middle + 6));
        CreateCell(new Vector2Int(middle - 2, middle + 6));
        CreateCell(new Vector2Int(middle - 6, middle + 6));
        CreateCell(new Vector2Int(middle - 7, middle + 6));
        CreateCell(new Vector2Int(middle - 7, middle + 7));
        CreateCell(new Vector2Int(middle - 1, middle + 7));

        CreateCell(new Vector2Int(middle - 7, middle));
        CreateCell(new Vector2Int(middle - 8, middle));
        CreateCell(new Vector2Int(middle - 9, middle));
        CreateCell(new Vector2Int(middle - 9, middle + 1));
        CreateCell(new Vector2Int(middle - 6, middle - 1));
        CreateCell(new Vector2Int(middle - 7, middle - 2));
        CreateCell(new Vector2Int(middle - 5, middle - 2));
        CreateCell(new Vector2Int(middle - 7, middle - 3));
        CreateCell(new Vector2Int(middle - 6, middle - 3));
        CreateCell(new Vector2Int(middle - 6, middle - 5));
        CreateCell(new Vector2Int(middle - 7, middle - 5));
        CreateCell(new Vector2Int(middle - 7, middle - 6));
        CreateCell(new Vector2Int(middle - 6, middle - 7));
        CreateCell(new Vector2Int(middle - 6, middle - 8));
        CreateCell(new Vector2Int(middle - 7, middle - 8));

        CreateCell(new Vector2Int(middle, middle - 8));
        CreateCell(new Vector2Int(middle, middle - 9));
        CreateCell(new Vector2Int(middle, middle - 10));
        CreateCell(new Vector2Int(middle - 1, middle - 10));
        CreateCell(new Vector2Int(middle + 1, middle - 7));
        CreateCell(new Vector2Int(middle + 2, middle - 6));
        CreateCell(new Vector2Int(middle + 3, middle - 7));
        CreateCell(new Vector2Int(middle + 3, middle - 8));
        CreateCell(new Vector2Int(middle + 2, middle - 8));
        CreateCell(new Vector2Int(middle + 5, middle - 8));
        CreateCell(new Vector2Int(middle + 6, middle - 8));
        CreateCell(new Vector2Int(middle + 5, middle - 7));
        CreateCell(new Vector2Int(middle + 7, middle - 7));
        CreateCell(new Vector2Int(middle + 8, middle - 7));
        CreateCell(new Vector2Int(middle + 8, middle - 8));
    }
    #endregion
}
