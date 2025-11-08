using System;
using CVS_Time;
using UnityEngine;


[TimeSaved]
internal class TransformSaver : MonoBehaviour
{
	[SerializeField] private Rigidbody _cubeRigidbody;

	private Vector3 _velocityBeforeFreeze;
	private Vector3 _angularVelocityBeforeFreeze;

	[TimeSaved] private Vector3 Position
	{
		get => transform.position;
		set => transform.position = value;
	}

	[TimeSaved] private Quaternion Rotation
	{
		get => transform.rotation;
		set => transform.rotation = value;
	}
	
	[TimeSaved] private Vector3 Velocity
	{
		get => _cubeRigidbody.velocity;
		set => _velocityBeforeFreeze = value;
	}
		
	[TimeSaved] private Vector3 AngularVelocity
	{
		get => _cubeRigidbody.angularVelocity;
		set => _angularVelocityBeforeFreeze = value;
	}
	
	private void Awake()
	{
		_cubeRigidbody ??= GetComponent<Rigidbody>();
	}

	private void Start()
	{
		TimeManager.Instance.OnSaveStart += StartPhysics;
		TimeManager.Instance.OnRewindStart += StopPhysics;
	}

	private void StartPhysics()
	{
		_cubeRigidbody.useGravity = true;
		_cubeRigidbody.isKinematic = false;
		_cubeRigidbody.velocity = _velocityBeforeFreeze;
		_cubeRigidbody.angularVelocity = _angularVelocityBeforeFreeze;
	}

	private void StopPhysics()
	{
		_velocityBeforeFreeze = _cubeRigidbody.velocity;
		_angularVelocityBeforeFreeze = _cubeRigidbody.angularVelocity;
		_cubeRigidbody.useGravity = false;
		_cubeRigidbody.isKinematic = true;
	}

	private void OnDestroy()
	{
		TimeManager.Instance.OnSaveStart -= StartPhysics;
		TimeManager.Instance.OnRewindStart -= StopPhysics;
	}
}