using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NW_Health : NetworkBehaviour {
    //-------------------------------------=
    public const int maxHP = 100;

    [SyncVar]
    public int iHP = maxHP;

	
	// Update is called once per frame
	void Update () {
        //Desplay the health amount
        GetComponentInChildren<TextMesh>().text = Mathf.Round(iHP).ToString();
		
	}//-----------------------------

    //--------------------------------
    //Damage receiver

    public void Damage(int _damage_amount)
    {
        if (!isServer)
            return;

        iHP -= _damage_amount;
        if(iHP <= 0)
        {
            iHP = maxHP;
            //called on the server, will be invoked on the clients
            RpcRespawn();
            
        }
    }//----------------
    //--------------------------------------------

    private NetworkStartPosition[] spawnPoints;

    [ClientRpc] // sent from objects on the server to objects on clients.
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();

            //Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            //If there is a spawn point array and the array is not empty, pick a spawn point at random
            if(spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            //set the player's pos to chosen spawn point
            transform.position = spawnPoint;
        }
    }//---------------------

}//=============================
