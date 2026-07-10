using System;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    public enum AnimationState
    {
        Walk,
        Run
    }

    public Transform targetPoint;
    public float speed = 5.0f;
    public AnimationState movementMode;
    public bool startMoveFromStart;

    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private bool _isMoving;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        if (startMoveFromStart)
        {
            _isMoving = true;
        }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     _isMoving = true;
        // }

        if (_isMoving)
        {
            MoveCharacter();
        }
    }

    public void StartMovement()
    {
        _isMoving = true;
    }

    private void MoveCharacter()
    {
        var step = speed * Time.deltaTime;

        var originPosition = transform.position;
        var targetPointPosition = targetPoint.position;
        transform.LookAt(new Vector3(targetPointPosition.x, originPosition.y, targetPointPosition.z));

        originPosition = Vector3.MoveTowards(originPosition, targetPointPosition, step);
        transform.position = originPosition;

        if (transform.position == targetPoint.position)
        {
            _isMoving = false;
            _animator.SetBool(IsRunning, false);
            _animator.SetBool(IsWalking, false);
        }
        else
        {
            switch (movementMode)
            {
                case AnimationState.Run:
                    _animator.SetBool(IsRunning, true);
                    _animator.SetBool(IsWalking, false);
                    break;
                case AnimationState.Walk:
                    _animator.SetBool(IsWalking, true);
                    _animator.SetBool(IsRunning, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}