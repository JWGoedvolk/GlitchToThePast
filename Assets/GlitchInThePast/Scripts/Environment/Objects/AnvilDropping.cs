using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilDropping : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject playerInRoom;

    [SerializeField] private float dropSpeed = 5f;
    [SerializeField] private float resetHeight = 10f;

    private Vector3 _anvilInitialPosition;
    private Vector3 _anvilLastFellPosition;

    private Transform _playerCurrentPosition;
    private Vector3 _targetDropPosition;

    private bool _isPlayerInRoom;
    private bool _isFalling;
    #endregion

    private void Start()
    {
        _anvilInitialPosition = transform.position;
        _anvilLastFellPosition = _anvilInitialPosition;
    }

    private void Update()
    {
        if (_isPlayerInRoom && playerInRoom != null)
        {
            _playerCurrentPosition = playerInRoom.transform;

            _targetDropPosition = new Vector3(_playerCurrentPosition.position.x, _playerCurrentPosition.position.y + 0.3f, _playerCurrentPosition.position.z);
            if (!_isFalling)
            {
                _isFalling = true;
            }
        }
        else if (!_isPlayerInRoom && !_isFalling)
        {
            _targetDropPosition = _anvilLastFellPosition;
        }

        if (_isFalling)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetDropPosition, dropSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetDropPosition) < 0.1f)
            {
                _anvilLastFellPosition = transform.position;
                _isFalling = false;

                StartCoroutine(ResetAnvilPosition());
            }
        }
    }

    #region Public Functions
    public void PlayerEnteredRoom(GameObject player)
    {
        playerInRoom = player;
        _isPlayerInRoom = true;
    }

    public void PlayerLeftRoom()
    {
        playerInRoom = null;
        _isPlayerInRoom = false;
    }
    #endregion

    private IEnumerator ResetAnvilPosition()
    {
        yield return new WaitForSeconds(2f);
        transform.position = _anvilInitialPosition;
    }
}