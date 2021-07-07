using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    

    public Transform cameraTarget;
    public float sSpeed = 10.0f;
    public Vector3 dist;
    public Transform lookTarget;

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (cameraTarget != null)
        {
            Vector3 dPos = cameraTarget.position + dist;
            Vector3 sPos = Vector3.Lerp(transform.position, dPos, sSpeed * Time.deltaTime);
            transform.position = sPos;
            transform.LookAt(lookTarget.position);
        }
        else if(GameObject.FindGameObjectsWithTag("Player").Length>0)
        {
            
            cameraTarget = GameObject.FindGameObjectsWithTag("Player")[0].transform.Find("CamTarget");
            lookTarget = GameObject.FindGameObjectsWithTag("Player")[0].transform.Find("CamLookAt");
        }
    }
}
