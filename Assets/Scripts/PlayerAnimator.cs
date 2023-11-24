using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private IPlayerController _player;
    private int flipVector;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<IPlayerController>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleSpriteFlipping();
    }

    private void HandleSpriteFlipping()
    {
        if (Mathf.Abs(_player.Input.x) > 0.1f) _renderer.flipX = _player.Input.x < 0;
    }
}
