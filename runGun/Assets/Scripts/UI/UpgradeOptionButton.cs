using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class UpgradeOptionButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button button;
    public Image qualityBorder;

    private Upgrade upgrade;
    private Action<Upgrade> onClickCallback;

    private Selectable selectable;
    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        selectable = GetComponent<Selectable>();
    }
    void OnEnable()
    {
        // Force the button to reset its state
        ResetButtonState();
    }

    public void ResetButtonState()
    {
        // This forces the button to exit the hover state
        if (selectable != null)
        {
            // Set the button to normal state
            selectable.OnPointerExit(new PointerEventData(EventSystem.current));

            // Force a visual update
            selectable.animationTriggers.normalTrigger = selectable.animationTriggers.normalTrigger;
        }
    }

    public void Setup(Upgrade upgrade, Action<Upgrade> callback)
    {
        this.upgrade = upgrade;
        onClickCallback = callback;

        // Set UI elements
        if (iconImage != null) iconImage.sprite = upgrade.icon;
        if (titleText != null) titleText.text = upgrade.upgradeName;
        if (descriptionText != null) descriptionText.text = upgrade.description;
        // Shows items quality by changing color of icon and border, should probably be more clear.
        if (qualityBorder != null)
        {
            Color qualityColor = upgrade.GetQualityColor();
            qualityBorder.color = qualityColor;
            iconImage.color = qualityColor;
            button.GetComponent<Image>().color = qualityColor;
        }

        // Set button click handler
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        onClickCallback?.Invoke(upgrade);
    }
}