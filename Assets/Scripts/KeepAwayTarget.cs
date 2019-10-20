using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KeepAwayTarget : MonoBehaviour {
    public float moveSpeed;
    [HideInInspector]
    public bool canMove = true;
    private Rigidbody rb;
    private GameObject[] hunters;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        hunters = GameObject.FindGameObjectsWithTag("Octopus");
    }

    void OnEnable() {
        canMove = true;
        rb.isKinematic = true;
    }

    private void FixedUpdate() {
        if (!canMove) return;

        var closest = closestHostile(hunters);

        var escapeVector = rb.position - closest.transform.position;
        escapeVector = new Vector3(rb.position.x + escapeVector.x, 0, rb.position.z + escapeVector.z);

        float moveStep = moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(Vector3.MoveTowards(rb.position, escapeVector, moveStep));
        rb.MoveRotation(Quaternion.LookRotation(escapeVector));
    }

    private GameObject closestHostile(GameObject[] foes) {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = rb.position;
        foreach (GameObject target in foes) {
            Vector3 diff = target.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                closest = target;
                distance = curDistance;
            }
        }
        return closest;
    }
}
