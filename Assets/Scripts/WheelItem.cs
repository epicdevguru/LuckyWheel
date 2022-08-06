/***********************************************************
 *                   WheelItem.cs
 *-Description: 
 *   It is the script for Wheel Item prefab game object.
 *   The wheel controller uses this prefab to add on the
 *   wheel at the beginning.
 *- CO-OP: 
 *                    2022-01-08
 * *********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelItem : MonoBehaviour
{
    #region Serialization Field
    [SerializeField]
    Image _itemSprite;
    [SerializeField]
    TMPro.TextMeshProUGUI _txtItemDescription;
    #endregion

    #region  private Props
    private int _nItemIndex;
    private int _nSpriteIndex;
    private string _description;
    public int ItemIndex
    {
        get { return _nItemIndex; }
        set { _nItemIndex = value; }
    }
    private string _strItemName;
    public string ItemName
    {
        get { return _strItemName; }
        set { _strItemName = value; }
    }
    private RectTransform _rectItem;
    private WheelHandler _wheelHandler;
    #endregion

    #region MonoBehaviour Callbacks

    private void OnDestroy()
    {
        _wheelHandler.OnFocusedItemChanged -= OnFocused;
    }
    #endregion

    #region Custom Public Methods
    public void InitWheelItem (int index, string itemName, int spriteIndex, Sprite sprite, 
        string description, float radius, int itemCount, WheelHandler handler)
    {
        _itemSprite.sprite = sprite;
        _txtItemDescription.text = description;
        _description = description;
        _nItemIndex = index;
        _strItemName = itemName;
        _rectItem = GetComponent<RectTransform>();
        CalculateItemTransform(index, radius, itemCount);
        gameObject.name = itemName;
        _wheelHandler = handler;
        _nSpriteIndex = spriteIndex;
        _wheelHandler.OnFocusedItemChanged += OnFocused;
    }

    public int GetSpriteIndex()
    {
        return _nSpriteIndex;
    }

    public string GetDescription()
    {
        return _description;
    }
    #endregion

    #region Custom Private Methods
    private void CalculateItemTransform(int index, float radius, int itemCount)
    {
        float fUnitAngleGap = 360f / itemCount;
        float fAngle = (index + 0.5f) * fUnitAngleGap;

        Vector2 v2Position = radius * new Vector2(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad));
        Vector3 v3Rotation = new Vector3(0, 0, fAngle - 90f);
        _rectItem.anchoredPosition = v2Position;
        _rectItem.localEulerAngles = v3Rotation;
    }

    private void OnFocused(int nIndex)
    {
        if (nIndex == _nItemIndex)
        {
            transform.localScale = 1.2f * Vector3.one;
        } else
        {
            transform.localScale = Vector3.one;
        }
    }
    #endregion
}
