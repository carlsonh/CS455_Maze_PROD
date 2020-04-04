using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level
{
    public abstract void startAt(Location loc);
    public abstract Location makeConnection(Location loc);
}

public class Location
{
    public int x;
    public int y;

    public Location(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }
}

public class Connections
{
    public bool bIsInMaze = false;

                                        //  +X     +Y     -Y      -X
                                        // 
    public bool[] directions = new bool[4] {false, false, false, false};
}

