/***********************************************************
 *                   WheelHandler.cs
 *-Description: 
 *  General wheel handler of this project, it loads the 
 *  wheel item controller scriptable object as well and
 *  generate the items on the wheel. 
 *  In addition it also handles the rotating of the wheel, etc.
 *- CO-OP: 
 *                    2022-01-09
 * *********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [SerializeField]
    [Tooltip("The min value of the random wheel rotation")]
    [Range(2, 8)]
    private int _nMinRandomRotation;

    [SerializeField]
    [Tooltip("The max value of the random wheel rotation")]
    [Range(5, 10)]
    private int _nMaxRandomRotation;

    [SerializeField]
    [Tooltip("The initial min value of the random wheel rotation speed")]
    [Range(240, 600)]
    private float _fMinRandomSpeed;

    [SerializeField]
    [Tooltip("The initial max value of the random wheel rotation speed")]
    [Range(540, 1080)]
    private float _fMaxRandomSpeed;

    [SerializeField]
    [Tooltip("The min value of avoiding the item choose ambiguity value in item borderline")]
    [Range(0.1f, 0.5f)]
    private float _fMinAvoidAmbiguity;

    [SerializeField]
    [Tooltip("The max value of avoiding the item choose ambiguity value in item borderline")]
    [Range(0.6f, 0.9f)]
    private float _fMaxAvoidAmbiguity;

    [SerializeField]
    [Tooltip("Result show Wait time")]
    [Range(0, 5f)]
    private float _fResultWaitTime = 2f;
    #endregion

    #region private props
    List<WheelItem> _listWheelItem = new List<WheelItem>();     // list of wheel item game objects that are instantiated
    List<float> _listCumulatedDropRates = new List<float>();    // list of cumulative drop rate of the wheel items such as (10, 20, 25, 45 ,..)
    List<int> _listDropRateMixer = new List<int>();             // Mix the overall drop rate digits in the list so that make the item drop more randomized.
    int _nChosenItemIndex = 0;                                  // Currently chosen wheel Item index for the result
    float _fRandomRotateTotal;                                  // Total wheel rotation angle value as random calculation
    float _fCurrentRotateValue;                                 // Currently rotated angle when the wheel is rotating process
    float _fRandomAngleSpeed;                                   // Random rotation speed as maximum for begining of rolling the wheel
    int _nChosenAnimationCurve;                                 // Randomly choosen roll animation curve, it is used to calculate the angle speed calculation.
    int _nCurrentFocusedItem = -1;                              // Currently focused item by arrow.
    bool _bInitialized = false;                                 // flag value to check the wheel initialized

    #endregion

    #region const value
    const float PI2_ANGLE = 360f;
    const float QUATER = 0.75f;
    #endregion

    public event Action<int> OnFocusedItemChanged;
    public event Action<Sprite, string> OnRollFinished;

    #region MonoBehavior Callbacks
    // Start is called before the first frame update
    void Start()
    {
        InitWheel();
    }
    #endregion

    #region Custom Private Methods
    private void GenerateWheelItem(int nIndex, int nItemCount)
    {
        GameObject objItem = Instantiate(_prefabItem, _transWheel);
        SO_WheelItem so_WheelItem = _wheelItemContainer.ListWheelItems[nIndex];
        Sprite itemSprite = _spriteStorage.listWheelSprites[so_WheelItem.SpriteId];
        WheelItem wItem = objItem.GetComponent<WheelItem>();
        wItem.InitWheelItem(nIndex, so_WheelItem.ItemName, so_WheelItem.SpriteId, itemSprite,
            so_WheelItem.ItemDescription, _wheelItemContainer.ItemAlignRadius,
            nItemCount, this);
        _listWheelItem.Add(wItem);
        if (nIndex > 0)
            _listCumulatedDropRates.Add(so_WheelItem.DropChance + _listCumulatedDropRates[nIndex - 1]);
        else
            _listCumulatedDropRates.Add(so_WheelItem.DropChance);
    }

    private void RotateWheelObject()
    {
        int nListCount = _listCumulatedDropRates.Count;
        float fAngleItemUnitGap = PI2_ANGLE / nListCount;
        float fDetailedAngleForItem = Random.Range(_fMinAvoidAmbiguity * fAngleItemUnitGap, _fMaxAvoidAmbiguity * fAngleItemUnitGap);

        fDetailedAngleForItem += _nChosenItemIndex * fAngleItemUnitGap;
        _fRandomRotateTotal = PI2_ANGLE * ((int)Random.Range(_nMinRandomRotation, _nMaxRandomRotation) + (QUATER + fDetailedAngleForItem / PI2_ANGLE));
        _fRandomAngleSpeed = Random.Range(_fMinRandomSpeed, _fMaxRandomSpeed);
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
            CalculateCurrentFocus();
            if (_fCurrentRotateValue >= _fRandomRotateTotal)
            {
                RollFinished();
            }
            yield return null;
        }
    }

    void CalculateCurrentFocus()
    {
        float fAngleUnit = PI2_ANGLE / _listWheelItem.Count;
        int nCountItem = (int)((_fCurrentRotateValue + PI2_ANGLE / 4f) / fAngleUnit);
        int nFocusVal = nCountItem % _listWheelItem.Count;
        if (nFocusVal != _nCurrentFocusedItem)
        {
            _nCurrentFocusedItem = nFocusVal;
            OnFocusedItemChanged.Invoke(_nCurrentFocusedItem);
        }
    }

    void RollFinished()
    {
        StartCoroutine(WaitAndInformRollResult(_fResultWaitTime));
    }

    IEnumerator WaitAndInformRollResult(float fWait)
    {
        yield return new WaitForSeconds(fWait);
       
        int nSpriteId =  _listWheelItem[_nCurrentFocusedItem].GetSpriteIndex();
        Sprite itemSprite = _spriteStorage.listWheelSprites[nSpriteId];
        string itemDescription = _listWheelItem[_nCurrentFocusedItem].GetDescription();
        OnRollFinished.Invoke(itemSprite, itemDescription);
        gameObject.SetActive(false);
    }

    void MixTheDropRateValues(float fTotalVal)
    {
        int nTotalVal = (int)fTotalVal;
        List<int> listOriginals = new List<int>();
        _listDropRateMixer.Clear();
        for (int i = 0; i < nTotalVal; i++)
        {
            listOriginals.Add(i);
        }

        while (listOriginals.Count > 0)
        {
            int nRandomChoose = Random.Range(0, listOriginals.Count);
            _listDropRateMixer.Add(listOriginals[nRandomChoose]);
            listOriginals.RemoveAt(nRandomChoose);
        }
    }
    #endregion

    #region Custom Public Methods
    public void InitWheel()
    {
        if (_bInitialized)
        {
            OnFocusedItemChanged.Invoke(-1);
            _transWheel.localEulerAngles = Vector3.zero;
            _fRandomRotateTotal = 0;
            _fCurrentRotateValue = 0;
            _nChosenItemIndex = 0;
            _nCurrentFocusedItem = -1;
            return;
        }

        int nItemCount = _wheelItemContainer.ListWheelItems.Count;
        for (int i = 0; i < nItemCount; i++)
        {
            GenerateWheelItem(i, nItemCount);
        }
        _bInitialized = true;
    }
    public void PlayLuckyWheel()
    {
        _transWheel.localEulerAngles = Vector3.zero;
        _nChosenItemIndex = GetRandomItemIndex();
        RotateWheelObject();
    }

    public void ShowWheel()
    {
        gameObject.SetActive(true);
    }

    public int GetRandomItemIndex ()
    {
        int nListCount = _listCumulatedDropRates.Count;
        float fCumulateTotalDropRate = _listCumulatedDropRates[nListCount - 1];
        MixTheDropRateValues(fCumulateTotalDropRate);
        int nRandomIndex = Random.Range(0, _listDropRateMixer.Count);
        int nRandomCumulateDropRate = _listDropRateMixer[nRandomIndex];
        float fRandomCumulateDropRate = (float)nRandomCumulateDropRate;

        for (int i = nListCount - 1; i > -1; i--)
        {
            if (fRandomCumulateDropRate > _listCumulatedDropRates[i])
            {
                 return i+1;
            }
        }
        return 0;
    }

    public SO_WheelItemContainer GetWheelItemContainer()
    {
        return _wheelItemContainer;
    }

    public int GetTestResult ()
    {
        return GetRandomItemIndex();
    }

    public void TestSectorOccurance (int nSectorId)
    {
        _transWheel.localEulerAngles = Vector3.zero;
        _nChosenItemIndex = nSectorId;
        RotateWheelObject();
    }
    #endregion
}
