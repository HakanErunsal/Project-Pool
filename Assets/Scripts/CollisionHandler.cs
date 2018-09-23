using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Yellow")
        {
            transform.parent.gameObject.GetComponent<PlayerController>().CollisionHandle(false);
        }
        else if (collision.gameObject.name == "Red")
        {
            transform.parent.gameObject.GetComponent<PlayerController>().CollisionHandle(true);
        }
        else
        {

        }
    }
}
