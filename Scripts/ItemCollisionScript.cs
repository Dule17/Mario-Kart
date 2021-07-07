using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollisionScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<KartLap>())
        {
            if(this.name=="Banana(Clone)")
            {
                Rigidbody r = other.GetComponent<Rigidbody>();
                other.GetComponent<KartItem>().rotate = true;
                r.velocity = Vector3.zero;
            }
            else if(this.name == "Bullet(Clone)")
            {
                Respawn.RespawnPlayer(other.gameObject);
            }
            Destroy(this.gameObject); 
        }
        else if(other.tag=="NPC")
        {
            Destroy(this.gameObject);
        }
    }
}
