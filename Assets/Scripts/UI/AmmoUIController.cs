using TMPro;
using UnityEngine;

public class AmmoUIController : MonoBehaviour
{
    public TextMeshProUGUI normalAmmoText;
    public TextMeshProUGUI bombAmmoText;
    public TextMeshProUGUI penetratingAmmoText;
    public TextMeshProUGUI destroyWallAmmoText;

    void Update()
    {
        if (Shoot.Instance == null) return;

        normalAmmoText.text = Shoot.Instance.GetAmmoText(Shoot.BulletType.Normal);
        bombAmmoText.text = Shoot.Instance.GetAmmoText(Shoot.BulletType.Bomb);
        penetratingAmmoText.text = Shoot.Instance.GetAmmoText(Shoot.BulletType.Penetrating);
        destroyWallAmmoText.text = Shoot.Instance.GetAmmoText(Shoot.BulletType.DestroyWall);
    }
}
