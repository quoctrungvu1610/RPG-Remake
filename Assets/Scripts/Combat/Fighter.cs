using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour,IAction,ISaveable,IModifierProvider
    {
        //[SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        //[SerializeField] float weaponDamage = 5f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        //125 Dynamic Resource Loading
        //[SerializeField] string defaultWeaponName = "Unarmed";

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        //112
        //Weapon currentWeapon = null;
        
        //152
       WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        
        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            
            return AttachWeapon(defaultWeapon);
        }
        private void Start()
        {
            //Look for resource in resources folder that has a type of weapon
            //Weapon weapon = Resources.Load<Weapon>(defaultWeaponName);

            //if(currentWeapon == null)
            //{
            //    EquipWeapon(defaultWeapon);
            //}
            //currentWeaponConfig.ForceInit();
            currentWeapon.ForceInit();
            
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            // Neu target == null thi khong lam gi
            if (target == null) return;
            // Neu target IsDead() thi khong lam gi
            if (target.IsDead()) return;
            //Neu target khac null va khong o trong tam ngam
            if (target != null && !GetIsInRange())
            {
                //Dau tien di den target
                GetComponent<Mover>().MoveTo(target.transform.position, 0.8f);
            }
            else
            {
                //Sau khi di den target roi thi dung lai sau do bat dau AttackBehaviour()
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }
        // Ham GetInRange() tra ve true khi player o trong tam ngam cua enemy
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeaponRange();
        }

        private void AttackBehaviour()
        {
            //Khi tan cong target thi nhin vao target
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;

            }

        }    //Animation Event
        //Ham Trigger Attack set Animations
        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        public void Hit()
        {
            if (target == null) { return; }
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target,gameObject,damage);
            }
            else
            {
                //target.TakeDamage(gameObject,currentWeapon.GetWeaponDamage());
                //146 Damage Progression
                
                target.TakeDamage(gameObject, damage);
            }
        }

        public void Shoot()
        {
            Hit();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;

            //Newwwwwwww
            GetComponent<Mover>().Cancel();
        }
        //Ham stop attack set Animation
        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
        //CanAttack nhan vao 1 combatTarget
        //Neu khong co combat target tra ve false
        //Gan targetToTest vao combatTarget sau do lay Health Component
        //Neu co targetToTest va targetToTest !isDead() thi tra ve true

        // Ham CanAttack tra ve true neu co target va target !isDead()
        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        //Ham Attack nhan vao 1 combatTarget
        //GetComponent ActionScheduler cua doi tuong va sau do goi ham StartAction va dua vao action hien tai
        //Gan target = combatTarget.GetComponent<Health>();
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        //Weapons Part
        public void EquipWeapon(WeaponConfig weapon)
        {
            //if(weapon == null) return;

            //old
            //currentWeapon = weapon;

            //151 Lazy Init
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);

        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        //132

        public Health GetTarget()
        {
            return target;
        }

        //126 Saving Weapon choice
        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
        //147
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetWeaponDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiesr(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
    
}

