using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonRespawn : MonoBehaviour,IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Respawn.RespawnPlayer(GameObject.FindGameObjectsWithTag("Player")[0]);
    }


}
