using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife3D : GameOfLife2D
{
    [SerializeField]
    private GameObject[,,] grid;

    private int width = 24;
    private int height = 24;
    public int depth = 24;

    public GameObject selectedCell;
    public float cameraDistancePlacement = 10.0f;

    // Start is called before the first frame update
    void Awake()
    {
        grid = new GameObject[width, height, depth];
        selectedCell = Instantiate(selectedCell);
        selectedCell.transform.SetParent(gameBoard);
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
            // Transforms a point from screen space into world space
            Vector3 pos = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistancePlacement));

            // Round to get values for creating 3D cell object in grid
            Vector3Int boardPos = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));

            // Setting position of selection indicator to light it up 
            selectedCell.transform.position = boardPos;

            // Checking borders
            if (boardPos.x >= 0 && boardPos.x <= width && boardPos.y >= 0 && boardPos.y <= height && boardPos.z >= 0 && boardPos.z <= depth)
                selectedCell.SetActive(true);
            // Don't show prefab if user is out of borders
            else
                selectedCell.SetActive(false);

            // For creating new cell
            bool mouseClicked = Input.GetMouseButtonDown(0);
            if (mouseClicked)
                ChangeCellState(boardPos);
        }
    }

    protected override void UpdateCells()
    {
        generation++;

        List<Vector3Int> toBeAlive = new List<Vector3Int>();
        List<Vector3Int> toBeDead = new List<Vector3Int>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < depth; k++)
                {
                    bool cellIsAlive = grid[i, j, k] != null;
                    int numNeighbours = GetNeighbours(i, j, k);
                    if (cellIsAlive)
                    {
                        if (numNeighbours < 7 || numNeighbours > 12)
                            toBeDead.Add(new Vector3Int(i, j, k));
                    }
                    else
                    {
                        if (numNeighbours >= 10 && numNeighbours <= 12)
                            toBeAlive.Add(new Vector3Int(i, j, k));
                    }
                }
            }
        }

        foreach (Vector3Int cell in toBeAlive)
            CreateCell(cell);
        foreach (Vector3Int cell in toBeDead)
            DestroyCell(cell);

        GameOfLifeManager.instance.genText.text = $"Generation: {generation}";
    }

    /// <summary>
    /// Counts number of alive neighbours of cell
    /// </summary>
    /// <param name="x">x-coordinate of cell</param>
    /// <param name="y">y-coordinate of cell</param>
    /// <param name="z">z-coordinate of cell</param>
    /// <returns></returns>
    protected int GetNeighbours(int x, int y, int z)
    {
        int neighbours = 0;

        int minXRange = x > 0 ? -1 : 0;
        int maxXRange = x < width - 1 ? 1 : 0;
        int minYRange = y > 0 ? -1 : 0;
        int maxYRange = y < height - 1 ? 1 : 0;
        int minZRange = z > 0 ? -1 : 0;
        int maxZRange = z < depth - 1 ? 1 : 0;

        for (int i = minXRange; i <= maxXRange; i++)
            for (int j = minYRange; j <= maxYRange; j++)
                for (int k = minZRange; k <= maxZRange; k++)
                {
                    if (i == 0 && j == 0 && k == 0) // Don't count ourselves
                        continue;
                    bool neighbourIsAlive = grid[x + i, y + j, z + k] != null;
                    neighbours += neighbourIsAlive ? 1 : 0;
                }

        return neighbours;
    }

    public override void NextStep() => UpdateCells();

    public override void ResetCells()
    {
        generation = 0;
        StopSim();

        foreach (GameObject cell in grid)
            if (cell != null)
                Destroy(cell);

        Array.Clear(grid, 0, grid.Length);

        GameOfLifeManager.instance.genText.text = $"Generation: {generation}";
    }

    private void ChangeCellState(Vector3Int cellPos)
    {
        // Position might be out of range
        try
        {
            GameObject cell = grid[cellPos.x, cellPos.y, cellPos.z];
            // If cell on selected position is dead, we create a new one
            if (cell == null)
                CreateCell(cellPos);
            // If cell on selected position  is alive, we destroy it
            else
                DestroyCell(cellPos);
        }
        catch
        {
            return;
        }
    }

    /// <summary>
    /// Method for creating new cell in 3D scene
    /// </summary>
    /// <param name="cellPosition">Cell's position</param>
    private void CreateCell(Vector3Int cellPosition)
    {
        GameObject newCell = Instantiate(cellPrefab);
        newCell.transform.SetParent(gameBoard);
        newCell.transform.position = cellPosition;
        grid[cellPosition.x, cellPosition.y, cellPosition.z] = newCell;
    }

    /// <summary>
    /// Method for destroying a cell in 3D scene
    /// </summary>
    /// <param name="cellPosition">Cell's position</param>
    private void DestroyCell(Vector3Int cellPosition)
    {
        GameObject deadCell = grid[cellPosition.x, cellPosition.y, cellPosition.z];
        if (deadCell != null)
            Destroy(deadCell);
        grid[cellPosition.x, cellPosition.y, cellPosition.z] = null;
    }
}
