namespace QModManager.HarmonyPatches.CustomLoadScreen
{
    using Harmony;
    using QModManager.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Emit;
    using UnityEngine;
    using UnityEngine.UI;
    using Logger = Utility.Logger;

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.Init))]
    internal static class uGUI_SceneLoading_Init_Patch
    {
        internal static GameObject progressBar;
        internal static AssetBundle bundle;
        internal static float targetAlphaBG;
        internal static float targetAlphaBar;

        [HarmonyPrefix]
        internal static void Prefix(uGUI_SceneLoading __instance)
        {
            try
            {
                if (!bundle)
                {
                    bundle = AssetBundle.LoadFromFile(Path.Combine(Environment.CurrentDirectory, "Subnautica_Data/Managed/loadingscreen"));
                    GameObject prefab = bundle.LoadAsset<GameObject>("Assets/LoadingScreen/LoadingBar.prefab");
                    progressBar = GameObject.Instantiate(prefab);
                    GameObject.DontDestroyOnLoad(progressBar);
                }

                GameObject bg = progressBar.transform.Find("Loading bar/Loading bar background").gameObject;
                GameObject bar = progressBar.transform.Find("Loading bar/Loading bar... bar?").gameObject;

                ImageWithRoundedCorners bgIwrc = bg.EnsureComponent<ImageWithRoundedCorners>();
                Material bgMat = bundle.LoadAsset<Material>("Assets/LoadingScreen/Background Rounded.mat");
                bgIwrc.material = bgMat;
                bgIwrc.radius = 20;
                bgIwrc.Refresh();

                LoadingBar loadingBar = bar.EnsureComponent<LoadingBar>();
                loadingBar.Refresh(100);
                ImageWithRoundedCorners barIwrc = bar.EnsureComponent<ImageWithRoundedCorners>();
                Material barMat = bundle.LoadAsset<Material>("Assets/LoadingScreen/Bar Rounded.mat");
                targetAlphaBar = barMat.color.a;
                barIwrc.material = barMat;
                barIwrc.radius = 20;
                barIwrc.Refresh();

                __instance.loadingBackground.graphic = __instance.loadingBackground.graphic.AddRangeToArray(new Graphic[] { bg.GetComponent<Image>(), bar.GetComponent<Image>() });
            }
            catch (Exception e)
            {
                Logger.Error("Could not patch loading screen!");
                Logger.Exception(e);
            }
        }
    }

    [HarmonyPatch(typeof(uGUI_Fader), nameof(uGUI_Fader.ApplyState))]
    internal static class uGUI_Fader_ApplyState_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> c = instructions.ToList();
            int index = c.FindLastIndex(ci => ci.opcode == OpCodes.Mul) + 1;
            c.InsertRange(index, new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(uGUI_SceneLoading_Init_Patch), nameof(uGUI_SceneLoading_Init_Patch.targetAlphaBar))),
                new CodeInstruction(OpCodes.Mul),
            });
            return c;
        }
    }

    internal class ImageWithRoundedCorners : MonoBehaviour
    {
        private static readonly int Props = Shader.PropertyToID("_WidthHeightRadius");

        public Material material;
        public float radius = 20;

        internal void Refresh()
        {
            if (material == null) return;
            var rect = ((RectTransform)transform).rect;
            material.SetVector(Props, new Vector4(rect.width, rect.height, radius, 0));
        }
    }

    internal class LoadingBar : MonoBehaviour
    {
        public float start = 398.15f;
        public float end = -298.15f;
        public float amount = 0.5f;

        public void Refresh()
        {
            ((RectTransform)transform).SetEdge(RectTransform.Edge.Right, start + amount * (-start + end));
        }

        public void Refresh(int amount)
        {
            this.amount = amount / 100f;
            Refresh();
        }
    }
}
