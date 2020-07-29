using RoR2;
using UnityEngine;
using EntityStates.GravekeeperBoss;
using UnityEngine.Networking;
using RoR2.Projectile;

namespace EntityStates.Grovetender
{
    public class PrepShotgun : BaseSkillState
    {
        public static float baseDuration = 2.5f;
        public static float maxSpread = 60;
        public static float minSpread = 2;

        private float duration;
        private float charge;
        private GameObject chargeInstance;
        private Animator modelAnimator;
        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            base.fixedAge = 0f;
            this.duration = PrepShotgun.baseDuration / this.attackSpeedStat;
            this.modelAnimator = base.GetModelAnimator();

            if (this.modelAnimator)
            {
                base.PlayCrossfade("Body", "PrepHook", "PrepHook.playbackRate", this.duration, 0.5f);
                this.modelAnimator.GetComponent<AimAnimator>().enabled = true;
            }

            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.inputBank.aimDirection;
            }

            Util.PlayScaledSound(PrepHook.attackString, base.gameObject, this.attackSpeedStat);

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                this.childLocator = modelTransform.GetComponent<ChildLocator>();
                if (this.childLocator)
                {
                    Transform transform = this.childLocator.FindChild(PrepHook.muzzleString);

                    if (transform && PrepHook.chargeEffectPrefab)
                    {
                        this.chargeInstance = UnityEngine.Object.Instantiate<GameObject>(PrepHook.chargeEffectPrefab, transform.position, transform.rotation);
                        this.chargeInstance.transform.parent = transform;
                        ScaleParticleSystemDuration component2 = this.chargeInstance.GetComponent<ScaleParticleSystemDuration>();
                        if (component2)
                        {
                            component2.newDuration = this.duration;
                        }
                    }
                }
            }
        }

        public override void OnExit()
        {
            if (this.chargeInstance)
            {
                EntityState.Destroy(this.chargeInstance);
            }

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.charge += Time.fixedDeltaTime;

            bool flag = base.fixedAge >= this.duration || !this.inputBank.skill4.down;

            if (flag && base.isAuthority)
            {
                if (this.charge >= this.duration) this.charge = this.duration;

                //uhh this doesn't work apparently
                //float spread = Mathf.Lerp(this.charge / this.duration, PrepShotgun.maxSpread, PrepShotgun.minSpread);
                float spread = maxSpread;

                this.outer.SetNextState(new FireShotgun
                {
                    projectileCount = 8,
                    spread = spread
                });
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }

    public class FireShotgun : BaseSkillState
    {
        public static float baseDuration = 0.5f;
        public static float projectileDamageCoefficient = 2.5f;
        public int projectileCount;
        public float spread;

        private float duration;
        private Animator modelAnimator;
        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            base.fixedAge = 0f;
            this.duration = FireShotgun.baseDuration / this.attackSpeedStat;
            this.modelAnimator = base.GetModelAnimator();

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                this.childLocator = modelTransform.GetComponent<ChildLocator>();
            }

            if (this.modelAnimator)
            {
                base.PlayCrossfade("Body", "FireHook", "FireHook.playbackRate", this.duration, 0.03f);
            }

            Util.PlayScaledSound(FireHook.soundString, base.gameObject, this.attackSpeedStat);
            EffectManager.SimpleMuzzleFlash(FireHook.muzzleflashEffectPrefab, base.gameObject, FireHook.muzzleString, false);

            Ray aimRay = base.GetAimRay();
            if (NetworkServer.active)
            {
                this.FireSingleHook(aimRay, 0f, 0f);
                for (int i = 0; i < this.projectileCount; i++)
                {
                    float bonusPitch = UnityEngine.Random.Range(-this.spread, this.spread) / 2f;
                    float bonusYaw = UnityEngine.Random.Range(-this.spread, this.spread) / 2f;
                    this.FireSingleHook(aimRay, bonusPitch, bonusYaw);
                }
            }
        }

        private void FireSingleHook(Ray aimRay, float bonusPitch, float bonusYaw)
        {
            Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, bonusPitch);
            ProjectileManager.instance.FireProjectile(FireHook.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireShotgun.projectileDamageCoefficient, FireHook.projectileForce, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
