using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Paths
{
    public Transform pathFolder;
    public Transform arrow;
}
public class BrancingPathHandler : SpaceHandler
{
    public List<Paths> pathFolders = new List<Paths>();
    [SerializeField] private Transform arrowSpawnPlace;
    [SerializeField] private GameObject arrow;

    private List<Transform> arrows = new List<Transform>();
    private bool _taskFinished;

    private int _currentIndex;
    private Transform _landedPlayer;
    public override async Task HandleAsyncLandedPlayer(Transform player, int currentIndex)
    {
        _currentIndex = currentIndex;
        _landedPlayer = player;

        SpawnArrows();
        GivePlayerControl();
        while (!_taskFinished)
        {
            await Task.Yield();
        }

        _taskFinished = false;
    }

    private void SpawnArrows()
    {
        for (int i = 0; i < pathFolders.Count; i++) 
        {
            HandleArrowPlacement(i, pathFolders[i].pathFolder);
        }
    }

    private void HandleArrowPlacement(int i, Transform path)
    {
        GameObject arrowClone = Instantiate(arrow, arrowSpawnPlace.position, Quaternion.identity);
        Vector3 lookRotation = Vector3.zero;
        if (path == _landedPlayer.GetComponent<HandleWalking>().pathFolder)
        {
            lookRotation = path.GetChild(_currentIndex + 1).position;          
        }

        else
        {
            lookRotation = path.GetChild(0).position;
        }

        lookRotation.y = arrowClone.transform.position.y;

        arrowClone.transform.LookAt(lookRotation);
        arrowClone.transform.position = arrowClone.transform.position + arrowClone.transform.forward * 3;
        arrows.Add(arrowClone.transform);

        pathFolders[i].arrow = arrowClone.transform;
    }

    private void GivePlayerControl()
    {
        _landedPlayer.GetComponent<HandleBrancingPathPlayer>().SetArrowsOn
            (arrows, this, pathFolders[0].pathFolder, pathFolders[1].pathFolder, _landedPlayer.GetComponent<HandleWalking>().currentlyOnThisSpace);
    }

    public void PlayerSelectsArrow(Transform rightArrow)
    {
        for (int i = 0; i < pathFolders.Count; i++)
        {
            if (pathFolders[i].arrow == rightArrow)
            {
                if (_landedPlayer.GetComponent<HandleWalking>().pathFolder != pathFolders[i].pathFolder)
                {
                    _landedPlayer.GetComponent<HandleWalking>().pathFolder = pathFolders[i].pathFolder;
                    _landedPlayer.GetComponent<HandleWalking>().currentlyOnThisSpace = -1;
                }
                
                DestroyArrows();
                break;
            }
        }

        _taskFinished = true;
    }

    private void DestroyArrows()
    {
        foreach (Transform a in arrows) 
        {
            Destroy(a.gameObject);
        
        }

        arrows.Clear();
   
    }
}
