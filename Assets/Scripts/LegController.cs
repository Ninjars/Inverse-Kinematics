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

    void FixedUpdate() {
        var elapsed = Time.time - startTime;
        var rawFraction = elapsed / movementDurationSeconds;
        if (elapsed > movementDurationSeconds) {
            if (isTargetOutOfRange(limb.getEndPosition())) {
                // Stretching beyond limb length; reset even though it's not this limb's time to be moving
                limb.setTarget(findTargetPosition(Vector3.zero));
            }
            return;
        }

        var horizontalFraction = Mathf.SmoothStep(0.0f, 1.0f, rawFraction);
        var verticalFraction = Mathf.SmoothStep(0.0f, 1.0f, 1 - Mathf.Abs(rawFraction - 0.5f) * 2);
        var position = Vector3.Lerp(initialPosition, targetPosition, horizontalFraction);
        position += Vector3.Lerp(Vector3.zero, Vector3.up * stepHeight, verticalFraction);

        if (isTargetOutOfRange(position)) {
            // Stretching beyond limb length; reset
            position = findTargetPosition(Vector3.zero);
        }

        limb.setTarget(position);
    }

    internal void takeStep(Vector3 movementVector) {
        initialPosition = limb.getEndPosition();
        targetPosition = findTargetPosition(movementVector);
        if (Vector3.SqrMagnitude(targetPosition - initialPosition) > 0.1) {
            startTime = Time.time;
        }
    }

    private bool isTargetOutOfRange(Vector3 targetPosition) {
        return Vector3.SqrMagnitude(targetPosition - transform.position) > (limb.length * limb.length);
    }

    private Vector3 findTargetPosition(Vector3 movementVector) {
        RaycastHit downHit;
        bool downDidHit = Physics.Raycast(transform.position, Vector3.up * -1, out downHit, 1000, layerMask);
        if (!downDidHit) {
            Debug.Log("unable to find down point");
            return transform.position - Vector3.up * 10;
        }
        RaycastHit legHit;
        bool didHit = Physics.Raycast(transform.position, baseRotation * Vector3.forward, out legHit, 100, layerMask);
        if (!didHit) {
            Debug.Log("unable to find leg point");
            return transform.position - Vector3.up * 10;
        }

        var position = Vector3.Lerp(legHit.point, downHit.point, 0.25f);
        position += new Vector3(movementVector.x, 0, movementVector.z) * stepReachFactor;
        return position;
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, targetPosition, Color.yellow, 0);
    }
}
