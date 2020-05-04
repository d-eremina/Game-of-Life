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
            Vector3 pos = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistancePlacement));
            Vector3Int boardPos = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
            selectedCell.transform.position = boardPos;
            if (boardPos.x >= 0 && boardPos.x <= width && boardPos.y >= 0 && boardPos.y <= height && boardPos.z >= 0 && boardPos.z <= depth)
            {
                selectedCell.SetActive(true);
            }
            else
            {
                selectedCell.SetActive(false);
            }
            bool mouseClicked = Input.GetMouseButtonDown(0);
            if (mouseClicked)
            {
                ChangeCell(boardPos);
            }
        }
    }

    private void ChangeCell(Vector3Int cellPos)
    {
        try
        {
            GameObject cell = grid[cellPos.x, cellPos.y, cellPos.z];
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

    private void CreateCell(Vector3Int cellPos)
    {
        GameObject newCell = Instantiate(cellPrefab);
        newCell.transform.SetParent(gameBoard);
        newCell.transform.position = cellPos;
        grid[cellPos.x, cellPos.y, cellPos.z] = newCell;
    }

    private void DestroyCell(Vector3Int cellPos)
    {
        GameObject deadCell = grid[cellPos.x, cellPos.y, cellPos.z];
        if (deadCell != null)
        {
            Destroy(deadCell);
        }
        grid[cellPos.x, cellPos.y, cellPos.z] = null;
    }
}
