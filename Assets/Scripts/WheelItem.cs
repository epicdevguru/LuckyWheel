/***********************************************************
 *                   WheelItem.cs
 *-Description: 
 *   It is the script for Wheel Item prefab game object.
 *   The wheel controller uses this prefab to add on the
 *   wheel at the beginning.
 *- Made By: Toshiyuki Hara
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

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Custom Public Methods

    #endregion
}
