using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class Ball : MonoBehaviour
{
    public const int Width = 9;
    public const int Height = 9;
    private static int _score;
    private static readonly Random _random = new Random();
    private static readonly Transform[,] Grid = new Transform[Width, Height];
    private static readonly HashSet<int> EmptySlots = new HashSet<int>(Enumerable.Range(0, Width * Height));
    private static readonly int Selected = Animator.StringToHash("Selected");

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToGrid(Vector2Int position)
    { 
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

    private static void RemoveFromGrid(Vector2Int position)
    {
        if (!IsSlotEmpty(position.x, position.y))
        {
            Grid[position.x, position.y] = null;
            EmptySlots.Add(PosToIndex(position));
        }
    }

    public bool MoveTo(Vector2Int to, Spawn spawner)
    {
        var from = Vector2Int.RoundToInt(transform.position);
        var path = IsMoveValid(from, to);
        if (path == null)
        {
            EditorApplication.Beep();
            return false;
        }
        RemoveFromGrid(from);
        AddToGrid(to);
        StartCoroutine(MoveAnimation(path, spawner));
        return true;
    }

    IEnumerator MoveAnimation(LinkedList<Vector2Int> path, Spawn spawner)
    {
        foreach (var point in path)
        {
            transform.position = (Vector2)point;
            yield return new WaitForSeconds(0.025f);
        }
        transform.GetComponent<Animator>().SetBool(Selected, false);
        var positions = transform.GetComponent<Ball>().CheckMatch(true);
        if (positions.Count > 0)
        {
            ClearLines(positions);
        }
        else
        {
            spawner.SpawnNext();
        }
    }

    private static bool IsSlotEmpty(int x, int y)
    {
        return EmptySlots.Contains(PosToIndex(x, y)) && Grid[x, y] == null;
    }
    
    public static bool IsSlotEmpty(Vector2Int pos)
    {
        return IsSlotEmpty(pos.x, pos.y);
    }

    public static Vector2Int RandomEmptySlot()
    {
        return IndexToPos(EmptySlots.ElementAt(_random.Next(EmptySlots.Count)));
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

    public static Vector2Int IndexToPos(int index)
    {
        return new Vector2Int(index / Width, index % Height);
    }

    private LinkedList<Vector2Int> IsMoveValid(Vector2Int from, Vector2Int to)
    {
        if (Grid[to.x, to.y] != null)
            return null;
        var goalIndex = PosToIndex(to);
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
                    currentIndex = cameFrom[currentIndex];
                    if (currentIndex != PosToIndex(from))
                    {
                        totalPath.AddFirst(IndexToPos(currentIndex));
                    }
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

    public HashSet<Vector2Int> CheckMatch(bool countScore)
    {
        var color = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        var pos = Vector2Int.RoundToInt(transform.position);
        var positions = new HashSet<Vector2Int>();
        var count = 0;
        var currentCount = 0;
        HashSet<Vector2Int> currentPositions;
        Vector2Int currentPos;
        // 0 deg
        currentPositions = new HashSet<Vector2Int>{pos};
        currentCount = 1;
        currentPos = pos + Vector2Int.right;
        while (currentPos.x < Width && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.right;
        }
        currentPos = pos + Vector2Int.left;
        while (currentPos.x >= 0 && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.left;
        }
        if (currentCount >= 5)
        {
            count += currentCount;
            positions.UnionWith(currentPositions);
        }
        // 45 deg
        currentPositions = new HashSet<Vector2Int>{pos};
        currentCount = 1;
        currentPos = pos + Vector2Int.right + Vector2Int.up;
        while (currentPos.x < Width && currentPos.y < Height && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.right + Vector2Int.up;
        }
        currentPos = pos + Vector2Int.left + Vector2Int.down;
        while (currentPos.x >= 0 && currentPos.y >= 0 && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.left + Vector2Int.down;
        }
        if (currentCount >= 5)
        {
            count += currentCount;
            positions.UnionWith(currentPositions);
        }
        // 90 deg
        currentPositions = new HashSet<Vector2Int>{pos};
        currentCount = 1;
        currentPos = pos + Vector2Int.up;
        while (currentPos.y < Height && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.up;
        }
        currentPos = pos + Vector2Int.down;
        while (currentPos.y >= 0 && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.down;
        }
        if (currentCount >= 5)
        {
            count += currentCount;
            positions.UnionWith(currentPositions);
        }
        // 235 deg
        currentPositions = new HashSet<Vector2Int>{pos};
        currentCount = 1;
        currentPos = pos + Vector2Int.left + Vector2Int.up;
        while (currentPos.x >= 0 && currentPos.y < Height && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.left + Vector2Int.up;
        }
        currentPos = pos + Vector2Int.right + Vector2Int.down;
        while (currentPos.x < Width && currentPos.y >= 0 && !IsSlotEmpty(currentPos) && Grid[currentPos.x, currentPos.y].GetChild(0).GetComponent<SpriteRenderer>().color == color)
        {
            currentCount += 1;
            currentPositions.Add(currentPos);
            currentPos += Vector2Int.right + Vector2Int.down;
        }
        if (currentCount >= 5)
        {
            count += currentCount;
            positions.UnionWith(currentPositions);
        }
        if (countScore)
            AddScoreByCount(count);
        return positions;
    }

    public static void ClearLines(HashSet<Vector2Int> positions)
    {
        foreach (var position in positions)
        {
            Transform transform = Grid[position.x, position.y];
            Debug.Log(transform);
            RemoveFromGrid(position);
            Destroy(transform.gameObject);
        }
    }

    private static void AddScoreByCount(int count)
    {
        if (count < 5)
            return;
        _score += 10 + (count - 5) * (count - 5) * 2;
        Debug.Log(_score);
    }

    [Serializable]
    class OccupiedSlotException : Exception
    {
        public OccupiedSlotException(){}
        public OccupiedSlotException(string message) : base(message) {}
    }
}
