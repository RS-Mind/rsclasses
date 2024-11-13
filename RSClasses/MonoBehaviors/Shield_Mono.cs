using RSClasses.Extensions;
using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.MonoBehaviours
{
    class Shield : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(shield);
            Destroy(shieldCollider);
        }
        private void Start()
        {
            player = GetComponentInParent<Player>();

            shield = Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Shield"), player.transform);
            shield.SetActive(true);
            shieldCollider = shield.transform.GetChild(0).gameObject.GetOrAddComponent<ShieldCollider>();
            collider = shieldCollider.GetComponent<PolygonCollider2D>();
            anim = shield.GetComponent<Animator>();
            origin = shield.GetComponentsInChildren<Transform>().Last();
        }

        public void UpdatePos(double angle, float radius)
        {
            double angle_radians = (angle * Math.PI) / 180;
            Vector3 position = new Vector3((float)(radius * Math.Sin(angle_radians)),
                (float)((radius * Math.Cos(angle_radians))), 0);
            Quaternion currentRotation = new Quaternion();
            currentRotation.eulerAngles = new Vector3(0, 0, -(float)angle);
            shield.transform.localPosition = position;
            shield.transform.rotation = currentRotation;
            shieldCollider.transform.position = shield.transform.position;
            shieldCollider.transform.rotation = shield.transform.rotation;
            shieldCollider.transform.localScale = Vector3.Scale(shield.transform.localScale, player.transform.localScale);
        }

        public void DoHit()
        {
            anim.SetTrigger("OnBlock");
            var radius = shield.transform.localScale.y * 10;
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
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)origin.transform.position).normalized * 0.25f * player.data.maxHealth,
                            (Vector2)origin.transform.position, shield.gameObject, player);
                    }
                }
            }
        }

        private void Update()
        {
            shieldCollider.gameObject.SetActive(!player.data.dead);
        }

        public void SetColor(Color color)
        {
            shield.GetComponent<SpriteRenderer>().color = color;
            shield.GetComponentsInChildren<SpriteRenderer>().Last().color = color;
        }

        public void SetScale(float scale)
        {
            shield.transform.localScale = new Vector3(scale, scale, scale);
        }

        private Transform origin;
        private Animator anim;
        private Player player;
        private PolygonCollider2D collider;
        ShieldCollider shieldCollider;
        public GameObject shield;
    }

    class ShieldCollider : MonoBehaviour
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

    public class ShieldMono : MonoBehaviour
    {
        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPickEnd, PickEnd);
            //RSClasses.instance.ExecuteAfterSeconds(1f, () => GameModeManager.RemoveHook(GameModeHooks.HookGameEnd, GameEnd));

            Block block = this.block;
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(OnBlock));

            while (shields.Count() > 0)
            {
                Destroy(shields[0]);
                shields.Remove(shields[0]);
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
            angle = (angle - (speed * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Shield shield in shields)
            {
                double thisAngle = angle + ((360f / (float)shields.Count()) * (float)index);
                shield.UpdatePos(thisAngle, radius);
                index++;
            }
        }

        private void OnBlock(BlockTrigger.BlockTriggerType blockTrigger)
        {
            if (shieldSpikes)
            {
                foreach (Shield shield in shields)
                {
                    shield.DoHit();
                }
            }
        }

        public void UpdateStats()
        {
            while (shields.Count() < player.data.GetAdditionalData().barrierCount)
            {
                GameObject shield = new GameObject("Shield", typeof(Shield));
                shield.transform.SetParent(player.transform);
                player.transform.position = player.transform.position;
                shields.Add(shield.GetComponent<Shield>());
            }
            while (shields.Count() > Math.Max(player.data.GetAdditionalData().barrierCount, 0))
            {
                Destroy(shields[0]);
                shields.Remove(shields[0]);
            }
            if (player.data.GetAdditionalData().barrierCount <= 0)
            {
                Destroy(this);
            }

            foreach (Shield shield in shields)
            {
                RSClasses.instance.ExecuteAfterSeconds(1f, () => shield.GetComponent<Shield>().SetColor(color));
                RSClasses.instance.ExecuteAfterSeconds(1f, () => shield.SetScale(radius * 9.5f));
            }
        }
        public void setColor(Color newColor)
        {
            color = newColor;
            foreach (Shield shield in shields)
            {
                shield.SetColor(color);
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
        public bool shieldSpikes = false;
        public float speed = 100f;
        public float radius = 0.0125f;
        private double angle = 0;
        Color color = new Color(1f, 1f, 0.7411765f);

        List<Shield> shields = new List<Shield>();
        Player player;
    }
}