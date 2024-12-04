using UnityEngine;

public class HealthBillboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Make the health bar face the camera
        transform.rotation = Camera.main.transform.rotation;
    }
}
