using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShopHandler : SpaceHandler
{
    [SerializeField] private float camSpeed; 
    [SerializeField] private Transform camPosition;
    [SerializeField] private Transform itemFolder;
    private Transform _player;
    private bool _hasTaskEnded;
    private Transform _cam;
    public override async Task HandleAsyncLandedPlayer(Transform player, int currentIndex)
    {
        _player = player;
        StartCoroutine(StartShopHandling());
        while (!_hasTaskEnded)
        {
            await Task.Yield();
        }

        _hasTaskEnded = false;      
    }

    private IEnumerator StartShopHandling()
    {    
        yield return StartCoroutine(ShowShopText());
        _player.GetComponent<PlayerShopHandling>().AskIfPlayerWantsToShop(this);    
    }

    private IEnumerator ShowShopText()
    {
        string shopText = "ShopText";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == shopText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings));
    }

    public IEnumerator MoveCameraToShop()
    {
        _cam = Camera.main.transform;
        _cam.rotation = camPosition.rotation;
        while (_cam.position != camPosition.position)
        {
            _cam.position = Vector3.MoveTowards(_cam.position, camPosition.position, camSpeed * Time.deltaTime);
            yield return null;
            
        }

        _player.GetComponent<PlayerShopHandling>().BeginShopping(itemFolder.GetChild(0));
    }
    
    public void ShopDone()
    {
        _hasTaskEnded = true;
    }
}
