using System;
using UnityEngine;
using R2API;
using RoR2;
using R2API.Utils;

namespace PlayableGrovetender
{
    public static class Skins
    {
        public static void RegisterSkins()
        {
            GameObject bodyPrefab = GrovetenderPlugin.myCharacter;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            if (model.GetComponent<ModelSkinController>()) GrovetenderPlugin.Destroy(model.GetComponent<ModelSkinController>());

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");

            LanguageAPI.Add("GROVETENDERBODY_DEFAULT_SKIN_NAME", "Default");

            LoadoutAPI.SkinDefInfo skinDefInfo = default(LoadoutAPI.SkinDefInfo);
            skinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            skinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];
            skinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];

            skinDefInfo.GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();

            skinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(0.09f, 0.03f, 0.03f), new Color(0.039f, 0.039f, 0.078f), new Color(0.61f, 0.59f, 0.5f), new Color(0.9f, 0.9f, 0.9f));
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[0];
            /*{
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };*/
            skinDefInfo.Name = "GROVETENDERBODY_DEFAULT_SKIN_NAME";
            skinDefInfo.NameToken = "GROVETENDERBODY_DEFAULT_SKIN_NAME";
            skinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            skinDefInfo.RootObject = model;
            skinDefInfo.UnlockableName = "";

            SkinDef defaultSkin = LoadoutAPI.CreateNewSkinDef(skinDefInfo);

            skinController.skins = new SkinDef[1]
            {
                defaultSkin
            };
        }
    }
}
