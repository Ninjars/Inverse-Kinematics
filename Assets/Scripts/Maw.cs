using UnityEngine;

public class Maw : MonoBehaviour {

    public Collider collider;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Target") {
            other.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other) {
        // TODO: work out how to filter such that only the maw collider triggers the object destruction 
        foreach (var coll in other.contacts) {
            if (coll.thisCollider == collider) {
                if (other.transform.tag == "Target") {
                    other.gameObject.SetActive(false);
                }
            }
        }
    }
}
