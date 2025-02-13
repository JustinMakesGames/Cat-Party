using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class HandleWalking : MonoBehaviour
{
    public Transform pathFolder;

    [SerializeField] private float walkSpeed;
    private bool isDoneWalking;

    private void Awake()
    {
        pathFolder = GameObject.FindGameObjectWithTag("PathFolder").transform;
    }
    public async Task<int> StartHandlingWalking(int diceRoll, int currentSpace, TMP_Text text)
    {
        int originalDiceRoll = diceRoll;
        for (int i = 0; i < originalDiceRoll; i++)
        {
            int nextSpace = CalculateNextSpace(currentSpace);
            Vector3 nextPosition = ReturnPosition(nextSpace);
            await WalkTowardsTile(nextPosition);
            currentSpace++;
            diceRoll--;
            text.text = diceRoll.ToString();
        }
        Destroy(text.gameObject);

        return currentSpace;
    }

    private int CalculateNextSpace(int currentSpace)
    {
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
            print("This is being played right now");
            transform.position = Vector3.MoveTowards(transform.position, endDestination, walkSpeed * Time.deltaTime);
            await Task.Yield();
        }
    }
}
