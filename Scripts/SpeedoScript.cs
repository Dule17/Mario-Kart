using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedoScript : MonoBehaviour
{
    float startPosition, endPosition, destinationPosition;
    public GameObject needle;
    public GameObject car;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = 223f;
        endPosition = -44f;
        //int index = PlayerPrefs.GetInt("Selected_index");
        
      
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (car != null) UpdateNeedle();
        else if(GameObject.FindGameObjectsWithTag("Player").Length>0)
        {
            car = GameObject.FindGameObjectsWithTag("Player")[0];
            rigidbody = car.GetComponent<Rigidbody>();
        }
    }

    void UpdateNeedle()
    {
        destinationPosition = startPosition - endPosition;
        float d = Mathf.Round(rigidbody.velocity.magnitude * 3.6f) / 180;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosition - d * destinationPosition));
    }
}
