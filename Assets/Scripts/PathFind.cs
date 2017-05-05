using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PathFind : MonoBehaviour {

    bool pathFound;

    Map worldMap = new Map();

    RaycastHit[] nodeResults = new RaycastHit[10];

    List<Vector3> openList = new List<Vector3>();
    List<Vector3> moveTargets = new List<Vector3>();

    public Vector3 worldSize;
    public Vector3 targetPos;

    Rigidbody cubeRB;

    int m_positionIndex = 0;

    // Use this for initialization
    void Start ()
    {
        cubeRB = GetComponent<Rigidbody>();
        pathFound = false;
        openList.Clear();
        worldMap.Init((int)worldSize.x, (int)worldSize.y, (int)worldSize.z);
        FindPath(targetPos);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Vector3.Distance(transform.position, moveTargets[m_positionIndex]) < 0.3f && pathFound)
        {
            ++m_positionIndex;
        }

        if (m_positionIndex == moveTargets.Count - 1)
        {
            pathFound = false;
            moveTargets.Clear();
            moveTargets.Add(targetPos);
            m_positionIndex = 0;
        }

        cubeRB.MovePosition(transform.position + (moveTargets[m_positionIndex] - transform.position) * Time.deltaTime);
    }

    void FindPath(Vector3 targetNode)
    {
        worldMap.Clear();
        openList.Add(transform.position);

        Node smallestFNode;

        while (openList.Count > 0 && !pathFound)
        {
            int smallestFNodeIndex = 0;
            for (int fNodeIndex = 0; fNodeIndex < openList.Count; ++fNodeIndex)
            {
                Node node1 = worldMap.GetNode(openList[fNodeIndex]);
                Node node2 = worldMap.GetNode(openList[smallestFNodeIndex]);

                if (node1.f < node2.f)
                {
                    smallestFNodeIndex = fNodeIndex;
                }
            }

            smallestFNode = worldMap.GetNode(openList[smallestFNodeIndex]);
            worldMap.GetNode(openList[smallestFNodeIndex]).position = openList[smallestFNodeIndex];
            //Debug.Log(worldMap.GetNode(openList[smallestFNodeIndex]).position);

            for (int oz = -1; oz < 2; ++oz)
            {
                for (int oy = -1; oy < 2; ++oy)
                {
                    for (int ox = -1; ox < 2; ++ox)
                    {
                        if (ox == 0 && oy == 0 && oz == 0)
                            continue;

                        Vector3 adjacentNodePos = smallestFNode.position + new Vector3(ox, oy, oz);

                        int nodeResultsSize = Physics.BoxCastNonAlloc(adjacentNodePos, new Vector3(0.5f, 0.5f, 0.5f), Vector3.forward, nodeResults, Quaternion.identity, 1f);
                        if (nodeResultsSize > 0)
                        {
                            Debug.Log(nodeResultsSize);
                            AdjacentNode(adjacentNodePos).walkable = false;
                        }

                        if (!AdjacentNode(adjacentNodePos).walkable)
                            continue;

                        int newG = smallestFNode.g + AdjacentNode(adjacentNodePos).c;

                        if ((int)AdjacentNode(adjacentNodePos).state == 1)
                        {
                            Debug.Log("Closed");
                            continue;
                        }
                        else if ((int)AdjacentNode(adjacentNodePos).state == 0 && newG < AdjacentNode(adjacentNodePos).g)
                        {
                            Debug.Log("open");
                            AdjacentNode(adjacentNodePos).g = newG;
                            AdjacentNode(adjacentNodePos).h = (int)(Mathf.Abs(AdjacentNode(adjacentNodePos).position.x - targetNode.x) + Mathf.Abs(AdjacentNode(adjacentNodePos).position.y - targetNode.y) + Mathf.Abs(AdjacentNode(adjacentNodePos).position.z - targetNode.z) + (AdjacentNode(adjacentNodePos).position - targetNode).magnitude);
                            AdjacentNode(adjacentNodePos).parent = smallestFNode.position;
                            AdjacentNode(adjacentNodePos).f = AdjacentNode(adjacentNodePos).g + AdjacentNode(adjacentNodePos).h;
                        }
                        else if ((int)AdjacentNode(adjacentNodePos).state == 2)
                        {
                            Debug.Log("none");
                            AdjacentNode(adjacentNodePos).g = newG;
                            AdjacentNode(adjacentNodePos).h = (int)(Mathf.Abs(AdjacentNode(adjacentNodePos).position.x - targetNode.x) + Mathf.Abs(AdjacentNode(adjacentNodePos).position.y - targetNode.y) + Mathf.Abs(AdjacentNode(adjacentNodePos).position.z - targetNode.z) + (AdjacentNode(adjacentNodePos).position - targetNode).magnitude);
                            AdjacentNode(adjacentNodePos).parent = smallestFNode.position;
                            AdjacentNode(adjacentNodePos).f = AdjacentNode(adjacentNodePos).g + AdjacentNode(adjacentNodePos).h;
                            AdjacentNode(adjacentNodePos).state = Node.NodeState.StateOpen;
                            openList.Add(AdjacentNode(adjacentNodePos).position);
                        }

                        if (AdjacentNode(adjacentNodePos).position == targetNode)
                            pathFound = true;
                    }
                }
            }
            openList.Remove(openList[smallestFNodeIndex]);
            smallestFNode.state = Node.NodeState.StateClosed;
        }

        Node nodeToDebug = AdjacentNode(targetNode);
        for (int i = 0; i < 10; ++i)
        {
            Debug.Log(nodeToDebug.parent);
            nodeToDebug = AdjacentNode(nodeToDebug.parent);
            moveTargets.Add(nodeToDebug.parent);
        }
        moveTargets.Reverse();
    }

    Node AdjacentNode (Vector3 nodePos)
    {
        return worldMap.GetNode(nodePos);
    }
}
