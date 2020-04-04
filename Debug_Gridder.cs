using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Debug_Gridder : MonoBehaviour
{
    public int mazeWidth;
    public int mazeHeight;
    GridLevel maze;
    public Location startCell = new Location(0,0);



    void Awake()
    {
        maze = new GridLevel(mazeWidth,mazeHeight);
        generateMaze(maze, startCell);

        GenerateHoudiniData();
    }

    ///From bslease
    ///     https://github.com/bslease/Procedural_Content/blob/93621be58df2e6f48a66ef0ff5b565b1191fb91f/Procedural%20Content%20Project/Assets/MazeMaker.cs#L139
    void generateMaze(Level level, Location start)
    {
        // a stack of locations we can branch from
        Stack<Location> locations = new Stack<Location>();
        locations.Push(start);
        level.startAt(start);

        while (locations.Count > 0)
        {
            Location current = locations.Peek();

            // try to connect to a neighboring location
            Location next = level.makeConnection(current);
            if (next != null)
            {
                // if successful, it will be our next iteration
                locations.Push(next);
            }
            else
            {
                locations.Pop();
            }
        }
    }


    void GenerateHoudiniData()
    {///Converts the internal maze representation into data that can be parsed by HEngine
        Mesh m = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = m;


        //Make arrs with a vert for each maze cell, and two Vec2's to split for each possible conenction the cell can make
        Vector3[] _verts  = new Vector3[maze.cells.Length]; 
        Vector2[] _connsA = new Vector2[maze.cells.Length];
        Vector2[] _connsB = new Vector2[maze.cells.Length];

        //Norms can probably be tossed
        Vector3[] _norms  = new Vector3[maze.cells.Length];
        ///IMPORTANT: If no tri's, mesh doesn't exist to HEngine
        int[]     _trias  = new   int[maze.cells.Length*3];



        //Set up garbage tri's so Unity can correctly export vert positions
        ///Tri's are immediately thrown away inside houdini and serve no purpose except to get point data
        for(int i = 0; i < _norms.Length; i++)
        {
            _norms[i] = Vector3.up;
            _trias[i] = i;
            _trias[i*2] = i;
            _trias[i*3] = i;
        }



        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                _verts[x + y*mazeWidth] = new Vector3((float)x, 0, (float)y);//Store position


                for (int _connDir = 0; _connDir <= 3; _connDir++)
                {///Loop through each cell's directions, convert bool to float, and store them

                    /////ISSUE: HEngine doesn't support Vec4 round-tripping, split into two Vec2 UV channels
                    if( _connDir <= 1)
                    {//Right - Top
                        _connsA[x + y*mazeWidth][_connDir    ] = maze.cells[x, y].directions[_connDir] ? 1 : 0;
                    } else
                    {//Left  - Down
                        _connsB[x + y*mazeWidth][_connDir - 2] = maze.cells[x, y].directions[_connDir] ? 1 : 0;
                    }
                } 

                ///Draw the cell locations
                Debug.DrawLine(_verts[x+y*mazeWidth], _verts[x+y*mazeWidth] + Vector3.up, Color.white, 7f);
            }
        }
    

        //Init mesh w/ maze data
        m.vertices  = _verts;
        m.normals   = _norms;
        m.triangles = _trias;
        
        //Store connection data in UV0 & UV1 channels
        m.SetUVs(0, _connsA);
        m.SetUVs(1, _connsB);

        m.Optimize();



        ///
        ///Mesh Data is sent to HEngine and processed
        ///


    }

    ///From bslease
    ///     https://github.com/bslease/Procedural_Content/blob/93621be58df2e6f48a66ef0ff5b565b1191fb91f/Procedural%20Content%20Project/Assets/MazeMaker.cs
    void Update()
    {
         for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Connections currentCell = maze.cells[x, y];
                if (maze.cells[x, y].bIsInMaze)
                { 
                    Vector3 cellPos = new Vector3(x, 0, y);
                    float lineLength = 1f;
                    Color _col = Random.ColorHSV();
                    if (currentCell.directions[0])
                    {
                        // positive x
                        Vector3 neighborPos = new Vector3(x + lineLength, 0, y);
                        Debug.DrawLine(cellPos, neighborPos, _col);
                    }
                    if (currentCell.directions[1])
                    {
                        // positive y
                        Vector3 neighborPos = new Vector3(x, 0, y + lineLength);
                        Debug.DrawLine(cellPos, neighborPos, _col);
                    }
                    if (currentCell.directions[2])
                    {
                        // negative y
                        Vector3 neighborPos = new Vector3(x, 0, y - lineLength);
                        Debug.DrawLine(cellPos, neighborPos, _col);
                    }
                    if (currentCell.directions[3])
                    {
                        // negative x
                        Vector3 neighborPos = new Vector3(x - lineLength, 0, y);
                        Debug.DrawLine(cellPos, neighborPos, _col);
                    }
                }
            }
        }
    }
    
}

