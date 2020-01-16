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
            room = false;

            for (int i = 0; i < 4; i++)
                walls[i] = true;
        }

        public Cell(Cell template) {

            this.position = template.position;
            this.visited = template.visited;
            this.walls = template.walls;
            this.room = template.room;
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
    private Vector2[] largeRoomPos;

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
        GameObject room;

        if (north && !west && !east && !south)
            room = rooms[0];

        else if (!north && !west && east && !south)
            room = rooms[1];

        else if (!north && !west && !east && south)
            room = rooms[2];

        else if (!north && west && !east && !south)
            room = rooms[3];
        
        else if (!north && west && east && !south)
            room = rooms[4];

        else if (north && !west && !east && south)
            room = rooms[5];

        else if (north && !west && east && !south)
            room = rooms[6];

        else if (!north && !west && east && south)
            room = rooms[7];

        else if (!north && west && !east && south)
            room = rooms[8];

        else if (north && west && !east && !south)
            room = rooms[9];

        else if (north && west && east && !south)
            room = rooms[10];

        else if (north && !west && east && south)
            room = rooms[11];

        else if (!north && west && east && south)
            room = rooms[12];

        else if (north && west && !east && south)
            room = rooms[13];

        else
            room = rooms[14];

        Vector2 position = new Vector2(x * RoomWidth, y * RoomLength);
        board[x, y] = Instantiate(room, position, Quaternion.identity) as GameObject;
        board[x, y].name = "Cell " + (x * Cols + y);
        board[x, y].transform.SetParent(boardHolder.transform);
    }

    void DrawRooms() {

        largeRoomPos = new Vector2[LargeRoomCount];

        // Starting room
        DrawRoom(0, 0);

        largeRoomPos[0] = new Vector2(0, 0);

        for (int i = 1; i < LargeRoomCount; i++) {

            Vector2 position = GetRandomPos();
            print(position.x + " " + position.y);
            DrawRoom((int)(position.x), (int)(position.y));
            largeRoomPos[i] = position;
        }
    }

    void DrawRoom(int x, int y) {

        grid[x, y].walls[0] = false;
        grid[x, y].walls[2] = false;

        grid[x, y + 1].walls[3] = false;
        grid[x, y + 1].walls[2] = false;

        grid[x + 1, y].walls[1] = false;
        grid[x + 1, y].walls[0] = false;

        grid[x + 1, y + 1].walls[3] = false;
        grid[x + 1, y + 1].walls[1] = false;
    }

    public void CompleteRooms() {

        GameObject room;
        GameObject toDestroy;

        for (int i = 0; i < LargeRoomCount; i++) {

            int x = (int)(largeRoomPos[i].x);
            int y = (int)(largeRoomPos[i].y);

            room = board[x, y];
            toDestroy = room.transform.GetChild(0).Find("NE Corner").gameObject;
            DestroyImmediate(toDestroy);

            room = board[x + 1, y];
            toDestroy = room.transform.GetChild(0).Find("NW Corner").gameObject;
            DestroyImmediate(toDestroy);

            room = board[x, y + 1];
            toDestroy = room.transform.GetChild(0).Find("SE Corner").gameObject;
            DestroyImmediate(toDestroy);

            room = board[x + 1, y + 1];
            toDestroy = room.transform.GetChild(0).Find("SW Corner").gameObject;
            DestroyImmediate(toDestroy);

            TileInBox(x, y, 4, 6, 2, 1);
        }
    }

    void TileInBox(int x, int y, int length, int width, int xOffset, int yOffset) {

        for (int i = 0; i < length; i++) {
            for (int j = 0; j < width; j++) {

                Vector2 position = new Vector2(x * RoomWidth + i + xOffset, y * RoomLength + j + yOffset + 0.5f);
                GameObject floorTile = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(floorTile, position, Quaternion.identity) as GameObject;
                instance.transform.SetParent(board[x, y].transform.GetChild(1));
            }
        }
    }

    Vector2 GetRandomPos() {

        int x, y;

        do {

            x = (int)Random.Range(0, Cols - 1);
            y = (int)Random.Range(0, Rows - 1);
        }
        while (grid[x, y].room);

        return new Vector2(x, y);
    }

    public void SetupScene() {

        SetupGrid();
        DrawRooms();
        DesignMaze();
        DrawMaze();
        CompleteRooms();
    }
}