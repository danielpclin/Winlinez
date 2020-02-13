using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject[] balls;

    // Start is called before the first frame update
    void Start()
    {
        // SpawnNext();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnNext()
    {
        Instantiate(balls[0]);
        // Instantiate(Balls[])
    }
}
