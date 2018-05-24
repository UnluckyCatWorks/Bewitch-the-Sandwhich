using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public abstract class Character : Pawn
{
	#region DATA
	[Header ("Basic settings")]
	public Transform grabHandle;
	public Characters ID;
	public Sprite avatar;
	public Color focusColor;
	public float crystalEmission;

	[Header ("Spell settings")]
	[ColorUsage(true, true, 0, 8, 0.125f, 3)]
	public Color areaColor;
	public float spellCooldown;

	// Internal info
	protected Dictionary<string, Locks> effects;
	internal Locks locks;

	protected SmartAnimator anim;
	protected CharacterController me;
	protected CharacterSFX sound;

	protected Marker areaOfEffect;
	protected SphereCollider areaCollider;

	internal Material mat;
	internal int _EmissionColor;

	// Specials
	internal Character other;
	internal bool simulateCarrying;
	private static Character[] cache = new Character[4];

	// Locomotion
	internal float speed = 8.5f;
	internal float angularSpeed = 120f;
	internal float gravityMul = 1f;

	// Control
	protected Quaternion targetRotation;
	internal Vector3 movingSpeed;
	internal Vector3 MovingDir 
	{
		get
		{ return (movingSpeed == Vector3.zero)?
				transform.forward : movingSpeed.normalized; }
	}

	internal CollisionFlags collision;
	private Vector3 lastAlivePos;

	// Capabilities
	internal static float ThrowForce = 30f;

	internal static float DashForce = 40f;
	internal static float DashCooldown = 0.50f;
	protected const float DashDuration = 0.25f;
	protected Coroutine dashCoroutine;

	internal bool knocked;
	internal Coroutine knockedCoroutine;

	protected const float spellSelfStun = 0.50f;
	protected SpellResult spellResult;

	protected Interactable lastMarked;
	internal Grabbable toy;

	// Animation
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
	public bool Casting 
	{
		get { return anim.GetBool ("Casting_Spell"); }
		set { anim.SetBool ("Casting_Spell", value); }
	}
	public bool Carrying 
	{
		get { return anim.GetBool ("Carrying_Stuff"); }
		set { anim.SetBool ("Carrying_Stuff", value); }
	}
	#endregion

	#region LOCOMOTION
	private void Movement () 
	{
		// Get input
		var input = Vector3.zero;
		input.x = Owner.GetAxis ("Horizontal");
		input.z = Owner.GetAxis ("Vertical");

		// Compute rotation equivalent to moving direction
		var dir = Vector3.Min (input, input.normalized);
		if (input != Vector3.zero)
		{
			// Transform direction to be camera-dependent
			dir = TranformToCamera (dir);
			targetRotation = Quaternion.LookRotation (dir);
		}

		// If burning
		if (locks.HasFlag (Locks.Burning))
		{
			// Can't stop moving
			Moving = true;
			float speedMul = Bobby.SpeedMultiplier * speed;

			if (input != Vector3.zero)	movingSpeed = dir * speedMul;
			else						movingSpeed = MovingDir * speedMul;
		}
		else
		// Modify speed to move character
		if (!locks.HasFlag (Locks.Movement))
		{
			if (input != Vector3.zero)
			{
				movingSpeed = dir * speed;
				Moving = true;
			}
			else
			{
				// Ice is resbalizou
				//if (Game.manager is MeltingRace) movingSpeed = Vector3.Lerp (movingSpeed, Vector3.zero, Time.deltaTime * 0.5f);
				// :(((

				movingSpeed = Vector3.zero;
				Moving = false;
			}
		}
		else Moving = false;
	}

	private void Rotation () 
	{
		if (locks.HasFlag (Locks.Rotation)) return;

		// Rotate character towards moving directions
		var factor = angularSpeed * Time.deltaTime;
		var newRot = Quaternion.Slerp (transform.rotation, targetRotation, factor);
		transform.rotation = newRot;
	}

	// Actual movement is held here
	private void Move () 
	{
		// Apply gravity
		var gravity = Physics.gravity * gravityMul;
		var finalSpeed = movingSpeed + gravity;

		// If bewitched
		if (locks.HasFlag (Locks.Crazy))
		{
			// Invert speed
			finalSpeed.x *= -1;
			finalSpeed.z *= -1;
		}

		// Move player
		collision = me.Move (finalSpeed * Time.deltaTime);
		TrackPosition ();
	}
	#endregion

	#region DEATH TRACKING
	private void TrackPosition () 
	{
		if (collision.HasFlag (CollisionFlags.Below))
			lastAlivePos = transform.position;
	}

	public void Respawn () 
	{
		// Find a viable position on the Nav Mesh
		NavMeshHit hit;
		float radius = 2f;
		while (!NavMesh.SamplePosition (lastAlivePos, out hit, radius, NavMesh.AllAreas)) 
		{
			radius += 0.5f;
			if (radius > 4f) throw new System.Exception ("wtf nav mesh");
		}
		// Teleport to place
		transform.position = hit.position;

		// Notify score
		if (Game.manager is MeltingRace) 
			Owner.ranking[MeltingRace.Scores.Deaths]++;
		else
		if (Game.manager is WizardWeather) 
			Owner.ranking[WizardWeather.Scores.Deaths]++;

		#region UNUSED
		/*
// Disable spells individually
if (effects.ContainsKey ("Spell: Stoned")) 
{
	Get (Characters.Milton).StopCoroutine (Milton.stoneConversion);
	other.mat.SetFloat ("_StoneLevel", 0f);
	effects.Remove ("Spell: Stoned");
}
else
if (effects.ContainsKey ("Spell: Crazy")) 
{
	(Get (Characters.Amy) as Amy).madnessVFX.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	effects.Remove ("Spell: Crazy");
}
else
if (effects.ContainsKey ("Spell: Burnt")) 
{
	(Get (Characters.Bobby) as Bobby).effectInstance.Stop (true, ParticleSystemStopBehavior.StopEmitting);
	effects.Remove ("Spell: Burnt");
}
*/
		#endregion
	}
	#endregion

	#region TOY
	// Keeps toy with the character
	private void HoldToy () 
	{
		if (simulateCarrying)
		{
			Carrying = true;
			return;
		}
		else
		if (toy == null) 
		{
			Carrying = false;
			return;
		}
		else Carrying = true;

		// Make toy follow smoothly
		var newPos = Vector3.Lerp (toy.body.position, grabHandle.position, Time.fixedDeltaTime * 7f);
		toy.body.MovePosition (newPos);
	}
	#endregion

	#region DASHING
	private void Dash () 
	{
		if (locks.HasFlag (Locks.Dash)) return;         // Is Dash up?
		else if (!Owner.GetButton ("Dash")) return;     // Has user pressed the button?
//		else if (toy) return;                           // Can't dash while holding stuff
		else if (Dashing) return;						// Can't dash if already dashing
		// If everything's ok
		else Dashing = true;

		// Start dash & put in cooldoown
		AddCC ("Dashing", Locks.Locomotion);
		dashCoroutine = StartCoroutine (InDash ());
	}

	private IEnumerator InDash () 
	{
		gravityMul = 0.6f;

		float factor = 0f;
		bool knockOcurred = false;
		while (factor <= 1.1f) 
		{
			// Move player at dash speed (slow as closer to end)
			movingSpeed = MovingDir * DashForce * (1f - factor);

			// Knock other character back if hit
			var dist = Vector3.Distance (transform.position, other.transform.position);
			if (dist <= 0.8 && !knockOcurred)
			{
				// Get force from movement & supress Y-force
				other.Knock (MovingDir, 0.25f);

				// Update score
				if (Game.manager is WetDeath)
					Owner.ranking[WetDeath.Scores.DashHits]++;
				else
				if (Game.manager is WizardWeather)
					Owner.ranking[WizardWeather.Scores.DashHits]++;

				// Hard-slow dash & avoid knocking again
				knockOcurred = true;
				factor = 0.6f;
			}

			factor += Time.deltaTime / DashDuration;
			yield return null;
		}
		// Reset
		RemoveCC ("Dashing");
		Dashing = false;

		// Restore gravity
		gravityMul = 1f;
	}
	#endregion

	#region KNOCKING
	public void Knock (Vector3 dir, float duration) 
	{
		// Only add CC if wasn't already knocked
		if (!effects.ContainsKey ("Knocked")) AddCC ("Knocked", Locks.All, interrupt: Locks.Dash | Locks.Spells);
		else if (knockedCoroutine != null) StopCoroutine (knockedCoroutine);

		anim.SetTrigger ("Hit");
		knockedCoroutine = StartCoroutine (KnockingTo (dir, duration));

		// Let go grabbed object, if any,
		// in opposite direction of knock
		if (toy) toy.Throw (-dir * 5f, owner: this);
	}

	private IEnumerator KnockingTo (Vector3 dir, float duration) 
	{
		// Supress vertical force
		dir.y = 0f;

		var factor = 0f;
		while (factor <= 1f)
		{
			// Move player during knock
			movingSpeed = dir * DashForce * (1f - factor);

			// Rotate player 'cause its cool
			transform.Rotate (Vector3.up, 771f * Time.deltaTime);
			targetRotation = transform.rotation;

			factor += Time.deltaTime / duration;
			yield return null;
		}
		yield return new WaitForSeconds (0.1f);

		// Reset
		effects.Remove ("Knocked");
		knocked = false;
	}
	#endregion

	#region SPELL CASTING
	private void CheckSpell () 
	{
		if (locks.HasFlag (Locks.Spells)) return;
		if (!Owner.GetButton ("Spell")) return;
		if (toy) return;
		// If everything's ok
		else Casting = true;
		var block = (Locks.Locomotion | Locks.Interaction);
		AddCC ("Spell Casting", block, Locks.NONE, spellSelfStun);

		// Self CC used as cooldown
		AddCC ("-> Spell", Locks.Spells);
		SwitchCrystal (value: false);

		// Cast spell & put it on CD
		StartCoroutine (CastSpell ());
		StartCoroutine (WaitSpellCD ());
	}

	private IEnumerator CastSpell () 
	{
		areaOfEffect.On (areaColor * 0.5f);						// Show area
		yield return new WaitForSeconds (spellSelfStun);		// Allow spell aiming while self-stunned
		areaOfEffect.Off ();									// Hide area

		// Start spell coroutine
		StartCoroutine (SpellEffect ());
		Casting = false;

		// If other player was hit
		var hits = Physics.OverlapSphere (areaOfEffect.transform.position, areaCollider.radius, 1<<14);
		if (hits.Any (c=> c.name == other.name))
		{
			// Notify that spell was a hit
			spellResult = SpellResult.Hit;
			Tutorial.SetCheckFor (ID, Tutorial.Phases.Casting_Spells, true);
		}
		else spellResult = SpellResult.Missed;
	}
	protected abstract IEnumerator SpellEffect ();

	private IEnumerator WaitSpellCD () 
	{
		// Just wait until CD is over
		yield return new WaitForSeconds (spellCooldown);
		SwitchCrystal (value: true);
		spellResult = SpellResult.Undefined;
		RemoveCC ("-> Spell");
	}

	public void SwitchCrystal (bool value) 
	{
		var color = Color.white * (value? crystalEmission : 0f);
		mat.SetColor (_EmissionColor, color);
	}
	#endregion

	#region INTERACTION
	private void CheckInteractions () 
	{
		if (locks.HasFlag (Locks.Interaction)) return;

		var ray = NewRay ();
		var hit = new RaycastHit ();
		if (Physics.Raycast (ray, out hit, 2f, 1 << 8 | 1 << 10))
		{
			// If valid target
			var interactable = hit.collider.GetComponent<Interactable> ();
			if (interactable && interactable.CheckInteraction (this))
			{
				if (!lastMarked)
				{
					// Focus object
					interactable.marker.On (focusColor);
					lastMarked = interactable;

					// Register player if it's a machine
					var m = interactable as MachineInterface;
					if (m) m.PlayerIsNear (near: true);
				}
				if (Owner.GetButton ("Action")) interactable.Action (this);
				return;
			}
		}

		// If not in front of any interactable
		// de-mark last one seen, if any
		if (lastMarked)
		{
			// Un-register player if it's a machine
			var m = lastMarked as MachineInterface;
			if (m) m.PlayerIsNear (near: false);

			// Un-focus
			lastMarked.marker.Off (focusColor);
			lastMarked = null;
		}

		// If not in front of any interactable,
		// or not executed any interaction
		if (Owner.GetButton ("Action", true) && toy)
		{
			// If other is infront, throw right towards
			var otherDir = (other.transform.position - transform.position).normalized;
			float angle = Vector3.Angle (otherDir, MovingDir);

			var throwDir = angle <= 50f?
				otherDir : MovingDir;

			toy.Throw (throwDir * ThrowForce, owner: this);
		}
	}
	#endregion

	#region EFFECTS MANAGEMENT
	private void ReadEffects () 
	{
		// Check if spells were originally blocked
		bool spellsWereBlocked = locks.HasFlag (Locks.Spells);

		locks = Locks.NONE;
		foreach (var e in effects)
		{
			// Resets CCs & then reads them every frame
			locks = locks.SetFlag (e.Value);
		}

		// Check if spells are NOW originally blocked
		bool spellsAreBlocked = locks.HasFlag (Locks.Spells);
		if (spellsWereBlocked && !spellsAreBlocked)
			SwitchCrystal (value: true);
	}

	// Helper for only adding CCs
	public void AddCC (string name, Locks cc, Locks interrupt = Locks.NONE, float duration = 0)
	{
		if (!effects.ContainsKey (name)) effects.Add (name, cc);
		if (duration != 0) StartCoroutine (RemoveEffectAfter (name, duration));

		// Interrupt any kind of movement
		if (cc.HasFlag (Locks.Movement))
		{
			movingSpeed = Vector3.zero;
			// Interrupt dash
			if (interrupt.HasFlag (Locks.Dash) && Dashing)
			{
				// Reset dashing
				RemoveCC ("Dashing");
				Dashing = false;
				gravityMul = 1f;

				if (dashCoroutine != null) 
				{
					StopCoroutine (dashCoroutine);
					dashCoroutine = null; 
				}
			}
		}

		// If spells are prevented, turn off Crystal
		if (cc.HasFlag(Locks.Spells)) 
		{
			SwitchCrystal (value: false);

			// Interrupt spell casting
			if (interrupt.HasFlag (Locks.Spells) && Casting)
				Casting = false;
		}

		// If interaction is prevented, de-mark last interactable
		if (cc.HasFlag(Locks.Interaction) && lastMarked)
		{
			// Un-register player if it's a machine
			var m = lastMarked as MachineInterface;
			if (m) m.PlayerIsNear (near: false);

			// Un-focus
			lastMarked.marker.Off (focusColor);
			lastMarked = null;
		}
	}

	public void RemoveCC (string name) 
	{
		if (effects.ContainsKey (name))
			effects.Remove (name);
	}

	// Internal helper for temporal CCs
	IEnumerator RemoveEffectAfter (string name, float delay) 
	{
		yield return new WaitForSeconds (delay);
		RemoveCC (name);
	}
	#endregion

	#region HELPERS
	private Vector3 TranformToCamera (Vector3 dir) 
	{
		// Get coorrect camera-dependent vector
		var cam = Camera.main.transform;
		// Ignore rotation (except Y) 
		var rot = cam.eulerAngles;
		rot.x = 0;
		rot.z = 0;
		#warning really bad practice man
		return Matrix4x4.TRS (cam.position, Quaternion.Euler (rot), Vector3.one).MultiplyVector (dir);
	}

	private Ray NewRay () 
	{
		// Returns 'Ray' for checking interactions
		var origin = transform.position;
		origin.y += 0.75f + 0.15f;
		return new Ray (origin, transform.forward);
	}

	public static List<Character> SpawnPack () 
	{
		// Spawn Players' Characters
		var list = new List<Character>
		{
			Instantiate(Resources.Load<Character>("Prefabs/Characters/" + Player.Get(1).playingAs)),
			Instantiate(Resources.Load<Character>("Prefabs/Characters/" + Player.Get(2).playingAs)),
		};

		// Correct instances
		var startPoints = Game.Get<Transform> ("Start_", false);
		for (int p=0; p!=list.Count; p++)
		{
			list[p].name = list[p].name.Replace ("(Clone)", string.Empty);
			list[p].transform.position = startPoints[p].position;
			list[p].transform.rotation = startPoints[p].rotation;
		}

		// Assign them their owners
		list[0].ownerID = 1;
		list[1].ownerID = 2;
		// Assing other
		list[0].other = list[1];
		list[1].other = list[0];

		// Return
		return list;
	}

	public void FindOther () 
	{
		var list = FindObjectsOfType<Character> ().ToList ();
		// Find first other character in the scene
		if (list != null && list.Count > 1)
			other = list.FirstOrDefault (c=> c != this);
	}

	public static Character Get (Characters character) 
	{
		int id = (int) character - 1;
		if (cache[id] == null) 
		{
			var p = GameObject.Find (character.ToString ());
			cache[id] = p.GetComponent<Character> ();
		}
		return cache[id];
	}
	#endregion

	#region UNITY CALLBACKS
	protected virtual void Update () 
	{
		if (Owner == null)
			return;

		// Initialization
		Owner.ResetInputs ();
		ReadEffects ();

		if (Game.stopped) 
		{
			Moving = false;
			return;
		}

		// Locomotion
		Movement ();
		Rotation ();
		Dash ();
		Move ();

		// Interaction
		CheckInteractions ();
		CheckSpell ();
	}

	protected virtual void Awake () 
	{
		// Initialize stuff
		anim = new SmartAnimator (GetComponent<Animator> ());
		effects = new Dictionary<string, Locks> ();

		// Initialize crystal
		mat = GetComponentInChildren<Renderer> ().sharedMaterial;
		_EmissionColor = Shader.PropertyToID ("_EmissionColor");
		SwitchCrystal (value: true);

		// Get some references
		me = GetComponent<CharacterController> ();
		areaOfEffect = GetComponentInChildren<Marker> ();
		areaCollider = areaOfEffect.GetComponent<SphereCollider> ();
		targetRotation = transform.rotation;

		// Find other playera
		FindOther ();

		// Spawn Audio prefab
		var prefab = Resources.Load<CharacterSFX> ("Prefabs/Audio/Character_SFX");
		sound = Instantiate (prefab, transform);
	}

	protected virtual void FixedUpdate () 
	{
		HoldToy ();
	}
	#endregion
}
