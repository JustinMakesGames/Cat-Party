using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private bool _taskEnded;
    public virtual IEnumerator UseItem()
    {
        while (!_taskEnded)
        {
            yield return null;
        }
    }
}
