using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class JumpBarUI : MonoBehaviour
{
    public PlayerStats playerStats;
    private Slider jumpSlider;
    public TextMeshProUGUI jumpText; 

    void Start()
    {
        jumpSlider = GetComponent<Slider>();

        if (playerStats == null)
            Debug.LogError("JumpBarUI: PlayerStats reference not assigned.");

        if (jumpSlider == null)
            Debug.LogError("JumpBarUI: Slider component missing.");

        if (jumpText == null)
            Debug.LogError("JumpBarUI: Jump text reference not assigned.");

        UpdateJumpBar();
    }

    void Update()
    {
        UpdateJumpBar();
    }

    void UpdateJumpBar()
    {
        if (playerStats != null && jumpSlider != null)
        {
            jumpSlider.maxValue = 100f;
            jumpSlider.value = playerStats.jumpForce;

            if (jumpText != null)
            {
                jumpText.text = $"JUMP: {Mathf.RoundToInt(playerStats.jumpForce)}";
            }
        }
    }
}
