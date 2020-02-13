using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const int Width = 9;
    private const int Height = 9;
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
        var position = transform.position;
        var roundedX = Mathf.RoundToInt(position.x);
        var roundedY = Mathf.RoundToInt(position.y);
        if (Grid[roundedX, roundedY] == null)
        {
            Grid[roundedX, roundedY] = transform;
        }
        else
        {
            throw new OccupiedSlotException("Piece Already in slot");
        }
    }

    private void RemoveFromGrid()
    {
        var position = transform.position;
        var roundedX = Mathf.RoundToInt(position.x);
        var roundedY = Mathf.RoundToInt(position.y);
        Grid[roundedX, roundedY] = null;
    }

    private void MoveTo(Vector2Int pos, List<Vector2Int> path)
    {
        
    }

    public void MoveTo(Vector2Int pos)
    {
        
    }

    public static void PrintGrid()
    {            
        // Debug.Log("==============START==============");
        // foreach (var VARIABLE in Grid)
        // {
        //     Debug.Log(VARIABLE);
        // }
        // Debug.Log(EmptySlots.Count);
        // Debug.Log("===============END===============");
    }

    [Serializable]
    class OccupiedSlotException : Exception
    {
        public OccupiedSlotException()
        {
        }
        
        public OccupiedSlotException(string message) : base(message)
        {
        }
    }
}
