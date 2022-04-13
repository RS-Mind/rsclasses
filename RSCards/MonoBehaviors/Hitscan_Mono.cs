using UnityEngine;
using Photon.Pun;
using RSCards;
using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib;

namespace RSCards.MonoBehaviors
{
    public class Hitscan_Mono : MonoBehaviour
    {
		private void OnDestroy()
		{
			this.trail.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            RSCards.instance.ExecuteAfterSeconds(2, () => UnityEngine.GameObject.Destroy(this.trail));
		}

		private void Start()
		{
			this.trail = new GameObject("Hitscan_Trail", new Type[] { typeof(TrailRenderer) });
			this.trail.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
			this.trail.GetComponent<TrailRenderer>().startWidth = 0.1f;
			this.trail.GetComponent<TrailRenderer>().endWidth = 0.1f;
			this.trail.GetComponent<TrailRenderer>().time = 1f;
			this.trail.GetComponent<TrailRenderer>().sharedMaterial = RSCards.ArtAssets.LoadAsset<Material>("Material");
		}

		private void Update()
		{
			this.trail.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        }

		GameObject trail;
		GameObject bullet;
	}
}

