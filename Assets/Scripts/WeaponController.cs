using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData[] weaponsData;
    [SerializeField] private Transform weaponHolder;
    public UnityEvent<WeaponData> OnWeaponChanged;

    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;

    public WeaponData SelectedWeapon => weaponsData[currentWeaponIndex];

    private void Start()
    {
        SelectWeapon(weaponIndex:0);
    }

    public void SelectWeapon(int weaponIndex)
    {
        var data = weaponsData[weaponIndex];
        var weaponPrefab = data.WeaponPrefab;

        if (currentWeapon != null)
        {
            Destroy(currentWeapon); // Destroy the current weapon if it exists
        }
        currentWeapon = Instantiate(weaponPrefab, weaponHolder);
        OnWeaponChanged.Invoke(SelectedWeapon);
    }

    public void NextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponsData.Length)
        {
            currentWeaponIndex = 0; // Loop back to the first weapon
        }
        SelectWeapon(currentWeaponIndex);
    }

    public void PreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weaponsData.Length - 1; // Loop back to the last weapon
        }
        SelectWeapon(currentWeaponIndex);
    }
}
