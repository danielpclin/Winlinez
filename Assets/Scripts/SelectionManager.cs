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
            var hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
            if (hit.collider != null)
            {
                var parent = hit.collider.gameObject.transform.parent;
                if (parent != _selectedTransform)
                {
                    if (_selectedTransform != null)
                        _selectedTransform.GetComponent<Animator>().SetBool(Selected, false);
                    _selectedTransform = parent;
                }
                parent.GetComponent<Animator>().SetBool(Selected, true);
            }
            Ball.PrintGrid();
            spawner.SpawnNext();
        }
    }
}
