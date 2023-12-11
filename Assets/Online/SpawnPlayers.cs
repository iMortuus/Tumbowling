using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] SpawnPoints;

    private void Start()
    {
        SpawnPoints = GetComponentsInChildren<Transform>();
        Vector3 _sp = SpawnPoints[Random.Range(1, SpawnPoints.Length)].position;
        PhotonNetwork.Instantiate(playerPrefab.name, _sp + Vector3.up * 2, Quaternion.identity);
    }


}
