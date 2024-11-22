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
    class Astro_Mono : MonoBehaviour
    {
        private void Start()
        {
            player = GetComponentInParent<Player>();
            scythes = player.gameObject.GetOrAddComponent<Scythe_Mono>();
            barriers = player.gameObject.GetOrAddComponent<Barrier_Mono>();
        }

        private void OnDestroy()
        {
            Destroy(scythes);
            Destroy(barriers);
        }
        private Player player;
        private Scythe_Mono scythes;
        private Barrier_Mono barriers;
    }
}