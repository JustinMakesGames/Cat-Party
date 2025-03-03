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
    private bool isDoneWalking;


    private void Awake()
    {
        pathFolder = GameObject.FindGameObjectWithTag("PathFolder").transform;
    }
    public async Task<int> StartHandlingWalking(int diceRoll, int currentSpace, TMP_Text text)
    {
        currentlyOnThisSpace = currentSpace;
        int originalDiceRoll = diceRoll;
        for (int i = 0; i < originalDiceRoll; i++)
        {
            
            int nextSpace = CalculateNextSpace(currentlyOnThisSpace);
            Vector3 nextPosition = ReturnPosition(nextSpace);
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

        return currentlyOnThisSpace;
    }

    private int CalculateNextSpace(int currentSpace)
    {
        if (currentSpace >= pathFolder.childCount - 1) return 0;
        return currentSpace + 1;
    }

    private Vector3 ReturnPosition(int index)
    {
        Vector3 position = new Vector3(pathFolder.GetChild(index).position.x, transform.position.y, pathFolder.GetChild(index).position.z);

        return position;
    }

    private async Task WalkTowardsTile(Vector3 endDestination)
    {
        while (Vector3.Distance(transform.position, endDestination) > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endDestination, walkSpeed * Time.deltaTime);
            await Task.Yield();
        }
    }
}
