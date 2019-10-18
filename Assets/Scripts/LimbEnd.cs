using System;
using UnityEngine;

[RequireComponent(typeof(FABRIKEffector))]
public class LimbEnd : MonoBehaviour {

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Target") {
            other.gameObject.SetActive(false);
        }
    }
}
