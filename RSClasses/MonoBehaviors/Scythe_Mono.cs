using Sonigon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.MonoBehaviors
{
    class Scythe : MonoBehaviour
    {
        private void OnDestroy()
        {
            Destroy(scythe);
        }

        private void Start()
        {
            Player = this.GetComponentInParent<Player>();

            scythe = GameObject.Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Scythe"), Player.transform);
            scythe.SetActive(true);
        }

        public void DoHit()
        {
            var radius = transform.localScale.y;
            var hits = Physics2D.OverlapCircleAll(scythe.transform.position, radius);

            foreach (var hit in hits)
            {
                var Keys = recent.Keys.ToArray();
                foreach (int Key in Keys)
                {
                    recent[Key] -= TimeHandler.deltaTime;
                }
                var bullet = hit.gameObject.GetComponentInParent<ProjectileHit>();
                var damageable = hit.gameObject.GetComponent<Damagable>();
                var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                if (healthHandler)
                {
                    healthHandler.CallTakeForce(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * 100, ForceMode2D.Impulse, true);
                    int id = ((Player)healthHandler.GetFieldValue("player")).playerID;
                    if (id == Player.playerID || (recent.ContainsKey(id) && recent[id] > 0)) { continue; }
                    SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform);
                    healthHandler.CallTakeForce(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * 10000, ForceMode2D.Impulse, true);
                    recent[id] = 0.5f;
                }
                if (damageable)
                {
                    damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)this.transform.position).normalized * damage,
                        (Vector2)this.transform.position, this.gameObject, Player);
                }
                if (bullet && hitBullets)
                {
                    bullet.deathEvent.Invoke();
                    Destroy(bullet.gameObject);
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
        private Dictionary<int, float> recent = new Dictionary<int, float>();
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
            angle = (angle + (speed * TimeHandler.deltaTime)) % 360;
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
                Player.transform.position = Player.transform.position;
                RSClasses.instance.ExecuteAfterSeconds(0.5f, () => scythe.GetComponent<Scythe>().SetColor(color));
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
                scythe.hitBullets = hitBullets;
                RSClasses.instance.ExecuteAfterSeconds(0.5f, () => scythe.SetScale(radius / 16f));
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
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            active = false;
            yield break;
        }

        public float speed = 100f;
        public float radius = 2.5f;
        private double angle = 0;
        private float rotation = 0;
        public int count = 0;
        public float damage = 55;
        private bool active = false;
        public bool hitBullets = true;
        Color color = new Color(1f, 1f, 0.7411765f);
        List<Scythe> scythes = new List<Scythe>();
        Player Player;
    }
}