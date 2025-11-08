using UnityEngine;

public class ThirdPersonRayProvider : MonoBehaviour, IRayProvider
{
    [SerializeField] private Transform _origin;
    
    public Ray CreateRay()
    {
        return new Ray(_origin.position + Vector3.up, _origin.forward);
    }
}