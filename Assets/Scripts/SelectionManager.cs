using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private static Transform _selectedTransform;
    private static readonly int Selected = Animator.StringToHash("Selected");

    public Spawn spawner;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var clickPos = Vector2Int.RoundToInt(mousePos);
            // var hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
            // hit.collider != null
            if (0 <= clickPos.x && clickPos.x < Ball.Width && 0 <= clickPos.y && clickPos.y < Ball.Height)
            {
                if (!Ball.IsSlotEmpty(clickPos))
                {
                    var ballTransform = Ball.GetBallOnGrid(clickPos);
                    if (ballTransform != _selectedTransform)
                    {
                        if (_selectedTransform != null)
                            _selectedTransform.GetComponent<Animator>().SetBool(Selected, false);
                        _selectedTransform = ballTransform;
                    }
                    ballTransform.GetComponent<Animator>().SetBool(Selected, true);
                }
                else
                {
                    // TODO: verify valid move
                    if (_selectedTransform.GetComponent<Ball>().MoveTo(clickPos))
                    {
                        _selectedTransform.GetComponent<Animator>().SetBool(Selected, false);
                        _selectedTransform = null;
                    }
                    else
                    {
                        // play error sound
                    }
                }
            }
            // Ball.PrintGrid();
            spawner.SpawnNext();
        }
    }
}
