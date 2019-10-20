using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Limb))]
public class ArmController : MonoBehaviour {

    public GameObject maw;
    public GameObject target;

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
        if (target == null || isTargetOutOfRange(target.transform.position)) {
            limb.setTarget(getRestingPosition());
        } else {
            limb.setTarget(target.transform.position);
        }
    }

    private bool isTargetOutOfRange(Vector3 targetPosition) {
        return Vector3.SqrMagnitude(targetPosition - transform.position) > (limb.length * limb.length);
    }

    private Vector3 getRestingPosition() {
        return transform.position + transform.rotation * restingOffset;
    }

    public void updateTargets(GameObject[] targets) {
        target = closestTarget(limb.getEndPosition(), targets);
    }

    private GameObject closestTarget(Vector3 position, GameObject[] targets) {
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
}
