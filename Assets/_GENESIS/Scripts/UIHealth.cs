using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealth : MonoBehaviour
{

    public static UIHealth instance;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image[] healthpackDisplay;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {

    }

    public void UpdateLivesDisplay()
    {
        healthText.text = PlayerHealth.instance.GetCurrentLives().ToString();

        if (PlayerHealth.instance.GetCurrentLives() > 1)
        {
            healthText.color = Color.white;
        }
        else
        {
            healthText.color = Color.red;
        }
        UpdateHealthpackDisplay();
    }

    private void UpdateHealthpackDisplay()
    {
        int currentLives = PlayerHealth.instance.GetCurrentLives();

        for (int i = 0; i < healthpackDisplay.Length; i++)
        {
            if (i < currentLives)
            {
                healthpackDisplay[i].enabled = true;
            }
            else
            {
                healthpackDisplay[i].enabled = false;
            }
        }
    }
}
