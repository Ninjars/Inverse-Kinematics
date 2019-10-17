using System.Collections.Generic;
using UnityEngine;

class Body : MonoBehaviour {
    private Limb[] limbs;

    private void Awake() {
        limbs = GetComponentsInChildren<Limb>();
        Debug.Log($"Limb count: {limbs.Length}");
    }

    private void Update() {
        var targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (var limb in limbs) {
            limb.updateTarget(targets);
        }
    }
}