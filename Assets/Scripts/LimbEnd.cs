using System;
using UnityEngine;

[RequireComponent(typeof(FABRIKEffector))]
[RequireComponent(typeof(Rigidbody))]
public class LimbEnd : MonoBehaviour {
    [HideInInspector]
    public Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Target") {
            other.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Target") {
            other.gameObject.SetActive(false);
        }
    }
}
