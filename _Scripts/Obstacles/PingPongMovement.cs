using UnityEngine;
public enum MovementDirection { X, Y, Z }
public class PingPongMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public MovementDirection direction = MovementDirection.X; // Hangi eksende hareket edecek
    public float distance = 10f; // Hareket mesafesi
    public float speed = 2f; // Hareket hýzý

    private Vector3 startPosition;
    private Vector3 movementAxis;

    void Start()
    {
        // Baþlangýç pozisyonunu kaydet
        startPosition = transform.position;

        // Hareket eksenini belirle
        switch (direction)
        {
            case MovementDirection.X:
                movementAxis = Vector3.right;
                break;
            case MovementDirection.Y:
                movementAxis = Vector3.up;
                break;
            case MovementDirection.Z:
                movementAxis = Vector3.forward;
                break;
        }
    }

    void Update()
    {
        // Ping-pong hareketi uygula
        float pingPong = Mathf.PingPong(Time.time * speed, distance);
        transform.position = startPosition + movementAxis * pingPong;
    }
}
