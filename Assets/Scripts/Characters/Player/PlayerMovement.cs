using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Label("Speed", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(5, 15f)] private float _speed = 5f;

    [Label("Rotation", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 20)] private float _rotationPerFrame = 10f;

    [Inject] private Joystick _joystick;
    private CharacterController _cc;
    private Player _player;
    private Vector3 moveDir;

    public float MoveVelocity => _cc.velocity.sqrMagnitude;
    public bool IsMove { get; private set; }

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _player = GetComponent<Player>();

        GameLogic.OnGameFinished += DisableJoystick;
    }

    private void Update()
    {
        Movement();
        Rotate();

        HandleGravity();
    }

    private void Movement()
    {
        moveDir.Set(_joystick.Horizontal, 0, _joystick.Vertical);

        _cc.SimpleMove(moveDir * _speed);
        IsMove = moveDir.magnitude > 0 && _cc.isGrounded;
    }
    private void Rotate()
    {
        if (moveDir.magnitude > 0)
        {
            Quaternion currentRot = transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);

            transform.rotation = Quaternion.Slerp(currentRot, targetRot, _rotationPerFrame * Time.deltaTime);
        }
    }
    private void HandleGravity()
    {
        if (_cc.isGrounded)
        {
            float groundedGravity = .05f;
            moveDir.y = groundedGravity;
        }
        else
        {
            moveDir.y = Physics.gravity.y;
        }
    }

    private void DisableJoystick()
    {
        if (_joystick != null)
            _joystick.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameLogic.OnGameFinished -= DisableJoystick;
    }
}
