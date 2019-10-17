using System;
using UnityEngine;

[RequireComponent(typeof(FABRIKEffector))]
public class LimbEnd : MonoBehaviour {

    private void OnCollisionEnter(Collision other) {
        Debug.Log($"on collision {other} {other.gameObject.tag}");
        if (other.gameObject.tag == "Target") {
            Debug.Log("target tagged");
            other.gameObject.SetActive(false);
        }
    }
}
