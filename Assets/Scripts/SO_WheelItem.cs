/***********************************************************
 *                   SO_WheelItem.cs
 *-Description: 
 *   It is the scriptable object of the wheel items
 *   It contains the wheel Item info such as sprite index
 *   and text string below the item sprite.
 *- Made By: Toshiyuki Hara
 *- CO-OP: 
 *                    2022-01-08
 * *********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelItem", menuName = "Lucky Wheel/Wheel Item", order = 3)]
public class SO_WheelItem : ScriptableObject
{
    #region Serialization Field
    [Multiline(2)]
    [SerializeField]
    string _itemName;                                                                                                                                           
    public string ItemName
    {
        get { return _itemName; }
        set { _itemName = value; }
    }

    [SerializeField]
    int _spriteId;
    public int SpriteId
    {
        get { return _spriteId; }
        set { _spriteId = value; }
    }

    [Multiline(3)]
    [SerializeField]
    string _itemDescription;
    public string ItemDescription
    {
        get { return _itemDescription; }
        set { _itemDescription = value; }
    }

    [SerializeField]
    float _dropChance;
    public float DropChance
    {
        get { return _dropChance; }
        set { _dropChance = value; }
    }
    #endregion
}
