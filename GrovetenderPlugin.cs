using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using KinematicCharacterController;
using RoR2.Projectile;
using EntityStates.Grovetender;
using RoR2.UI;

namespace PlayableGrovetender
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin(MODUID, "Playable Grovetender", "0.0.3")]
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(SurvivorAPI), nameof(LoadoutAPI), nameof(LanguageAPI), nameof(BuffAPI), nameof(EffectAPI))]

    public class GrovetenderPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.rob.PlayableGrovetender";

        internal static GrovetenderPlugin instance;

        public static GameObject myCharacter;
        public static GameObject characterDisplay;
        public static GameObject doppelganger;

        public static GameObject wispPrefab;
        public static GameObject healWispPrefab;
        public static GameObject healWispGhost;

        public static GameObject grovetenderCrosshair;

        private static readonly Color CHAR_COLOR = new Color(0.09f, 0.03f, 0.03f);
        private static readonly Color HEAL_COLOR = new Color(0.27f, 1, 0.32f);

        private static ConfigEntry<bool> originalSize;
        private static ConfigEntry<float> baseHealth;
        private static ConfigEntry<float> healthGrowth;
        private static ConfigEntry<float> baseArmor;
        private static ConfigEntry<float> baseDamage;
        private static ConfigEntry<float> damageGrowth;
        private static ConfigEntry<float> baseRegen;
        private static ConfigEntry<float> regenGrowth;


        public void Awake()
        {
            instance = this;

            ReadConfig();
            Assets.PopulateAssets();
            RegisterStates();
            RegisterCharacter();
            ItemDisplays.RegisterDisplays();
            Skins.RegisterSkins();
            RegisterProjectiles();
            CreateMaster();
        }

        private void ReadConfig()
        {
            originalSize = base.Config.Bind<bool>(new ConfigDefinition("10 - Misc", "Original Size"), false, new ConfigDescription("Keeps the original size of the Grovetenders", null, Array.Empty<object>()));
            baseHealth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Health"), 200f, new ConfigDescription("Base health", null, Array.Empty<object>()));
            healthGrowth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Health growth"), 48f, new ConfigDescription("Health per level", null, Array.Empty<object>()));
            baseArmor = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Armor"), 20f, new ConfigDescription("Base armor", null, Array.Empty<object>()));
            baseDamage = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Damage"), 12f, new ConfigDescription("Base damage", null, Array.Empty<object>()));
            damageGrowth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Damage growth"), 2.4f, new ConfigDescription("Damage per level", null, Array.Empty<object>()));
            baseRegen = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Regen"), 2.5f, new ConfigDescription("Base HP regen", null, Array.Empty<object>()));
            regenGrowth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Regen growth"), 0.5f, new ConfigDescription("HP regen per level", null, Array.Empty<object>()));
        }

        private void RegisterCharacter()
        {
            //create a clone of the grovetender prefab
            myCharacter = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/GravekeeperBody"), "Prefabs/CharacterBodies/GrovetenderBody", true);
            //create a display prefab
            characterDisplay = PrefabAPI.InstantiateClone(myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "GrovetenderDisplay", true);

            //add custom menu animation script
            characterDisplay.AddComponent<MenuAnim>();

            //lets the grovetender be frozen
            var component1 = myCharacter.AddComponent<SetStateOnHurt>();
            component1.canBeHitStunned = false;
            component1.canBeStunned = true;
            component1.canBeFrozen = true;

            CharacterBody charBody = myCharacter.GetComponent<CharacterBody>();
            charBody.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;

            //swap to generic mainstate to fix clunky controls
            myCharacter.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));

            myCharacter.GetComponentInChildren<Interactor>().maxInteractionDistance = 5f;

            charBody.portraitIcon = Assets.charPortrait.texture;


            bool flag = originalSize.Value;

            if (!flag)
            {
                //resize the grovetender

                myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.localScale = Vector3.one * 0.3f;
                myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.Translate(new Vector3(0f, 5.6f, 0f));
                charBody.aimOriginTransform.Translate(new Vector3(0f, -2.5f, 0f));

                charBody.baseJumpPower = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().baseJumpPower;
                charBody.baseMoveSpeed = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().baseMoveSpeed;
                charBody.levelMoveSpeed = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().levelMoveSpeed;
                charBody.sprintingSpeedMultiplier = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().sprintingSpeedMultiplier;

                myCharacter.GetComponentInChildren<CharacterMotor>().mass = 300;

                myCharacter.GetComponent<CameraTargetParams>().cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody").GetComponent<CameraTargetParams>().cameraParams;

                foreach (KinematicCharacterMotor kinematicCharacterMotor in myCharacter.GetComponentsInChildren<KinematicCharacterMotor>())
                {
                    kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 0.25f, kinematicCharacterMotor.Capsule.height * 0.25f, 0f);
                }
            }

            //crosshair stuff
            charBody.SetSpreadBloom(0, false);
            charBody.spreadBloomCurve = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CharacterBody>().spreadBloomCurve;
            charBody.spreadBloomDecayTime = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CharacterBody>().spreadBloomDecayTime;

            charBody.hullClassification = HullClassification.Human;




            characterDisplay.transform.localScale = Vector3.one * 0.15f;
            characterDisplay.AddComponent<NetworkIdentity>();

            //create the custom crosshair
            grovetenderCrosshair = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Crosshair/LoaderCrosshair"), "GrovetenderCrosshair", true);
            grovetenderCrosshair.AddComponent<NetworkIdentity>();
            Destroy(grovetenderCrosshair.GetComponent<LoaderHookCrosshairController>());
            //Destroy(grovetenderCrosshair.transform.GetChild(1));
            //Destroy(grovetenderCrosshair.transform.GetChild(0));

            //networking

            if (myCharacter) PrefabAPI.RegisterNetworkPrefab(myCharacter);
            if (characterDisplay) PrefabAPI.RegisterNetworkPrefab(characterDisplay);
            if (doppelganger) PrefabAPI.RegisterNetworkPrefab(doppelganger);
            if (grovetenderCrosshair) PrefabAPI.RegisterNetworkPrefab(grovetenderCrosshair);



            string desc = "The Grovetender is a slow, tanky survivor who makes use of Wisps to heal and deal damage.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Disciple Swarm's low damage is made up for with its consistent item procs." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Disciple Swarm can be held during your other skills." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Piercing Wisp has the potential for massive heals when lined up right." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Scorched Shotgun can be used to pull flying enemies into terrain for lethal impact damage.</color>" + Environment.NewLine;

            LanguageAPI.Add("GROVETENDER_NAME", "Grovetender");
            LanguageAPI.Add("GROVETENDER_DESCRIPTION", desc);
            LanguageAPI.Add("GROVETENDER_SUBTITLE", "Wisp Cultivator");


            charBody.name = "GROVETENDER_NAME";
            charBody.baseNameToken = "GROVETENDER_NAME";
            charBody.subtitleNameToken = "GROVETENDER_SUBTITLE";
            charBody.crosshairPrefab = grovetenderCrosshair;

            charBody.baseMaxHealth = baseHealth.Value;
            charBody.levelMaxHealth = healthGrowth.Value;
            charBody.baseRegen = baseRegen.Value;
            charBody.levelRegen = regenGrowth.Value;
            charBody.baseDamage = baseDamage.Value;
            charBody.levelDamage = damageGrowth.Value;
            charBody.baseArmor = baseArmor.Value;
            charBody.levelArmor = 1;
            charBody.baseCrit = 1;

            charBody.preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody").GetComponent<CharacterBody>().preferredPodPrefab;


            //create a survivordef for our grovetender
            SurvivorDef survivorDef = new SurvivorDef
            {
                name = "GROVETENDER_NAME",
                unlockableName = "",
                descriptionToken = "GROVETENDER_DESCRIPTION",
                primaryColor = CHAR_COLOR,
                bodyPrefab = myCharacter,
                displayPrefab = characterDisplay
            };


            SurvivorAPI.AddSurvivor(survivorDef);


            SkillSetup();


            //add it to the body catalog
            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(myCharacter);
            };
        }

        private void RegisterStates()
        {
            LoadoutAPI.AddSkill(typeof(DiscipleSwarm));
            LoadoutAPI.AddSkill(typeof(HealingWisp));
            LoadoutAPI.AddSkill(typeof(PrepShotgun));
            LoadoutAPI.AddSkill(typeof(FireShotgun));
        }

        private void SkillSetup()
        {
            foreach (GenericSkill obj in myCharacter.GetComponentsInChildren<GenericSkill>())
            {
                BaseUnityPlugin.DestroyImmediate(obj);
            }

            PassiveSetup();
            PrimarySetup();
            SecondarySetup();
            UtilitySetup();
            SpecialSetup();
        }

        private void PassiveSetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            LanguageAPI.Add("GROVETENDER_PASSIVE_NAME", "Rejuvenation");
            LanguageAPI.Add("GROVETENDER_PASSIVE_DESCRIPTION", "<style=cIsHealing>Heal</style> for a portion of all <style=cIsDamage>damage dealt</style> with <style=cIsUtility>Wisps</style>.");

            component.passiveSkill.enabled = true;
            component.passiveSkill.skillNameToken = "GROVETENDER_PASSIVE_NAME";
            component.passiveSkill.skillDescriptionToken = "GROVETENDER_PASSIVE_DESCRIPTION";
            component.passiveSkill.icon = Assets.iconP;
        }

        private void PrimarySetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "Fire small <style=cIsUtility>tracking Wisps</style> that explode upon contact for <style=cIsDamage>" + DiscipleSwarm.damageCoefficient * 100f + "% damage</style>.";

            LanguageAPI.Add("GROVETENDER_PRIMARY_WISP_NAME", "Disciple Swarm");
            LanguageAPI.Add("GROVETENDER_PRIMARY_WISP_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(DiscipleSwarm));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 0f;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon1;
            mySkillDef.skillDescriptionToken = "GROVETENDER_PRIMARY_WISP_DESCRIPTION";
            mySkillDef.skillName = "GROVETENDER_PRIMARY_WISP_NAME";
            mySkillDef.skillNameToken = "GROVETENDER_PRIMARY_WISP_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.primary = myCharacter.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.primary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.primary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private void SecondarySetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "Fire a <style=cIsUtility>piercing Wisp</style> that deals <style=cIsDamage>" + HealingWisp.damageCoefficient * 100f + "% damage</style>.";

            LanguageAPI.Add("GROVETENDER_SECONDARY_HEAL_NAME", "Piercing Wisp");
            LanguageAPI.Add("GROVETENDER_SECONDARY_HEAL_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(HealingWisp));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 5f;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = false;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon2;
            mySkillDef.skillDescriptionToken = "GROVETENDER_SECONDARY_HEAL_DESCRIPTION";
            mySkillDef.skillName = "GROVETENDER_SECONDARY_HEAL_NAME";
            mySkillDef.skillNameToken = "GROVETENDER_SECONDARY_HEAL_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.secondary = myCharacter.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.secondary.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.secondary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private void UtilitySetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "<style=cIsUtility>Leap</style> a short distance.";

            LanguageAPI.Add("GROVETENDER_UTILITY_DODGE_NAME", "Sidestep");
            LanguageAPI.Add("GROVETENDER_UTILITY_DODGE_DESCRIPTION", desc);

            SkillDef tempDef = Resources.Load<GameObject>("Prefabs/CharacterBodies/CrocoBody").GetComponentInChildren<SkillLocator>().utility.skillFamily.variants[1].skillDef;
            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Croco.ChainableLeap));
            mySkillDef.activationStateMachineName = "Body";
            mySkillDef.baseRechargeInterval = 8;
            mySkillDef.baseMaxStock = 1;
            mySkillDef.beginSkillCooldownOnSkillEnd = tempDef.beginSkillCooldownOnSkillEnd;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = tempDef.fullRestockOnAssign;
            mySkillDef.interruptPriority = tempDef.interruptPriority;
            mySkillDef.isBullets = tempDef.isBullets;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = tempDef.mustKeyPress;
            mySkillDef.noSprint = false;
            mySkillDef.rechargeStock = tempDef.rechargeStock;
            mySkillDef.requiredStock = tempDef.requiredStock;
            mySkillDef.shootDelay = tempDef.shootDelay;
            mySkillDef.stockToConsume = tempDef.stockToConsume;
            mySkillDef.icon = tempDef.icon;
            mySkillDef.skillDescriptionToken = "GROVETENDER_UTILITY_DODGE_DESCRIPTION";
            mySkillDef.skillName = "GROVETENDER_UTILITY_DODGE_NAME";
            mySkillDef.skillNameToken = "GROVETENDER_UTILITY_DODGE_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.utility = myCharacter.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.utility.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.utility.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = tempDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private void SpecialSetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "Fire a burst of <style=cIsUtility>chains</style> that deal <style=cIsDamage>8x" + FireShotgun.projectileDamageCoefficient * 100f + "% damage</style> and <style=cIsUtility>pull</style> enemies hit.";

            LanguageAPI.Add("GROVETENDER_SPECIAL_CHAINS_NAME", "Scorched Shotgun");
            LanguageAPI.Add("GROVETENDER_SPECIAL_CHAINS_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(PrepShotgun));
            mySkillDef.activationStateMachineName = "Body";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 8;
            mySkillDef.beginSkillCooldownOnSkillEnd = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            mySkillDef.isBullets = true;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = true;
            mySkillDef.noSprint = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            mySkillDef.icon = Assets.icon4;
            mySkillDef.skillDescriptionToken = "GROVETENDER_SPECIAL_CHAINS_DESCRIPTION";
            mySkillDef.skillName = "GROVETENDER_SPECIAL_CHAINS_NAME";
            mySkillDef.skillNameToken = "GROVETENDER_SPECIAL_CHAINS_NAME";

            LoadoutAPI.AddSkillDef(mySkillDef);

            component.special = myCharacter.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            component.special.SetFieldValue("_skillFamily", newFamily);
            SkillFamily skillFamily = component.special.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private void RegisterProjectiles()
        {
            wispPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/GravekeeperTrackingFireball"), "TrackingWisp", true);

            wispPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>().fractionOfDamage = 0.4f;

            healWispPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang"), "HealingWisp", true);

            healWispPrefab.transform.localScale /= 6f;
            healWispPrefab.transform.GetChild(0).localScale *= 4f;

            Destroy(healWispPrefab.GetComponent<BoxCollider>());
            Destroy(healWispPrefab.GetComponent<ProjectileDotZone>());
            healWispPrefab.AddComponent<ProjectileHealOwnerOnDamageInflicted>().fractionOfDamage = 0.8f;

            var projectileComponent = healWispPrefab.GetComponent<ProjectileController>();
            projectileComponent.startSound = "";
            projectileComponent.procCoefficient = 1f;

            var hitboxComponent = healWispPrefab.GetComponent<ProjectileOverlapAttack>();
            hitboxComponent.damageCoefficient = 1f;
            hitboxComponent.impactEffect = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXLightning");

            var boomerangComponent = healWispPrefab.GetComponent<BoomerangProjectile>();
            boomerangComponent.canHitWorld = false;
            boomerangComponent.impactSpark = null;
            boomerangComponent.travelSpeed = 60;


            healWispGhost = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/ProjectileGhosts/GravekeeperTrackingFireballGhost"), "HealingWispGhost", true);
            healWispGhost.AddComponent<NetworkIdentity>();

            foreach (ParticleSystem i in healWispGhost.GetComponentsInChildren<ParticleSystem>())
            {
                if (i)
                {
                    var main = i.main;
                    main.startColor = HEAL_COLOR;
                }
            }

            foreach (Light i in healWispGhost.GetComponentsInChildren<Light>())
            {
                if (i) i.color = HEAL_COLOR;
            }


            projectileComponent.ghostPrefab = healWispGhost;



            ProjectileCatalog.getAdditionalEntries += list =>
            {
                list.Add(wispPrefab);
                list.Add(healWispPrefab);
            };
        }



        private void CreateMaster()
        {
            //create the doppelganger, uses commando ai bc i can't be bothered writing my own
            doppelganger = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "GrovetenderMonsterMaster", true);

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(doppelganger);
            };

            CharacterMaster component = doppelganger.GetComponent<CharacterMaster>();
            component.bodyPrefab = myCharacter;
        }

        public class MenuAnim : MonoBehaviour
        {
            //animates him in character select
            internal void OnEnable()
            {
                bool flag = base.gameObject.transform.parent.gameObject.name == "CharacterPad";
                if (flag)
                {
                    base.StartCoroutine(this.SpawnAnim());
                }
            }

            private IEnumerator SpawnAnim()
            {
                Animator animator = base.GetComponentInChildren<Animator>();
                Transform effectTransform = base.gameObject.transform;

                ChildLocator component = base.gameObject.GetComponentInChildren<ChildLocator>();

                if (component) effectTransform = component.FindChild("Root");

                GameObject.Instantiate<GameObject>(EntityStates.GravekeeperBoss.SpawnState.spawnEffectPrefab, effectTransform.position, Quaternion.identity);

                Util.PlayScaledSound(EntityStates.GravekeeperBoss.SpawnState.spawnSoundString, base.gameObject, 1.5f);

                PlayAnimation("Body", "Spawn", "Spawn.playbackRate", 3, animator);

                yield break;
            }

            private void PlayAnimation(string layerName, string animationStateName, string playbackRateParam, float duration, Animator animator)
            {
                int layerIndex = animator.GetLayerIndex(layerName);
                animator.SetFloat(playbackRateParam, 1f);
                animator.PlayInFixedTime(animationStateName, layerIndex, 0f);
                animator.Update(0f);
                float length = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
                animator.SetFloat(playbackRateParam, length / duration);
            }
        }
    }











    public static class Assets
    {
        public static Sprite charPortrait;

        public static Sprite iconP;
        public static Sprite icon1;
        public static Sprite icon2;
        //public static Sprite icon3;
        public static Sprite icon4;

        public static void PopulateAssets()
        {
            charPortrait = Assets.CreateSprite(Properties.Resources.GrovetenderBody);

            iconP = Assets.CreateSprite(Properties.Resources.PassiveIcon);
            icon1 = Assets.CreateSprite(Properties.Resources.SwarmIcon);
            icon2 = Assets.CreateSprite(Properties.Resources.HealIcon);
            icon4 = Assets.CreateSprite(Properties.Resources.ChainIcon);
        }

        static Sprite CreateSprite(Byte[] resourceBytes)
        {
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            Texture2D temp = new Texture2D(128, 128, TextureFormat.RGBAFloat, false);
            temp.LoadImage(resourceBytes, false);

            Sprite newSprite = Sprite.Create(temp, new Rect(0f, 0f, (float)temp.width, (float)temp.height), new Vector2(0.5f, 0.5f));

            return newSprite;
        }
    }
}