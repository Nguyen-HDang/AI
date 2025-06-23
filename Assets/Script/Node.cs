using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public bool walkable;

    public int gCost;
    public int hCost;
    public Node cameFrom;

    public Node(int x, int y, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        gCost = int.MaxValue;
        hCost = 0;
        cameFrom = null;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}
