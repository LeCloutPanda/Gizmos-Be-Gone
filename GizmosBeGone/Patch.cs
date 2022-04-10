using HarmonyLib;
using NeosModLoader;
using BaseX;
using FrooxEngine;
using System;
using System.Collections.Generic;

namespace GizmosBeGone
{
    public class Patch : NeosMod
    {
        public override string Name => "Gizmos be gone";
        public override string Author => "LeCloutPanda";
        public override string Version => "1.0.0";

        private static string randomId;

        public override void OnEngineInit()
        {
            Random rand = new Random();
            randomId = rand.Next(32767).ToString();

            Harmony harmony = new Harmony("net.LeCloutPanda.ClearGizmo");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(DevToolTip), "GenerateMenuItems")]
        class GizmoPatcher
        {
            [HarmonyPostfix]
            static void AddCustomContextMenuItem(ref DevToolTip __instance, CommonTool tool, ContextMenu menu)
            {
                // Create new context menu button only on dev tip
                Uri crashedNode = new Uri("neosdb:///19928b0d3941f7d878343f6001eced1c3dda0ffa00b2d2f31cf30a1503b1dbf2.png");
                var item = menu.AddItem("Deselect Owned", crashedNode, new color(0.1f, 0.75f));

                // Make the button do the funny
                item.Button.LocalPressed += (a, b) =>
                {
                    // Get all gizmo slots by grabbing the SlotGizmo component to filter out none gizmo slots name gizmos
                    List<SlotGizmo> gizmos = tool.World.RootSlot.GetComponentsInChildren<SlotGizmo>();

                    if (gizmos == null)
                        return;

                    // Iterate all gizmos and check if the slot tag is equal to your unique id
                    for (int i = 0; i < gizmos.Count; i++)
                    {
                        if (gizmos[i].Slot.Tag == randomId)
                            gizmos[i].Slot.Destroy();
                    }
                };
            }
        }

        [HarmonyPatch(typeof(SlotGizmo), "OnAttach")]
        class AddTagToGizmo
        {
            // Self explanatory, just grab the slot on gizmo creation and set its tag
            [HarmonyPostfix]
            static void ModifyGizmoSlot(ref SlotGizmo __instance)
            {
                __instance.Slot.Tag = randomId;
            }
        }
    }
}
