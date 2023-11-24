using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    IPlayerController _player;
    public SpriteRenderer _renderer;
    private void Awake()
    {
        _player = GetComponent<IPlayerController>();
    }
        // Update is called once per frame
        void Update()
    {
        if (Mathf.Abs(_player.Input.x) > 0.1f) _renderer.flipX = _player.Input.x < 0;
        
    }
}
