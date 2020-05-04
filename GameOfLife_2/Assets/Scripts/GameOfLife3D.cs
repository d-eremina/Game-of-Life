using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife3D : GameOfLife2D
{
    private GameObject[,,] grid;
    public int width3D = 24;
    public int height3D = 24;
    public int depth = 24;

    public GameObject selectedCell;
    public float cameraDistancePlacement = 10.0f;

    // Start is called before the first frame update
    void Awake()
    {
        grid = new GameObject[width3D, height3D, depth];
        selectedCell = Instantiate(selectedCell);
        selectedCell.transform.SetParent(gameBoard);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {

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

    private void ChangeCellState(Vector3Int cellPos)
    {
        // Position might be out of range
        try
        {
            GameObject cell = grid[cellPos.x, cellPos.y, cellPos.z];
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
