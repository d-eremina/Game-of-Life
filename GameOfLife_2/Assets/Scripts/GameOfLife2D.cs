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
                //UpdateCells();
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

    private void UpdateMaterial(Vector2Int cellPos)
    {
        try
        {
            GameObject cell = grid[cellPos.x, cellPos.y];
            if (cell == null)
            {
                CreateCell(cellPos);
            }
            else
            {
                DestroyCell(cellPos);
            }
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
        {
            Destroy(deadCell);
        }
        grid[cellPosition.x, cellPosition.y] = null;
    }
}
