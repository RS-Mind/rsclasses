using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using RSClasses.MonoBehaviors;
using RSClasses.Utilities;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Extensions;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    public class Comet_Mono : MonoBehaviour
    {
        private bool active = false;
        private List<Comet> comets = new List<Comet>();
        private Player player;
        private void Start()
        {
            player = GetComponentInParent<Player>();

            GameModeManager.AddHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
        }

        private void FixedUpdate()
        {
            if (active)
            {
                foreach (Comet comet in comets)
                {
                    comet.UpdatePos(player.transform.position);

                    if (player.data.view.IsMine)
                    {
                        comet.DoHit();
                    }
                }
            }
        }

        public void UpdateStats()
        {
            if (player.data.view.IsMine)
            {
                while (comets.Count() < player.data.GetAdditionalData().cometCount)
                {
                    GameObject comet = PhotonNetwork.Instantiate("Comet", Vector3.zero, Quaternion.identity);
                    comets.Add(comet.GetComponent<Comet>());
                }
                while (comets.Count() > Math.Max(player.data.GetAdditionalData().cometCount, 0))
                {
                    PhotonNetwork.Destroy(comets[0].gameObject);
                }

                foreach (Comet comet in comets)
                {
                    comet.photonView.RPC("SetAttributes", RpcTarget.All, new object[] { player.playerID, (player.data.GetAdditionalData().orbitalRadius) + 0.5f });
                }
            }
            // Regenerate Comet List
            comets.Clear();
            foreach (Comet comet in FindObjectsOfType(typeof(Comet)))
            {
                if (comet.name == "Comet " + player.playerID)
                {
                    comets.Add(comet);
                }
            }
        }

        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.RemoveHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.RemoveHook(GameModeHooks.HookPointEnd, PointEnd);

            while (comets.Count() > 0)
            {
                Destroy(comets[0]);
                comets.Remove(comets[0]);
            }
        }

        IEnumerator PickEnd(IGameModeHandler gm)
        {
            UpdateStats();
            yield break;
        }
        IEnumerator PointStart(IGameModeHandler gm)
        {
            int index = 0;
            foreach (Comet comet in comets)
            {
                comet.transform.position = player.transform.position + new Vector3(0, 7.5f - (15*index), 0);
                comet.velocity = new Vector3(0, 0, 0);
                index++;
            }
            yield break;
        }IEnumerator BattleStart(IGameModeHandler gm)
        {
            active = true;
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            active = false;
            yield break;
        }
    }
}