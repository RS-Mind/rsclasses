using HarmonyLib;
using UnityEngine;
using UnboundLib;
using RSClasses.Extensions;
using RSClasses.Cards.MirrorMage;
using Photon.Pun;

namespace RSClasses.MonoBehaviours
{
    [HarmonyPatch(typeof(GeneralInput), "Update")]
    class GeneralInputPatchUpdate // Don't put code here. This is wrong
    {
        static GameObject mirror;
        static GameObject prism;
        static GameObject kaleido1;
        static GameObject kaleido2;
        private static void Postfix(GeneralInput __instance)
        {
            Player player = ((CharacterData)__instance.GetFieldValue("data")).player;
            if (!player.data.view.IsMine && !PhotonNetwork.OfflineMode) return;
            if (!player.data.currentCards.Contains(MirrorMage.Card))
            {
                // The below causes visual issues with multiple local players. For now, losing Mirror Mage cards won't get rid of the mirror object
                // Object.Destroy(mirror);
                // Object.Destroy(prism);
                return;
            }
            if (mirror == null)
            {
                mirror = new GameObject();
                LineRenderer lineRenderer = mirror.GetOrAddComponent<LineRenderer>();
                //LineEffect lineEffect = mirror.GetOrAddComponent<LineEffect>();
                lineRenderer.startWidth = 0.25f;
                lineRenderer.endWidth = 0.25f;
                lineRenderer.material = RSClasses.ArtAssets.LoadAsset<Material>("Mirror");
                lineRenderer.SetPositions(new Vector3[] { new Vector3(0, -1000, 0), new Vector3(0, 1000, 0) });
                //lineEffect.lineType = 0;
                //lineEffect.segments = 1;
                //try { lineEffect.DrawLine(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0)); } catch { }
            }

            if (player.data.GetAdditionalData().prism)
            {
                if (prism == null)
                {
                    prism = new GameObject();
                    LineRenderer lineRenderer = prism.GetOrAddComponent<LineRenderer>();
                    //LineEffect lineEffect = prism.GetOrAddComponent<LineEffect>();
                    lineRenderer.startWidth = 0.25f;
                    lineRenderer.endWidth = 0.25f;
                    lineRenderer.material = RSClasses.ArtAssets.LoadAsset<Material>("Prism");
                    lineRenderer.SetPositions(new Vector3[] { new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0) });
                    //lineEffect.lineType = 0;
                    //lineEffect.segments = 1;
                    //try { lineEffect.DrawLine(new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0)); } catch { }
                }
            }
            else Object.Destroy(prism);

            if (player.data.GetAdditionalData().kaleido)
            {
                if (kaleido1 == null)
                {
                    kaleido1 = new GameObject();
                    LineRenderer lineRenderer = kaleido1.GetOrAddComponent<LineRenderer>();
                    //LineEffect lineEffect = kaleido1.GetOrAddComponent<LineEffect>();
                    lineRenderer.startWidth = 0.25f;
                    lineRenderer.endWidth = 0.25f;
                    lineRenderer.material = RSClasses.ArtAssets.LoadAsset<Material>("Kaleido");
                    lineRenderer.SetPositions(new Vector3[] { new Vector3(-1000, -1000, 0), new Vector3(1000, 1000, 0) });
                    //lineEffect.lineType = 0;
                    //lineEffect.segments = 1;
                    //try { lineEffect.DrawLine(new Vector3(-1000, -1000, 0), new Vector3(1000, 1000, 0)); } catch { }
                }
                if (kaleido2 == null)
                {
                    kaleido2 = new GameObject();
                    LineRenderer lineRenderer = kaleido2.GetOrAddComponent<LineRenderer>();
                    //LineEffect lineEffect = kaleido2.GetOrAddComponent<LineEffect>();
                    lineRenderer.startWidth = 0.25f;
                    lineRenderer.endWidth = 0.25f;
                    lineRenderer.material = RSClasses.ArtAssets.LoadAsset<Material>("Kaleido");
                    lineRenderer.SetPositions(new Vector3[] { new Vector3(1000, -1000, 0), new Vector3(-1000, 1000, 0) });
                    //lineEffect.lineType = 0;
                    //lineEffect.segments = 1;
                    //try { lineEffect.DrawLine(new Vector3(1000, -1000, 0), new Vector3(-1000, 1000, 0)); } catch { }
                }
            }
            else { 
                Object.Destroy(kaleido1);
                Object.Destroy(kaleido2);
            }

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