using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour {

    public AudioClip BallHit, SideHit;

    private AudioSource Source;

    private Rigidbody Rigidbody_;

    void Start()
    {
        Source = gameObject.GetComponent<AudioSource>();
        Rigidbody_ = gameObject.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        //Collision handle
        if (transform.parent)
        {
            if (collision.gameObject.tag == "Yellow")
            {
                transform.parent.gameObject.GetComponent<PlayerController>().CollisionHandle(false);
            }
            else if (collision.gameObject.tag == "Red")
            {
                transform.parent.gameObject.GetComponent<PlayerController>().CollisionHandle(true);
            }
        }

        if (collision.gameObject.tag == "Red" || collision.gameObject.tag == "Yellow")
        {
            Source.PlayOneShot(BallHit, Rigidbody_.velocity.magnitude);
        }
        else if (collision.gameObject.tag == "TableSide")
        {
            Source.PlayOneShot(SideHit, Rigidbody_.velocity.magnitude);
        }
    }
}
