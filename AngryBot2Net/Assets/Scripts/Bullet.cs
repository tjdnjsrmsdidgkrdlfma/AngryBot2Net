using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject effect;

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000);
        Destroy(gameObject, 3);
    }

    void OnCollisionEnter(Collision other)
    {
        var contact = other.GetContact(0);
        var obj = Instantiate(effect,
                              contact.point,
                              Quaternion.LookRotation(-contact.normal));
        Destroy(obj, 2);
        Destroy(gameObject);
    }
}
