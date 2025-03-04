using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemScriptableObject itemScriptableObject;
    public string itemName;
    public int price;
    public GameObject animationPrefab;
    private bool _taskEnded;

    private void Awake()
    {
        itemName = itemScriptableObject.name;
        price = itemScriptableObject.price;
        animationPrefab = itemScriptableObject.animationPrefab;
    }
    public virtual IEnumerator UseItem()
    {
        while (!_taskEnded)
        {
            yield return null;
        }
    }
}
