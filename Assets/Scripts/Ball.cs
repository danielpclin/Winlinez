using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Ball : MonoBehaviour
{
    public const int Width = 9;
    public const int Height = 9;
    private static readonly Transform[,] Grid = new Transform[Width, Height];
    private static readonly HashSet<int> EmptySlots = new HashSet<int>(Enumerable.Range(0, Width * Height));
    
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            AddToGrid();
        }
        catch (OccupiedSlotException)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddToGrid()
    { 
        var position = (Vector2Int) Vector3Int.RoundToInt(transform.position);
        if (IsSlotEmpty(position.x, position.y))
        {
            Grid[position.x, position.y] = transform;
            EmptySlots.Remove(PosToIndex(position));
        }
        else
        {
            throw new OccupiedSlotException("Piece Already in slot");
        }
    }

    private void RemoveFromGrid()
    {
        var position = (Vector2Int) Vector3Int.RoundToInt(transform.position);
        if (!IsSlotEmpty(position.x, position.y))
        {
            Grid[position.x, position.y] = null;
            EmptySlots.Add(PosToIndex(position));
        }
    }

    /// <returns>bool is_move_valid</returns>
    public bool MoveTo(Vector2Int pos)
    {
        var path = IsMoveValid(pos);
        if (path == null) return false;
        foreach (var VARIABLE in path)
        {
            Debug.Log(VARIABLE);
        }
        // if move is not valid return false
        RemoveFromGrid();
        transform.position = (Vector2)pos;
        AddToGrid();
        return true;
    }

    public static bool IsSlotEmpty(int x, int y)
    {
        return EmptySlots.Contains(PosToIndex(x, y)) && Grid[x, y] == null;
    }
    
    public static bool IsSlotEmpty(Vector2Int pos)
    {
        return IsSlotEmpty(pos.x, pos.y);
    }

    public static Transform GetBallOnGrid(Vector2Int pos)
    {
        return Grid[pos.x, pos.y];
    }

    public static int PosToIndex(Vector2Int pos)
    {
        return PosToIndex(pos.x, pos.y);
    }
    
    public static int PosToIndex(int x, int y)
    {
        return x * Width + y;
    }

    public LinkedList<Vector2Int> IsMoveValid(Vector2Int pos)
    {
        if (Grid[pos.x, pos.y] != null)
            return null;
        var goalIndex = PosToIndex(pos);
        var startIndex = PosToIndex(Vector2Int.RoundToInt(transform.position));
        var openSet = new HashSet<int>{startIndex};
        var cameFrom = new Dictionary<int, int>();
        var gScore = new Dictionary<int, int> {[startIndex] = 0};
        var fScore = new Dictionary<int, int> {[startIndex] = 16};
        Vector2Int[] directions = {Vector2Int.down, Vector2Int.up, Vector2Int.right, Vector2Int.left};
        while (openSet.Count > 0)
        {
            var currentIndex = openSet.First();
            foreach (var possibleIndex in openSet)
            {
                if ((fScore.ContainsKey(possibleIndex) ? fScore[possibleIndex] : 100000) < fScore[currentIndex])
                    currentIndex = possibleIndex;
            }
            if (currentIndex == goalIndex)
            {
                var totalPath = new LinkedList<Vector2Int>();
                totalPath.AddLast(IndexToPos(currentIndex));
                while (cameFrom.ContainsKey(currentIndex))
                {
                    // Debug.Log($"{IndexToPos(cameFrom[currentIndex])} => {IndexToPos(currentIndex)}");
                    currentIndex = cameFrom[currentIndex];
                    totalPath.AddFirst(IndexToPos(currentIndex));
                }
                return totalPath;
            }
            openSet.Remove(currentIndex);
            foreach (var direction in directions)
            {
                var neighborVector = IndexToPos(currentIndex) + direction;
                if (neighborVector.x < 0 || neighborVector.x >= Width || neighborVector.y < 0 || neighborVector.y >= Height || !IsSlotEmpty(neighborVector))
                    continue;
                var neighborIndex = PosToIndex(neighborVector);
                var tentativeGScore = gScore[currentIndex] + 1;
                if (tentativeGScore < (gScore.ContainsKey(neighborIndex) ? gScore[neighborIndex] : 100000))
                {
                    cameFrom[neighborIndex] = currentIndex;
                    gScore[neighborIndex] = tentativeGScore;
                    fScore[neighborIndex] = gScore[neighborIndex] + 16 - neighborVector.x - neighborVector.y;
                    if (!openSet.Contains(neighborIndex))
                    {
                        openSet.Add(neighborIndex);
                    }
                }
            }
        }
        return null;
    }

    public static Vector2Int IndexToPos(int index)
    {
        return new Vector2Int(index / Width, index % Height);
    }

    public static void PrintGrid()
    {            
        Debug.Log("==============START==============");
        foreach (var _transform in Grid)
        {
            if (_transform)
            {
                Debug.Log(_transform.position);
            }
        }
        Debug.Log("Count: " + EmptySlots.Count);
        Debug.Log("===============END===============");
    }

    [Serializable]
    class OccupiedSlotException : Exception
    {
        public OccupiedSlotException(){}
        public OccupiedSlotException(string message) : base(message) {}
    }
}
