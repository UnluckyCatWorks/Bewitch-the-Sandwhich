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

[SelectionBase]
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

	/// Death avoiding
	internal Vector3 lastAlivePos;
	#endregion

	#region ANNIMATION
	SmartAnimator anim;

	public bool Moving 
	{
		get { return anim.GetBool ("Moving"); }
		set { anim.SetBool ("Moving", value); }
	}
	public bool Dashing 
	{
		get { return anim.GetBool ("Dashing"); }
		set { anim.SetBool ("Dashing", value); }
	}
	public bool Carrying 
	{
		get { return anim.GetBool ("Carrying_Stuff"); }
		set { anim.SetBool ("Carrying_Stuff", value); }
	}
	#endregion

	#region LOCOMOTION
	protected CharacterController me;

	[NonSerialized]
	public Vector3 movingSpeed;
	private void Movement () 
    {
		var input = Vector3.zero;
		input.x = Input.GetAxis(GetInputName("Horizontal"));
		input.z = Input.GetAxis(GetInputName("Vertical"));

		/// Get speed & calculate rotation
		var dir = Vector3.Min (input, input.normalized);

		/// Face player to movement direction
		if (input != Vector3.zero)
		{
			/// Transform direction to be camera-dependent
			dir = TranformToCamera (dir);
			targetRotation = Quaternion.LookRotation (dir);
		}

		/// Modify speed to move player
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

		var factor = angularSpeed * Time.deltaTime;
		var newRot = Quaternion.Slerp(transform.rotation, targetRotation, factor);
		transform.rotation = newRot;
    }

	private void Move () 
	{
		/// Apply gravity
		var gravity = Physics.gravity * gravityMul;
		var finalSpeed = movingSpeed + gravity;
		/// Move player
		collision = me.Move (finalSpeed * Time.deltaTime);
	}

	[NonSerialized]
	public Grabbable grab;
	private void HoldGrabbed () 
	{
		if (grab == null)
		{
			Carrying = false;
			return;
		}
		else Carrying = true;

		var newPos = Vector3.Lerp(grab.body.position, grabHandle.position, Time.fixedDeltaTime * 7f);
		grab.body.MovePosition(newPos);
	}
	#endregion

	// ACTUALLY, sprint and spells are broken
	// cause CD runs even if 'Game.paused' is true
	// (I'm guessing)

	#region DASHING
	private float lastDashTime;
	private bool dashIsUp 
	{
		get { return Time.time > lastDashTime + sprintCooldown; }
	}

	private void Dash () 
	{
		if (grab) return;
		if (!dashIsUp || locks.HasFlag(Locks.Dash)) return;
		else if (!GetButtonDown("Dash")) return;
		else Dashing = true;

		/// Start dash & cooldoown
		AddCC("Dashing", Locks.Locomotion);
		StartCoroutine (DashingTime ());
		lastDashTime = Time.time;
	}

	protected const float dashDuration = 0.25f;
	private IEnumerator DashingTime () 
	{
		/// Disable graivty
		gravityMul = 0.8f;

		var factor = 0f;
		while (factor <= 1f)
		{
			/// Move player at dash speed
			movingSpeed = movingDir * sprintForce * (1f-factor);

			/// Knock player back if needed
			var dist = Vector3.Distance (transform.position, other.transform.position);
			if (dist <= 0.8)
			{
				/// If knock, stop dashing
				other.Knock (movingDir);
				effects.Remove ("Dashing");
				factor = 1f;
			}

			factor += Time.deltaTime / dashDuration;
			yield return null;
		}
		effects.Remove ("Dashing");
		Dashing = false;

		/// Re-enable graivty
		gravityMul = 1f;
	}
	#endregion

	#region KNOCKING
	protected Character other;
	public bool Knocked { get; private set; }

	public void Knock (Vector3 dir)
	{
		if (Knocked) return;
		else Knocked = true;

		AddCC ("Knocked", Locks.All);
		StartCoroutine (KnockingTo (dir));
		/// Let go grabbed object, if any
		if (grab) grab.Throw (-dir, 2f, null);
		grab = null;
	}

	protected const float knockDuration = 0.35f;
	private IEnumerator KnockingTo (Vector3 dir) 
	{
		/// Supress vertical knock
		dir.y = 0f;

		var factor = 0f;
		while (factor <= 1f) 
		{
			/// Move player during knock
			movingSpeed = dir * sprintForce * (1f-factor) * 0.8f;

			/// Rotate player 'cause its cool
			transform.Rotate (Vector3.up, 771f * Time.deltaTime);
			targetRotation = transform.rotation;

			factor += Time.deltaTime / knockDuration;
			yield return null;
		}
		yield return new WaitForSeconds (0.1f);
		effects.Remove ("Knocked");
		Knocked = false;
	}
	#endregion

	#region SPELL CASTING
	private float lastSpellTime;
	private bool spellIsUp 
	{
		get { return Time.time > lastSpellTime + spellCooldown; }
	}
	protected const float spellSelfStun = 0.75f;

	private void CheckSpell ()
	{
		if (grab) return;
		if (!spellIsUp) return;
		if (locks.HasFlag (Locks.Spells)) return;
		if (!GetButtonDown ("Special")) return;

		// If everything's ok
		var block = (Locks.Locomotion | Locks.Abilities);
		AddCC ("Spell Casting", block, spellSelfStun);

		anim.SetTrigger ("Cast_Spell");
		StartCoroutine (CastSpell ());
		lastSpellTime = Time.time;
	}
	protected abstract IEnumerator CastSpell ();
	#endregion

	#region INTERACTION
	Interactable lastMarked;
	private void CheckInteractions () 
	{
		if (locks.HasFlag(Locks.Interaction)) return;

		var ray = NewRay();
		var hit = new RaycastHit ();
		if (Physics.Raycast (ray, out hit, 2f, 1<<8 | 1<<10))
		{
			Interactable interactable;
			/// Collider is on a child in grabbable objects
			if (hit.collider.tag == "Grab_Helper")
				interactable = hit.collider.GetComponentInParent<Interactable> ();
			else
				interactable = hit.collider.GetComponent<Interactable>();

			/// If valid target
            if (interactable && interactable.CheckInteraction (this))
            {
				if (!lastMarked)
				{
					/// Hightlight object
					interactable.marker.On (id);
					lastMarked = interactable;
					/// Register player if it's a machine
					var m = interactable as MachineInterface;
					if (m) m.PlayerIsNear (true);
				}
				if (GetButtonDown ("Action")) interactable.Action (this);
				return;
			}
		}

		/// If not in front of any interactable
		/// de-mark last one seen, if any
		if (lastMarked)
		{
			/// Un-register player if it's a machine
			var m = lastMarked as MachineInterface;
			if (m) m.PlayerIsNear (false);
			/// Un-highlight
			lastMarked.marker.Off (id);
			lastMarked = null;
		}

		/// If not in front of any interactable
		/// or not executed any interaction
        if (GetButtonDown("Action", true))
        {
			/// Throw grabbed object, if any
			if (grab == null) return;
			grab.Throw (movingDir, throwForce, this);
            grab = null;
        }
	}

	#endregion

	#region EFFECTS MANAGEMENT
	protected Dictionary<string, Effect> effects;
	internal Locks locks;

	/// Applies the effects
	private void ReadEffects () 
	{
		/// Reset CCs and then read them
		locks = Locks.NONE;
		foreach (var e in effects)
		{
			locks = locks.SetFlag(e.Value.cc);
		}
	}

	/// Helper for only adding CCs
	public void AddCC (string name, Locks cc, float duration=0) 
	{
		var e = new Effect() { cc = cc };

		if (duration != 0) StartCoroutine (RemoveEffectAfter (name, duration));
		effects.Add (name, e);

		if (cc.HasFlag (Locks.Movement)) movingSpeed = Vector3.zero;
	}

	/// Manually remove a CC effect
	public void RemoveCC (string name) 
	{
		if (effects.ContainsKey (name))
			effects.Remove (name);
	}

	IEnumerator RemoveEffectAfter (string name, float duration) 
	{
		var timer = 0f;
		while (timer < duration)
		{
			/// Time doesn't run on effects if game is paused
			if (!Game.paused) timer += Time.deltaTime;
			yield return null;
		}
		effects.Remove (name);
	}
	#endregion

	#region HELPERS
	/// Get coorrect camera-dependent vector
	private Vector3 TranformToCamera (Vector3 dir) 
	{
		var cam = Camera.main.transform;
		/// Ignoring rotation (except Y) 
		var rot = cam.eulerAngles;
		rot.x = 0;
		rot.z = 0;
		return Matrix4x4.TRS (cam.position, Quaternion.Euler (rot), Vector3.one).MultiplyVector (dir);
	}

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
		/// Initialization
		ResetInputs ();
		ReadEffects ();

		/// Stop player if game is paused
		if (Game.paused)
		{
			Moving = false;
			return;
		}

		/// Locomotion
		Movement();
		Rotation();
		Dash();
		Move ();

		/// Interaction
		CheckInteractions ();
		CheckSpell ();
	}

	protected virtual void Awake () 
    {
		/// Find other player
		other = FindObjectsOfType<Character> ().First (c=> c != this);

		effects = new Dictionary<string, Effect> ();
		consumedInputs = new List<string> ();

		anim = new SmartAnimator ( GetComponent<Animator> () );
		me = GetComponent<CharacterController> ();
		targetRotation = transform.rotation;
    }

	protected virtual void FixedUpdate () 
	{
		HoldGrabbed ();
	}

	private void OnEnable () 
	{
		/// Start avoiding death
		StartCoroutine (DeathSaving ());
	}
	#endregion

	#region DEATH
	IEnumerator DeathSaving () 
	{
		float rate = 2f;
		float clock = 0f;

		while (true)
		{
			if (clock > rate && collision.HasFlag(CollisionFlags.Below))
			{
				lastAlivePos = transform.position;
				clock = 0f;
			}
			yield return null;
			clock += Time.deltaTime;
		}
	}
	#endregion
}
