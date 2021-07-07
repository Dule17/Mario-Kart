using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    GameObject[] list;
    int selected_index;
    void Start()
    {
        list = GameObject.FindGameObjectsWithTag("Player");
        if (GameObject.FindGameObjectsWithTag("EndGame").Length > 0)
        {
            GameObject.FindGameObjectsWithTag("EndGame")[0].active = false;
            //selected_index = PlayerPrefs.GetInt("Selected_index");
            selected_index = Client.instance.selectedCharater;
        }
        else selected_index = 0;
        
        for(int i=0;i<list.Length;i++)
        {
            list[i].active = false;
        }
        list[selected_index].active = true;

    }

    public void Left()
    {
        if(selected_index > 0)
        {
            list[selected_index].active = false;
            selected_index--;
            list[selected_index].active = true;
        }
    }
    public void Right()
    {
        if (selected_index < list.Length-1)
        {
            list[selected_index].active = false;
            selected_index++;
            list[selected_index].active = true;
        }
    }

    public void SetPlayerPerf()
    {
        //PlayerPrefs.SetInt("Selected_index", selected_index);
        Client.instance.selectedCharater = selected_index;
    }
}
