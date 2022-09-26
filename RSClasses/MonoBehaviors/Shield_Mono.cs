using RSClasses.Extensions;
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

            shield = GameObject.Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Shield"), player.transform);
            shield.SetActive(true);
            shieldCollider = shield.transform.GetChild(0).gameObject.GetOrAddComponent<ShieldCollider>();
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

        private void Update()
        {
            shieldCollider.gameObject.SetActive(!player.data.dead);
        }

        public void SetColor(Color color)
        {
            shield.GetComponent<SpriteRenderer>().color = color;
        }

        public void SetScale(float scale)
        {
            shield.transform.localScale = new Vector3(scale, scale, scale);
        }

        private Player player;
        ShieldCollider shieldCollider;
        GameObject shield;
    }

    class ShieldCollider : MonoBehaviour
    {
        void Start()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Projectile");
            player = this.GetComponentInParent<Player>();
            this.gameObject.transform.SetParent(null, true);
        }

        public void Update()
        {
            this.gameObject.SetActive(!player.data.dead);
        }

        Player player;
    }

    public class ShieldMono : MonoBehaviour
    {
        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, RoundStart);
            //RSClasses.instance.ExecuteAfterSeconds(1f, () => GameModeManager.RemoveHook(GameModeHooks.HookGameEnd, GameEnd));

            while (shields.Count() > 0)
            {
                Destroy(shields[0]);
                shields.Remove(shields[0]);
            }
        }
        private void Start()
        {
            player = this.GetComponentInParent<Player>();

            GameModeManager.AddHook(GameModeHooks.HookPointStart, RoundStart);
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
                RSClasses.instance.ExecuteAfterSeconds(1f, () => shield.SetScale(radius / 11.9f));
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

        IEnumerator RoundStart(IGameModeHandler gm)
        {
            angle = 0.0;
            this.UpdateStats();
            yield break;
        }

        IEnumerator GameEnd(IGameModeHandler gm)
        {
            Destroy(this);
            yield break;
        }

        public float speed = 100f;
        public float radius = 1.5f;
        private double angle = 0;
        Color color = new Color(1f, 1f, 0.7411765f);

        List<Shield> shields = new List<Shield>();
        Player player;
    }
}