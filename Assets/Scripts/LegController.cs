using UnityEngine;

[RequireComponent(typeof(Limb))]
public class LegController : MonoBehaviour {
    public float stepHeight = 10f;
    public float stepReachFactor = 10f;
    public LayerMask layerMask;
    [HideInInspector]
    public Quaternion baseRotation;
    [HideInInspector]
    public float movementDurationSeconds;

    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private float startTime;
    private Limb limb;

    private void Awake() {
        this.limb = GetComponent<Limb>();
    }

    private void Start() {
        targetPosition = findTargetPosition(Vector3.zero);
        initialPosition = targetPosition;
    }

    void Update() {
        var elapsed = Time.time - startTime;
        var rawFraction = elapsed / movementDurationSeconds;
        if (elapsed > movementDurationSeconds) return;

        var horizontalFraction = Mathf.SmoothStep(0.0f, 1.0f, rawFraction);
        var verticalFraction = Mathf.SmoothStep(0.0f, 1.0f, 1 - Mathf.Abs(rawFraction - 0.5f) * 2);
        var position = Vector3.Lerp(initialPosition, targetPosition, horizontalFraction);
        position += Vector3.Lerp(Vector3.zero, Vector3.up * stepHeight, verticalFraction);
        limb.setTarget(position);
    }

    internal void takeStep(Vector3 movementVector) {
        initialPosition = limb.getEndPosition();
        targetPosition = findTargetPosition(movementVector);
        if (Vector3.SqrMagnitude(targetPosition - initialPosition) > 0.1) {
            startTime = Time.time;
        }
    }

    private Vector3 findTargetPosition(Vector3 movementVector) {
        RaycastHit legHit;
        bool didHit = Physics.Raycast(transform.position, baseRotation * Vector3.forward, out legHit, 100, layerMask);
        if (!didHit) {
            return transform.position - Vector3.up * 10;
        } else {
            RaycastHit downHit;
            bool downDidHit = Physics.Raycast(transform.position, Vector3.up * -1, out downHit, 1000, layerMask);
            if (!downDidHit) return transform.position - Vector3.up * 10;

            var position = Vector3.Lerp(legHit.point, downHit.point, 0.25f);
            position += movementVector * stepReachFactor;
            return position;
        }
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, targetPosition, Color.yellow, 0);
    }
}
