using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Text gemText;
    public Text killText;
    public Slider healthSlider;
    public GameObject pausePanel;

    private int gemCount = 0;
    private int killCount = 0;

    void Awake() { if (instance == null) instance = this; }

    // This fixes the 'TogglePause' error
    public void TogglePause()
    {
        if (pausePanel != null)
        {
            bool isPaused = !pausePanel.activeSelf;
            pausePanel.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }

    public void AddKill() { killCount++; killText.text = "Kills: " + killCount; }
    public void AddGem() { gemCount++; gemText.text = "Gems: " + gemCount; }
    public void UpdateHealth(float val) { if (healthSlider != null) healthSlider.value = val; }
}