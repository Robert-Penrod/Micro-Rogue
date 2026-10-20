using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public abstract bool DoPickup(ActorPickupSystem pickupSystem);
    public abstract bool CanPickUp(ActorPickupSystem pickupSystem);
    public abstract void IsGrabbing(float deltaTime);
}
