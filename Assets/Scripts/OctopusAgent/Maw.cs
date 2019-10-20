using UnityEngine;

public class Maw : MonoBehaviour {

    public GameObject effectPrefab;
    public Collider collider;

    private static ObjectPool effectPool;

    void Awake() {
        if (effectPool == null) {
            effectPool = new ObjectPool(effectPrefab, 10);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Target") {
            playEffectAtPosition(other.transform.position);
            other.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other) {
        // TODO: work out how to filter such that only the maw collider triggers the object destruction 
        foreach (var coll in other.contacts) {
            if (coll.thisCollider == collider) {
                if (other.transform.tag == "Target") {
                    playEffectAtPosition(other.transform.position);
                    other.gameObject.SetActive(false);
                }
            }
        }
    }

    private void playEffectAtPosition(Vector3 position) {
        var effect = effectPool.getObject();
        effect.transform.position = position;
        effect.SetActive(true);
    }
}
