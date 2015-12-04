using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveForce;
    public float MoveFriction;
    public float MaxSpeed;

    Rigidbody _rb;
    Camera _camera;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        var moveVec = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveVec += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveVec -= transform.forward;
        if (Input.GetKey(KeyCode.D)) moveVec += transform.right;
        if (Input.GetKey(KeyCode.A)) moveVec -= transform.right;

        if (moveVec.sqrMagnitude < .5f) {
            _rb.AddForce(MoveFriction * -_rb.velocity);
        } else {
            _rb.AddForce(MoveForce * moveVec.normalized);
        }

        if (_rb.velocity.sqrMagnitude > MaxSpeed*MaxSpeed) {
            _rb.velocity = _rb.velocity.normalized * MaxSpeed;
        }

        _rb.MoveRotation(Quaternion.Euler(0, _rb.rotation.eulerAngles.y + 0.5f*Input.GetAxis("Horizontal"), 0));
        var newPitch = _camera.transform.localRotation.eulerAngles.x - 0.2f*Input.GetAxis("Vertical");
        if (newPitch > 45 && newPitch < 180) newPitch = 45;
        if (newPitch < 330 && newPitch > 180) newPitch = 330;
        _camera.transform.localRotation = Quaternion.Euler(newPitch, 0, 0);
    }
}
