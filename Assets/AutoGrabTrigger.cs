using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;
using Autohand;

/// <summary>
/// Calls grab when an Autohand enters its trigger collider
/// </summary>
public class AutoGrabTrigger : MonoBehaviour
{
    [Tooltip("Limit the automatic grab interaction to one hand at a time")]
    [SerializeField] private bool _oneHandLimit = true;

    [SerializeField] private Collider _grabbableToGrab = null;

    [Header("Debug")]
    [SerializeField] private bool _log = true;

    private Autohand.Hand _handL = null;
    private Autohand.Hand _handR = null;

    [SerializeField][Range(0.05f, 1.0f)] private float _grabCooldown = 0.1f;
    private float _grabCooldownMax;

    [SerializeField] HandType _typeThatCanGrab = HandType.both;


    public void Instantiate(HandType handType, Collider collider, bool oneHanded)
    {
        _grabbableToGrab = collider;
        _typeThatCanGrab = handType;
        _oneHandLimit = oneHanded;
        _grabCooldownMax = _grabCooldown;
        _grabCooldown = 0.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
        {
            Debug.Log($"Grabbed by: {other.gameObject.name}", other.gameObject);

            // Find the hand that is interacting
            Autohand.Hand hand = other.GetComponent<Autohand.Hand>();
            if (hand != null)
            {
                    Grab(hand);
            }
        }
    }

    private void Update()
    {
        if (_grabCooldown > 0.0f)
        {
            _grabCooldown -= Time.deltaTime;
        }
    }

    // Release hands on disable
    private void OnDisable()
    {
        _handL = null;
        _handR = null;
    }

    private void Grab(Autohand.Hand hand)
    {
        Grabbable grabbable = null;

        GrabbableChild child = _grabbableToGrab.GetComponent<GrabbableChild>();
        if (child != null)
            grabbable = child.grabParent;

        if (grabbable == null)
            grabbable = _grabbableToGrab.GetComponent<Grabbable>();

        //make sure that the hand is of a type that is allowed to autograb this object
        if (_typeThatCanGrab == HandType.both || (hand.left && _typeThatCanGrab == HandType.left) || (!hand.left && _typeThatCanGrab == HandType.right))
        {
            if(_log)
            {
                Debug.Log(this + " hand left: " + hand.left + " grabbable type: " + _typeThatCanGrab.ToString());
                Debug.Log(this + " " + hand.gameObject.name + " is allowed to autograb " + grabbable.gameObject.name);
            }
        }
        else
        {
            if (_log)
            {
                Debug.Log(this + " hand left: " + hand.left + " grabbable type: " + _typeThatCanGrab.ToString());
                Debug.Log(this + " " + hand.gameObject.name + " is not allowed to autograb " + grabbable.gameObject.name);
            }

            return;
        }

        //TODO double check if this actually works
        // Check if there is a one hand limit before grabbing.
        if (hand.left && _handL == null)
        {
            Debug.Log("Left");

            if (_oneHandLimit && _handR != null)
            {
                Debug.LogError("One hand limit");
                return;
            }

            if (_grabCooldown > 0.0f)
            {
                Debug.LogError("Cooldown");
                return;
            }

            if (_log)
                Debug.Log($"Grab Left ({hand.gameObject.name}) ({grabbable.gameObject.name})");

            _handL = hand;
            
            if (_grabbableToGrab != null)
            {
                hand.TryGrab(grabbable);
                GrabLock grablock = grabbable.GetComponent<GrabLock>();
                if (hand.holdingObj != null && grablock != null)
                {
                    grablock.OnGrabPressed?.Invoke(hand, grabbable);
                    //hand.GetComponent<OpenXRHandTrackingLink>().LockGrabbing(true);
                }
            }
            else
                hand.Grab();
        }
        else if (!hand.left && _handR == null)
        {
            Debug.Log("Right");

            if (_oneHandLimit && _handL != null)
            {
                Debug.LogError("One hand limit");
                return;
            }

            if (_grabCooldown > 0.0f)
            {
                Debug.LogError("Cooldown");
                return;
            }

            if (_log)
                Debug.Log($"Grab Right ({hand.gameObject.name}) ({grabbable.gameObject.name})");

            _handR = hand;

            if (_grabbableToGrab != null)
            {
                hand.TryGrab(grabbable);
                if (hand.holdingObj != null)
                {
                    grabbable.GetComponent<GrabLock>().OnGrabPressed?.Invoke(hand, grabbable);
                    //hand.GetComponent<OpenXRHandTrackingLink>().LockGrabbing(true);
                }
            }
            else
                hand.Grab();
        }

        //cooldown to ensure both hands don't one-handed grab the same object 
        //moved this to update loop
        //StartCoroutine(Cooldown());
    }

    public void Debug_GrabLocked(Autohand.Hand hand, Autohand.Grabbable grabbable)
    {
        Debug.Log($"(Grab Lock) | {hand.gameObject.name} locked to {grabbable.gameObject.name}");
    }
}
