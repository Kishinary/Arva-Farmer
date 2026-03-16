using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class inventoryScript : MonoBehaviour
{
    
    public Image playerAvatarImage;

    public Image playerWeaponImage;
    //weapon
    [Header("Current Weapon!")]
    public Transform weaponParent;
    

    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer weaponSpriteRenderer;
 



    private void Start()
    {
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();



        changeWeaponSprite();


    }
    private void Update()
    {
        playerAvatarImage.sprite = playerSpriteRenderer.sprite;
        playerAvatarImage.color = playerSpriteRenderer.color;


        
        if (weaponSpriteRenderer != null && weaponSpriteRenderer.sprite != null)
        {
            playerWeaponImage.enabled = true;
            playerWeaponImage.sprite = weaponSpriteRenderer.sprite;
            playerWeaponImage.color = weaponSpriteRenderer.color;
        }else
        {
          
            playerWeaponImage.enabled = false;
        }

    }
    public void changeWeaponSprite()
    {
        weaponSpriteRenderer = weaponParent.GetComponentInChildren<SpriteRenderer>();
    }


}
