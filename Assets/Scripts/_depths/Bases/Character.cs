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

	#region LOCOMOTION
	CharacterController me;

	[NonSerialized]
	public Vector3 movingSpeed;
	private void Movement () 
    {
		var input = movingSpeed = Vector3.zero;
		input.x = -Input.GetAxis(GetInputName("Horizontal"));
		input.z = -Input.GetAxis(GetInputName("Vertical"));

		// Get speed & calculate rotation
		var dir = input.normalized;
		if (!locks.HasFlag(Locks.Movement)) movingSpeed = dir * speed;
		if (input != Vector3.zero) targetRotation = Quaternion.LookRotation(dir);
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
	public Grabbable gobj;
	private void HoldGrabbed ()
	{
		if (gobj == null) return;
		var newPos = Vector3.Lerp(gobj.body.position, grabHandle.position, Time.fixedDeltaTime * 7f);
		gobj.body.MovePosition(newPos);
	}
	#endregion

	#region SPRINT
	private float lastSprintTime;
	private bool sprintIsUp 
	{
		get { return Time.time > lastSprintTime + sprintCooldown; }
	}

	private void Sprint () 
	{
		// If sprinting already
		if (effects.ContainsKey("Sprint"))
		{
			// If on sprint-time
			var duration = 0.15f;   /*sprint duration*/
			if (Time.time < lastSprintTime + duration)
			{
				var targetSpeed = movingDir * sprintForce;
				var factor = (Time.time - lastSprintTime) / duration; // [0,1]
				movingSpeed = targetSpeed * (factor + 1f);
			}
			// Otherwise
			else effects.Remove("Sprint");
		}

		// If not sprinting
		else
		{
            if (!sprintIsUp || locks.HasFlag(Locks.Sprint)) return;
			if (!GetButtonDown("Sprint")) return;

			// Start sprint & cooldoown
			AddCC("Sprint", Locks.Locomotion);
			lastSprintTime = Time.time;
		}
	}
	#endregion

	#region INTERACTION
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
					interactable.Mark ( /*check*/ );

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

		// If not in front of any interactable
		// or not executed any interaction
        if (GetButtonDown("Action", true))
        {
			// Throw grabbed object, if any
			if (gobj == null) return;
			gobj.Throw (movingDir, throwForce);
            gobj = null;
        }
	}

	private float lastSpellTime;
	private bool spellIsUp 
	{
		get { return Time.time > lastSpellTime + spellCooldown; }
	}
	Coroutine castingSpell;
	private void CheckSpell () 
	{
		if (!spellIsUp) return;
		if (locks.HasFlag(Locks.Spells)) return;
		if (!GetButtonDown("Special")) return;

		// If everything's ok
		castingSpell = StartCoroutine ( CastSpell () );
		lastSpellTime = Time.time;
	}
	protected abstract IEnumerator CastSpell ();
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

		// Stop casting spell if CC'ed
		if (cc.HasFlag(Locks.Spells) && castingSpell != null)
			StopCoroutine (castingSpell);
	}

	public void AddTemporalEffect (string name, Effect effect, float duration ) 
	{
		effects.Add (name, effect);
		StartCoroutine (RemoveTemporalEffect (name, duration));
	}
	IEnumerator RemoveTemporalEffect (string name, float duration) 
	{
		var startTime = Time.time;
		while (Time.time < startTime + duration) yield return null;
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

		// Locomotion
		Movement();
		Rotation();
		Sprint();
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
		effects = new Dictionary<string, Effect> ();
		consumedInputs = new List<string> ();

		me = GetComponent<CharacterController> ();
		targetRotation = Quaternion.identity;
    }
	#endregion
}
