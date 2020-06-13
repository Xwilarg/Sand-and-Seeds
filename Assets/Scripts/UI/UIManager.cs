﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManager { private set; get; }

    [SerializeField]
    private ActionBar _actionBar;

    [SerializeField]
    private GameObject _inventoryPanel;

    [SerializeField]
    private Image _crosshair;

    [SerializeField]
    private Sprite _emptyCrosshair;

    private void Awake()
    {
        uiManager = this;

        // Make sure inventory panel is not active at game start
        _inventoryPanel.SetActive(false);
    }

    private PlayerInfo _info = null; // Keep track of keys to press to toggle different UI parts
    private Inventory _inventory;
    private List<ActionBarSlot> _inventorySlots;

    public void SetPlayerInfo(PlayerInfo info)
        => _info = info;

    public ActionBar GetActionBar() => _actionBar;

    private void Start()
    {
        _inventorySlots = _inventoryPanel.GetComponentsInChildren<ActionBarSlot>().ToList();
    }

    public void InitInventory(Inventory inventory)
    {
        _inventory = inventory;
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        var items = _inventory.GetInventory();
        _actionBar.UpdateSlots(items);
        int i;
        for (i = 0; i < items.Length; i++)
        {
            _inventorySlots[i].SetItem(items[i].item, items[i].amount);
        }
        for (; i < _inventorySlots.Count; i++)
        {
            _inventorySlots[i].SetItem(null, 0);
        }
    }

    /// <summary>
    /// Called when the player press escape, close all opened windows
    /// </summary>
    /// <returns>Returns if there was anything to close</returns>
    public bool CloseUI()
    {
        if (!_inventoryPanel.activeInHierarchy)
            return false;
        _inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        return true;
    }

    private void Update()
    {
        if (_info == null)
            return;
        if (Input.GetKeyDown(Options.S.GetInfo().inventoryKey)) // TODO: Need to lock camera movements when triggered
        {
            if (_inventoryPanel.activeInHierarchy)
            {
                _inventoryPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                _inventoryPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }
        foreach (var action in _actionBar.GetActions()) // We check if any key of the action bar is pressed
            if (Input.GetKeyDown(action.key))
            {
                _actionBar.SelectSlot(action.slot);
                PlayerController.LOCAL.WEAPON_CONTROLLER.ResetZoom();
                PlayerController.LOCAL.UpdateSelectionColor();
                _crosshair.sprite = _actionBar.GetCurrentlySelectedItem()?.GetCrosshair() ?? _emptyCrosshair;
            }
    }
    
    public bool TradeObjectPosition(Item item, Vector2 pos)
    {
        int i = 0;
        foreach (var slot in _inventorySlots)
        {
            if (TradeObjectPosition(slot, item, pos, i))
                return true;
            i++;
        }
        i = 0;
        foreach (var slot in _actionBar.GetActionBarSlots())
        {
            if (TradeObjectPosition(slot, item, pos, i))
                return true;
            i++;
        }
        return false;
    }

    private bool TradeObjectPosition(ActionBarSlot slot, Item item, Vector2 pos, int slotId)
    {
        var rTransform = (RectTransform)slot.transform;
        var oPos = rTransform.position;
        var s = rTransform.sizeDelta.x / 2f; // It's a square to X is equal to Y
        if (pos.x > oPos.x - s && pos.x < oPos.x + s
            && pos.y > oPos.y - s && pos.y < oPos.y + s)
        {
            _inventory.Swap(item.GetId(), slotId);
            return true;
        }
        return false;
    }
}
