using System;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour {
    public LimbData config;

    public Vector3 limbTarget;

    private FABRIKChain limbChain;
    private GameObject targetObject;

    private void Awake() {
        limbChain = buildLimb(transform);
    }

    // this can be expanded to build branching limbs and so return a list of chains.
    // the root chain should be tracked, and in the update function each chain will need backward() called on it
    private FABRIKChain buildLimb(Transform limbOrigin, FABRIKChain parentChain = null, int layer = 0) {
        List<FABRIKEffector> sections = new List<FABRIKEffector>(config.sectionCount + (parentChain == null ? 2 : 3));

        if (parentChain != null) sections.Add(parentChain.EndEffector);

        buildLimbSection(config.sectionCount, limbOrigin, Vector3.zero, in sections);

        return new FABRIKChain(parentChain, sections, layer);
    }

    private void buildLimbSection(int remainingChildren, Transform parent, Vector3 offset, in List<FABRIKEffector> sections) {
        LimbSection section = GameObject.Instantiate(config.limbSectionPrefab, parent);
        section.transform.Translate(offset, parent);

        var effector = section.GetComponent<FABRIKEffector>();
        sections.Add(effector);

        effector.twistConstraint = Mathf.Lerp(config.endTwist, config.initialTwist, remainingChildren / (float)config.sectionCount);
        effector.swingConstraint = Mathf.Lerp(config.endSwing, config.initialSwing, remainingChildren / (float)config.sectionCount);

        Debug.Log($"{transform.name} section {remainingChildren} swing {effector.swingConstraint} twist {effector.twistConstraint}");

        if (remainingChildren > 0) {
            buildLimbSection(remainingChildren - 1, section.transform, section.childOffset, in sections);
        } else {
            buildLimbEnd(section.transform, section.childOffset, in sections);
        }
    }

    private void buildLimbEnd(Transform parent, Vector3 offset, in List<FABRIKEffector> sections) {
        LimbEnd section = GameObject.Instantiate(config.limbEndPrefab, parent);
        section.transform.Translate(offset, parent);

        var effector = section.GetComponent<FABRIKEffector>();
        sections.Add(effector);

        effector.twistConstraint = 60;
        effector.swingConstraint = 80;

        var endEffector = new GameObject();
        endEffector.transform.position = section.transform.position;
        endEffector.transform.parent = section.transform;
        endEffector.AddComponent(typeof(FABRIKEffector));
        sections.Add(endEffector.GetComponent<FABRIKEffector>());
    }

    private void Update() {
        if (targetObject != null) {
            limbTarget = targetObject.transform.position;
        }
        limbChain.Target = Vector3.MoveTowards(limbChain.EndEffector.Position, limbTarget, Time.deltaTime * config.speed);

        // propagate update up the chain from the end
        limbChain.Backward();

        // propagate results forward down the chain again
        limbChain.ForwardMulti();
    }

    private void OnDrawGizmos() {
        if (limbChain != null) {
            Debug.DrawLine(limbChain.EndEffector.transform.position, limbChain.Target, Color.red, 0);
            Debug.DrawLine(limbChain.EndEffector.transform.position, limbTarget, Color.green, 0);
        }
    }

    internal void updateTarget(GameObject[] targets) {
        targetObject = closestTarget(targets);
    }

    private GameObject closestTarget(GameObject[] targets) {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = limbChain.EndEffector.transform.position;
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