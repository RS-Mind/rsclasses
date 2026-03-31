using Photon.Pun;
using RSClasses.Utilities;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnboundLib;
using UnboundLib.Extensions;
using UnboundLib.GameModes;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace RSClasses.MonoBehaviours
{
    class Stardust_Mono : MonoBehaviour
    {
        public Player player;
        public float rotationDirection = 90;

        private void Start()
        {

        }

        private void Update()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + (Time.deltaTime * rotationDirection));
        }

        private void FixedUpdate()
        {
            if (player.data.view.IsMine)
            {
                var hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
                foreach (var hit in hits)
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>();
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        if (hitPlayer == player) continue;
                        SoundManager.Instance.PlayAtPosition(RSClasses.stardustSound, this.transform, damageable.transform);
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame)
                        {
                            PhotonNetwork.Destroy(this.gameObject);
                            continue;
                        }
                    }
                    if (damageable)
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * (player.data.GetAdditionalData().cometDamage / 5),
                            (Vector2)this.transform.position, this.gameObject, player);
                        PhotonNetwork.Destroy(this.gameObject);
                    }
                }
            }
        }

        [PunRPC]
        public void SetValues(int ownerID, int rotation)
        {
            rotationDirection = rotation; // And random rotation directions
            player = PlayerManager.instance.players.Find(p => p.playerID == ownerID); // Set stardust player and color
            GetComponent<SpriteRenderer>().color = player.GetTeamColors().color * 1.75f;
        }
    }
}