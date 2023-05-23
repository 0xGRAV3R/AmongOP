using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyPhotonPlayer : MonoBehaviour
{
    PhotonView myPV;
    GameObject myPlayerAvatar;

    Player[] allPlayers;
    int myNumberInRoom;

    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();

        allPlayers = PhotonNetwork.PlayerList;
        foreach (Player p in allPlayers)
        {
            if (p != PhotonNetwork.LocalPlayer)
            {
                myNumberInRoom++;
            }
        }


        if (myPV.IsMine)
        {
            myPlayerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Player"), AU_GameController.instance.spawnPoints[myNumberInRoom].position, Quaternion.identity);
        }
    }


}