using UnityEngine;

public class TheSwarmInitialiser : MonoBehaviour {

    public Body octopus;
    public GameObject target;
    public int octopusCount = 5;

    public int minArms = 1;
    public int maxArms = 6;
    public int minLegs = 3;
    public int maxLegs = 8;

    public int targetCount = 300;
    public float targetRadius = 100f;
    public float hunterRadius = 200f;

    void Start() {
        octopus.gameObject.SetActive(false);
        for (int i = 0; i < octopusCount; i++) {
            var position = Random.insideUnitCircle * hunterRadius;
            var octo = Instantiate(octopus, new Vector3(position.x, 12, position.y), Quaternion.identity);
            octo.armCount = Random.Range(minArms, maxArms);
            octo.legCount = Random.Range(minLegs, maxLegs);
            octo.gameObject.SetActive(true);
        }
        octopus.gameObject.SetActive(true);
        
        for (int i = 0; i < targetCount; i++) {
            var position = Random.insideUnitCircle * targetRadius;
            Instantiate(target, new Vector3(position.x, 1, position.y), Quaternion.identity);
        }
    }
}
