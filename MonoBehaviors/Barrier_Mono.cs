using RSClasses.Extensions;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    class Barrier : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(barrier);
            Destroy(barrierCollider);
        }
        private void Start()
        {
            player = GetComponentInParent<Player>();

            barrier = Instantiate(RSClasses.assets.LoadAsset<GameObject>("Barrier"), player.transform);
            barrier.SetActive(true);
            barrierCollider = barrier.transform.GetChild(0).gameObject.GetOrAddComponent<BarrierCollider>();
            collider = barrierCollider.GetComponent<PolygonCollider2D>();
            anim = barrier.GetComponent<Animator>();
            origin = barrier.GetComponentsInChildren<Transform>().Last();
        }

        public void UpdatePos(double angle, float radius)
        {
            double angle_radians = (angle * Math.PI) / 180;
            Vector3 position = new Vector3((float)(radius * Math.Sin(angle_radians)),
                (float)((radius * Math.Cos(angle_radians))), 0);
            Quaternion currentRotation = new Quaternion();
            currentRotation.eulerAngles = new Vector3(0, 0, -(float)angle);
            barrier.transform.localPosition = position;
            barrier.transform.rotation = currentRotation;
            barrierCollider.transform.position = barrier.transform.position;
            barrierCollider.transform.rotation = barrier.transform.rotation;
            barrierCollider.transform.localScale = Vector3.Scale(barrier.transform.localScale, player.transform.localScale);
        }

        public void DoHit()
        {
            anim.SetTrigger("OnBlock");
            var radius = barrier.transform.localScale.y * 10;
            var hits = Physics2D.OverlapCircleAll(origin.transform.position, radius);
            if (player.data.view.IsMine)
            {
                foreach (var hit in hits)
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>();
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, origin.transform, damageable.transform);
                        healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)player.transform.position).normalized * 5000, ForceMode2D.Impulse, true);
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; }
                    }
                    if (damageable)
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)origin.transform.position).normalized * ((0.10f * player.data.maxHealth) + 10f),
                            (Vector2)origin.transform.position, barrier.gameObject, player);
                    }
                }
            }
        }

        private void Update()
        {
            barrierCollider.gameObject.SetActive(!player.data.dead);
        }

        public void SetColor(Color color)
        {
            barrier.GetComponent<SpriteRenderer>().color = color;
            barrier.GetComponentsInChildren<SpriteRenderer>().Last().color = color;
        }

        public void SetScale(float scale)
        {
            barrier.transform.localScale = new Vector3(scale, scale, scale);
        }

        private Transform origin;
        private Animator anim;
        private Player player;
        private PolygonCollider2D collider;
        BarrierCollider barrierCollider;
        public GameObject barrier;
    }

    class BarrierCollider : MonoBehaviour
    {
        void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Projectile");
            player = GetComponentInParent<Player>();
            gameObject.transform.SetParent(null, true);
        }

        public void Update()
        {
            gameObject.SetActive(!player.data.dead);
        }

        Player player;
    }

    public class Barrier_Mono : MonoBehaviour
    {
        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPickEnd, PickEnd);
            //RSClasses.instance.ExecuteAfterSeconds(1f, () => GameModeManager.RemoveHook(GameModeHooks.HookGameEnd, GameEnd));

            Block block = this.block;
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));

            while (barriers.Count() > 0)
            {
                Destroy(barriers[0]);
                barriers.Remove(barriers[0]);
            }
        }
        private void Start()
        {
            player = this.GetComponentInParent<Player>();
            block = this.GetComponentInParent<CharacterData>().block;
			block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));

            GameModeManager.AddHook(GameModeHooks.HookPickEnd, PickEnd);
            //GameModeManager.AddHook(GameModeHooks.HookGameEnd, GameEnd);
        }

        private void Update()
        {
            angle = (angle - (player.data.GetAdditionalData().barrierSpeed * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Barrier barrier in barriers)
            {
                double thisAngle = (angle + (float)((index%2*180) + (45*(index/2)))) % 360f;
                barrier.UpdatePos(thisAngle, player.data.GetAdditionalData().orbitalRadius * 0.0125f);
                index++;
            }
        }

        private void OnBlock(BlockTrigger.BlockTriggerType blockTrigger)
        {
            if (player.data.currentCards.Contains(CardHolder.cards["Shield Spikes"]))
            {
                foreach (Barrier barrier in barriers)
                {
                    barrier.DoHit();
                }
            }
        }

        public void UpdateStats()
        {
            while (barriers.Count() < player.data.GetAdditionalData().barrierCount)
            {
                GameObject shield = new GameObject("Shield", typeof(Barrier));
                shield.transform.SetParent(player.transform);
                player.transform.position = player.transform.position;
                barriers.Add(shield.GetComponent<Barrier>());
            }
            while (barriers.Count() > Math.Max(player.data.GetAdditionalData().barrierCount, 0))
            {
                Destroy(barriers[0]);
                barriers.Remove(barriers[0]);
            }


            if (player.data.currentCards.Contains(CardHolder.cards["Guardian"])) color = new Color(0.4f, 1f, 1f);
            else if (player.data.currentCards.Contains(CardHolder.cards["Harvester"])) color = new Color(178f / 255f, 0f, 1f);

            foreach (Barrier shield in barriers)
            {
                RSClasses.instance.ExecuteAfterSeconds(1f, () => shield.GetComponent<Barrier>().SetColor(color));
                RSClasses.instance.ExecuteAfterSeconds(1f, () => shield.SetScale(player.data.GetAdditionalData().orbitalRadius * 0.11875f)); // I guess and checked this value
            }
        }

        IEnumerator PickEnd(IGameModeHandler gm)
        {
            angle = 0.0;
            UpdateStats();
            yield break;
        }

        IEnumerator GameEnd(IGameModeHandler gm)
        {
            Destroy(this);
            yield break;
        }

        private Block block;
        private double angle = 0;
        Color color = new Color(1f, 1f, 0.7411765f);

        List<Barrier> barriers = new List<Barrier>();
        Player player;
    }
}