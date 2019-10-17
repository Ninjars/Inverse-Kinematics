using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour {
    private void OnCollisionEnter(Collision other) {
        other.gameObject.SetActive(false);
        Debug.Log("Target Missed");
    }
}
