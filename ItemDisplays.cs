using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PlayableGrovetender
{
    public static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        public static void RegisterDisplays()
        {
            GameObject bodyPrefab = GrovetenderPlugin.myCharacter;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            PopulateDisplays();

            //apply displays here

            ItemDisplayRuleSet itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();

            List<ItemDisplayRuleSet.NamedRuleGroup> list = new List<ItemDisplayRuleSet.NamedRuleGroup>();
            List<ItemDisplayRuleSet.NamedRuleGroup> list2 = new List<ItemDisplayRuleSet.NamedRuleGroup>();

            ItemDisplayRuleSet original = Resources.Load<GameObject>("Prefabs/CharacterBodies/GravekeeperBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet;
            ItemDisplayRuleSet loader = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet;

            //add item displays here

            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Jetpack",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
                            childName = "Head",
                            localPos = new Vector3(0, 1, -2),
                            localAngles = new Vector3(45, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "GoldGat",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(0, -9, 0),
                            localAngles = new Vector3(180, 270, 180),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AffixRed",
                displayRuleGroup = original.FindEquipmentDisplayRuleGroup("AffixRed")
            });
            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AffixBlue",
                displayRuleGroup = original.FindEquipmentDisplayRuleGroup("AffixBlue")
            });
            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AffixWhite",
                displayRuleGroup = original.FindEquipmentDisplayRuleGroup("AffixWhite")
            });
            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AffixPoison",
                displayRuleGroup = original.FindEquipmentDisplayRuleGroup("AffixPoison")
            });
            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AffixHaunted",
                displayRuleGroup = original.FindEquipmentDisplayRuleGroup("AffixHaunted")
            });
            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "IncreaseHealing",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                            childName = "Head",
                            localPos = new Vector3(1.5f, 2, 0.017f),
                            localAngles = new Vector3(358.0388f, 93.19246f, 1.436809f),
                            localScale = new Vector3(3, 3, 3),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
                            childName = "Head",
                            localPos = new Vector3(-1.5f, 2, 0.017f),
                            localAngles = new Vector3(358.030f, 266.808f, 1.436998f),
                            localScale = new Vector3(-3, 3, 3),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "NovaOnHeal",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                            childName = "Head",
                            localPos = new Vector3(1.25f, -1, 0),
                            localAngles = new Vector3(3.536957f, 0.01489126f, 14.99564f),
                            localScale = new Vector3(5, 5, 5),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                            childName = "Head",
                            localPos = new Vector3(-1.25f, -1, 0),
                            localAngles = new Vector3(3.536963f, 0.0149023f, 344.9955f),
                            localScale = new Vector3(-5, 5, 5),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "KillEliteFrenzy",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                            childName = "Head",
                            localPos = new Vector3(0, 1.5f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Clover",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
                            childName = "Head",
                            localPos = new Vector3(-1.5f, 3, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(4, 4, 4),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "CritGlasses",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
                            childName = "Mask",
                            localPos = new Vector3(0, 0, 0.5f),
                            localAngles = new Vector3(-90, 180, 0),
                            localScale = new Vector3(4, 4, 4),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "GhostOnKill",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
                            childName = "Head",
                            localPos = new Vector3(0, 1, 2),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(4, 4, 4),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AttackSpeedOnCrit",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                            childName = "Head",
                            localPos = new Vector3(0, 2, 1),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(4, 4, 4),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "GoldOnHit",
                displayRuleGroup = loader.FindItemDisplayRuleGroup("GoldOnHit")
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ShieldOnly",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                            childName = "Head",
                            localPos = new Vector3(0.75f, -0.2f, 0.5f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(4, 4, 4),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
                            childName = "Head",
                            localPos = new Vector3(-0.75f, -0.2f, 0.5f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(-4, 4, 4),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "HeadHunter",
                displayRuleGroup = loader.FindItemDisplayRuleGroup("HeadHunter")
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "JumpBoost",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
                            childName = "Head",
                            localPos = new Vector3(0, -0.5f, -1),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(5, 5, 5),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Bandolier",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(7, 8, 10),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "DeathMark",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.75f, 0.75f, 0.75f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "WarCryOnMultiKill",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
                            childName = "Head",
                            localPos = new Vector3(0, 1, 2),
                            localAngles = new Vector3(-90, 180, 0),
                            localScale = new Vector3(5, 5, 5),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Mushroom",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
                            childName = "Mask",
                            localPos = new Vector3(0.5f, -0.5f, 2.4f),
                            localAngles = new Vector3(90, 25, 0),
                            localScale = new Vector3(0.5f, 0.5f, 0.5f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BarrierOnOverHeal",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(3.5f, -2, 0),
                            localAngles = new Vector3(90, -90, 0),
                            localScale = new Vector3(3, 3, 3),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Behemoth",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(0, -2, -5),
                            localAngles = new Vector3(180, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "NearbyDamageBonus",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
                            childName = "HandL",
                            localPos = new Vector3(0, 0, -2.75f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.7f, 0.7f, 0.7f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "FireRing",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
                            childName = "HandL",
                            localPos = new Vector3(0.25f, 0, 0),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(4, 4, 4),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "IceRing",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
                            childName = "HandR",
                            localPos = new Vector3(-0.25f, 0, 0),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(4, 4, 4),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ArmorPlate",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                            childName = "Head",
                            localPos = new Vector3(0, 1.5f, -2),
                            localAngles = new Vector3(-25, 0, 180),
                            localScale = new Vector3(3, 3, 3),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Bear",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
                            childName = "Head",
                            localPos = new Vector3(-1.9f, 0.5f, -1.5f),
                            localAngles = new Vector3(0, -90, 0),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Medkit",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
                            childName = "Head",
                            localPos = new Vector3(2, 0, 0),
                            localAngles = new Vector3(-90, 90, 0),
                            localScale = new Vector3(3, 3, 3),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Dagger",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
                            childName = "Head",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(6, 6, 6),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ChainLightning",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(-3.2f, -2.5f, 1),
                            localAngles = new Vector3(0, -90, 180),
                            localScale = new Vector3(300, 300, 300),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Syringe",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
                            childName = "Head",
                            localPos = new Vector3(0, 2, 0),
                            localAngles = new Vector3(300, 0, 0),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ArmorReductionOnHit",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(0, -4, -3),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "FallBoots",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                            childName = "FootL",
                            localPos = new Vector3(0, -0.05f, 0),
                            localAngles = new Vector3(45, 0, 0),
                            localScale = new Vector3(0.25f, 0.25f, 0.25f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
                            childName = "FootR",
                            localPos = new Vector3(0, -0.05f, 0),
                            localAngles = new Vector3(70, 0, 0),
                            localScale = new Vector3(0.25f, 0.25f, 0.25f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BounceNearby",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
                            childName = "HandR",
                            localPos = new Vector3(0, 0, -0.25f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "SprintWisp",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
                            childName = "Mask",
                            localPos = new Vector3(0, 0.25f, 1.5f),
                            localAngles = new Vector3(-110, 0, 180),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Hoof",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
                            childName = "FootL",
                            localPos = new Vector3(0, 0.25f, 0.75f),
                            localAngles = new Vector3(20, 0, 180),
                            localScale = new Vector3(0.75f, 0.5f, 0.25f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Incubator",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAncestralIncubator"),
                            childName = "MuzzleJar",
                            localPos = new Vector3(0, -4.5f, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1.25f, 1, 1.25f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ExplodeOnDeath",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
                            childName = "Head",
                            localPos = new Vector3(-8.5f, 2.75f, 0),
                            localAngles = new Vector3(0, 0, 45),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ExecuteLowHealthElite",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
                            childName = "Head",
                            localPos = new Vector3(8.5f, 2.75f, 0),
                            localAngles = new Vector3(0, 0, -45),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });


            //something something

            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Meteor",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
                            childName = "Root",
                            localPos = new Vector3(0, 10.5f, -12),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Saw",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySawmerang"),
                            childName = "Root",
                            localPos = new Vector3(0, 10.5f, -12),
                            localAngles = new Vector3(-90, 0, 0),
                            localScale = new Vector3(0.4f, 0.4f, 0.4f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Blackhole",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
                            childName = "Root",
                            localPos = new Vector3(0, 16, -6),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Icicle",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
                            childName = "Root",
                            localPos = new Vector3(-2, 13, -4),
                            localAngles = new Vector3(90, 0, 0),
                            localScale = new Vector3(2, 2, 2),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Talisman",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
                            childName = "Root",
                            localPos = new Vector3(4, 12, -6),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            /*list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "LaserTurbine",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
                            childName = "Root",
                            localPos = new Vector3(0, 0, 0),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });*/

            list.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "FocusConvergence",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
                            childName = "Root",
                            localPos = new Vector3(0, 16, -12),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.25f, 0.25f, 0.25f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });


            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            ItemDisplayRuleSet.NamedRuleGroup[] value = list.ToArray();
            ItemDisplayRuleSet.NamedRuleGroup[] value2 = list2.ToArray();
            typeof(ItemDisplayRuleSet).GetField("namedItemRuleGroups", bindingAttr).SetValue(itemDisplayRuleSet, value);
            typeof(ItemDisplayRuleSet).GetField("namedEquipmentRuleGroups", bindingAttr).SetValue(itemDisplayRuleSet, value2);

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        private static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }
            return null;
        }

        private static void PopulateDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            ItemDisplayRuleSet.NamedRuleGroup[] array = typeof(ItemDisplayRuleSet).GetField("namedItemRuleGroups", bindingAttr).GetValue(itemDisplayRuleSet) as ItemDisplayRuleSet.NamedRuleGroup[];
            ItemDisplayRuleSet.NamedRuleGroup[] array2 = typeof(ItemDisplayRuleSet).GetField("namedEquipmentRuleGroups", bindingAttr).GetValue(itemDisplayRuleSet) as ItemDisplayRuleSet.NamedRuleGroup[];
            ItemDisplayRuleSet.NamedRuleGroup[] array3 = array;
            for (int i = 0; i < array3.Length; i++)
            {
                ItemDisplayRule[] rules = array3[i].displayRuleGroup.rules;
                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (!(followerPrefab == null))
                    {
                        string name = followerPrefab.name;
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
            array3 = array2;
            for (int i = 0; i < array3.Length; i++)
            {
                ItemDisplayRule[] rules = array3[i].displayRuleGroup.rules;
                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab2 = rules[j].followerPrefab;
                    if (!(followerPrefab2 == null))
                    {
                        string name2 = followerPrefab2.name;
                        string key2 = (name2 != null) ? name2.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key2))
                        {
                            itemDisplayPrefabs[key2] = followerPrefab2;
                        }
                    }
                }
            }
        }
    }
}
