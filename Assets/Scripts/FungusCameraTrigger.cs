﻿//Trying to replicate pauls work
using Fungus;
using UnityEngine;

public class FungusCameraTrigger : MonoBehaviour {
    public string pickupMessage = "PickupQuestItem";
    public string pickupMessage2 = "";

    //Needs renderer and collider attached, this won't work without them
    Collider col;
    public Renderer ren;

    private void Start() {
        //Gets values from the components

        col = GetComponent<Collider>();
        //ren = GetComponent<Renderer>();

        col.enabled = true;
        //ren.enabled = true;
    }

    //This is the bit that only lets the player trigger it, the player needs to be on the "PlayerObject" layer
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // Disable the collider immediately to prevent this method triggering again.
            Flowchart.BroadcastFungusMessage(pickupMessage);
            if (pickupMessage2 != "") {
                Flowchart.BroadcastFungusMessage(pickupMessage2);
            }
            //This is the default fungus message that will be shown, keeping it the same as pauls in case things brea
            Debug.Log($"{name}: {other.name}");
        }
    }
}