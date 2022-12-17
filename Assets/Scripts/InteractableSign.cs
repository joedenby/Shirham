using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSign : Interactable
{
    private new BoxCollider2D collider;

    private void Start()
    {
        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new(1, 1);
    }

}
