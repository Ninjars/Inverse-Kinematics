using System.Collections.Generic;
using UnityEngine;

class Body : MonoBehaviour {
    private Limb[] limbs;

    private void Awake() {
        limbs = GetComponentsInChildren<Limb>();
        Debug.Log($"Limb count: {limbs.Length}");
    }
}