using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartItem : MonoBehaviour
{
    public Item item;
    public bool CanPickup;
    public GameItemHandle handler;
    public float delay;
    public bool rotate;
    public ButtonFire joystickFire;
    float angle;
    void Start()
    {
        handler = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameItemHandle>();
        delay = 1;
        rotate = false;
        angle = 0;
        joystickFire = FindObjectOfType<ButtonFire>();
        Reset();
    }
    void Rotate360()
    {
        if(rotate)
        {
            Invoke("Rotate30", 0.08f);
            angle += 30f;
            if(angle>=720f)
            {
                angle = 0;
                rotate = false;
            }
        }
    }

    void Rotate30()
    {
        gameObject.transform.Rotate(0,30f, 0);
    }
    void FixedUpdate()
    {
        Rotate360();
        UseItem();
    }
    public void StartPickUp()
    {
        StartCoroutine(PickUp());
    }

    void UseItem()
    {

        if (item != null&&((joystickFire==null&&Input.GetAxisRaw("Fire1") != 0) ||(joystickFire != null && joystickFire.pressed==true)))
        {
            if (item.name == "Banana")
            {
               // item.prefab.transform.position = GameObject.FindGameObjectsWithTag("ItemBack")[0].transform.position;
                //Instantiate(item.prefab);
                ClientSend.placeItemReceived(0);
            }
            else if(item.name=="Bullet")
            {  
               /* GameObject bullet=Instantiate(item.prefab, GameObject.FindGameObjectsWithTag("ItemFront")[0].transform.position, transform.rotation);
                bullet.transform.Rotate(90, 0, 0);
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);
                Destroy(bullet, 10f);*/
                ClientSend.placeItemReceived(1);
            }
            else
            {

                gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f, ForceMode.Acceleration);
            }
            
            Reset();
        }
    }
    public IEnumerator PickUp()
    {      
        CanPickup = false;
        yield return new WaitForSeconds(delay);
        item = handler.items[Random.Range(0, handler.items.Length)];
    }
    void Reset()
    {
        item = null;
        CanPickup = true;
    }
}
