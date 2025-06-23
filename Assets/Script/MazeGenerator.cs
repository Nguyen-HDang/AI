using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    public int width = 21;  
    public int height = 21; 

    public int[,] grid;

    private System.Random rand = new System.Random();

    public Vector2Int start = new Vector2Int(1, 1);
    public Vector2Int end;

    void Start()
    {
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        end = new Vector2Int(width - 2, height - 2); 

        GenerateMazeData();
        AddExtraPaths(20); 
        GenerateMaze();
    }

    void GenerateMazeData()
    {
        grid = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = 1;

        List<Vector2Int> walls = new List<Vector2Int>();

        grid[start.x, start.y] = 0;

        AddWalls(start, walls);

        while (walls.Count > 0)
        {
            int idx = rand.Next(walls.Count);
            Vector2Int wall = walls[idx];
            walls.RemoveAt(idx);

            Vector2Int[] neighbors = GetAdjacentCells(wall);

            if (neighbors.Length == 2)
            {
                Vector2Int cellA = neighbors[0];
                Vector2Int cellB = neighbors[1];

                bool cellAIsPath = IsInBounds(cellA) && grid[cellA.x, cellA.y] == 0;
                bool cellBIsPath = IsInBounds(cellB) && grid[cellB.x, cellB.y] == 0;

                if (cellAIsPath ^ cellBIsPath) 
                {
                    grid[wall.x, wall.y] = 0;
                    Vector2Int newCell = cellAIsPath ? cellB : cellA;
                    grid[newCell.x, newCell.y] = 0;

                    AddWalls(newCell, walls);
                }
            }
        }
    }

    void AddWalls(Vector2Int cell, List<Vector2Int> walls)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 2),
            new Vector2Int(0, -2),
            new Vector2Int(2, 0),
            new Vector2Int(-2, 0)
        };

        foreach (var dir in directions)
        {
            Vector2Int wallPos = new Vector2Int(cell.x + dir.x / 2, cell.y + dir.y / 2);
            Vector2Int nextCell = new Vector2Int(cell.x + dir.x, cell.y + dir.y);

            if (IsInBounds(nextCell) && grid[nextCell.x, nextCell.y] == 1)
            {
                if (IsInBounds(wallPos) && grid[wallPos.x, wallPos.y] == 1)
                {
                    if (!walls.Contains(wallPos))
                        walls.Add(wallPos);
                }
            }
        }
    }

    Vector2Int[] GetAdjacentCells(Vector2Int wall)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        if (wall.x % 2 == 1)
        {
            cells.Add(new Vector2Int(wall.x, wall.y - 1));
            cells.Add(new Vector2Int(wall.x, wall.y + 1));
        }
        else 
        {
            cells.Add(new Vector2Int(wall.x - 1, wall.y));
            cells.Add(new Vector2Int(wall.x + 1, wall.y));
        }
        return cells.ToArray();
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    void AddExtraPaths(int count)
    {
        int attempts = 0;
        int maxAttempts = count * 10;

        while (count > 0 && attempts < maxAttempts)
        {
            attempts++;

            int x = rand.Next(1, width - 1);
            int y = rand.Next(1, height - 1);

            if (grid[x, y] == 1)
            {
                int openNeighbors = 0;
                if (IsInBounds(new Vector2Int(x - 1, y)) && grid[x - 1, y] == 0) openNeighbors++;
                if (IsInBounds(new Vector2Int(x + 1, y)) && grid[x + 1, y] == 0) openNeighbors++;
                if (IsInBounds(new Vector2Int(x, y - 1)) && grid[x, y - 1] == 0) openNeighbors++;
                if (IsInBounds(new Vector2Int(x, y + 1)) && grid[x, y + 1] == 0) openNeighbors++;

                if (openNeighbors >= 2)
                {
                    grid[x, y] = 0;
                    count--;
                }
            }
        }
    }

    void GenerateMaze()
    {
        float tileSize = 1.0f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, y * tileSize, 0);
                if (grid[x, y] == 0)
                {
                    Instantiate(floorPrefab, pos, Quaternion.identity);
                }
                else
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity);
                }
            }
        }

        Vector3 startPos = new Vector3(start.x * tileSize, start.y * tileSize, 0);
        Vector3 endPos = new Vector3(end.x * tileSize, end.y * tileSize, 0);

        var startMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        startMarker.transform.position = startPos + Vector3.up * 0.5f;
        startMarker.GetComponent<Renderer>().material.color = Color.green;
        startMarker.name = "Start";

        var endMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        endMarker.transform.position = endPos + Vector3.up * 0.5f;
        endMarker.GetComponent<Renderer>().material.color = Color.red;
        endMarker.name = "End";
    }
}
