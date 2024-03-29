﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem {
    public class InventoryObject : MonoBehaviour {
        // the item which will be added to the inventory of the player
        public InventoryItem item;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            // if item is unset
            if (item == null) {
                // log a warning
                Debug.LogWarning($"[WARNING]: variable 'item' not defined in 'InventoryObject' class on object '{transform.name}'.");
            }
        }
    }
}