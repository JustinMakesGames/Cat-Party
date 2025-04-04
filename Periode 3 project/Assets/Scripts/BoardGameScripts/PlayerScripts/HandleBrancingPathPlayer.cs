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

    [SerializeField] private GameObject mapLookScreen;

    private BrancingPathHandler brancingPathHandler;
    private bool _isScalingUp;
    private List<Transform> arrows = new List<Transform>();
    private Transform selectedArrow;
    private bool _isChoosingArrow;
    private bool _canCheckMap;

    //CPU Handling

    private List<Transform> pathFolders = new List<Transform>();
    private int _currentPlayerPlacement;
    private bool _isCPUChoosingArrow;
    public void SetArrowsOn(List<Transform> arrows, BrancingPathHandler script, Transform pathFolder1, Transform pathFolder2, int index)
    {
        isPlayer = false;
        if (GetComponent<PlayerHandler>().isPlayer)
        {
            mapLookScreen.SetActive(true);
            isPlayer = true;
            _isChoosingArrow = true;
        }

        
        brancingPathHandler = script;
        this.arrows = arrows;
        
        selectedArrow = arrows[0];

        if (isPlayer)
        {
            _canCheckMap = true;
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

    public void HandleCancelMap()
    {
        mapLookScreen.SetActive(true);
        _isChoosingArrow = true;
        _canCheckMap = true;
        StartCoroutine(SelectedArrowScaling());
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
            mapLookScreen.SetActive(false);
            brancingPathHandler.PlayerSelectsArrow(selectedArrow);
            _canCheckMap = false;
            _isChoosingArrow = false;
        }
    }

    public void CheckMap(InputAction.CallbackContext context)
    {
        if (context.performed && _canCheckMap)
        {
            mapLookScreen.SetActive(false);
            _isChoosingArrow = false;
            _canCheckMap = false;
            GetComponent<PlayerMapHandling>().StartMapHandling(true);
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
        _canCheckMap = false;
        _isCPUChoosingArrow = false;

        pathFolders.Clear();
    }

    private Transform CalculateRightPath()
    {
        int path = int.MaxValue;
        int indexArrow = -1;

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

        float smallestDistance = Mathf.Infinity;
        if (indexArrow == -1)
        {
            for (int i = 0; i < pathFolders.Count; i++)
            {
                int nextPath = pathFolders[i] == GetComponent<HandleWalking>().pathFolder ? _currentPlayerPlacement + 1 : 0;

                Transform firstSpace = pathFolders[i].GetChild(nextPath);
                Transform allSpaces = GameObject.FindGameObjectWithTag("PathFolder").transform;
                Transform yarnPlace = null;
                foreach (Transform child in allSpaces)
                {
                    foreach (Transform grandChild in child)
                    {
                        if (grandChild.GetComponent<SpaceHandler>().isYarnPlace)
                        {
                            yarnPlace = grandChild;
                            break;
                        }
                    }

                    if (yarnPlace != null) break;
                    
                }

                print("The distance is " + Vector3.Distance(firstSpace.position, yarnPlace.position));
                print(pathFolders[i].GetChild(nextPath));
                if (Vector3.Distance(firstSpace.position, yarnPlace.position) < smallestDistance)
                {
                    indexArrow = i;
                    smallestDistance = Vector3.Distance(firstSpace.position, yarnPlace.position);
                }


            }
        }

        return arrows[indexArrow];
    }
    
}
