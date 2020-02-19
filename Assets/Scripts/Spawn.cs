using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Spawn : MonoBehaviour
{
    public GameObject[] ballsRef;
    public GameObject[] nextBallsRef;
    private GameObject[] _nextBalls = new GameObject[3];
    private int[] _nextBallsInt = new int[3];
    private static Random _random = new Random();
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnFirst();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnNext()
    {
        HashSet<Vector2Int> toClear = new HashSet<Vector2Int>();
        foreach (var i in _nextBallsInt)
        {
            var currentBall = Instantiate(ballsRef[i],  (Vector2)Ball.RandomEmptySlot(), Quaternion.identity).GetComponent<Ball>();
            currentBall.AddToGrid(Vector2Int.RoundToInt(currentBall.transform.position));
            var currentClear = currentBall.CheckMatch(false);
            toClear.UnionWith(currentClear);
        }
        
        Ball.ClearLines(toClear);
        PrepareNext();
    }

    private void SpawnFirst()
    {
        for (int i = 0; i < 5; i++)
        {
            var currentBall = Instantiate(ballsRef[_random.Next(7)],  (Vector2)Ball.RandomEmptySlot(), Quaternion.identity).GetComponent<Ball>();
            currentBall.AddToGrid(Vector2Int.RoundToInt(currentBall.transform.position));
        }
        PrepareNext();
    }

    private void PrepareNext()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_nextBalls[i] != null)
            {
                Destroy(_nextBalls[i]);
            }
            _nextBallsInt[i] = _random.Next(7);
            _nextBalls[i] = Instantiate(nextBallsRef[_nextBallsInt[i]],  new Vector3(3 + i, 9.5f, 0), Quaternion.identity);
        }
    }
}
