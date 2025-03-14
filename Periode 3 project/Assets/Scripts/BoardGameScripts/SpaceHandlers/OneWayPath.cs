using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OneWayPath : SpaceHandler
{
    [SerializeField] private Transform pathFolder;
    [SerializeField] private Transform rightSpace;
    public override async Task HandleAsyncLandedPlayer(Transform player, int currentIndex)
    {
        ChangePathFolder(player);
        await Task.Yield();
    }

    private void ChangePathFolder(Transform player)
    {
        player.GetComponent<HandleWalking>().pathFolder = pathFolder;
        player.GetComponent<HandleWalking>().currentlyOnThisSpace = rightSpace.GetSiblingIndex() - 1;
    }

    public Transform GetRightPathFolder()
    {
        return pathFolder;
    }

    public int GetIndex()
    {
        return rightSpace.GetSiblingIndex() - 1;
    }
}
