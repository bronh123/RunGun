using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WaveDisplay : MonoBehaviour
{

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TextMeshProUGUI waveDisplayText;
    private int finalWave;
    private int currentWave;

    void Start()
    {
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
            currentWave = waveManager.GetCurrentWave();
        }

    }
    void Update()
    {
        if (currentWave < waveManager.GetCurrentWave() && waveDisplayText != null)
        {
            currentWave = waveManager.GetCurrentWave();
            finalWave = waveManager.GetMaxWaves();

            waveDisplayText.text = $"Wave {currentWave} / {finalWave}";
        }
    }
}
