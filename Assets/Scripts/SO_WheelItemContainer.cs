/***********************************************************
 *                   SO_WheelItemContainer.cs
 *-Description: 
 *   It is the scriptable object that stores the 
 *   wheel item scriptable objects in a list.
 *   In addition it also contains the info related to wheel.
 *- Made By: Toshiyuki Hara
 *- CO-OP: 
 *                    2022-01-08
 * *********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelItemContainer", menuName = "Lucky Wheel/Wheel Item Container", order = 3)]
public class SO_WheelItemContainer : ScriptableObject
{
    [SerializeField]
    [Tooltip ("The list of wheel item Scriptable objects")]
    List<SO_WheelItem> _listWheelItems = new List<SO_WheelItem>();
    public List<SO_WheelItem> ListWheelItems {
        get { return _listWheelItems; }
    }

    [SerializeField]
    [Tooltip ("The radius of the items from the wheel center.")]
    [Range (0, 500)]
    float _itemAlignRadius;
    public float ItemAlignRadius
    {
        get { return _itemAlignRadius; }
    }

    [SerializeField]
    List<AnimationCurve> _listAnimationCurves = new List<AnimationCurve>(); 
    public List<AnimationCurve> ListAnimationCurve
    {
        get { return _listAnimationCurves; }
    }
}
