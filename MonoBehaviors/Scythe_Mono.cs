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
    class Scythe : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(scythe);
        }

        private void Start()
        {
            player = GetComponentInParent<Player>();

            scythe = Instantiate(RSClasses.assets.LoadAsset<GameObject>("Scythe"), player.transform);
            scythe.SetActive(true);
        }

        public void DoHit()
        {
            var radius = transform.localScale.y;
            var hits = Physics2D.OverlapCircleAll(scythe.transform.position, radius);

            if (player.data.view.IsMine)
            {
                foreach (var hit in hits)
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>();
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform);
                        healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)scythe.transform.position).normalized * 2500, ForceMode2D.Impulse, true);
                        this.ableToHit = false;
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; }
                    }
                    if (damageable)
                    {
                        float bonusDamage = 0f;
                        if (player.data.currentCards.Contains(CardHolder.cards["Dark Harvest"]))
                        {
                            bonusDamage = (player.data.stats.lifeSteal * 50f);
                        }
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * (player.data.GetAdditionalData().scytheDamage + bonusDamage),
                            (Vector2)this.transform.position, this.gameObject, player);
                    }
                }
            }
        }

        public void UpdatePos(double angle, float rotation, float radius)
        {
            double angle_radians = (angle * Math.PI) / 180;
            Vector3 position = new Vector3((float)(radius * Math.Sin(angle_radians)),
                (float)((radius * Math.Cos(angle_radians))), 0);
            Quaternion currentRotation = new Quaternion();
            currentRotation.eulerAngles = new Vector3(0, 0, rotation);
            scythe.transform.localPosition = position;
            scythe.transform.rotation = currentRotation;
        }

        public void SetColor(Color color)
        {
            scythe.GetComponent<SpriteRenderer>().color = color;
        }

        public void SetScale(float scale)
        {
            scythe.transform.localScale = new Vector3(scale, scale, scale);
        }

        public bool active = true;
        public bool hitBullets = true;
        public bool ableToHit = true;
        private Player player;
        GameObject scythe;
    }

    public class Scythe_Mono : MonoBehaviour
    {
        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.RemoveHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.RemoveHook(GameModeHooks.HookPointEnd, PointEnd);
            //RSClasses.instance.ExecuteAfterSeconds(1f, () => GameModeManager.RemoveHook(GameModeHooks.HookGameEnd, GameEnd));

            while (scythes.Count() > 0)
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }
        }
        private void Start()
        {
            player = GetComponentInParent<Player>();

            GameModeManager.AddHook(GameModeHooks.HookPickEnd, PickEnd);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
            //GameModeManager.AddHook(GameModeHooks.HookGameEnd, GameEnd);
        }

        private void Update()
        {
            angle = (angle + (player.data.GetAdditionalData().scytheSpeed * TimeHandler.deltaTime));
            if (angle > 360)
            {
                foreach (Scythe scythe in scythes)
                {
                    scythe.ableToHit = true;
                }
                angle -= 360;
            }
            rotation = (rotation - (1200 * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Scythe scythe in scythes)
            {
                double thisAngle = angle + ((360f / (float)scythes.Count()) * (float)index);
                scythe.UpdatePos(thisAngle, rotation, player.data.GetAdditionalData().orbitalRadius * 2.5f);
                index++;
                if (scythe.ableToHit)
                {
                    Color setColor = color;
                    setColor.a = 1;
                    scythe.SetColor(setColor);
                }
                else
                {
                    Color setColor = color;
                    setColor.a = 0.5f;
                    scythe.SetColor(setColor);
                }
            }
        }

        private void FixedUpdate()
        {
            if (active)
            {
                foreach (Scythe scythe in scythes)
                {
                    if (scythe.ableToHit)
                    {
                        scythe.DoHit();
                    }
                }
            }
        }

        public void UpdateStats()
        {
            while (scythes.Count() < player.data.GetAdditionalData().scytheCount)
            {
                GameObject scythe = new GameObject("Scythe", typeof(Scythe));
                scythe.transform.SetParent(player.transform);
                scythes.Add(scythe.GetComponent<Scythe>());
            }
            while (scythes.Count() > Math.Max(player.data.GetAdditionalData().scytheCount, 0))
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }

            if (player.data.currentCards.Contains(CardHolder.cards["Harvester"])) color = new Color(178f / 255f, 0f, 1f);
            else if (player.data.currentCards.Contains(CardHolder.cards["Guardian"])) color = new Color(0.4f, 1f, 1f);

            foreach (Scythe scythe in scythes)
            {
                RSClasses.instance.ExecuteAfterSeconds(1f, () => scythe.GetComponent<Scythe>().SetColor(color));
                RSClasses.instance.ExecuteAfterSeconds(1f, () => scythe.SetScale(player.data.GetAdditionalData().orbitalRadius * 0.15625f)); // 5 / 32
            }
        }

        IEnumerator BattleStart(IGameModeHandler gm)
        {
            active = true;
            rotation = 0f;
            angle = 0.0;
            yield break;
        }

        IEnumerator PickEnd(IGameModeHandler gm)
        {
            this.UpdateStats();
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            active = false;
            yield break;
        }

        IEnumerator GameEnd(IGameModeHandler gm)
        {
            Destroy(this);
            yield break;
        }

        private double angle = 0;
        private float rotation = 0;
        private bool active = false;
        Color color = new Color(1f, 1f, 0.7411765f);
        List<Scythe> scythes = new List<Scythe>();
        Player player;
    }
}