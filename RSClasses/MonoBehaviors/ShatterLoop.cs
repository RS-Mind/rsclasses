using ClassesManagerReborn.Util;
using RSClasses.Cards.MirrorMage;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace RSClasses.MonoBehaviors
{
    internal class ShatterLoop : MonoBehaviour
    {
        public ClassNameMono parent;
        public void Update()
        {
            gameObject.SetActive(parent.isActiveAndEnabled);
        }
    }
}
