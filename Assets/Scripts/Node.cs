using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Node
{

    public enum NodeState
    {
        StateOpen,
        StateClosed,
        StateNone
    };

    public int c;
    public int f;
    public int g;
    public int h;

    public bool walkable;

    public Vector3 position;
    public Vector3 parent;

    public NodeState state;

    public void Clear(Vector3 newPosition)
    {
        c = 1;
        f = 0;
        g = 0;
        h = 0;
        parent.Set(-1, -1, -1);
        position = newPosition;
        walkable = true;
        state = NodeState.StateNone;
    }
}

[System.Serializable]
public class Map
{
    public List<Node> mapNodes = new List<Node>();
    public int mapWidth;
    public int mapHeight;
    public int mapDepth;

    public void Clear()
    {
        for (int z = 0; z < mapDepth; ++z)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = 0; x < mapWidth; ++x)
                {
                    GetNode(new Vector3(x, y, z)).Clear(new Vector3(x,y,z));
                }
            }
        }
    }

    public void Init(int width, int height, int depth)
    {
        mapWidth = width;
        mapHeight = height;
        mapDepth = depth;

        for (int z = 0; z < depth; ++z)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    mapNodes.Add(new Node());
                }
            }
        }
        Clear();
    }

    public Node GetNode(Vector3 np)
    {
        if (np.x < 0 || np.y < 0 || np.z < 0 || np.x >= mapWidth || np.y >= mapHeight || np.z >= mapDepth)
        {

            Debug.Log("Invalid node coordinate: " + np.x + "," + np.y + "," + np.z);
            return null;
        }

        return mapNodes[((int)np.z * mapWidth * mapHeight)
            + ((int)np.y * mapWidth)
            + (int)np.x];
        //(z * xMax * yMax) + (y * xMax) + x;
    }
}
