using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private IRayProvider _rayProvider;
    private ISelector _selector;
    private ISelectionResponse _selectionResponse;
    
    internal Transform CurrentSelection;

    private void Awake()
    {
        _rayProvider = GetComponent<IRayProvider>();
        _selector = GetComponent<ISelector>();
        _selectionResponse = GetComponent<ISelectionResponse>();
    }

    private void Update()
    {
        if (CurrentSelection != null) 
            _selectionResponse.OnDeselect(CurrentSelection);
        
        _selector.Check(_rayProvider.CreateRay());
        CurrentSelection = _selector.GetSelection();
        
        if (CurrentSelection != null) 
            _selectionResponse.OnSelect(CurrentSelection);
    }
}