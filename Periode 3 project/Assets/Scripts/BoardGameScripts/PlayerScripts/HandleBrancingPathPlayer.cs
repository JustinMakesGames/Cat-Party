using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleBrancingPathPlayer : MonoBehaviour
{

    public Vector3 minScale;
    public Vector3 maxScale;
    public float speed;

    private BrancingPathHandler brancingPathHandler;
    private bool _isScalingUp;
    private List<Transform> arrows = new List<Transform>();
    private Transform selectedArrow;
    private bool _isChoosingArrow;
    public void SetArrowsOn(List<Transform> arrows, BrancingPathHandler script)
    {
        brancingPathHandler = script;
        this.arrows = arrows;
        _isChoosingArrow = true;
        selectedArrow = arrows[0];

        StartCoroutine(HandlePlayerChoosingArrow());
    }

    public void SwitchArrow(InputAction.CallbackContext context)
    {
        if (_isChoosingArrow && context.started)
        {
            selectedArrow.localScale = new Vector3(3, 2, 2);
            if (selectedArrow == arrows[0])
            {
                selectedArrow = arrows[1];
            }

            else
            {
                selectedArrow = arrows[0];
            }
        }
    }

    public void SelectArrow(InputAction.CallbackContext context)
    {
        if (_isChoosingArrow && context.performed)
        {
            brancingPathHandler.PlayerSelectsArrow(selectedArrow);
            _isChoosingArrow = false;
        }
    }

    private IEnumerator HandlePlayerChoosingArrow()
    {
        while (_isChoosingArrow)
        {
            HandleSelectedArrow();
            yield return null;
        }
    }

    private void HandleSelectedArrow()
    {
        Vector3 targetScale = _isScalingUp ? maxScale : minScale;

        selectedArrow.localScale = Vector3.MoveTowards(selectedArrow.localScale, targetScale, Time.deltaTime * speed);

        if (Vector3.Distance(selectedArrow.localScale, targetScale) < 0.05f)
        {
            _isScalingUp = !_isScalingUp;
        }
    }
    
}
