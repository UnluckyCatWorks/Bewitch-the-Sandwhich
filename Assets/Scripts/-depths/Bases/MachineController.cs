using UnityEngine;
using UnityEngine.Animations;
using System;
using System.Collections;
using System.Collections.Generic;

[SharedBetweenAnimators]
public class MachineController : StateMachineBehaviour
{
	#region ANIMATOR IDs
	public static bool hashd;
	public static int Start_Working;
	public static int Work_Completed;
	public static int Start_Overheat;
	public static int Start_Overload;
	public static int Pickup;
	#endregion

	[NonSerialized] public MachineState state;		// The current status of the machine
	[NonSerialized] public Animator anim;           // The machine animator
	protected MachineInterface bridge;              // The link with the actual machine parameters
	protected Timer timer;                          // The timer of the machine 

	private enum Theme 
	{
		Waiting,
		InSafeTime,
		Overloading
	}
	private float clock;

	#region CALLBACKS
	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (stateInfo.IsName("Base.Waiting"))
		{
			// Initizalize references
			if (bridge == null) 
			{
				anim = animator;
				bridge = anim.GetComponent<MachineInterface>();
				timer = bridge.GetComponentInChildren<Timer> ();
			}
			if (!hashd)
			{
				// Assign IDs
				Start_Working = Animator.StringToHash("Start_Working");
				Work_Completed = Animator.StringToHash("Work_Completed");
				Start_Overheat = Animator.StringToHash("Start_Overheat");
				Start_Overload = Animator.StringToHash("Start_Overload");
				Pickup = Animator.StringToHash("Pickup");
				hashd = true;
			}
			OnEnterWaiting();
		}
		if (stateInfo.IsName("Base.Working"))		OnEnterWorking ();
		if (stateInfo.IsName("Base.Completed"))		OnEnterCompleted ();
		if (stateInfo.IsName("Base.Overheating"))	OnEnterOverheat ();
		if (stateInfo.IsName("Base.Overloading"))	OnEnterOverload ();

	}

	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		if (stateInfo.IsName("Base.Waiting"))		OnUpdateWaiting ();
		if (stateInfo.IsName("Base.Working"))		OnUpdateWorking ();
		if (stateInfo.IsName("Base.Completed"))		OnUpdateCompleted ();
		if (stateInfo.IsName("Base.Overheating"))	OnUpdateOverheat ();
		if (stateInfo.IsName("Base.Overloading"))	OnUpdateOverload ();

	}

	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		base.OnStateExit(animator, stateInfo, layerIndex);
		if (stateInfo.IsName("Base.Waiting"))		OnExitUnready ();
		if (stateInfo.IsName("Base.Working"))		OnExitWorking ();
		if (stateInfo.IsName("Base.Completed"))		OnExitCompleted ();
		if (stateInfo.IsName("Base.Overheating"))	OnExitOverheat ();
		if (stateInfo.IsName("Base.Overloading"))	OnExitOverload ();
	}
	#endregion

	#region WAITING
	public virtual void OnEnterWaiting () 
	{
		state = MachineState.Waiting;
		timer.ChangeTo (Theme.Waiting);
	}
	public virtual void OnUpdateWaiting () { }
	public virtual void OnExitUnready () { } 
	#endregion

	#region WORKING
	public virtual void OnEnterWorking ()
	{
		state = MachineState.Working;
		clock = 0f;
	}
	public virtual void OnUpdateWorking () 
	{
		if (anim.IsInTransition(0)) return;
		if (clock > bridge.duration)
		{
			// Complete the work if time has finished
			anim.SetTrigger(Work_Completed);
		}
		else
		if (!Game.paused) 
		{
			var factor = clock / bridge.duration;
			timer.SetSlider (factor);
			clock += Time.deltaTime;
		}
	}
	public virtual void OnExitWorking () { } 
	#endregion

	#region COMPLETION
	public virtual void OnEnterCompleted ()
	{
		state = MachineState.Completed;
		timer.ChangeTo (Theme.InSafeTime);
		bridge.ProcessObject();
		clock = 0f;
	}
	public virtual void OnUpdateCompleted () 
	{
		if (anim.IsInTransition(0)) return;
		if (clock > bridge.safeTime) 
		{
			// If time runs out and player hasn't
			// picked up yet, start overheating
			anim.SetTrigger(Start_Overheat);
		}
		else
		if (!Game.paused) clock += Time.deltaTime;
	}
	public virtual void OnExitCompleted () { }
	#endregion

	#region OVERHEAT
	public virtual void OnEnterOverheat ()
	{
		state = MachineState.Overheating;
		clock = 0f;
	}
	public virtual void OnUpdateOverheat ()
	{
		if (anim.IsInTransition(0)) return;
		if (clock > bridge.overheatTime)
		{
			// If time runs out and player hasn't
			// picked up yet, go full overload
			anim.SetTrigger(Start_Overload);
		}
		else
		if (!Game.paused)
		{
			var factor = clock / bridge.overheatTime;
			timer.SetSlider (factor);
			clock += Time.deltaTime;
		}
	}
	public virtual void OnExitOverheat () { timer.ChangeTo (Theme.Overloading); } 
	#endregion

	#region OVERLOAD
	public virtual void OnEnterOverload ()
	{
		// Lock outbox again
		state = MachineState.Overloading;
		bridge.Overload();
	}
	public virtual void OnUpdateOverload () { }
	public virtual void OnExitOverload () { }  
	#endregion
}
