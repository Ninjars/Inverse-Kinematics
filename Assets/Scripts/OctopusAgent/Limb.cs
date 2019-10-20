using System;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour {
    public LimbData config;

    public Vector3 limbTarget { private set; get; }
    private FABRIKChain limbChain;
    private LimbEnd end;
    public float length { private set; get; }

    private void Awake() {
        limbChain = buildLimb(transform);
        length = config.sectionCount * config.limbSectionPrefab.childOffset.z;
    }

    // this can be expanded to build branching limbs and so return a list of chains.
    // the root chain should be tracked, and in the update function each chain will need backward() called on it
    private FABRIKChain buildLimb(Transform limbOrigin, FABRIKChain parentChain = null, int layer = 0) {
        List<FABRIKEffector> sections = new List<FABRIKEffector>(config.sectionCount + (parentChain == null ? 1 : 2));

        if (parentChain != null) sections.Add(parentChain.EndEffector);

        buildLimbSection(config.sectionCount, limbOrigin, Vector3.zero, in sections);

        return new FABRIKChain(parentChain, sections, layer);
    }

    private void buildLimbSection(int remainingChildren, Transform parent, Vector3 offset, in List<FABRIKEffector> sections) {
        LimbSection section = GameObject.Instantiate(config.limbSectionPrefab, parent);
        section.transform.Translate(offset, parent);
        section.transform.name = $"{transform.name} {remainingChildren}";

        var effector = section.GetComponent<FABRIKEffector>();
        sections.Add(effector);

        effector.twistConstraint = Mathf.Lerp(config.endTwist, config.initialTwist, remainingChildren / (float)config.sectionCount);
        effector.swingConstraint = Mathf.Lerp(config.endSwing, config.initialSwing, remainingChildren / (float)config.sectionCount);

        if (remainingChildren > 0) {
            buildLimbSection(remainingChildren - 1, section.transform, section.childOffset, in sections);
        } else {
            buildLimbEnd(section.transform, section.childOffset, in sections);
        }
    }

    internal Vector3 getEndPosition() {
        return end.rb.position;
    }

    internal Quaternion getEndRotation() {
        return end.rb.rotation;
    }

    private void buildLimbEnd(Transform parent, Vector3 offset, in List<FABRIKEffector> sections) {
        LimbEnd section = GameObject.Instantiate(config.limbEndPrefab, parent);
        section.transform.Translate(offset, parent);
        section.transform.name = $"{transform.name} END";
        end = section;

        var effector = section.GetComponent<FABRIKEffector>();
        sections.Add(effector);

        effector.twistConstraint = 0;
        effector.swingConstraint = 0;
    }

    private void FixedUpdate() {
        limbChain.Target = Vector3.MoveTowards(limbChain.EndEffector.Position, limbTarget, Time.fixedDeltaTime * config.speed);

        // propagate update up the chain from the end
        limbChain.Backward();

        // propagate results forward down the chain again
        limbChain.ForwardMulti();
    }

    public void setTarget(Vector3 target) {
        this.limbTarget = target;
    }

    private void OnDrawGizmos() {
        if (limbChain != null) {
            Debug.DrawLine(end.rb.position, limbChain.Target, Color.red, 0);
            Debug.DrawLine(end.rb.position, limbTarget, Color.green, 0);
        }
    }
}
