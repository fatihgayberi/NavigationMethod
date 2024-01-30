using UnityEngine;
using System.Collections;

public class Node
{
    public Vector3 worldPosition;
    public int seekerGridX;
    public int seekerGridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(Vector3 _worldPos, int _seekerGridX, int _seekerGridY)
    {
        worldPosition = _worldPos;
        seekerGridX = _seekerGridX;
        seekerGridY = _seekerGridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
