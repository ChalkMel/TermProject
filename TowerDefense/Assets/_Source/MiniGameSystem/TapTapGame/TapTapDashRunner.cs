using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.TapTapDash
{
    public enum TurnDirection { Right = 0, Up = 1, Left = 2, Down = 3 }

    public class TapTapDashRunner : MonoBehaviour
    {
        [Header("Path")]
        [SerializeField] private TapTapDashPath path;

        [Header("Finish")]
        [SerializeField] private TapTapDashFinish finish;
        [SerializeField] private int finishBonus = 100;

        [Header("Start")]
        [SerializeField] private Vector2Int initialDirection = Vector2Int.right;

        [Header("Turn Sequence")]
        [SerializeField] private List<TurnDirection> turnSequence = new();

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Fall")]
        [SerializeField] private float fallGravity = 20f;
        [SerializeField] private float fallRotateSpeed = 360f;
        [SerializeField] private float fallYThreshold = -15f;

        private Vector2 _position;
        private Vector2Int _direction;
        private float _distance;
        private int _score;
        private int _turnIndex;
        private bool _isFalling;
        private bool _isDead;
        private bool _isFinished;
        private float _fallVelocityY;

        public int Score => _score;

        public event Action<int> OnScoreChanged;
        public event Action OnFell;
        public event Action OnFinished;

        private void Awake()
        {
            _position = transform.position;
            _direction = initialDirection;
            ApplyTransform();
        }

        private void Update()
        {
            if (_isDead || _isFinished) return;
            if (_isFalling) { UpdateFalling(); return; }

            if (IsTapPressed())
                ConsumeNextTurn();

            float step = moveSpeed * Time.deltaTime;
            _position += (Vector2)_direction * step;
            _distance += step;
            ApplyTransform();
            
            if (Vector2.Distance(_position, finish.transform.position) <= finish.Radius)
            {
                ReachFinish();
                return;
            }

            if (!path.IsOnPath(_position))
            {
                StartFalling();
                return;
            }

            int newScore = Mathf.FloorToInt(_distance);
            if (newScore != _score)
            {
                _score = newScore;
                OnScoreChanged?.Invoke(_score);
            }
        }

        private void ConsumeNextTurn()
        {
            if (_turnIndex < turnSequence.Count)
            {
                _direction = ToVector(turnSequence[_turnIndex]);
                _turnIndex++;
            }
            
        }

        private void ApplyTransform()
        {
            transform.position = _position;
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private static bool IsTapPressed()
        {
            if (Input.GetMouseButtonDown(0)) return true;
            if (Input.GetKeyDown(KeyCode.Space)) return true;
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) return true;
            return false;
        }

        private static Vector2Int ToVector(TurnDirection d) => d switch
        {
            TurnDirection.Right => Vector2Int.right,
            TurnDirection.Up    => Vector2Int.up,
            TurnDirection.Left  => Vector2Int.left,
            TurnDirection.Down  => Vector2Int.down,
            _                   => Vector2Int.right,
        };

        private void StartFalling()
        {
            if (_isFalling) return;
            _isFalling = true;
            _fallVelocityY = 0f;
            OnFell?.Invoke();
        }

        private void UpdateFalling()
        {
            _fallVelocityY -= fallGravity * Time.deltaTime;
            _position.y += _fallVelocityY * Time.deltaTime;
            transform.position = _position;
            transform.Rotate(0f, 0f, -fallRotateSpeed * Time.deltaTime);

            if (_position.y < fallYThreshold)
                _isDead = true;
        }

        private void ReachFinish()
        {
            if (_isFinished) return;
            _isFinished = true;
            _score += finishBonus;
            OnScoreChanged?.Invoke(_score);
            OnFinished?.Invoke();
        }
    }
}