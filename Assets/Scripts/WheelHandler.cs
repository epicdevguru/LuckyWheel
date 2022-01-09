/***********************************************************
 *                   WheelHandler.cs
 *-Description: 
 *  General wheel handler of this project, it loads the 
 *  wheel item controller scriptable object as well and
 *  generate the items on the wheel. 
 *  In addition it also handles the rotating of the wheel, etc.
 *- Made By: Toshiyuki Haras
 *- CO-OP: 
 *                    2022-01-09
 * *********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelHandler : MonoBehaviour
{
    #region Serialization Field
    [SerializeField]
    [Tooltip("The reference of the wheel item container scriptable object ")]
    private SO_WheelItemContainer _wheelItemContainer;

    [SerializeField]
    [Tooltip("the reference of the item image sprite storage scriptable object")]
    private SO_SpriteStorage _spriteStorage;

    [SerializeField]
    [Tooltip("The prefab game object of the wheel item")]
    private GameObject _prefabItem;

    [SerializeField]
    [Tooltip("The transform of the wheel game object")]
    private Transform _transWheel;
    #endregion

    #region private props
    List<WheelItem> _listWheelItem = new List<WheelItem>();     // list of wheel item game objects that are instantiated
    List<float> _listCumulatedDropRates = new List<float>();    // list of cumulative drop rate of the wheel items such as (10, 20, 25, 45 ,..)
    int _nChosenItemIndex = 0;                                  // Currently chosen wheel Item index for the result
    float _fRandomRotateTotal;                                  // Total wheel rotation angle value as random calculation
    float _fCurrentRotateValue;                                 // Currently rotated angle when the wheel is rotating process
    float _fRandomAngleSpeed;                                   // Random rotation speed as maximum for begining of rolling the wheel
    int _nChosenAnimationCurve;                                 // Randomly choosen roll animation curve, it is used to calculate the angle speed calculation.
    #endregion

    #region MonoBehavior Callbacks
    // Start is called before the first frame update
    void Start()
    {
        InitWheel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Custom Private Methods
    private void InitWheel()
    {
        int nItemCount = _wheelItemContainer.ListWheelItems.Count;
        for (int i = 0; i < nItemCount; i++)
        {
            GenerateWheelItem(i, nItemCount);
        }
    }

    private void GenerateWheelItem(int nIndex, int nItemCount)
    {
        GameObject objItem = Instantiate(_prefabItem, _transWheel);
        SO_WheelItem so_WheelItem = _wheelItemContainer.ListWheelItems[nIndex];
        Sprite itemSprite = _spriteStorage.listWheelSprites[so_WheelItem.SpriteId];
        WheelItem wItem = objItem.GetComponent<WheelItem>();
        wItem.InitWheelItem(nIndex, so_WheelItem.ItemName, itemSprite,
            so_WheelItem.ItemDescription, _wheelItemContainer.ItemAlignRadius, nItemCount);
        _listWheelItem.Add(wItem);
        if (nIndex > 0)
            _listCumulatedDropRates.Add(so_WheelItem.DropChance + _listCumulatedDropRates[nIndex - 1]);
        else
            _listCumulatedDropRates.Add(so_WheelItem.DropChance);
    }

    private void RotateWheelObject()
    {
        int nListCount = _listCumulatedDropRates.Count;
        float fAngleItemUnitGap = 360f / nListCount;
        float fDetailedAngleForItem = Random.Range(0.2f * fAngleItemUnitGap, 0.8f * fAngleItemUnitGap);

        fDetailedAngleForItem += _nChosenItemIndex * fAngleItemUnitGap;

        _fRandomRotateTotal = 360f * ((int)Random.Range(3, 10) + (0.75f + fDetailedAngleForItem / 360f));
        _fRandomAngleSpeed = Random.Range(540f, 1080f);
        _nChosenAnimationCurve = (int)Random.Range(0, _wheelItemContainer.ListAnimationCurve.Count);
        _fCurrentRotateValue = 0;

        StartCoroutine(RotateWheel());
    }

    IEnumerator RotateWheel()
    {
        while (_fCurrentRotateValue < _fRandomRotateTotal)
        {
            float fVal = _fCurrentRotateValue / _fRandomRotateTotal;
            float fRotationSpeed = _fRandomAngleSpeed * _wheelItemContainer.ListAnimationCurve[_nChosenAnimationCurve].Evaluate(fVal);
            _fCurrentRotateValue += fRotationSpeed * Time.deltaTime;

            _transWheel.Rotate(new Vector3(0, 0, -fRotationSpeed * Time.deltaTime));

            yield return null;
        }
    }
    #endregion

    #region Custom Public Methods
    public void PlayLuckyWheel()
    {
        _transWheel.localEulerAngles = Vector3.zero;
        _nChosenItemIndex = GetRandomItemIndex();
    }

    public int GetRandomItemIndex ()
    {
        int nListCount = _listCumulatedDropRates.Count;
        float fCumulateTotalDropRate = _listCumulatedDropRates[nListCount - 1];
        float fRandomCumulateDropRate = Random.Range(0, fCumulateTotalDropRate);

        for (int i = nListCount - 1; i > -1; i--)
        {
            if (fRandomCumulateDropRate > _listCumulatedDropRates[i])
            {
                 return _nChosenItemIndex;
            }
        }

        return 0;
    }
    #endregion
}
