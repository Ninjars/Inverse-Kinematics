using System;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {
    public Limb armPrefab;
    public int armCount;
    public Limb legPrefab;
    public int legCount;
    public float limbOffsetRadius = 2.5f;
    
    public float optimumHeightFromGround = 12f;
    public Vector3 moveTarget;
    public float moveSpeed = 5;

    private List<Limb> arms;
    private List<Limb> legs;

    private void Awake() {
        transform.position = new Vector3(transform.position.x, optimumHeightFromGround, transform.position.z);
        moveTarget = transform.position;

        var rotationAnglePer = 360 / (armCount + 1);
        
        arms = new List<Limb>(armCount);
        for (int i = 0; i < armCount; i++) {
            var elevationAngle = UnityEngine.Random.Range(-40, -10);
            var rotation = Quaternion.Euler(elevationAngle, rotationAnglePer * i, 0);
            var vector = rotation * Vector3.forward;
            var limb = Instantiate(armPrefab, transform);
            limb.transform.name = $"Arm {i}";
            limb.transform.Translate(vector * limbOffsetRadius);
            limb.transform.rotation = rotation;
            arms.Add(limb);
        }

        legs = new List<Limb>(legCount);
        for (int i = 0; i < legCount; i++) {
            var elevationAngle = UnityEngine.Random.Range(40, 10);
            var rotation = Quaternion.Euler(elevationAngle, rotationAnglePer * i, 0);
            var vector = rotation * Vector3.forward;
            var limb = Instantiate(legPrefab, transform);
            limb.transform.name = $"Leg {i}";
            limb.transform.Translate(vector * limbOffsetRadius);
            limb.transform.rotation = Quaternion.RotateTowards(rotation, Quaternion.Euler(-elevationAngle, rotationAnglePer * i, 0), 20);
            limb.limbTarget = limb.getEndPosition();
            legs.Add(limb);
        }
    }

    private void Update() {
        var targets = GameObject.FindGameObjectsWithTag("Target");
        // TODO: handle positioning arms when there's no targets
        foreach (var limb in arms) {
            updateTarget(limb, targets);
        }

        float moveStep = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveStep);
    }

    internal void setTargetPosition(Vector3 point) {
        moveTarget = new Vector3(point.x, optimumHeightFromGround, point.z);
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