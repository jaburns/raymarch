using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;

    Rigidbody _rb;
    Camera _camera;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        var newPos = _rb.position;
        if (Input.GetKey(KeyCode.A)) {
            newPos -= Speed * transform.right;
        }
        else if (Input.GetKey(KeyCode.D)) {
            newPos += Speed * transform.right;
        }
        if (Input.GetKey(KeyCode.S)) {
            newPos -= Speed * transform.forward;
        }
        else if (Input.GetKey(KeyCode.W)) {
            newPos += Speed * transform.forward;
        }
        _rb.MovePosition(newPos);

        _rb.MoveRotation(Quaternion.Euler(0, _rb.rotation.eulerAngles.y + 0.5f*Input.GetAxis("Horizontal"), 0));
        var newPitch = _camera.transform.localRotation.eulerAngles.x - 0.2f*Input.GetAxis("Vertical");
        if (newPitch > 45 && newPitch < 180) newPitch = 45;
        if (newPitch < 330 && newPitch > 180) newPitch = 330;
        _camera.transform.localRotation = Quaternion.Euler(newPitch, 0, 0);
    }
}
