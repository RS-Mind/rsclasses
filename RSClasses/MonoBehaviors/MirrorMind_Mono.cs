using HarmonyLib;
using UnityEngine;
using UnboundLib;
using RSClasses.Extensions;
using RSClasses.Cards.MirrorMage;

namespace RSClasses.MonoBehaviours
{
    [HarmonyPatch(typeof(GeneralInput), "Update")]
    class GeneralInputPatchUpdate // Don't put code here. This is wrong
    {
        static GameObject mirror;
        static GameObject prism;
        private static void Postfix(GeneralInput __instance)
        {
            Player player = ((CharacterData)__instance.GetFieldValue("data")).player;
            if (!player.data.view.IsMine) return;
            if (!player.data.currentCards.Contains(MirrorMage.Card))
            {
                Object.Destroy(mirror);
                Object.Destroy(prism);
                return;
            }
            if (mirror == null)
            {
                mirror = new GameObject();
                LineRenderer lineRenderer = mirror.GetOrAddComponent<LineRenderer>();
                LineEffect lineEffect = mirror.GetOrAddComponent<LineEffect>();
                lineRenderer.startWidth = 0.25f;
                lineRenderer.endWidth = 0.25f;
                lineRenderer.material = RSClasses.ArtAssets.LoadAsset<Material>("Mirror");
                lineRenderer.SetPositions(new Vector3[] { new Vector3(0, -1000, 0), new Vector3(0, 1000, 0) });
                lineEffect.lineType = 0;
                lineEffect.segments = 1;
                try { lineEffect.DrawLine(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0)); } catch { }
            }

            if (player.data.GetAdditionalData().prism)
            {
                if (prism == null)
                {
                    prism = new GameObject();
                    LineRenderer lineRenderer = prism.GetOrAddComponent<LineRenderer>();
                    LineEffect lineEffect = prism.GetOrAddComponent<LineEffect>();
                    lineRenderer.startWidth = 0.125f;
                    lineRenderer.endWidth = 0.125f;
                    lineRenderer.material = RSClasses.ArtAssets.LoadAsset<Material>("Prism");
                    lineRenderer.SetPositions(new Vector3[] { new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0) });
                    lineEffect.lineType = 0;
                    lineEffect.segments = 1;
                    try { lineEffect.DrawLine(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0)); } catch { }
                }
            }
            else Object.Destroy(prism);

            if (!player.data.currentCards.Contains(MirrorMind.Card)) return;

            if (player.transform.position.x * player.data.GetAdditionalData().posMult > 5)
            {
                player.data.GetAdditionalData().posMult *= -1;
            }

            if (player.transform.position.x * player.data.GetAdditionalData().posMult > 0)
            {
                player.data.GetAdditionalData().invert = !player.data.GetAdditionalData().invert;
                player.transform.position = new Vector3(0, player.transform.position.y, player.transform.position.z);
                player.data.playerVel.SetFieldValue("velocity", Vector2.Scale(new Vector2(-1, 1), (Vector2)player.data.playerVel.GetFieldValue("velocity")));
            }

            if (player.data.GetAdditionalData().invert)
                __instance.direction = new UnityEngine.Vector3(-__instance.direction.x, __instance.direction.y, __instance.direction.z); // This one's ok
        }
    }
}