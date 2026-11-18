using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 35.0f;
    public float xSpeed = 0.25f;
    public float ySpeed = 0.25f;
    public float zoomSpeed = 2.0f;

    private float x = 45.0f;
    private float y = 25.0f;

    void Start()
    {
        if (target == null)
        {
            GameObject go = new GameObject("AutoTarget");
            go.transform.position = new Vector3(0, 2f, 0);
            target = go.transform;
        }
    }

    void LateUpdate()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        // Rodar à volta ao arrastar com o botão esquerdo do rato
        if (mouse.leftButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            x += delta.x * xSpeed;
            y -= delta.y * ySpeed;
            y = Mathf.Clamp(y, 5f, 85f);
        }

        // Zoom com a roda do rato
        float scroll = mouse.scroll.ReadValue().y;
        distance -= scroll * 0.01f * zoomSpeed;
        distance = Mathf.Clamp(distance, 8f, 80f);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}