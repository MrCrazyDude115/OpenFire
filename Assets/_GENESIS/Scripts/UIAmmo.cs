using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAmmo : MonoBehaviour
{

    public static UIAmmo instance;

    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image[] ammoBulletDisplay;

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

    public void UpdateAmmoDisplay()
    {
        ammoText.text = PlayerShooter.instance.GetCurrentAmmo().ToString();

        if (PlayerShooter.instance.GetCurrentAmmo() <= 2 && PlayerShooter.instance.GetCurrentAmmo() != 0)
        {
            ammoText.color = Color.orange;
        }
        else if (PlayerShooter.instance.GetCurrentAmmo() == 0)
        {
            ammoText.color = Color.red;
        }
        else
        {
            if (ammoText.color != Color.white)
            {
                ammoText.color = Color.white;
            }
        }
        UpdateBulletDisplay();
    }

    private void UpdateBulletDisplay()
    {
        int currentAmmo = PlayerShooter.instance.GetCurrentAmmo();

        for (int i = 0; i < ammoBulletDisplay.Length; i++)
        {
            if (i < currentAmmo)
            {
                ammoBulletDisplay[i].enabled = true;
            }
            else
            {
                ammoBulletDisplay[i].enabled = false;
            }
        }
    }
}
