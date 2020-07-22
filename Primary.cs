using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.GravekeeperMonster.Weapon;

namespace EntityStates.Grovetender
{
    public class DiscipleSwarm : BaseState
    {
        public static float damageCoefficient = 0.5f;
        public static float projectileForce = 5f;

        private float stopwatch;
        private float missileStopwatch;
        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.missileStopwatch -= GravekeeperBarrage.missileSpawnDelay;

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                this.childLocator = modelTransform.GetComponent<ChildLocator>();
                if (this.childLocator)
                {
                    this.childLocator.FindChild("JarEffectLoop").gameObject.SetActive(true);
                }
            }

            base.PlayAnimation("Jar, Override", "BeginGravekeeperBarrage");

            EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.jarOpenEffectPrefab, base.gameObject, GravekeeperBarrage.jarEffectChildLocatorString, false);
            Util.PlaySound(GravekeeperBarrage.jarOpenSoundString, base.gameObject);

            base.characterBody.SetAimTimer(2f);
        }

        private void FireBlob(Ray projectileRay, float bonusPitch, float bonusYaw)
        {
            projectileRay.direction = Util.ApplySpread(projectileRay.direction, 0f, GravekeeperBarrage.maxSpread, 1f, 1f, bonusYaw, bonusPitch);
            EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.muzzleflashPrefab, base.gameObject, GravekeeperBarrage.muzzleString, false);

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(GravekeeperBarrage.projectilePrefab, projectileRay.origin, Util.QuaternionSafeLookRotation(projectileRay.direction), base.gameObject, this.damageStat * DiscipleSwarm.damageCoefficient, DiscipleSwarm.projectileForce, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }

        public override void OnExit()
        {
            base.PlayCrossfade("Jar, Override", "EndGravekeeperBarrage", 0.06f);

            EffectManager.SimpleMuzzleFlash(GravekeeperBarrage.jarCloseEffectPrefab, base.gameObject, GravekeeperBarrage.jarEffectChildLocatorString, false);
            Util.PlaySound(GravekeeperBarrage.jarCloseSoundString, base.gameObject);

            if (this.childLocator)
            {
                this.childLocator.FindChild("JarEffectLoop").gameObject.SetActive(false);
            }

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.stopwatch += Time.fixedDeltaTime;
            this.missileStopwatch += Time.fixedDeltaTime;

            if (this.missileStopwatch >= 1f / GravekeeperBarrage.missileSpawnFrequency)
            {
                this.missileStopwatch -= 1f / GravekeeperBarrage.missileSpawnFrequency;
                Transform transform = this.childLocator.FindChild(GravekeeperBarrage.muzzleString);

                if (transform)
                {
                    Ray projectileRay = default(Ray);
                    projectileRay.origin = transform.position;
                    projectileRay.direction = base.GetAimRay().direction;
                    float maxDistance = 1000f;
                    RaycastHit raycastHit;
                    if (Physics.Raycast(base.GetAimRay(), out raycastHit, maxDistance, LayerIndex.world.mask))
                    {
                        projectileRay.direction = raycastHit.point - transform.position;
                    }
                    this.FireBlob(projectileRay, 0f, 0f);
                }
            }

            if (!this.inputBank.skill1.down && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
