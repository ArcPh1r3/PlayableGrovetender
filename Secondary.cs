using RoR2;
using RoR2.Projectile;
using UnityEngine;
using EntityStates.GravekeeperMonster.Weapon;

namespace EntityStates.Grovetender
{
    public class HealingWisp : BaseSkillState
    {
        public static float damageCoefficient = 3f;
        public static float baseDuration = 0.5f;
        public static float recoil = 1;

        private float duration;
        private float fireDuration;
        private bool hasFired;
        private Animator animator;
        private ChildLocator childLocator;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = HealingWisp.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.15f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "MuzzleHook";

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                this.childLocator = modelTransform.GetComponent<ChildLocator>();
                if (this.childLocator)
                {
                    this.childLocator.FindChild("JarEffectLoop").gameObject.SetActive(true);
                }
            }

            EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.jarOpenEffectPrefab, base.gameObject, GravekeeperBarrage.jarEffectChildLocatorString, false);
            base.PlayAnimation("Jar, Override", "BeginGravekeeperBarrage");
            Util.PlayScaledSound(GravekeeperBarrage.jarOpenSoundString, base.gameObject, 2.5f);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.childLocator)
            {
                this.childLocator.FindChild("JarEffectLoop").gameObject.SetActive(false);
            }

            EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.jarCloseEffectPrefab, base.gameObject, GravekeeperBarrage.jarEffectChildLocatorString, false);
            base.PlayCrossfade("Jar, Override", "EndGravekeeperBarrage", 0.06f);
            Util.PlayScaledSound(GravekeeperBarrage.jarCloseSoundString, base.gameObject, 1.5f);
        }

        private void FireWisp()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(0.75f);
                Ray aimRay = base.GetAimRay();
                EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.muzzleflashPrefab, base.gameObject, this.muzzleString, false);

                if (base.isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(PlayableGrovetender.GrovetenderPlugin.healWispPrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, HealingWisp.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                FireWisp();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
