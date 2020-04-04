using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLevel : Level
{

    //dx, dy, & index into the Connections.directions array
    ///Offsets to access neighbor cells
    private static Vector3[] NEIGHBORS = new Vector3[4] {new Vector3(1,0,0), new Vector3(0,1,1), new Vector3(0,-1,2), new Vector3(-1,0,3)};
    int mazeWidth;
    int mazeHeight;
    public Connections[,] cells;


    //Constructor
    public GridLevel(int width, int height)
    {
        mazeWidth = width;
        mazeHeight = height;
        cells = new Connections[width, height];


        for (int wTrav = 0; wTrav < mazeWidth; wTrav++)
        {
            for (int hTrav = 0; hTrav < mazeHeight; hTrav++)
            {//Init maze with empty cells
                cells[wTrav, hTrav] = new Connections();
            }
        }
    }


    //Set the starting cell for maze generation
    public override void startAt(Location loc)
    {
        cells[loc.x, loc.y].bIsInMaze = true;
    }

    private bool canPlaceCorridor(int x, int y)
    {///Check if the neighbor cell can be added to the maze (isn't already in the maze, isn't backtracking, isn't oob)
        return (x >= 0 && x < mazeWidth) && (y >= 0 && y < mazeHeight) && (!cells[x, y].bIsInMaze);
    }


    ///Shuffle modified from bslease
    ///         https://github.com/bslease/Procedural_Content/blob/93621be58df2e6f48a66ef0ff5b565b1191fb91f/Procedural%20Content%20Project/Assets/GridLevel.cs#L47
    void shuffleNeighbors(ref Vector3[] nieghbArray)
    {
        // Fisher-Yates shuffle
        int n = nieghbArray.Length;
        while (n > 1)
        {
            n--;
            int k = (int)Random.Range(0, n);
            Vector3 v = nieghbArray[k];
            nieghbArray[k] = nieghbArray[n];
            nieghbArray[n] = v;
        }
    }

    public override Location makeConnection(Location loc)
    {///Checks if the given cell can connect into any of its neighbors and does so then immediately returns if it finds one

        ///Copy neighbors into a temp arr
        Vector3[] _neighbors = (Vector3[])NEIGHBORS.Clone();
        shuffleNeighbors(ref _neighbors);///Commenting makes a square-wave-like maze

        //Host cell to check neighbors of
        int _x = loc.x;
        int _y = loc.y;

        foreach (Vector3 _v in _neighbors)
        {
            //Collect relative info of neighbor to be tested
            int dx = (int)_v.x;
            int dy = (int)_v.y;
            int dirn = (int)_v.z;

            //Get absolute pos of neighbor to be tested
            int nx = _x + dx;
            int ny = _y + dy;
            int fromDirn = 3 - dirn;



            if (canPlaceCorridor(nx, ny))
            {//If corridor can be placed, add that cell to the maze 
                cells[_x,_y].directions[dirn] = true;
                cells[nx, ny].bIsInMaze = true;
                cells[nx, ny].directions[fromDirn] = true;

                //Connection made, return and ignore other neighbors
                return new Location(nx, ny);
            }
        }
        ///Connection making finished
        return null;

    }


}
