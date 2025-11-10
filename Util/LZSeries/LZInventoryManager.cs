using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LZInventoryManager : MonoBehaviour
{
    public static LZInventoryManager Instance = null;

    public List<LZItemSlot> equipSlots;
    public List<Sprite> itemSprites;
    // public List<string> itemSpriteNames;

    private void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        // Invoke(nameof(SetupEquiptSlot), 1f);
        SetupEquiptSlot();
    }

    void SetupEquiptSlot()
    {
        for (int i = 0; i < itemSprites.Count; ++i)
        {
            LZItemData nItem = new LZItemData();
            nItem.sprite = itemSprites[i];

            equipSlots[i].SetSlot(nItem);
        }
    }

    public void TakeOffFromSceen()
    {
        for (int i = 0; i < equipSlots.Count; i++)
        {
            equipSlots[i].JustTakeOffFromSceen();
        }
    }
}
