using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<KartLap>())
        {
            CheckPointCounter kart = other.GetComponent<CheckPointCounter>();
            GameObject[] g = GameObject.FindGameObjectsWithTag("CheckPoint");
            for (int i = 0; i < g.Length; i++)
            if(int.Parse(g[i].name)==kart.checkpoint){
                
                other.transform.rotation = g[i].transform.rotation;
                other.transform.position = g[i].transform.position;
                    break;
            }
            VehicleController v= other.GetComponent<VehicleController>();
            v.rigidbody.velocity = Vector3.zero;
        }
    }
    public static void RespawnPlayer(GameObject player)
    {
        CheckPointCounter kart = player.GetComponent<CheckPointCounter>();
        GameObject[] g = GameObject.FindGameObjectsWithTag("CheckPoint");
        for (int i = 0; i < g.Length; i++)
            if (int.Parse(g[i].name) == kart.checkpoint)
            {

                player.transform.rotation = g[i].transform.rotation;
                player.transform.position = g[i].transform.position;
                break;
            }
        VehicleController v = player.GetComponent<VehicleController>();
        v.rigidbody.velocity = Vector3.zero;
    }
}
