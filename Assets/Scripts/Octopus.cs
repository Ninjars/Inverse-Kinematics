using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Body))]
public class Octopus : MonoBehaviour {
    public bool directControl = false;

    private Body body;

    private void Awake() {
        body = GetComponent<Body>();
    }

    private void Update() {
        var targets = GameObject.FindGameObjectsWithTag("Target");
        var capturedTargets = body.updateArmTargets(targets);
        
        if (directControl) return;

        var uncapturedTargets = new List<GameObject>(targets);
        uncapturedTargets.RemoveAll(arg => capturedTargets.Contains(arg));

        var closest = closestTarget(transform.position, uncapturedTargets);
        if (closest != null) {
            body.setMoveTarget(closest.transform.position);
        }
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
}