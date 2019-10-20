using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Limb))]
public class ArmController : MonoBehaviour, OnTargetTouchedHandler {

    public GameObject maw;
    public GameObject target;
    public Rigidbody targetRb;

    [Range(0, 1)]
    public float restingOffsetFactor = 0.75f;

    private Vector3 restingOffset;
    private Limb limb;

    private bool hasCapturedTarget;

    private void Awake() {
        this.limb = GetComponent<Limb>();
        restingOffset = new Vector3(0, 0, limb.length * restingOffsetFactor);
    }

    private void FixedUpdate() {
        if (!isTargetInRange()) {
            limb.setTarget(getRestingPosition());

        } else if (hasCapturedTarget) {
            if (!target.activeInHierarchy) {
                hasCapturedTarget = false;
                target = null;
                targetRb = null;
            } else {
                limb.setTarget(maw.transform.position);
                targetRb.MovePosition(limb.getEndPosition() + limb.getEndRotation() * Vector3.forward);
            }

        } else {
            limb.setTarget(target.transform.position);
        }
    }

    private bool isTargetInRange() {
        if (target == null) return false;
        return isPositionInRange(target.transform.position);
    }

    private bool isPositionInRange(Vector3 targetPosition) {
        return Vector3.SqrMagnitude(targetPosition - transform.position) <= (limb.length * limb.length);
    }

    private Vector3 getRestingPosition() {
        return transform.position + transform.rotation * restingOffset;
    }

    public GameObject getCurrentTarget() {
        if (hasCapturedTarget || isTargetInRange()) {
            return target;
        } else {
            return null;
        }
    }

    public GameObject updateTargets(List<GameObject> targets) {
        if (!hasCapturedTarget) {
            if (!isTargetInRange()) {
                var targ = closestTarget(limb.getEndPosition(), targets);
                if (isPositionInRange(targ.transform.position)) {
                    target = targ;
                } else {
                    target = null;
                }
            }
        }
        return target;
    }

    private GameObject closestTarget(Vector3 position, List<GameObject> targets) {
        GameObject closest = null;
        float distance = Mathf.Infinity;
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

    public void onTargetTouched(GameObject obj) {
        if (hasCapturedTarget) return;
        if (obj.transform.tag != "Target") return;

        hasCapturedTarget = true;
        target = obj;

        targetRb = obj.GetComponent<Rigidbody>();
        if (targetRb != null) {
            targetRb.isKinematic = true;
        }

        var targetScript = obj.GetComponent<KeepAwayTarget>();
        if (targetScript != null){
            targetScript.canMove = false;
        }
    }
}
