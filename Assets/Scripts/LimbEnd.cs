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

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Target") {
            other.gameObject.SetActive(false);
        }
    }
}
