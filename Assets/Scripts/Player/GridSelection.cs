﻿using UnityEngine;

public class GridSelection : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabSelection;

    private bool _isMe = true;
    private bool _isPlacementEnabled = false;
    private GameObject _selectionGo = null;

    public void SetMe(bool value) => _isMe = value;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to enable/disable selection mode
        {
            if (_isPlacementEnabled)
            {
                Destroy(_selectionGo);
                _isPlacementEnabled = false;
            }
            else
            {
                _selectionGo = Instantiate(_prefabSelection, transform);
                _isPlacementEnabled = true;
                UpdateSelectionPosition();
            }
        }
        if (_isPlacementEnabled)
        {
            UpdateSelectionPosition();
        }
    }

    /// <summary>
    /// Update selection position depending of cursor position
    /// </summary>
    private void UpdateSelectionPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("Floor")))
        {
            var pos = hit.point;
            int x = Mathf.RoundToInt(transform.position.x), z = Mathf.RoundToInt(transform.position.z);
            if (pos.x < x - .5f) x--;
            else if (pos.x > x + .5f) x++;
            if (pos.z < z) z--;
            else if (pos.z > z + 1) z++;
            _selectionGo.transform.position = new Vector3(x, 0.001f, z);
        }
    }
}
