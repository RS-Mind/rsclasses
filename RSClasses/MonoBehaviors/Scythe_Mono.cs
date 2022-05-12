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
            Player = GetComponentInParent<Player>();

            scythe = Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Scythe"), Player.transform);
            scythe.SetActive(true);
        }

        public void DoHit()
        {
            var radius = transform.localScale.y;
            var hits = Physics2D.OverlapCircleAll(scythe.transform.position, radius);

            if (Player.data.view.IsMine)
            {
                foreach (var hit in hits)
                {
                    var damageable = hit.gameObject.GetComponent<Damagable>();
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        if (hitPlayer.playerID == Player.playerID || recent.ContainsKey(hitPlayer.playerID)) { continue; }
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform);
                        healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)scythe.transform.position).normalized * 5000, ForceMode2D.Impulse, true);
                        recent[hitPlayer.playerID] = 0.5f;
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; }
                    }
                    if (damageable)
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * damage,
                            (Vector2)this.transform.position, this.gameObject, Player);
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
        public float damage = 55;
        public bool hitBullets = true;
        private Player Player;
        public Dictionary<int, float> recent = new Dictionary<int, float>();
        GameObject scythe;
    }

    public class ScytheMono : MonoBehaviour
    {
        private void OnDestroy()
        {
            while (scythes.Count() > 0)
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }

            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.RemoveHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.RemoveHook(GameModeHooks.HookPointEnd, PointEnd);
        }
        private void Start()
        {
            Player = this.GetComponentInParent<Player>();

            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
        }

        private void Update()
        {
            angle = (angle + (speed * TimeHandler.deltaTime));
            if (angle > 360)
            {
                foreach (Scythe scythe in scythes)
                {
                    scythe.recent.Clear();
                }
                angle -= 360;
            }
            rotation = (rotation - (1200 * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Scythe scythe in scythes)
            {
                double thisAngle = angle + ((360f / (float)scythes.Count()) * (float)index);
                scythe.UpdatePos(thisAngle, rotation, radius);
                index++;
            }
        }

        private void FixedUpdate()
        {
            if (active)
            {
                foreach (Scythe scythe in scythes)
                {
                    scythe.DoHit();
                }
            }
        }

        public void UpdateStats()
        {
            if (count <= 0)
            {
                Destroy(this);
            }
            while (scythes.Count() < count)
            {
                GameObject scythe = new GameObject("Scythe", typeof(Scythe));
                scythe.transform.SetParent(Player.transform);
                scythes.Add(scythe.GetComponent<Scythe>());
            }
            while (scythes.Count() > Math.Max(count, 0))
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }
            if (count <= 0)
            {
                Destroy(this);
            }

            foreach (Scythe scythe in scythes)
            {
                scythe.damage = damage;
                RSClasses.instance.ExecuteAfterSeconds(1f, () => scythe.GetComponent<Scythe>().SetColor(color));
                RSClasses.instance.ExecuteAfterSeconds(1f, () => scythe.SetScale(radius / 16f));
            }
        }

        public void setColor(Color newColor)
        {
            color = newColor;
            foreach (Scythe scythe in scythes)
            {
                scythe.SetColor(color);
            }
        }

        IEnumerator BattleStart(IGameModeHandler gm)
        {
            active = true;
            yield break;
        }

        IEnumerator PointStart(IGameModeHandler gm)
        {
            rotation = 0f;
            angle = 0.0;
            this.UpdateStats();
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            active = false;
            yield break;
        }

        public float speed = 250f;
        public float radius = 2.5f;
        private double angle = 0;
        private float rotation = 0;
        public int count = 0;
        public float damage = 20;
        private bool active = false;
        Color color = new Color(1f, 1f, 0.7411765f);
        List<Scythe> scythes = new List<Scythe>();
        Player Player;
    }
}