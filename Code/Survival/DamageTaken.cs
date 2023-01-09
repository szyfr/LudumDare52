using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "enemy")
        {
            print("hit");
            col.GetComponent<Enemy>().Damaged();
        }if(col.tag == "bullet")
        {
            Destroy(col.gameObject);
        }
    }
}
