/*************************************
 *           UIHandler.cs
 *-Description: UI component handler.
 *  It handles the UI button show as 
 *  well as ui animations handling etc.
 *-Made by: Toshiyuki Hara
 *-CO-OP:
 *           2022/01/10
 * **********************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    #region Serialization
    [SerializeField]
    [Tooltip("WheelHandler object")]
    private WheelHandler _wheelHandler;

    [SerializeField]
    [Tooltip("Arrow object Animator")]
    private Animator _animArrow;

    [SerializeField]
    [Tooltip("Wheel Object Animator")]
    private Animator _animWheel;

    [SerializeField]
    [Tooltip("Play Button")]
    private GameObject _playButton;

    [SerializeField]
    [Tooltip("Claim Button")]
    private GameObject _claimButton;

    [SerializeField]
    [Tooltip("Result BG panel")]
    private GameObject _resultBG;

    [SerializeField]
    [Tooltip("Result Item Image")]
    private Image _resultItemImage;

    [SerializeField]
    [Tooltip("Result Item Text")]
    private TMPro.TextMeshProUGUI _txtDescription;

    #endregion
    #region MonoBehavior callbacks
    // Start is called before the first frame update
    void Start()
    {
        SubscribeEvent();
    }

    private void OnDestroy()
    {
        UnsubscribeEvent();
    }
    #endregion

    #region Custom private methods
    private void SubscribeEvent ()
    {
        _wheelHandler.OnFocusedItemChanged += PlayArrowPongAnimation;
        _wheelHandler.OnRollFinished += GetRollResult;
    }

    private void UnsubscribeEvent()
    {
        _wheelHandler.OnFocusedItemChanged -= PlayArrowPongAnimation;
        _wheelHandler.OnRollFinished -= GetRollResult;
    }

    private void PlayArrowPongAnimation(int nIndex)
    {
        _animArrow.Play("Pong");
        _animWheel.Play("WheelAnim");
    }

    private void GetRollResult(Sprite sprite, string description)
    {
        _claimButton.SetActive(true);
        _resultBG.SetActive(true);
        _txtDescription.text = description;
        _resultItemImage.sprite = sprite;
    }
    #endregion

    #region Custom public methods
    public void OnClickPlayButton()
    {
        _wheelHandler.PlayLuckyWheel();
        _playButton.SetActive(false);
    }

    public void OnClickClaimButton()
    {
        _wheelHandler.gameObject.SetActive(true);
        _wheelHandler.InitWheel();
        _resultBG.SetActive(false);
        _claimButton.SetActive(false);
        _playButton.SetActive(true);
    }
    #endregion
}
