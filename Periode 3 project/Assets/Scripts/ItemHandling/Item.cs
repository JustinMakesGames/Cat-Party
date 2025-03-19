using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemScriptableObject itemScriptableObject;
    public string itemName;
    public int price;
    public GameObject animationPrefab;
    public GameObject uiPrefab;
    public GameObject obj;
    public Transform player;
    protected bool taskEnded;

    protected virtual void Awake()
    {
        itemName = itemScriptableObject.name;
        price = itemScriptableObject.price;
        animationPrefab = itemScriptableObject.animationPrefab;
        uiPrefab = itemScriptableObject.uiPrefab;
        obj = itemScriptableObject.obj;
    }

    public void StartUsingItem()
    {
        player = transform.parent.parent.parent.parent;
        player.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(null);
        GameObject itemClone = Instantiate(obj, null);
        itemClone.GetComponent<Item>().CallUseItem(player);
        transform.parent.parent.gameObject.SetActive(false);

        print("removing item from inventory");
        player.GetComponent<PlayerInventory>().RemoveItem(gameObject);
        Destroy(gameObject);
    }

    public void CallUseItem(Transform player)
    {
        this.player = player;
        StartCoroutine(UseItem());
    }

    private IEnumerator UseItem()
    {
        print("This is so sigma i cant believe this");
        yield return StartCoroutine(ShowItemUseAnimation());
        print("Sigma gaming XD");
        StartCoroutine(BeginUsingItem());
        while (!taskEnded)
        {
            yield return null;
        }

        taskEnded = false;
    }

    private IEnumerator ShowItemUseAnimation()
    {
        GameObject cloneObj = Instantiate(obj, player.GetChild(0).position, Quaternion.identity);
        yield return new WaitForSeconds(2);
        Destroy(cloneObj);
    }
    protected virtual IEnumerator BeginUsingItem()
    {
        yield return null;
        taskEnded = true;
    }
}
