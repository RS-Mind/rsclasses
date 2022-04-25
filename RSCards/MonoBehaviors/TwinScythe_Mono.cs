﻿using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Sonigon;
using System.Collections;

namespace RSCards.MonoBehaviors
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

            scythe = GameObject.Instantiate(RSCards.ArtAssets.LoadAsset<GameObject>("Scythe"), Player.transform);
            scythe.SetActive(true);
            scythe.GetComponent<SpriteRenderer>().color = Player.GetTeamColors().color;
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
                if (bullet)
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

		public bool active = true;
		public float damage = 55;
		private Player Player;
        private Dictionary<int, float> recent = new Dictionary<int, float>();
		GameObject scythe;
	}

    public class TwinScytheMono : MonoBehaviour
    {
		private void Start()
		{
			Player = this.GetComponentInParent<Player>();

			GameModeManager.AddHook(GameModeHooks.HookPointStart, RoundStart);
			GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
		}

		private void Update()
		{
            radius = 2f;
            angle = (angle + (200 * TimeHandler.deltaTime)) % 360;
            rotation = (rotation - (1200 * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Scythe scythe in scythes)
            {
                double thisAngle = angle + ((360 / scythes.Count()) * index);
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

		public void UpdateCard()
        {
			int scytheCount = Math.Min(count, 4);
			if (scythes.Count() < scytheCount)
            {
				GameObject scythe = new GameObject("Scythe", typeof(Scythe));
				scythe.transform.SetParent(Player.transform);
				Player.transform.position = Player.transform.position;
				scythes.Add(scythe.GetComponent<Scythe>());
            }
			else if (scythes.Count() > scytheCount)
            {
                Destroy(scythes[0]);
				scythes.Remove(scythes[0]);
            }

			foreach (Scythe scythe in scythes)
            {
				scythe.damage = 27.5f + (27.5f * count);
            }
		}

		IEnumerator BattleStart(IGameModeHandler gm)
		{
			active = true;
			yield break;
		}

		IEnumerator RoundStart(IGameModeHandler gm)
		{
			rotation = 0f;
			angle = 0.0;
			active = false;
			yield break;
		}

		private float radius = 2.5f;
		private double angle = 0;
		private float rotation = 0;
		public int count = 0;
		private float damage = 55;
		private bool active = true;
		List<Scythe> scythes = new List<Scythe>();
        Player Player;
	}
}