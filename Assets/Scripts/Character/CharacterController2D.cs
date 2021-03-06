﻿using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] public float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[SerializeField] public float m_CrouchForce = 400f;						   // Amount of force added when the player crouches.
	[Range(0, 1)] [SerializeField] public float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] public float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] public bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] public LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] public LayerMask m_WhatIsClimbable;                          // A mask determining what is climbable to the character
	[SerializeField] public Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] public Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	//[SerializeField] public Transform m_FallCheck;								// A position marking where to check for the end of ground
	[SerializeField] public Transform m_ClimbCheck;								// A position marking where to check for the end of ground


	[SerializeField] public Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	//const float k_FallRadius = .2f; // Radius of the overlap circle to determine if there is ground to walk towards
	private bool m_Climbing;            // Whether or not the player is grounded.
	const float k_ClimbingRadius = .45f;

	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
                {
					OnLandEvent.Invoke();
				}
			}
		}

		//print(m_Grounded.ToString() + " " + wasGrounded.ToString());

		// The player is grounded if a circlecast to the climbing check position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		bool wasClimbing = m_Climbing;
		m_Climbing = false;

		if (m_ClimbCheck)
        {
			Collider2D[] climb_colliders = Physics2D.OverlapCircleAll(m_ClimbCheck.position, k_ClimbingRadius, m_WhatIsClimbable);
			for (int i = 0; i < climb_colliders.Length; i++)
			{
				if (climb_colliders[i].gameObject != gameObject)
				{
					m_Climbing = true;
				}
			}
		}

	}


	public bool Move(float xMove, float yMove, bool crouch, bool jump)
	{

		/*if (m_FallCheck)
        {
			if (!Physics2D.OverlapCircle(m_FallCheck.position, k_FallRadius, m_WhatIsGround))
            {
				return;
            }
		}*/

		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					m_Rigidbody2D.AddForce(new Vector2(0f, -m_CrouchForce));
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				xMove *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(xMove * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (xMove > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (xMove < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}

		if (m_Climbing)
        {
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(m_Rigidbody2D.velocity.x, yMove * 10f);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			return (m_Climbing); // make shift crackhead fix
		}
		return m_Climbing;
	}


	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		transform.Rotate(0f, 180f, 0f);
	}
}