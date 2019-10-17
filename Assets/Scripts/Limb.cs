using System;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour {
    public LimbSection limbSectionPrefab;
    public LimbEnd limbEndPrefab;
    public int sectionCount = 10;
    public float speed = 100f;
    public float initialTwist = 60;
    public float endTwist = 10;
    public float initialSwing = 60;
    public float endSwing = 20;

    public Vector3 limbTarget;

    private FABRIKChain limbChain;

    private void Awake() {
        limbChain = buildLimb(transform);
    }

    // this can be expanded to build branching limbs and so return a list of chains.
    // the root chain should be tracked, and in the update function each chain will need backward() called on it
    private FABRIKChain buildLimb(Transform limbOrigin, FABRIKChain parentChain = null, int layer = 0) {
        List<FABRIKEffector> sections = new List<FABRIKEffector>(sectionCount + (parentChain == null ? 2 : 3));
        
        if (parentChain != null) sections.Add(parentChain.EndEffector);

        buildLimbSection(sectionCount, limbOrigin, Vector3.zero, in sections);

        return new FABRIKChain(parentChain, sections, layer);
    }

    private void buildLimbSection(int remainingChildren, Transform parent, Vector3 offset, in List<FABRIKEffector> sections) {
        LimbSection section = GameObject.Instantiate(limbSectionPrefab, parent);
        section.transform.Translate(offset, parent);

        var effector = section.GetComponent<FABRIKEffector>();
        sections.Add(effector);

        effector.twistConstraint = Mathf.Lerp(initialTwist, endTwist, remainingChildren / (float)sectionCount);
        effector.swingConstraint = Mathf.Lerp(initialSwing, endSwing, remainingChildren / (float)sectionCount);

        if (remainingChildren > 0) {
            buildLimbSection(remainingChildren - 1, section.transform, section.childOffset, in sections);
        } else {
            buildLimbEnd(section.transform, section.childOffset, in sections);
        }
    }

    private void buildLimbEnd(Transform parent, Vector3 offset, in List<FABRIKEffector> sections) {
        LimbEnd section = GameObject.Instantiate(limbEndPrefab, parent);
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
        limbChain.Target = Vector3.MoveTowards(limbChain.EndEffector.Position, limbTarget, Time.deltaTime * speed);

        // propagate update up the chain from the end
        limbChain.Backward();

        // propagate results forward down the chain again
        limbChain.ForwardMulti();
    }
}