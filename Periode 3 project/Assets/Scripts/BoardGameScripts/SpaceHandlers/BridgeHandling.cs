using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BridgeHandling : SpaceHandler
{
    public float yValue;
    public override async Task HandleAsyncLandedPlayer(Transform player, int currentIndex)
    {
        player.GetComponent<HandleWalking>().SetYValue(yValue + 0.854945f);
        await Task.Yield();
    }
}
