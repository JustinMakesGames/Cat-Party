using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WarpItem : Item
{
    [SerializeField] private GameObject warpBox;
    private GameObject _warpBoxClone;
    private Transform _playerFolder;
    private Transform _playerToSwitchWith;
    private List<Transform> _players = new List<Transform>();
    private Animator _animator, _otherAnimator;
    private Vector3 _pos1, _pos2;
    private Transform _pathFolder1, _pathFolder2;
    private int _currentIndex1, _currentIndex2;
    private Animator _blackScreenAnimator;
    protected override IEnumerator BeginUsingItem()
    {
        _playerFolder = GameObject.FindGameObjectWithTag("PlayerFolder").transform;
        _animator = player.GetComponentInChildren<Animator>();
        for (int i = 0; i < _playerFolder.childCount; i++)
        {
            if (_playerFolder.GetChild(i) == player) continue;
            _players.Add(_playerFolder.GetChild(i));
        }

        ChoosePlayer();
        CalculatePositions();
        yield return StartCoroutine(PlaySwitchAnimation());
        yield return StartCoroutine(SwitchCameraAnimation());
    }
    
    private void ChoosePlayer()
    {
        int randomIndex = Random.Range(0, _players.Count);
        _playerToSwitchWith = _players[randomIndex];
        _otherAnimator = _playerToSwitchWith.GetComponentInChildren<Animator>();
    }

    private IEnumerator PlaySwitchAnimation()
    {
        _warpBoxClone = Instantiate(warpBox, player.GetChild(0).position, Quaternion.identity);

        yield return new WaitForSeconds(1);
        _animator.SetTrigger("BigJump");
        yield return new WaitForSeconds(0.1f);
        SwitchEnabledRenderer(false);
        Camera.main.transform.parent = null;
        WarpPlayers();
        yield return new WaitForSeconds(1);
        _otherAnimator.SetTrigger("JumpOut");
        SwitchEnabledRenderer(true);
        yield return new WaitForSeconds(2);

    }

    private void CalculatePositions()
    {
        _pathFolder1 = player.GetComponent<HandleWalking>().pathFolder;
        _pathFolder2 = _playerToSwitchWith.GetComponent<HandleWalking>().pathFolder;
        _pos1 = player.position;
        _pos2 = _playerToSwitchWith.position;
        _currentIndex1 = player.GetComponent<PlayerHandler>().currentSpace;
        _currentIndex2 = _playerToSwitchWith.GetComponent<PlayerHandler>().currentSpace;

        player.GetComponent<PlayerHandler>().pathFolder = _pathFolder2;
        _playerToSwitchWith.GetComponent<PlayerHandler>().pathFolder = _pathFolder1;
        player.GetComponent<HandleWalking>().pathFolder = _pathFolder2;
        _playerToSwitchWith.GetComponent<HandleWalking>().pathFolder = _pathFolder1;
        player.GetComponent<PlayerHandler>().currentSpace = _currentIndex2;
        _playerToSwitchWith.GetComponent<PlayerHandler>().currentSpace = _currentIndex1;
    }

    private void WarpPlayers()
    {
        player.position = _pos2;
        _playerToSwitchWith.position = _pos1;
    }

    private void SwitchEnabledRenderer(bool enabled)
    {
        Renderer[] playerRenderers = player.GetComponentsInChildren<Renderer>();
        Renderer[] otherPlayerRenderers = _playerToSwitchWith.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = enabled;
        }

        foreach (Renderer renderer in otherPlayerRenderers)
        {
            renderer.enabled = enabled;
        }
    }

    private IEnumerator SwitchCameraAnimation()
    {
        _blackScreenAnimator = GameObject.FindGameObjectWithTag("BlackScreen").GetComponent<Animator>();
        _blackScreenAnimator.SetTrigger("FadeInOut");
        yield return new WaitForSeconds(0.2f);
        Camera.main.transform.position = player.GetChild(1).position;
        Camera.main.transform.parent = player;
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerInventory>().OpenChooseScreen();

    }
}
