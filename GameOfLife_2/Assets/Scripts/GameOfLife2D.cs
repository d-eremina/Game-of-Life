using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife2D : MonoBehaviour
{
    public Transform gameBoard;
    public Camera gameCamera;

    public GameObject[,] grid;

    public int width = 100;
    public int height = 100;

    protected int generation = 0;
    protected bool isPlaying = false;

    protected float counter = 0f;

    // All for cells working
    public GameObject cellPrefab;

    private void Awake()
    {
        grid = new GameObject[width, height];
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

    void UpdateCells()
    {
        generation++;
        List<Vector2Int> toBeAlive = new List<Vector2Int>();
        List<Vector2Int> toBeDead = new List<Vector2Int>();

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                int neighbours = GetNeighbours(i, j);
                if (grid[i, j] != null)
                {
                    if (neighbours < 2 || neighbours > 3)
                        toBeDead.Add(new Vector2Int(i, j));
                    else
                    {
                        if (neighbours == 3)
                            toBeAlive.Add(new Vector2Int(i, j));
                    }
                }
            }

        foreach (var position in toBeAlive)
            CreateCell(position);
            
        foreach (var position in toBeDead)
            DestroyCell(position);

        GameOfLifeManager.instance.genText.text = $"Generation: {generation}";
    }

    // Method for one-step updating
    public virtual void NextStep() => UpdateCells();

    /// <summary>
    /// Resets board
    /// </summary>
    public virtual void ResetCells()
    {
        generation = 0;
        StopSim();

        List<Vector2Int> toBeDead = new List<Vector2Int>();

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (grid[i, j] != null)
                    DestroyCell(new Vector2Int(i, j));

        Array.Clear(grid, 0, grid.Length);
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

    private int GetNeighbours(int x, int y)
    {
        int neighbours = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            // Checking borders
            if (i < 0 || i >= width)
                continue;

            for (int j = y - 1; j <= y + 1; j++)
            {
                // Checking borders
                if (j < 0 || j >= height)
                    continue;

                // Not counting current cell
                if (i == x && j == y)
                    continue;

                // From warmer cell to less warm
                if (grid[i, j] != null)
                    neighbours++;
            }
        }

        return neighbours;
    }

    private void UpdateMaterial(Vector2Int cellPos)
    {
        try
        {
            GameObject cell = grid[cellPos.x, cellPos.y];
            if (cell == null)
                CreateCell(cellPos);
            else
                DestroyCell(cellPos);
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
        GameObject newCell = Instantiate(cellPrefab);
        newCell.transform.SetParent(gameBoard);
        // Moving on (0.5, 0.5) to set center of cell correctly
        newCell.transform.position = cellPosition + new Vector2(0.5f, 0.5f);
        grid[cellPosition.x, cellPosition.y] = newCell;
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
    }
}
