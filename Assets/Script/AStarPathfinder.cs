using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public MazeGenerator mazeGen;
    public Vector2Int startPos = new Vector2Int(1, 1);
    public Vector2Int targetPos = new Vector2Int(19, 19);

    Node[,] grid;

    public GameObject agentPrefab; 

    IEnumerator Start()
    {
        while (mazeGen == null || mazeGen.grid == null)
            yield return null;

        InitializeGrid();

        List<Node> path = FindPath(startPos, targetPos);

        if (path != null)
        {
            Debug.Log("Đã tìm thấy đường đi, bắt đầu di chuyển...");
            StartCoroutine(MoveAgent(path));
        }
        else
        {
            Debug.Log("Không tìm thấy đường đi!");
        }
    }

    void InitializeGrid()
    {
        int width = mazeGen.grid.GetLength(0);
        int height = mazeGen.grid.GetLength(1);
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Node(x, y, mazeGen.grid[x, y] == 0);
            }
        }
    }

    List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        Node startNode = grid[startPos.x, startPos.y];
        Node targetNode = grid[targetPos.x, targetPos.y];

        // Reset node
        foreach (Node node in grid)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.cameFrom = null;
        }

        List<Node> openList = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost ||
                    (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            if (currentNode == targetNode)
            {
                return ReconstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int tentativeGCost = currentNode.gCost + 1;

                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.cameFrom = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Node> ReconstructPath(Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != null)
        {
            path.Add(current);
            current = current.cameFrom;
        }

        path.Reverse();
        return path;
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int[,] directions = new int[,]
        {
            {1, 0}, {-1, 0}, {0, 1}, {0, -1}
        };

        for (int i = 0; i < 4; i++)
        {
            int nx = node.x + directions[i, 0];
            int ny = node.y + directions[i, 1];

            if (nx >= 0 && nx < grid.GetLength(0) && ny >= 0 && ny < grid.GetLength(1))
            {
                neighbors.Add(grid[nx, ny]);
            }
        }

        return neighbors;
    }

    int GetDistance(Node a, Node b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    IEnumerator MoveAgent(List<Node> path)
    {
        GameObject agent = Instantiate(agentPrefab);
        agent.transform.position = new Vector3(startPos.x, startPos.y, -1);

        foreach (Node node in path)
        {
            Vector3 targetPos = new Vector3(node.x, node.y, -1);

            while (Vector3.Distance(agent.transform.position, targetPos) > 0.01f)
            {
                agent.transform.position = Vector3.MoveTowards(agent.transform.position, targetPos, 5f * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Agent đã đi tới Target!");
    }
}
