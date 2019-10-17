using UnityEngine;

class TargetSpawner : MonoBehaviour {
    public GameObject targetPrefab;
    public float spawnRadius;
    public float spawnInterval;

    private ObjectPool targetPool;
    private float spawnTimer;

    private void Awake() {
        targetPool = new ObjectPool(targetPrefab, 20);
    }

    private void Update() {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnInterval) {
            spawnTimer -= spawnInterval;
            var target = targetPool.getObject();
            initialiseTarget(target);
        }
    }

    private GameObject initialiseTarget(GameObject target) {
        var offset = Random.insideUnitCircle * spawnRadius;
        target.transform.position = new Vector3(transform.position.x + offset.x, transform.position.y, transform.position.z + offset.y);
        target.transform.rotation = Random.rotation;

        var rb = target.GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(Random.value, Random.value, Random.value);
        rb.velocity = new Vector3(0, -0.5f, 1);

        target.SetActive(true);
        return target;
    }
}