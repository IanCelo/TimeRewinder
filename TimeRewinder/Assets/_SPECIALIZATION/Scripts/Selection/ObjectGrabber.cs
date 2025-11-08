using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class ObjectGrabber : MonoBehaviour
{
	[SerializeField] private SelectionManager _selector;
	[SerializeField] private Transform _owner;
	[SerializeField] private Vector3 _offset;

	private Transform _selectedObject;

	public void OnGrab(InputAction.CallbackContext context)
	{
		if (!context.performed)
			return;

		if (_selectedObject)
		{
			Unselect();
			return;
		}

		Select(); 
	}

	private void Select()
	{
		if (!_selector.CurrentSelection)
		{
			return;
		}
		
		_selectedObject = _selector.CurrentSelection;
		_selectedObject.GetComponent<Rigidbody>().isKinematic = true;
	}

	private void Unselect()
	{
		_selectedObject.GetComponent<Rigidbody>().isKinematic = false;
		_selectedObject = null;
	}

	private void Update()
	{
		if (!_selectedObject)
			return;

		Vector3 offset = _owner.right * _offset.x + _owner.up * _offset.y + _owner.forward * _offset.z;
		_selectedObject.position = _owner.position + offset;
	}

	private void OnDrawGizmos()
	{
		Vector3 offset = _owner.right * _offset.x + _owner.up * _offset.y + _owner.forward * _offset.z;
		Gizmos.DrawSphere(_owner.position + offset, 0.15f);
	}
}
