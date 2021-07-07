using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        int index = int.Parse(this.name);
        if (other.GetComponent<KartLap>())
        {
            
            if (other.GetComponent<KartLap>())
                {
                    KartLap kart = other.GetComponent<KartLap>();
                    kart.Check(index);
                }
        }
        else if (other.tag == "NPC")
        {
            if (other.GetComponent<CheckPointCounter>().checkpoint > index && other.GetComponent<CheckPointCounter>().checkpoint == KartLap.CheckpointsCount)
            {
                other.GetComponent<CheckPointCounter>().lap++;
                other.GetComponent<CheckPointCounter>().checkpoint = index;
            }
            if(index> other.GetComponent<CheckPointCounter>().checkpoint)other.GetComponent<CheckPointCounter>().checkpoint = index;
        }
    }
}
