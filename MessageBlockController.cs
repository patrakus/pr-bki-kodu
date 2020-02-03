using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class MessageBlockController : MonoBehaviour
{
    // references for this variable are auto collected upon initialization
    private RectTransform parentRectTransform;
    private RectTransform rectTransform;

    [FoldoutGroup("References")]
    [SerializeField]
    private RectTransform textBlockRectTransform;
    [FoldoutGroup("References")]
    [SerializeField]
    private TextMeshProUGUI messageTextBlock;
    [FoldoutGroup("References")]
    public TextMeshProUGUI header;
    [FoldoutGroup("References")]
    public TextMeshProUGUI timestamp;

    [FoldoutGroup("General Properties")]
    [SerializeField]
    [Tooltip("Max size for given UI element. Input value is in % between 0 - 1")]
    [MinValue(0), MaxValue(1)]
    private float maxWidth;

    [FoldoutGroup("General Properties")]
    [SerializeField]
    [Tooltip("Max size for given UI element. Input value is in % between 0 - 1")]
    [MinValue(0), MaxValue(1)]
    private float textBlockMaxWidth;

    private float maxWidthInUnityUnitsForTextBlock;
    private float maxWidthInUnityUnitsForText;
    private float predefinedHeigh; // Predefined height from unity editor
    private float newWidth;
    private float currentLineCount;

    //*********************************************
    //*********** Settings ************************
    // Used in Editor code to set corresponding properties for given element

    [FoldoutGroup("Settings")]
    public Color messageBlockColor = Color.white;
    [FoldoutGroup("Settings")]
    public Image messageBlockImage;
    [FoldoutGroup("Settings")]
    public Image profileImage;
    [FoldoutGroup("Settings")]
    public Sprite profileSprite;


    private void Awake()
    {
        //Wait for the first frame
        Invoke("DelayedInit", 0.01f); ;
        //SetInitHeight();
    }

    private void DelayedInit()
    {
        Init();
        //Debug.Log(messageTextBlock.rectTransform.rect);
        // The rest is not required, manly because end user should used apropiate methods to set given message
        // Those method are left because new message is created from prefab and prefab already has some text
        SetRectTransformToMaxWidth();
        //SetText("New Text New Text New Text New Text New Text Text New Text New Text");
        RefreshTextMeshProUGUI(messageTextBlock);
        SetWidthForContainer();
        SetWidthForMessageBlock();

    }

    /// <summary>
    /// Used for testing message fitting.
    /// </summary>
    /// <param name="text"></param>
    [Button]
    private void Settext(string text)
    {
        SetRectTransformToMaxWidth();
        SetMessageText(text);
        RefreshTextMeshProUGUI(messageTextBlock); // required for refreshing all data in given TextMeshPro
        SetWidthForMessageBlock();// After using this method, it is necessary to use RefreshTextMeshProUGUI 
        
        // If this method is omitted then lineCount will have obselete data and text will be not correctly alligned
        RefreshTextMeshProUGUI(messageTextBlock); //

        if (messageTextBlock.textInfo.lineCount > currentLineCount)
        {
            currentLineCount = messageTextBlock.textInfo.lineCount; // this is used in next methods, so it has to be up to date
            StretchContainer();
            StretchMessageBlock();
            
        }
    }

    private void SetMessageText(string text)
    {
        messageTextBlock.SetText(text);
    }

    private void SetHeaderText(string text)
    {
        header.SetText(text);
    }

    private void SetTimestamp(string text)
    {
        timestamp.SetText(text);
    }

    /// <summary>
    /// Refresh mesh and recalculate thing in TextMeshProUGUI object
    /// Use this after setting new text or size in one of parent RectTransform object.
    /// </summary>
    /// <param name="textMeshProUGUI"> Object to refresh </param>
    private void RefreshTextMeshProUGUI(TextMeshProUGUI textMeshProUGUI)
    {
        // After setting new text, we should use this method to refresh mesh containing text. Found this info on Unity forum
        textMeshProUGUI.ForceMeshUpdate();
        // just a guess, but i think if this is not called after changing text and seting new size for container, then text will be not alligned correctly
        textMeshProUGUI.CalculateLayoutInputHorizontal(); 
        
        textMeshProUGUI.ComputeMarginSize(); // Just to be sure
    }

    // TODO: Change this
    private void SetRectTransformToMaxWidth()
    {
        textBlockRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidthInUnityUnitsForText);
    }

    private void StretchContainer()
    {
        #if UNITY_EDITOR
        if (parentRectTransform == null || rectTransform == null)
        {
            // Run this only in editor
            Init();
        }
        #endif

        if (rectTransform != null)
        {

            rectTransform.offsetMin -= new Vector2(0, predefinedHeigh * currentLineCount);
        }
    }

    private void StretchMessageBlock()
    {
        #if UNITY_EDITOR
        if (parentRectTransform == null || rectTransform == null)
        {
            // Run this only in editor
            Init();
        }
        #endif

        if (textBlockRectTransform != null)
        {

            textBlockRectTransform.offsetMin -= new Vector2(0, predefinedHeigh * currentLineCount);
        }
    }

    
    private void SetWidthForMessageBlock()
    {
        if (IsTextWidthExceedingMaxWidth())
        {
            newWidth = maxWidthInUnityUnitsForText;
        }
        else
        {
            newWidth = messageTextBlock.GetRenderedValues(false).x + messageTextBlock.margin.x + messageTextBlock.margin.z+10;
        }

        textBlockRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    private void SetWidthForContainer()
    {
        #if UNITY_EDITOR
        if (parentRectTransform == null || rectTransform == null)
        {
            // Run this only in editor
            Init();
        }
        #endif

        
        newWidth = maxWidthInUnityUnitsForTextBlock;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    private void Init()
    {
        parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        // If null then get first found reference
        if (messageTextBlock == null)
        {
            messageTextBlock = GetComponentInChildren<TextMeshProUGUI>();
        }

        predefinedHeigh = messageTextBlock.rectTransform.rect.height;
        //predefinedHeigh = textBlockRectTransform.rect.height;

        maxWidthInUnityUnitsForTextBlock = parentRectTransform.rect.width * maxWidth;
        maxWidthInUnityUnitsForText = rectTransform.rect.width * textBlockMaxWidth;
        currentLineCount = 1;
    }

    private bool IsTextWidthExceedingMaxWidth()
    {
        return (messageTextBlock.GetRenderedValues(false).x > maxWidthInUnityUnitsForText);
    }

}
