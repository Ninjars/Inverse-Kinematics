using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Limbs/LimbData", order = 1)]
public class LimbData : ScriptableObject {
    public LimbSection limbSectionPrefab;
    public LimbEnd limbEndPrefab;
    public int sectionCount = 10;
    public float speed = 100f;
    public float initialTwist = 60;
    public float endTwist = 10;
    public float initialSwing = 60;
    public float endSwing = 20;
}