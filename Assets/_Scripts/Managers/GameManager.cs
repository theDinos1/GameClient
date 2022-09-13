using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Dictionary<int, PlayerManager> Players = new Dictionary<int, PlayerManager>();

    [SerializeField] private GameObject LocalPlayerPrefab;
    [SerializeField] private GameObject PlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.Instance.MyId)
        {
            _player = Instantiate(LocalPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(PlayerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().Id = _id;
        _player.GetComponent<PlayerManager>().Username = _username;
        Players.Add(_id, _player.GetComponent<PlayerManager>());
    }
}