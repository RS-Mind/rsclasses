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
    public class AberrantMotion_Mono : MonoBehaviour
    {
        private Comet_Mono cometHolder;
        private Player player;
        private Block block;
        private void Start()
        {
            player = GetComponentInParent<Player>();
            cometHolder = player.GetComponentInChildren<Comet_Mono>();
            block = GetComponentInParent<CharacterData>().block; // Get player's block
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(Trigger));
        }

        private void OnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(Trigger));
        }

        public void Trigger(BlockTrigger.BlockTriggerType blockTrigger)
        {
            if (!player.data.view.IsMine || blockTrigger != BlockTrigger.BlockTriggerType.Default)
            {
                return;
            }
            foreach (var comet in cometHolder.comets)
            {
                Vector3 vector = Vector3.right;
                if (player.data.playerActions.Device != null)
                {
                    vector = player.data.input.aimDirection * 20f;
                    if (vector == Vector3.zero)
                        vector = player.data.weaponHandler.gun.transform.rotation * Vector3.up * 20f;
                }
                else
                {
                    vector = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition) - comet.transform.position;
                    vector.z = 0f;
                    vector.Normalize();
                    vector *= 20f;
                }
                comet.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { vector });
            }
        }
    }
}