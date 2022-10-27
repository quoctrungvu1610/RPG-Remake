
using RPG.Core;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        //110
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        //111
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] bool isRightHanded = true;
        //115
        [SerializeField] Projectile projectile = null;
        

        //118 Destroy old weapon
        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            //118 Destroy old weapon
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;
            if(equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            //124 Restting the default animator
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            //-----------------------------------

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            //124 Restting the default animator
            else if (overrideController != null)
            {

                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        //118 Destroy old weapon
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            //looking for something in the right hand
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
            
        }
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        // Shoot Projectile Part
        //146 old  public void LaunchProjectile (Transform rightHand, Transform lefthand, Health target, GameObject instigator)
        public void LaunchProjectile(Transform rightHand, Transform lefthand, Health target, GameObject instigator, float calculateDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, lefthand).position, Quaternion.identity);
            //old
            //projectileInstance.SetTarget(target, instigator, weaponDamage);
            projectileInstance.SetTarget(target, instigator, calculateDamage);
        }
        
        public bool HasProjectile()
        {
            return projectile != null;
        }

        

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }
        public float GetWeaponDamage()
        {
            return weaponDamage;
        }
    }
}