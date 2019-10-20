using System;
using UnityEngine;

[RequireComponent(typeof(FABRIKEffector))]
[RequireComponent(typeof(Rigidbody))]
public class LimbEnd : MonoBehaviour {
    public bool interactWithTargets;
    public float attachDistance;

    [HideInInspector]
    public Rigidbody rb;
    private OnTargetTouchedHandler handler;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        if (interactWithTargets) {
            handler = GetComponentInParent<OnTargetTouchedHandler>();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!interactWithTargets) return;

        if (other.transform.tag == "Target") {
            if (handler != null) {
                handler.onTargetTouched(other.transform.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (!interactWithTargets) return;

        if (other.transform.tag == "Target") {
            if (handler != null) {
                handler.onTargetTouched(other.transform.gameObject);
            }
        }
    }
}
