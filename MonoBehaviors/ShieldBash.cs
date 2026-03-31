using System.Collections;
using System.Collections.Generic;
using UnboundLib;
using UnityEngine;

namespace RSClasses
{
    public class ShieldBash : MonoBehaviour
    {
        private Player player;
        private Animator animator;
        AttackLevel attackLevel;
        private Transform shieldCollider;
        const float force = 20000f;
        const float mapObjForce = 60000000f;

        void Start()
        {
            player = GetComponentInParent<Player>();
            var shield = player.GetComponentInChildren<HeaterShield>();
            animator = shield.GetComponentInChildren<Animator>();
            shieldCollider = shield.shieldCollider.transform;
            attackLevel = GetComponent<AttackLevel>();
            player.data.block.BlockAction += OnBlock;
        }

        void OnDestroy()
        {
            try
            {
                player.data.block.BlockAction -= OnBlock;
            }
            catch { }
        }

        void OnBlock(BlockTrigger.BlockTriggerType trigger)
        {
            animator.Play("Shield Bash");
            var hits = Physics2D.BoxCastAll(shieldCollider.position, shieldCollider.lossyScale, shieldCollider.eulerAngles.z, shieldCollider.up, 1f, PlayerManager.instance.canSeePlayerMask);
            foreach (var hit in hits)
            {
                UnityEngine.Debug.Log(hit.transform.gameObject.name);
                var character = hit.collider?.GetComponentInParent<CharacterData>();
                if (character != null && player.data.view.IsMine)
                {
                    character.view.RPC("RPCA_AddSlow", Photon.Pun.RpcTarget.All, 0.75f);
                    character.healthHandler.CallTakeForce(shieldCollider.up * force * attackLevel.LevelScale());
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(shieldCollider.up * mapObjForce * attackLevel.LevelScale());
                }
            }
        }
    }
}