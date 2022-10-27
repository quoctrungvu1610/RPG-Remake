﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreGround = null;
        [SerializeField] Canvas rootCanvas = null;
        void Update()
        {
            //|| Mathf.Approximately(healthComponent.GetFraction(),1)
            if (Mathf.Approximately(healthComponent.GetFraction(), 0))
            {
                rootCanvas.enabled = false;
                return;
            }
            rootCanvas.enabled = true;
            foreGround.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}

