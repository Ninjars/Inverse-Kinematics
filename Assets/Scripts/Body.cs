using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Maw))]
public class Body : MonoBehaviour {
    public ArmController armPrefab;
    public int armCount;
    public LegController legPrefab;
    public int legCount;
    public float limbOffsetRadius = 2.5f;

    public float optimumHeightFromGround = 12f;
    public Vector3 moveTarget;
    public float moveSpeed = 5;
    public float stepCyclePeriod = 5f;

    public AnimationCurve breathingCurve;
    private float breathingCurveOffset;

    private Maw maw;
    private Rigidbody rb;
    private List<ArmController> arms;
    private List<LegController> legs;
    private float stepPeriod;
    private int activeLeg;
    private float stepTimer;

    private void Awake() {
        maw = GetComponent<Maw>();
        rb = GetComponent<Rigidbody>();
        rb.position = new Vector3(rb.position.x, optimumHeightFromGround, rb.position.z);
        moveTarget = rb.position;

        breathingCurveOffset = UnityEngine.Random.value;

        arms = new List<ArmController>(armCount);
        if (armCount > 0) {
            var rotationAnglePer = 360 / armCount;
            for (int i = 0; i < armCount; i++) {
                var elevationAngle = UnityEngine.Random.Range(-40, -10);
                var rotation = Quaternion.Euler(elevationAngle, rotationAnglePer * i, 0);
                var vector = rotation * Vector3.forward;
                var armController = Instantiate(armPrefab, transform);
                armController.maw = maw.gameObject;
                armController.transform.name = $"Arm {i}";
                armController.transform.Translate(vector * limbOffsetRadius);
                armController.transform.rotation = rotation;
                arms.Add(armController);
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
        float moveStep = moveSpeed * Time.fixedDeltaTime;
        var verticalOffset = breathingCurve.Evaluate(Time.time + breathingCurveOffset);
        rb.position = new Vector3(rb.position.x, optimumHeightFromGround + verticalOffset, rb.position.z);
        rb.MovePosition(Vector3.MoveTowards(rb.position, moveTarget, moveStep));

        stepTimer += Time.fixedDeltaTime;
        if (stepTimer > stepPeriod) {
            stepTimer -= stepPeriod + UnityEngine.Random.value;
            activeLeg = (activeLeg + 1) % legs.Count;
            legs[activeLeg].takeStep((moveTarget - rb.position).normalized);
        }
    }

    internal void updateArmTargets(GameObject[] targets) {
        foreach (var arm in arms) {
            arm.updateTargets(targets);
        }
    }

    internal void setMoveTarget(Vector3 point) {
        moveTarget = new Vector3(point.x, optimumHeightFromGround, point.z);
    }
}