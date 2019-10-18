using UnityEngine;

public class Commander : MonoBehaviour {
    public Body unit;
    public LayerMask inputLayerMask;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f, inputLayerMask)) {
                unit.setTargetPosition(hit.point);
            }
        }
    }
}
