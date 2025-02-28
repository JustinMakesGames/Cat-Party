using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandleBrancingPathPlayer : MonoBehaviour
{

    public Vector3 minScale;
    public Vector3 maxScale;
    public float speed;
    public bool isPlayer;

    private BrancingPathHandler brancingPathHandler;
    private bool _isScalingUp;
    private List<Transform> arrows = new List<Transform>();
    private Transform selectedArrow;
    private bool _isChoosingArrow;

    //CPU Handling

    private List<Transform> pathFolders = new List<Transform>();
    private int _currentPlayerPlacement;
    private bool _isCPUChoosingArrow;
    public void SetArrowsOn(List<Transform> arrows, BrancingPathHandler script, Transform pathFolder1, Transform pathFolder2, int index)
    {
        if (GetComponent<PlayerHandler>().isPlayer)
        {
            isPlayer = true;
            _isChoosingArrow = true;
        }
        brancingPathHandler = script;
        this.arrows = arrows;
        
        selectedArrow = arrows[0];

        if (isPlayer)
        {
            StartCoroutine(SelectedArrowScaling());
        }

        else
        {
            pathFolders.Add(pathFolder1);
            pathFolders.Add(pathFolder2);
            _currentPlayerPlacement = index;
            _isCPUChoosingArrow = true;

            StartCoroutine(SelectedArrowScaling());
            StartCoroutine(HandleCPUChoosingArrow());
        }
        
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

    private IEnumerator SelectedArrowScaling()
    {
        while (_isChoosingArrow || _isCPUChoosingArrow)
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

    private IEnumerator HandleCPUChoosingArrow()
    {
        yield return new WaitForSeconds(2);
        selectedArrow = CalculateRightPath();
        yield return new WaitForSeconds(1);
        brancingPathHandler.PlayerSelectsArrow(selectedArrow);
        _isCPUChoosingArrow = false;

        pathFolders.Clear();
    }

    private Transform CalculateRightPath()
    {
        int path = int.MaxValue;
        int indexArrow = 0;

        for (int i = 0; i < pathFolders.Count; i++)
        {
            int index = 0;
            int nextPath = pathFolders[i] == GetComponent<HandleWalking>().pathFolder ? _currentPlayerPlacement : 0;

            int originalNextPath = nextPath;

            Transform pathFolder = pathFolders[i];
            while (true)
            {
                
                if (pathFolder.GetChild(nextPath).GetComponent<SpaceHandler>().isYarnPlace)
                {
                    if (path > index)
                    {
                        path = index;
                        indexArrow = i;
                    }
                    break;
                }

                else 
                {
                    index++;
                    nextPath = nextPath == (pathFolder.childCount - 1) ? 0 : nextPath + 1;

                    if (pathFolder.GetChild(nextPath).GetComponent<OneWayPath>() != null) 
                    {
                        int originalPath = nextPath;
                        nextPath = pathFolder.GetChild(nextPath).GetComponent<OneWayPath>().GetIndex();
                        pathFolder = pathFolder.GetChild(originalPath).GetComponent<OneWayPath>().GetRightPathFolder();
                    }
                    if (nextPath == originalNextPath)
                    {
                        break;
                    }
                }
            }
        }

        return arrows[indexArrow];
    }
    
}
