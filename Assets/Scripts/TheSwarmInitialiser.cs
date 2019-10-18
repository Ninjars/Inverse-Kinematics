using UnityEngine;

public class TheSwarmInitialiser : MonoBehaviour {

    public GameObject octopus;
    public GameObject target;
    public int octopusCount = 5;
    public int targetCount = 200;
    public float radius = 100f;

    void Start() {
        for (int i = 0; i < octopusCount; i++) {
            var position = Random.insideUnitCircle * radius;
            Instantiate(octopus, new Vector3(position.x, 12, position.y), Quaternion.identity);
        }
        
        for (int i = 0; i < targetCount; i++) {
            var position = Random.insideUnitCircle * radius;
            Instantiate(target, new Vector3(position.x, 1, position.y), Quaternion.identity);
        }
    }
}
