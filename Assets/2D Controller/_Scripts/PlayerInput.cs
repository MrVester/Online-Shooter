using Unity.VisualScripting;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


    public class PlayerInput : MonoBehaviour
    {
        public FrameInput FrameInput { get; private set; }

        private void Update() => FrameInput = Gather();

#if ENABLE_INPUT_SYSTEM
        private PlayerInputActions _actions;
        private InputAction _move, _jump, _dash, _attack,_equip, _look,_mousePos;

        private void Awake() {
            _actions = new PlayerInputActions();
            _move = _actions.Player.Move;
            _look = _actions.Player.Look;
            _jump = _actions.Player.Jump;
            _dash = _actions.Player.Dash;
            _attack = _actions.Player.Attack;
            _mousePos = _actions.Player.MousePosition;
            _equip = _actions.Player.Equip;
          
        }

        private void OnEnable() => _actions.Enable();

        private void OnDisable() => _actions.Disable();

        private FrameInput Gather() {
            return new FrameInput {
                JumpDown = _jump.WasPressedThisFrame(),
                JumpHeld = _jump.IsPressed(),
                DashDown = _dash.WasPressedThisFrame(),
                AttackDown = _attack.WasPressedThisFrame(),
                AttackHold = _attack.WasPerformedThisFrame(),
                AttackReleased = _attack.WasReleasedThisFrame(),
                Move = _move.ReadValue<Vector2>(),
                Look = _look.ReadValue<Vector2>(),
                MousePos = _mousePos.ReadValue<Vector2>(),
                Equip = _equip.IsPressed(),
                //CurrentDevice = _look.activeControl.device.name,

            };
        }

#elif ENABLE_LEGACY_INPUT_MANAGER
        private FrameInput Gather() {
            return new FrameInput {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                DashDown = Input.GetKeyDown(KeyCode.X),
                AttackDown = Input.GetKeyDown(KeyCode.Z),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
                Equip = Input.GetKey(KeyCode.E),
            };
        }
#endif
}

public struct FrameInput {
        public Vector2 Move;
        public Vector2 Look;
        public Vector2 MousePos;
        //public string CurrentDevice;
        public bool JumpDown;
        public bool JumpHeld;
        public bool DashDown;
        public bool AttackDown;
        public bool AttackHold;
        public bool AttackReleased;
        public bool Equip;
   
    }
