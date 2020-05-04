using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife3D : GameOfLife2D
{
    private GameObject[,,] grid;
    private int width3D = 24;
    private int height3D = 24;
    private int depth = 24;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[width3D, height3D, depth];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
