using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    private static Transform _selectedTransform;
    private static readonly int Selected = Animator.StringToHash("Selected");
    public Spawn spawner;
    public Text ScoreText;
    public Restart restartManager;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!Restart.GameOver && Input.GetMouseButtonDown(0))
        {
            if (Camera.main != null)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var clickPos = Vector2Int.RoundToInt(mousePos);
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
                    else if(_selectedTransform != null)
                    {
                        if (_selectedTransform.GetComponent<Ball>().MoveTo(clickPos, spawner))
                        {
                            _selectedTransform = null;
                        }
                    }
                }
            }
        }
        if (Ball.EmptySlotsCount == 0)
        {
            restartManager.Pause();
        }
        ScoreText.text = "Score: " + Ball.GetScore();
    }
}
