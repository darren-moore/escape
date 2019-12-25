using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class BoardManager : MonoBehaviour {

    /*
     * Class used to store information from maze generation.
     */
    [Serializable]
    public class Cell {

        public Vector2 position;
        public bool visited;
        public bool[] walls = new bool[4];

        public Cell(int x, int y) {

            position = new Vector2(x, y);
            visited = false;

            for (int i = 0; i < 4; i++)
                walls[i] = true;
        }

        public Cell(Cell template) {

            this.position = template.position;
            this.visited = template.visited;
            this.walls = template.walls;
        }

        public List<Cell> GetNeighbours(Cell[,] grid) {

            // Adjacent cells that aren't null (they exist on the floor)
            List<Cell> neighbours = new List<Cell>();

            // [0]: North, [1]: East, [2]: South, [3]: West
            Cell[] adjacentCells = new Cell[4] { null, null, null, null };

            int x = (int)(position.x);
            int y = (int)(position.y);
            int length = (int)Math.Sqrt(grid.Length);

            if (y + 1 < length)
                adjacentCells[0] = new Cell(grid[x, y + 1]);

            if (x + 1 < length)
                adjacentCells[1] = new Cell(grid[x + 1, y]);

            if (y - 1 > -1)
                adjacentCells[2] = new Cell(grid[x, y - 1]);

            if (x - 1 > -1)
                adjacentCells[3] = new Cell(grid[x - 1, y]);

            for (int i = 0; i < 4; i++)
                if (adjacentCells[i] != null && !adjacentCells[i].visited)
                    neighbours.Add(adjacentCells[i]);

            return neighbours;
        }

        public Cell GetRandomNeighbour(Cell[,] grid) {

            List<Cell> neighbours = GetNeighbours(grid);

            if (neighbours.Count > 0) {

                int randomIndex = (int)(Random.Range(0, neighbours.Count));
                return neighbours[randomIndex];
            }  

            return null;
        }
    }

    private const int Rows = 6;
    private const int Cols = 6;
    private const int RoomLength = 9;
    private const int RoomWidth = 8;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    private GameObject[,] board;
    private Transform boardHolder;
    protected Cell[,] grid;

    void SetupGrid() {

        grid = new Cell[Cols, Rows];

        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                grid[i, j] = new Cell(i, j);
    }

    void CreateMaze() {
        
        List<Cell> queue = new List<Cell>();
        Cell currentRoom = grid[0, 0];

        do {

            currentRoom.visited = true;
            grid[(int)(currentRoom.position.x), (int)(currentRoom.position.y)] = new Cell(currentRoom);

            Cell nextRoom = currentRoom.GetRandomNeighbour(grid);

            if (nextRoom != null) {

                nextRoom.visited = true;
                queue.Add(currentRoom);
                RemoveWalls(currentRoom, nextRoom);
                currentRoom = nextRoom;
            }

            else if (queue.Count > 0) {

                currentRoom = queue[queue.Count - 1];
                queue.RemoveAt(queue.Count - 1);
            }

        } while (queue.Count > 0);
    }

    void RemoveWalls(Cell a, Cell b) {

        int colDiff = (int)(a.position.x) - (int)(b.position.x);
        int rowDiff = (int)(a.position.y) - (int)(b.position.y);

        if (colDiff == 1) {

            a.walls[1] = false;
            b.walls[2] = false;
        }

        else if (colDiff == -1) {

            a.walls[2] = false;
            b.walls[1] = false;
        }

        if (rowDiff == -1) {

            a.walls[0] = false;
            b.walls[3] = false;
        }

        else if (rowDiff == 1) {

            a.walls[3] = false;
            b.walls[0] = false;
        }
    }

    void DrawMaze() {

        board = new GameObject[Cols, Rows];
        boardHolder = new GameObject("Board").transform;

        // Iterate through each Cell in grid.
        for (int x = 0; x < Cols; x++) {
            for (int y = 0; y < Rows; y++) {

                board[x, y] = new GameObject("Cell");
                board[x, y].transform.position = new Vector2(x * Cols + RoomWidth / 2, y * Rows + RoomLength / 2);
                board[x, y].transform.SetParent(boardHolder);

                // Iterate through each tile in Cell.
                for (int i = 0; i < RoomWidth; i++)
                    for (int j = 0; j < RoomLength; j++)
                        if (i < 2 || i > 5 || j < 3 || j > 5)
                            DrawWall(x, y, i, j);

                DrawFloor(x, y);
            }
        }
    }

    void DrawWall(int x, int y, int i, int j) {

        GameObject wallTile = null;

        // [0]: North, [1]: West, [2]: East, [3]: South
        bool[] directions = GetDirection(x, y);

        // Northwest corner
        if (i == 0 && j == 6) {

            if (directions[0] && directions[1])
                wallTile = wallTiles[4];

            else if (directions[1] && directions[2])
                wallTile = wallTiles[1];

            else if (directions[0] && directions[3])
                wallTile = wallTiles[7];

            else if (directions[0])
                wallTile = wallTiles[7];

            else if (directions[1])
                wallTile = wallTiles[1];

            else
                wallTile = wallTiles[0];
        }

        // North wall
        else if ((i == 2 || i == 4) && j == 6 && !directions[0])
            wallTile = wallTiles[1];

        // Northeast corner
        else if (i == 6 && j == 6) {

            if (directions[0] && directions[2])
                wallTile = wallTiles[6];

            else if (directions[1] && directions[2])
                wallTile = wallTiles[1];

            else if (directions[0] && directions[3])
                wallTile = wallTiles[3];

            else if (directions[0])
                wallTile = wallTiles[3];

            else if (directions[2])
                wallTile = wallTiles[1];

            else
                wallTile = wallTiles[2];
        }

        // East wall
        else if (i == 6 && j == 3 && !directions[2])
            wallTile = wallTiles[3];

        // Southeast corner
        else if (i == 6 && j == 0) {

            if (directions[2] && directions[3])
                wallTile = wallTiles[0];

            else if (directions[1] && directions[2])
                wallTile = wallTiles[5];

            else if (directions[0] && directions[3])
                wallTile = wallTiles[3];

            else if (directions[2])
                wallTile = wallTiles[5];

            else if (directions[3])
                wallTile = wallTiles[3];

            else
                wallTile = wallTiles[4];
        }

        // South Wall
        else if ((i == 2 || i == 4) && j == 0 && !directions[3])
            wallTile = wallTiles[5];

        // Southwest Corner
        else if (i == 0 && j == 0) {

            if (directions[3] && directions[1])
                wallTile = wallTiles[2];

            else if (directions[1] && directions[2])
                wallTile = wallTiles[5];

            else if (directions[0] && directions[3])
                wallTile = wallTiles[7];

            else if (directions[3])
                wallTile = wallTiles[7];

            else if (directions[1])
                wallTile = wallTiles[5];

            else
                wallTile = wallTiles[6];
        }

        // West Wall
        else if (i == 0 && j == 3 && !directions[1])
            wallTile = wallTiles[7];

        if (wallTile == null)
            return;

        GameObject instance = Instantiate(wallTile, new Vector2(x * RoomWidth + i, y * RoomLength + j), Quaternion.identity) as GameObject;
        instance.transform.SetParent(board[x, y].transform);
    }

    void DrawFloor(int x, int y) {

        TileInBox(x, y, 4, 4, 2, 3);

        bool[] directions = GetDirection(x, y);

        if (directions.Contains(true))
            DrawHallFloor(x, y, directions);
    }

    void DrawHallFloor(int x, int y, bool[] directions) {

        // North hallway
        if (directions[0])
            TileInBox(x, y, 4, 2, 2, 7);

        // West hallway
        if (directions[1])
            TileInBox(x, y, 2, 3, 0, 3);

        // East hallway
        if (directions[2])
            TileInBox(x, y, 2, 3, 6, 3);

        // South hallway
        if (directions[3])
            TileInBox(x, y, 4, 3, 2, 0);
    }

    bool[] GetDirection(int x, int y) {

        // [0]: North, [1]: West, [2]: East, [3]: South
        bool[] directions = new bool[] { false, false, false, false };

        for (int i = 0; i < 4; i++)
            if (grid[x, y].walls[i] == false)
                directions[i] = true;

        return directions;
    }

    void TileInBox(int x, int y, int length, int width, int lengthOffset, int widthOffset) {

        for (int i = 0; i < length; i++) {
            for (int j = 0; j < width; j++) {

                Vector2 position = new Vector2(x * RoomWidth + i + lengthOffset, y * RoomLength + j + widthOffset);
                GameObject floorTile = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(floorTile, position, Quaternion.identity) as GameObject;
                instance.transform.SetParent(board[x, y].transform);
            }
        }
    }

    public void SetupScene() {

        SetupGrid();
        CreateMaze();
        DrawMaze();
    }
}