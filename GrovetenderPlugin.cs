using System;
using System.Reflection;
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

namespace PlayableGrovetender
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin(MODUID, "Playable Grovetender", "0.0.1")]
    [R2APISubmoduleDependency(nameof(PrefabAPI), nameof(SurvivorAPI), nameof(LoadoutAPI), nameof(LanguageAPI), nameof(BuffAPI), nameof(EffectAPI))]

    public class GrovetenderPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.rob.PlayableGrovetender";

        internal static GrovetenderPlugin instance;

        public GameObject myCharacter;
        public GameObject characterDisplay;
        public GameObject doppelganger;

        private static readonly Color CHAR_COLOR = new Color(0.64f, 0.31f, 0.22f);

        private static ConfigEntry<bool> originalSize;
        /*private static ConfigEntry<float> baseHealth;
        private static ConfigEntry<float> healthGrowth;
        private static ConfigEntry<float> baseArmor;
        private static ConfigEntry<float> baseDamage;
        private static ConfigEntry<float> damageGrowth;
        private static ConfigEntry<float> baseRegen;
        private static ConfigEntry<float> regenGrowth;*/


        public void Awake()
        {
            instance = this;

            ReadConfig();
            Assets.PopulateAssets();
            RegisterStates();
            RegisterCharacter();
            CreateMaster();
        }

        void ReadConfig()
        {
            originalSize = base.Config.Bind<bool>(new ConfigDefinition("10 - Misc", "Original Size"), false, new ConfigDescription("Keeps the original size of the Grovetenders", null, Array.Empty<object>()));
            /*baseHealth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Health"), 200f, new ConfigDescription("Base health", null, Array.Empty<object>()));
            healthGrowth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Health growth"), 48f, new ConfigDescription("Health per level", null, Array.Empty<object>()));
            baseArmor = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Armor"), 20f, new ConfigDescription("Base armor", null, Array.Empty<object>()));
            baseDamage = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Damage"), 12f, new ConfigDescription("Base damage", null, Array.Empty<object>()));
            damageGrowth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Damage growth"), 2.4f, new ConfigDescription("Damage per level", null, Array.Empty<object>()));
            baseRegen = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Regen"), 2.5f, new ConfigDescription("Base HP regen", null, Array.Empty<object>()));
            regenGrowth = base.Config.Bind<float>(new ConfigDefinition("01 - General Settings", "Regen growth"), 0.5f, new ConfigDescription("HP regen per level", null, Array.Empty<object>()));*/
        }

        void RegisterCharacter()
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
            myCharacter.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));

            myCharacter.GetComponentInChildren<Interactor>().maxInteractionDistance = 5f;

            //charBody.portraitIcon = 


            bool flag = originalSize.Value;

            if (!flag)
            {
                //resize the grovetender

                myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.localScale = Vector3.one * 0.3f;
                myCharacter.GetComponent<ModelLocator>().modelBaseTransform.gameObject.transform.Translate(new Vector3(0f, 5.6f, 0f));
                charBody.aimOriginTransform.Translate(new Vector3(0f, -2f, 0f));

                charBody.baseJumpPower = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().baseJumpPower;
                charBody.baseMoveSpeed = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().baseMoveSpeed;
                charBody.levelMoveSpeed = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().levelMoveSpeed;
                charBody.sprintingSpeedMultiplier = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponent<CharacterBody>().sprintingSpeedMultiplier;

                myCharacter.GetComponentInChildren<CharacterMotor>().mass = 100;

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




            characterDisplay.transform.localScale = Vector3.one * 0.1f;
            characterDisplay.AddComponent<NetworkIdentity>();

            //networking

            if (myCharacter) PrefabAPI.RegisterNetworkPrefab(myCharacter);
            if (characterDisplay) PrefabAPI.RegisterNetworkPrefab(characterDisplay);
            if (doppelganger) PrefabAPI.RegisterNetworkPrefab(doppelganger);

            /*ProjectileCatalog.getAdditionalEntries += list =>
            {
                list.Add();
            };*/



            string desc = "The Grovetender something something<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > 1." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > 2." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > 3." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > 4.</color>" + Environment.NewLine;

            LanguageAPI.Add("GROVETENDER_NAME", "Grovetender");
            LanguageAPI.Add("GROVETENDER_DESCRIPTION", desc);
            LanguageAPI.Add("GROVETENDER_SUBTITLE", "Wisp Cultivator");


            charBody.name = "GROVETENDER_NAME";
            charBody.baseNameToken = "GROVETENDER_NAME";
            charBody.subtitleNameToken = "GROVETENDER_SUBTITLE";
            charBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/Crosshair/SimpleDotCrosshair");

            charBody.baseMaxHealth = 160f;
            charBody.levelMaxHealth = 48f;
            charBody.baseRegen = 0.5f;
            charBody.levelRegen = 0.2f;
            charBody.baseDamage = 15f;
            charBody.levelDamage = 3f;
            charBody.baseArmor = 0f;
            charBody.baseCrit = 1f;

            //use this instead when config is ready
            /*charBody.baseMaxHealth = baseHealth.Value;
            charBody.levelMaxHealth = healthGrowth.Value;
            charBody.baseRegen = baseRegen.Value;
            charBody.levelRegen = regenGrowth.Value;
            charBody.baseDamage = baseDamage.Value;
            charBody.levelDamage = damageGrowth.Value;
            charBody.baseArmor = baseArmor.Value;
            charBody.baseCrit = 1;*/

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

        void RegisterStates()
        {
            LoadoutAPI.AddSkill(typeof(EntityStates.Grovetender.DiscipleSwarm));
        }

        void SkillSetup()
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

        void PassiveSetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            LanguageAPI.Add("GROVETENDER_PASSIVE_NAME", "A passive");
            LanguageAPI.Add("GROVETENDER_PASSIVE_DESCRIPTION", "Sample Text.");

            component.passiveSkill.enabled = false;
            component.passiveSkill.skillNameToken = "GROVETENDER_PASSIVE_NAME";
            component.passiveSkill.skillDescriptionToken = "GROVETENDER_PASSIVE_DESCRIPTION";
            //component.passiveSkill.icon = ;
        }

        void PrimarySetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "Fire small <style=cIsUtility>homing Wisps</style> that explode upon contact for <style=cIsDamage>" + EntityStates.Grovetender.DiscipleSwarm.damageCoefficient * 100f + "% damage</style>.";

            LanguageAPI.Add("GROVETENDER_PRIMARY_WISP_NAME", "Disciple Swarm");
            LanguageAPI.Add("GROVETENDER_PRIMARY_WISP_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.GravekeeperMonster.Weapon.GravekeeperBarrage));
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
            //mySkillDef.icon = ;
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

        void SecondarySetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "Fire a <style=cIsUtility>piercing Wisp</style> that deals <style=cIsDamage>" + 0.5f * 100f + "% damage</style> and <style=cIsHealing>heals 5% max health</style> for each enemy hit.";
            LanguageAPI.Add("GROVETENDER_SECONDARY_HEAL_NAME", "Rejuvenation");
            LanguageAPI.Add("GROVETENDER_SECONDARY_HEAL_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.TitanMonster.FireMegaLaser));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 6f;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.isBullets = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.noSprint = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.shootDelay = 0f;
            mySkillDef.stockToConsume = 1;
            //mySkillDef.icon = ;
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

        void UtilitySetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "<style=cIsUtility>Dash</style> a short distance.";

            LanguageAPI.Add("GROVETENDER_UTILITY_DODGE_NAME", "Sidestep");
            LanguageAPI.Add("GROVETENDER_UTILITY_DODGE_DESCRIPTION", desc);

            SkillDef tempDef = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<SkillLocator>().utility.skillFamily.variants[0].skillDef;
            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Commando.CombatDodge));
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
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        void SpecialSetup()
        {
            SkillLocator component = myCharacter.GetComponent<SkillLocator>();

            string desc = "Fire a burst of <style=cIsUtility>chains</style> that deal <style=cIsDamage>" + EntityStates.GravekeeperBoss.FireHook.projectileDamageCoefficient * 100f + "% damage</style> and  <style=cIsUtility>pull</style> enemies hit. <style=cIsUtility>Hold to lower the attack's spread</style>.";

            LanguageAPI.Add("GROVETENDER_SPECIAL_CHAINS_NAME", "Scorched Shotgun");
            LanguageAPI.Add("GROVETENDER_SPECIAL_CHAINS_DESCRIPTION", desc);

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.GravekeeperBoss.PrepHook));
            mySkillDef.activationStateMachineName = "Body";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 1;
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
            //mySkillDef.icon = ;
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


        private void CreateMaster()
        {
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

                Util.PlayScaledSound(EntityStates.GravekeeperBoss.SpawnState.spawnSoundString, base.gameObject, 2.5f);

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
        //will use this all later
        /*public static AssetBundle MainAssetBundle = null;

        public static Sprite iconP;
        public static Sprite icon1;
        public static Sprite icon2;
        public static Sprite icon3;
        public static Sprite icon4;*/

        public static void PopulateAssets()
        {
            /*if (MainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PlayableGrovetender.grovetender"))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }*/

            /*using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PlayableSephiroth.SephirothBank.bnk"))
            {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                SoundBanks.Add(bytes);
            }*/

            // gather assets

            /*iconP = MainAssetBundle.LoadAsset<Sprite>("PassiveIcon");
            icon1 = MainAssetBundle.LoadAsset<Sprite>("PrimaryIcon");
            icon2 = MainAssetBundle.LoadAsset<Sprite>("SecondaryIcon");
            icon3 = MainAssetBundle.LoadAsset<Sprite>("UtilityIcon");
            icon4 = MainAssetBundle.LoadAsset<Sprite>("SpecialIcon");*/
        }
    }
}