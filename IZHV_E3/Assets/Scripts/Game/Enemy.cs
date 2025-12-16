using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Main Enemy behavior script.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// Current health of the Enemy.
    /// </summary>
    [ Header("Gameplay") ]
    public float health = 10.0f;

    /// <summary>
    /// Current speed of the Enemy.
    /// </summary>
    public float speed = 1.0f;
    
    /// <summary>
    /// Rigid body of the Enemy.
    /// </summary>
    private Rigidbody mRigidBody;
    
    /// <summary>
    /// Trigger of the Enemy.
    /// </summary>
    private BoxCollider mBoxTrigger;
    
    /// <summary>
    /// Collider of the Enemy.
    /// </summary>
    private BoxCollider mBoxCollider;

    /// <summary>
    /// Called when the script instance is first loaded.
    /// </summary>
    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        
        // Get the collider and the trigger, making sure we got the correct one.
        var colliders = GetComponents<BoxCollider>();
        mBoxTrigger = colliders[0]; Assert.IsTrue(mBoxTrigger.isTrigger);
        mBoxCollider = colliders[1];
    }

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    { SetupCollider(); }

    /// <summary>
    /// Update called at fixed time delta.
    /// </summary>
     void FixedUpdate()
    {
        // 1. Find Player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestPlayer = null;
        float closestDistance = float.MaxValue;

        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        // 2. Move
        if (closestPlayer != null)
        {
            Vector3 direction = (closestPlayer.transform.position - transform.position).normalized;

            if (mRigidBody != null)
            {
                mRigidBody.velocity = direction * speed;
            }
            
            // Rotate 
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Stop moving
            if (mRigidBody != null) mRigidBody.velocity = Vector3.zero;
        }
    }


    /// <summary>
    /// Triggered when a collision is detected.
    /// </summary>
    /// <param name="other">The other collidable.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        { DestroyEnemy(); }
    }

    /// <summary>
    /// Destroy this Enemy.
    /// </summary>
    public void DestroyEnemy()
    { Destroy(gameObject); }

    /// <summary>
    /// Setup collider for better collision detection.
    /// </summary>
    private void SetupCollider()
    {
        // Modify the rigid body to allow collisions on the y axis.
        var colliderCenter = mBoxCollider.center;
        mBoxCollider.center = new Vector3 {
            x = colliderCenter.x,
            y = colliderCenter.y, 
            z = transform.localScale.y, 
        };
        var colliderSize = mBoxCollider.size;
        mBoxCollider.size = new Vector3 {
            x = colliderSize.x,
            y = colliderSize.y, 
            //z = 1.0f / transform.localScale.y, 
            z = colliderSize.z, 
        };
    }
}
