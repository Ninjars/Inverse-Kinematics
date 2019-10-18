using System.Collections.Generic;
using UnityEngine;

class Body : MonoBehaviour {
    public Limb limbPrefab;
    public int limbCount;
    public float limbOffsetRadius = 2.5f;

    private List<Limb> limbs;

    private void Awake() {
        limbs = new List<Limb>(limbCount);
        var rotationAnglePer = 360 / limbCount;
        for (int i = 0; i < limbCount; i++) {
            var elevationAngle = Random.Range(-76, 0);
            var rotation = Quaternion.Euler(elevationAngle, rotationAnglePer * i, 0);
            var vector = rotation * Vector3.forward;
            var limb = Instantiate(limbPrefab, transform);
            limb.transform.name = $"Limb {i}";
            limb.transform.Translate(vector * limbOffsetRadius);
            limb.transform.rotation = rotation;
            limbs.Add(limb);
        }
    }

    private void Update() {
        var targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (var limb in limbs) {
            limb.updateTarget(targets);
        }
    }
}