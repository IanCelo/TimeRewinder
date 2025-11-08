using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace CMF
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
    public class CharacterKeyboardInput : CharacterInput
	{
		public string horizontalInputAxis = "Horizontal";
		public string verticalInputAxis = "Vertical";
		public KeyCode jumpKey = KeyCode.Space;

		//If this is enabled, Unity's internal input smoothing is bypassed;
		public bool useRawInput = true;

		private bool _jumpInput;
		private bool _useInputSystem;
		
		private Vector2 _movementInput;
		private Vector3 _direction;

	#region Input Events
		public void OnMove(InputAction.CallbackContext context)
		{
			_movementInput = context.ReadValue<Vector2>();
			_direction = new Vector3(_movementInput.x, 0.0f, _movementInput.y);
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				_jumpInput = true;
				return;
			}
			
			if (context.canceled)
				_jumpInput = false;
		}
	#endregion
		
		
		public override float GetHorizontalMovementInput()
		{
			if (_useInputSystem)
				return _movementInput.x;

			return useRawInput ? Input.GetAxisRaw(horizontalInputAxis) : Input.GetAxis(horizontalInputAxis);
		}

		public override float GetVerticalMovementInput()
		{
			if (_useInputSystem)
				return _movementInput.y;
			
			return useRawInput ? Input.GetAxisRaw(verticalInputAxis) : Input.GetAxis(verticalInputAxis);
		}

		public override bool IsJumpKeyPressed()
		{
			return _useInputSystem ? _jumpInput : Input.GetKey(jumpKey);
		}

	#region Unity Callbacks
		private void Start()
		{
			_useInputSystem = GetComponent<PlayerInput>();
		}
	#endregion
    }
}
