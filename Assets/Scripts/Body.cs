using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Body : MonoBehaviour {
    public Limb armPrefab;
    public int armCount;
    public LegController legPrefab;
    public int legCount;
    public float limbOffsetRadius = 2.5f;

    public float optimumHeightFromGround = 12f;
    public Vector3 moveTarget;
    public float moveSpeed = 5;
    public float stepCyclePeriod = 5f;

    private Rigidbody rb;
    private List<Limb> arms;
    private List<LegController> legs;
    private float stepPeriod;
    private int activeLeg;
    private float stepTimer;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.position = new Vector3(rb.position.x, optimumHeightFromGround, rb.position.z);
        moveTarget = rb.position;

        arms = new List<Limb>(armCount);
        if (armCount > 0) {
            var rotationAnglePer = 360 / armCount;
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
        }

        legs = new List<LegController>(legCount);
        stepPeriod = stepCyclePeriod / (float)legCount;
        if (legCount > 0) {
            var rotationAnglePer = 360 / legCount;
            for (int i = 0; i < legCount; i++) {
                var elevationAngle = UnityEngine.Random.Range(30, 30);
                var rotation = Quaternion.Euler(elevationAngle, rotationAnglePer * i, 0);
                var vector = rotation * Vector3.forward;
                LegController legController = Instantiate(legPrefab, transform);
                legController.transform.name = $"Leg {i}";
                legController.transform.Translate(vector * limbOffsetRadius);
                legController.transform.rotation = Quaternion.RotateTowards(rotation, Quaternion.Euler(-elevationAngle, rotationAnglePer * i, 0), 20);
                legController.movementDurationSeconds = stepPeriod;
                legController.baseRotation = rotation;
                legs.Add(legController);
            }
        }
    }

    private void FixedUpdate() {
        var targets = GameObject.FindGameObjectsWithTag("Target");
        // TODO: handle positioning arms when there's no targets
        foreach (var limb in arms) {
            updateTarget(limb, targets);
        }

        float moveStep = moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(Vector3.MoveTowards(rb.position, moveTarget, moveStep));

        // TODO: update one foot at a time with pause
        stepTimer += Time.fixedDeltaTime;
        if (stepTimer > stepPeriod) {
            stepTimer -= stepPeriod;
            activeLeg = (activeLeg + 1) % legs.Count;
            legs[activeLeg].takeStep((moveTarget - rb.position).normalized);
        }
    }

    internal void setTargetPosition(Vector3 point) {
        moveTarget = new Vector3(point.x, optimumHeightFromGround, point.z);
    }

    private void updateTarget(Limb limb, GameObject[] targets) {
        var targetObject = closestTarget(limb, targets);
        if (targetObject != null) {
            limb.setTarget(targetObject.transform.position);
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