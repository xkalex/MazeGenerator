using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public enum cellType { Cell, Wall, Road }

public class Cell
{
    int x, y;
    public int X { get { return x; } }
    public int Y { get { return y; } }

    cellType type;
    public cellType Type { get { return type; } set { type = value; } }

    public Cell(int X, int Y, cellType type)
    {
        this.x = X;
        this.y = Y;
        this.Type = type;
    }
}

public class Maze
{

    public Cell[,] CellMatrix;
    public List<Cell> History = new List<Cell>();


    public Maze(int lenghtX, int lenghtY, int startX, int startY)
    {
        CellMatrix = new Cell[lenghtX, lenghtY];

        for(int x = 0; x < lenghtX; x++)
        {
            for (int y = 0; y < lenghtY; y++)
            {
                if ((x % 2 != 0 && y % 2 != 0) && (x < lenghtX - 1 && y < lenghtY - 1))  // Если ячейка нечетная и находится в пределах стен, то делаем её клеткой, иначе это стена.
                {
                    CellMatrix[x, y] = new Cell(x, y, cellType.Cell);
                }
                else
                {
                    CellMatrix[x, y] = new Cell(x, y, cellType.Wall);
                }
            }
        }
        Cell startCell = CellMatrix[startX, startY];
        History.Add(startCell);
        startCell.Type = cellType.Road;
    }

    public Cell getRandomEmptyNeighbour(Cell cell)
    {
        Cell[] allDir = new Cell[]  //Берем ячейки со всех сторон
        {
            (cell.X - 2 > 0) ? CellMatrix[cell.X - 2, cell.Y] : null,
            (cell.X + 2 < CellMatrix.GetLength(0)) ? CellMatrix[cell.X + 2, cell.Y] : null,
            (cell.Y - 2 > 0) ? CellMatrix[cell.X, cell.Y - 2] : null,
            (cell.Y + 2 < CellMatrix.GetLength(1)) ? CellMatrix[cell.X, cell.Y + 2] : null
        };

        allDir = allDir.Where(c => c != null && c.Type != cellType.Road).ToArray();

        return (allDir.Length > 0) ? allDir[Random.Range(0, allDir.Length)] : null;
    }

    public void removeWall(Cell cell1, Cell cell2)
    {
        CellMatrix[cell1.X, cell1.Y].Type = cellType.Road;
        CellMatrix[cell2.X, cell2.Y].Type = cellType.Road;
        if (cell1.X == cell2.X)
        {
            if(cell1.Y - cell2.Y == 2)
            {
                CellMatrix[cell1.X, cell1.Y - 1].Type = cellType.Road;
            }
            else
            {
                CellMatrix[cell1.X, cell1.Y + 1].Type = cellType.Road;
            }
        }
        else
        {
            if (cell1.X - cell2.X == 2)
            {
                CellMatrix[cell1.X - 1, cell1.Y].Type = cellType.Road;
            }
            else
            {
                CellMatrix[cell1.X + 1, cell1.Y].Type = cellType.Road;
            }
        }
    }
}
public class mazeGenerator : MonoBehaviour
{
    public GameObject cellPrefab;
    public int lenghtX;
    public int lenghtY;
    public int startX;
    public int startY;


void Start()
    {
        if (lenghtX < 7)
        {
            lenghtX = 7;
            Debug.LogWarning("Минимальная ширина лабиринта - 7 клеток! ");
        }
        if (lenghtY < 7)
        {
            lenghtY = 7;
            Debug.LogWarning("Минимальная высота лабиринта - 7 клеток! ");
        }

        Maze maze = new Maze(lenghtX, lenghtY, startX, startY);


        Cell curCell;
        Cell nextCell;

        do
        {

            curCell = maze.History.Last();
            nextCell = maze.getRandomEmptyNeighbour(curCell);
            if(nextCell != null)
            {
                maze.removeWall(curCell, nextCell);
                maze.History.Add(nextCell);
            }
            else
            {
                maze.History.RemoveAt(maze.History.Count - 1);
            }

        } while (maze.History.Count > 0);

        foreach (Cell cell in maze.CellMatrix)
        {
            if (cell.Type == cellType.Wall)
            {
                Instantiate(cellPrefab, new Vector3(cell.X, 0, cell.Y), Quaternion.identity); 
                // Работает только с префабами 1х1, при желании можно учитывать размер клетки, просто у меня сейчас нет возможности это тестировать.
                // Я бы заменил foreach на for и каждую итерацию учитывал бы ширину/длину префаба.
            }
        }
    }
}
