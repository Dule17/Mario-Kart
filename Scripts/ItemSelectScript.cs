using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
       if(other.GetComponent<KartItem>())
        {
            
            KartItem kart = other.GetComponent<KartItem>();
            if (kart.CanPickup == true)
            {
                kart.StartPickUp();
                this.gameObject.active = false;
            }
        }
       else if(other.tag=="NPC")
        {
            if (other.GetComponent<CheckPointCounter>().CanPickup == true)
            {
                this.gameObject.active = false;
                other.GetComponent<CheckPointCounter>().CanPickup = false;
            }
        }
    }
}
