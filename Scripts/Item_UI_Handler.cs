using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item_UI_Handler : MonoBehaviour
{

    public  Sprite[] items;
    public Sprite emptyItem;
    public Image image;
    public float time_between_shufle;
    public KartItem kart;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (kart == null)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
            {
                kart = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<KartItem>();
            }
        }
        else if (kart.item == null)
        {
            if (kart.CanPickup)
            {
                image.sprite = emptyItem;
            }
            else
            {
                Invoke("Shuffle", time_between_shufle);
            }
        }
        else image.sprite = kart.item.visual;
    }


    void Shuffle()
    {
        image.sprite = items[Random.Range(0, items.Length)];
    }
}
