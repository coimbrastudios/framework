﻿using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Listen to <see cref="Collider2D"/>'s <see cref="OnTriggerEnter2D"/> callback.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(CoimbraUtility.GeneralMenuPath + "Trigger Enter 2D Listener")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter2D.html")]
    public sealed class TriggerEnter2DListener : Trigger2DListenerBase
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Trigger(other);
        }
    }
}