using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointCounter : MonoBehaviour
{
      public int checkpoint;
      public  int lap;
      public int rank;

    public bool CanPickup;

    void Start()
    {
        checkpoint = 1;
        lap = 1;
        CanPickup = true;
    }
}
