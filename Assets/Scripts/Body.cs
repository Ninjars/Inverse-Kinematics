using System.Collections.Generic;
using UnityEngine;

class Body : MonoBehaviour {
    public Limb armPrefab;
    public int armCount;
    public float limbOffsetRadius = 2.5f;

    private List<Limb> arms;

    private void Awake() {
        arms = new List<Limb>(armCount);
        var rotationAnglePer = 360 / armCount;
        for (int i = 0; i < armCount; i++) {
            var elevationAngle = Random.Range(-40, -10);
            var rotation = Quaternion.Euler(elevationAngle, rotationAnglePer * i, 0);
            var vector = rotation * Vector3.forward;
            var limb = Instantiate(armPrefab, transform);
            limb.transform.name = $"Arm {i}";
            limb.transform.Translate(vector * limbOffsetRadius);
            limb.transform.rotation = rotation;
            arms.Add(limb);
        }
    }

    private void Update() {
        var targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (var limb in arms) {
            updateTarget(limb, targets);
        }
    }

    private void updateTarget(Limb limb, GameObject[] targets) {
        var targetObject = closestTarget(limb, targets);
        if (targetObject != null) {
            limb.limbTarget = targetObject.transform.position;
        }
    }

    private GameObject closestTarget(Limb limb, GameObject[] targets) {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = limb.getEndPosition();
        foreach (GameObject target in targets) {
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