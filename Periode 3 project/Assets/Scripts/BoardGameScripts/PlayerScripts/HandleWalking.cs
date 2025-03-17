using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HandleWalking : MonoBehaviour
{
    public Transform pathFolder;

    public int currentlyOnThisSpace;
    [SerializeField] private float walkSpeed;
    [SerializeField] private Transform playerModel;
    private bool isDoneWalking;
    private bool _canChangeYValue;
    private float _yValue;


    private void Awake()
    {
        pathFolder = GameObject.FindGameObjectWithTag("PathFolder").transform.GetChild(0);
    }
    public async Task<int> StartHandlingWalking(int diceRoll, int currentSpace, TMP_Text text)
    {
        Quaternion originalRotation = playerModel.rotation;
        currentlyOnThisSpace = currentSpace;
        int originalDiceRoll = diceRoll;
        for (int i = 0; i < originalDiceRoll; i++)
        {
            
            int nextSpace = CalculateNextSpace(currentlyOnThisSpace);
            Vector3 nextPosition = ReturnPosition(nextSpace);

            playerModel.LookAt(new Vector3(nextPosition.x, playerModel.position.y, nextPosition.z));
            await WalkTowardsTile(nextPosition);

            currentlyOnThisSpace = nextSpace == 0 ? 0 : currentlyOnThisSpace + 1;
            if (pathFolder.GetChild(currentlyOnThisSpace).GetComponent<SpaceHandler>().spaceKind == SpaceHandler.SpaceKind.Special)
            {
                originalDiceRoll++;
                await pathFolder.GetChild(currentlyOnThisSpace).GetComponent<SpaceHandler>().HandleAsyncLandedPlayer(transform, currentlyOnThisSpace);
            }
            else if (pathFolder.GetChild(currentlyOnThisSpace).GetComponent<SpaceHandler>().isYarnPlace)
            {
                originalDiceRoll++;
                await GetComponent<HandleReachingYarn>().HandleReachingYarnSpace(pathFolder.GetChild(currentlyOnThisSpace).GetComponent<SpaceHandler>());
            }

            else
            {
                diceRoll--;
                text.text = diceRoll.ToString();
            }
            
        }
        Destroy(text.gameObject);

        playerModel.rotation = originalRotation;

        return currentlyOnThisSpace;
    }
    
    public void SetYValue(float yValue)
    {
        _yValue = yValue;
        _canChangeYValue = true;

    }

    private int CalculateNextSpace(int currentSpace)
    {
        if (currentSpace >= pathFolder.childCount - 1) return 0;
        return currentSpace + 1;
    }

    private Vector3 ReturnPosition(int index)
    {
        Vector3 position = Vector3.zero;
        if (!_canChangeYValue)
        {
            position = new Vector3(pathFolder.GetChild(index).position.x, transform.position.y, pathFolder.GetChild(index).position.z);
        }

        else
        {
            position = new Vector3(pathFolder.GetChild(index).position.x, _yValue, pathFolder.GetChild(index).position.z);
            _canChangeYValue = false;
        }
        

        return position;
    }

    private async Task WalkTowardsTile(Vector3 endDestination)
    {
        playerModel.GetComponent<Animator>().SetFloat("IsWalking", 1);
        while (Vector3.Distance(transform.position, endDestination) > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endDestination, walkSpeed * Time.deltaTime);
            await Task.Yield();
        }

        playerModel.GetComponent<Animator>().SetFloat("IsWalking", 0);
    }
}
