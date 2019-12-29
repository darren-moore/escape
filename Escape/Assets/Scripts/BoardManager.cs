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
        public bool room;
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

        public bool Exists(Cell[,] grid) {

            int length = (int)Math.Sqrt(grid.Length);
            return position.x > -1 && position.x < length && position.y > -1 && position.y < length;
        }

        public List<Cell> GetNeighbours(Cell[,] grid) {

            // Adjacent cells that aren't null (they exist on the floor).
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
    private const int LargeRoomCount = Rows / 2;

    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] rooms;
    private GameObject[,] board;
    private Transform boardHolder;
    protected Cell[,] grid;

    void SetupGrid() {

        grid = new Cell[Cols, Rows];

        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                grid[i, j] = new Cell(i, j);
    }

    void DesignMaze() {
        
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

                DrawCell(x, y);
            }
        }
    }

    void DrawCell(int x, int y) {

        bool north = !grid[x, y].walls[0];
        bool west = !grid[x, y].walls[1];
        bool east = !grid[x, y].walls[2];
        bool south = !grid[x, y].walls[3];
        GameObject roomTile;

        if (north && !west && !east && !south)
            roomTile = rooms[0];

        else if (!north && !west && east && !south)
            roomTile = rooms[1];

        else if (!north && !west && !east && south)
            roomTile = rooms[2];

        else if (!north && west && !east && !south)
            roomTile = rooms[3];
        
        else if (!north && west && east && !south)
            roomTile = rooms[4];

        else if (north && !west && !east && south)
            roomTile = rooms[5];

        else if (north && !west && east && !south)
            roomTile = rooms[6];

        else if (!north && !west && east && south)
            roomTile = rooms[7];

        else if (!north && west && !east && south)
            roomTile = rooms[8];

        else if (north && west && !east && !south)
            roomTile = rooms[9];

        else if (north && west && east && !south)
            roomTile = rooms[10];

        else if (north && !west && east && south)
            roomTile = rooms[11];

        else if (!north && west && east && south)
            roomTile = rooms[12];

        else if (north && west && !east && south)
            roomTile = rooms[13];

        else
            roomTile = rooms[14];

        Vector2 position = new Vector2(x * RoomWidth, y * RoomLength);
        board[x, y] = Instantiate(roomTile, position, Quaternion.identity) as GameObject;
        board[x, y].name = "Cell " + (x * RoomWidth + y);
        board[x, y].transform.SetParent(boardHolder.transform);
    }

    void TileInBox(int x, int y, int length, int width, int xOffset, int yOffset) {

        for (int i = 0; i < length; i++) {
            for (int j = 0; j < width; j++) {

                Vector2 position = new Vector2(x * RoomWidth + i + xOffset, y * RoomLength + j + yOffset);
                GameObject floorTile = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(floorTile, position, Quaternion.identity) as GameObject;
                instance.transform.SetParent(board[x, y].transform.GetChild(1));
            }
        }
    }

    void DrawRooms() {

        DrawRoom(0, 0);

        for (int i = 1; i < LargeRoomCount; i++) {

            // Choose random (x, y) coordinates in grid
            // DrawRoom(x, y);
        }
    }

    void DrawRoom(int x, int y) {

        grid[x, y].walls[0] = false;
        grid[x, y].walls[2] = false;

        grid[x, x + 1].walls[3] = false;
        grid[x, x + 1].walls[2] = false;

        grid[x + 1, y].walls[1] = false;
        grid[x + 1, y].walls[0] = false;

        grid[x + 1, x + 1].walls[3] = false;
        grid[x + 1, x + 1].walls[1] = false;

        // Destroy inner walls

    }

    public void SetupScene() {

        SetupGrid();
        DrawRooms();
        DesignMaze();
        DrawMaze();
    }
}