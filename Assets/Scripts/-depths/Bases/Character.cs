using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CONTROLES 
 * - Sprint: SPACE
 * - Coger cosas: J
 * - Lanzar cosas: J (w/o target)
 * - Dejar cosas (en sitios): J
 * - Dejar cosas (en maquinas): J
 * - Habilidad espesial: K
 * - Menú pausa: ESC
 */

public abstract class Character : MonoBehaviour
{
	#region DATA
	[Header("Player info")]
	public int id;
	public ControllerType controller;

	[Header ("Locomotion")]
	public float speed;
	public float angularSpeed;
	public float gravityMul;
	[Space]
	public Transform grabHandle;

	[Header("Skills")]
	public float throwForce;
	[Space]
    public float sprintForce;
	public float sprintCooldown;
	[Space]
	public float spellCooldown;
	#endregion

	#region ANIMATOR STUFF
	private static bool animInit;
	private static void InitAnimator ()
	{
		if (animInit) return;

		CastSpellID = Animator.StringToHash ("CastSpell");
		DashingID = Animator.StringToHash ("Dashing");
		MovingID = Animator.StringToHash ("Moving");

		animInit = true;
	}

	private static int CastSpellID;

	public bool Dashing 
	{
		set { anim.SetBool (DashingID, value); }
	}
	private static int DashingID;

	private bool _moving;
	public bool Moving 
	{
		set
		{
			if (_moving == value) return;
			anim.SetBool (MovingID, value);
			_moving = value;
		}
	}
	private static int MovingID;
	#endregion

	#region LOCOMOTION
	Animator anim;
	CharacterController me;

	[NonSerialized]
	public Vector3 movingSpeed;
	private void Movement () 
    {
		var input = movingSpeed = Vector3.zero;
		input.x = -Input.GetAxis(GetInputName("Horizontal"));
		input.z = -Input.GetAxis(GetInputName("Vertical"));

		// Get speed & calculate rotation
		var dir = Vector3.Min (input, input.normalized);
		if (input != Vector3.zero) targetRotation = Quaternion.LookRotation (dir);
		if (!locks.HasFlag (Locks.Movement))
		{
			movingSpeed = dir * speed;
			if (input != Vector3.zero) Moving = true;
			else Moving = false;
		}
		else Moving = false;
	}

	public Vector3 movingDir 
	{ get { return (movingSpeed == Vector3.zero) ? transform.forward : movingSpeed.normalized; } }
	Quaternion targetRotation;
	private void Rotation () 
    {
		if (locks.HasFlag(Locks.Rotation)) return;

		var factor = Time.deltaTime * angularSpeed;
		var newRot = Quaternion.Slerp(transform.rotation, targetRotation, factor);
		transform.rotation = newRot;
    }

	private void Move () 
	{
		var finalSpeed = movingSpeed + Physics.gravity;
		collision = me.Move (finalSpeed * Time.deltaTime);
	}

	[NonSerialized]
	public Grabbable grab;
	private void HoldGrabbed ()
	{
		if (grab == null) return;
		var newPos = Vector3.Lerp(grab.body.position, grabHandle.position, Time.fixedDeltaTime * 7f);
		grab.body.MovePosition(newPos);
	}
	#endregion

	// ACTUALLY,W sprint and spells are broken
	// cause CD runs even if 'Game.paused' is true

	#region DASHING
	private float lastDashTime;
	private bool dashIsUp 
	{
		get { return Time.time > lastDashTime + sprintCooldown; }
	}

	private void Dash () 
	{
		// If sprinting already
		if (effects.ContainsKey("Dashing"))
		{
			// If on sprint-time
			var duration = 0.15f;   /*sprint duration*/
			if (Time.time < lastDashTime + duration)
			{
				var targetSpeed = movingDir * sprintForce;
				var factor = (Time.time - lastDashTime) / duration; // [0,1]
				movingSpeed = targetSpeed * (factor + 1f);
			}
			// Otherwise
			else
			{
				effects.Remove ("Dashing");
				Dashing = false;
			}
		}

		// If not sprinting
		else
		{
            if (!dashIsUp || locks.HasFlag(Locks.Dash)) return;
			if (!GetButtonDown("Dash")) return;

			// Start sprint & cooldoown
			Dashing = true;
			AddCC("Dashing", Locks.Locomotion);
			lastDashTime = Time.time;
		}
	}
	#endregion

	#region SPELL CASTING
	private float lastSpellTime;
	private bool spellIsUp 
	{
		get { return Time.time > lastSpellTime + spellCooldown; }
	}
	private const float spellSelfStun = 0.75f;

