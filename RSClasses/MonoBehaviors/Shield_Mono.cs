using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace RSClasses.MonoBehaviors
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
            Player = this.GetComponentInParent<Player>();

            shield = GameObject.Instantiate(RSClasses.ArtAssets.LoadAsset<GameObject>("Shield"), Player.transform);
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
            shieldCollider.transform.localScale = Vector3.Scale(shield.transform.localScale, Player.transform.localScale);
        }

        

        public void SetColor(Color color)
        {
            shield.GetComponent<SpriteRenderer>().color = color;
        }

        public void SetScale(float scale)
        {
            shield.transform.localScale = new Vector3(scale, scale, scale);
        }

        private Player Player;
        ShieldCollider shieldCollider;
        GameObject shield;
    }

    class ShieldCollider : MonoBehaviour
    {
        void Start()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Projectile");
            Player = this.GetComponentInParent<Player>();
            this.gameObject.transform.SetParent(null, true);
        }

        public void Update()
        {
            this.gameObject.SetActive(Player.isActiveAndEnabled);
        }

        Player Player;
    }

    public class ShieldMono : MonoBehaviour
    {
        private void OnDestroy()
        {
            while (shields.Count() > 0)
            {
                Destroy(shields[0]);
                shields.Remove(shields[0]);
            }
        }
        private void Start()
        {
            Player = this.GetComponentInParent<Player>();

            GameModeManager.AddHook(GameModeHooks.HookPointStart, RoundStart);
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
            while (shields.Count() < count)
            {
                GameObject shield = new GameObject("Shield", typeof(Shield));
                shield.transform.SetParent(Player.transform);
                Player.transform.position = Player.transform.position;
                RSClasses.instance.ExecuteAfterSeconds(0.5f, () => shield.GetComponent<Shield>().SetColor(color));
                shields.Add(shield.GetComponent<Shield>());
            }
            while (shields.Count() > Math.Max(count, 0))
            {
                Destroy(shields[0]);
                shields.Remove(shields[0]);
            }
            if (count <= 0)
            {
                Destroy(this);
            }

            foreach (Shield shield in shields)
            {
                RSClasses.instance.ExecuteAfterSeconds(0.5f, () => shield.SetScale(radius / 11.9f));
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
            yield break;
        }

        public float speed = 100f;
        public float radius = 1.5f;
        private double angle = 0;
        public int count = 0;
        Color color = new Color(1f, 1f, 0.7411765f);

        List<Shield> shields = new List<Shield>();
        Player Player;
    }
}