	private void CheckSpell ()
	{
		if (!spellIsUp) return;
		if (locks.HasFlag (Locks.Spells)) return;
		if (!GetButtonDown ("Special")) return;

		// If everything's ok
		var block = (Locks.Locomotion | Locks.Abilities);
		AddCC ("Spell Casting", block, spellSelfStun);
		anim.SetTrigger (CastSpellID);
		lastSpellTime = Time.time;
	}
	protected abstract void CastSpell ();
	#endregion

	#region INTERACTION
	Interactable lastMarked;
	private void CheckInteractions () 
	{
		if (locks.HasFlag(Locks.Interaction)) return;

		var ray = NewRay();
		var hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 1f, 1 << 8))
		{
            var interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
				var check = interactable.CheckInteraction (this);

				// Highlight object
				if (check != PlayerIsAbleTo.None)
				{
					interactable.marker.On ( /*check*/ );
					lastMarked = interactable;
				}

				// Interact
				var action = (check == PlayerIsAbleTo.Action || check == PlayerIsAbleTo.Both);
				if (action && GetButtonDown("Action"))
                    interactable.Action (this);

				// Special action
				var special = (check == PlayerIsAbleTo.Special || check == PlayerIsAbleTo.Both);
				if (special && GetButtonDown("Special"))
					interactable.Special (this);
			}
		}
		else
		{
			// If not in front of any interactable
			// de-mark last one seen, if any
			if (lastMarked) lastMarked.marker.Off ();
			lastMarked = null;
		}

		// If not in front of any interactable
		// or not executed any interaction
        if (GetButtonDown("Action", true))
        {
			// Throw grabbed object, if any
			if (grab == null) return;
			grab.Throw (movingDir, throwForce);
            grab = null;
        }
	}

	#endregion

	#region EFFECTS MANAGEMENT
	Dictionary<string, Effect> effects;
	[NonSerialized] public Locks locks;

	// Applies the effects
	private void ReadEffects () 
	{
		// Reset CCs and then read them
		locks = Locks.NONE;
		foreach (var e in effects)
		{
			locks = locks.SetFlag(e.Value.cc);
		}
	}

	// Helper for only adding CCs
	public void AddCC (string name, Locks cc, float duration=0) 
	{
		var e = new Effect() { cc = cc };

		if (duration == 0) effects.Add (name, e);
		else AddTemporalEffect (name, e, duration);
	}

	public void AddTemporalEffect (string name, Effect effect, float duration ) 
	{
		effects.Add (name, effect);
		StartCoroutine (RemoveTemporalEffect (name, duration));
	}
	IEnumerator RemoveTemporalEffect (string name, float duration) 
	{
		var timer = 0f;
		while (timer < duration)
		{
			// Time doesn't run on effects if game is paused
			if (!Game.paused) timer += Time.deltaTime;
			yield return null;
		}
		effects.Remove (name);
	}
	#endregion

	#region HELPERS
	/// Returns 'Ray' for checking interactions
	private Ray NewRay () 
	{
		/// Generates ray for raycasting
		var origin = transform.position;
		origin.y += 0.75f + 0.15f;
		return new Ray(origin, transform.forward);
	}

	/// Returns where character is colliding from below
	CollisionFlags collision;
	public bool grounded 
	{
		get { return (collision & CollisionFlags.Below) == CollisionFlags.Below; }
	}

	/// For when a object hits a player
	public IEnumerator Knock (float duration, Vector3 force) 
	{
		// TODO
		throw new NotImplementedException ();
	}

	#region SPECIAL INPUT HELPERS
	/* Gets input based on player controller and
	taking into account if it's already been consumed*/

	List<string> consumedInputs;
	public bool GetButtonDown (string button, bool consume=true)
	{
		var input = GetInputName(button);
		// If not consumed, return input value
		if (!consumedInputs.Contains(input))
		{
			var result = Input.GetButtonDown (input);
			if (result && consume) consumedInputs.Add(input);
			return result;
		}
		else return false;
	}

	// Generates correct name
	public string GetInputName (string input)
	{
		var prefix = controller.ToString();
		return prefix + "_" + input;
	}

	private void ResetInputs () { consumedInputs.Clear (); }
	#endregion
	#endregion

	#region UNITY CALLBACKS
	protected virtual void Update () 
    {
		// Initialization
		ResetInputs ();
		ReadEffects ();

		// Stop player if game is paused
		if (Game.paused) return;

		// Locomotion
		Movement();
		Rotation();
		Dash();
		Move();

		// Interaction
		CheckInteractions ();
		CheckSpell ();
	}

	protected virtual void FixedUpdate () 
	{
		HoldGrabbed ();
	}

	protected virtual void Awake () 
    {
		InitAnimator ();

		effects = new Dictionary<string, Effect> ();
		consumedInputs = new List<string> ();

		anim = GetComponent<Animator> ();
		me = GetComponent<CharacterController> ();
		targetRotation = Quaternion.identity;
    }
	#endregion
}
