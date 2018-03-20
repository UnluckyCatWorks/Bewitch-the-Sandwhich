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
	[NonSerialized] public Animator anim;			// The machine animator
	protected MachineInterface bridge;              // The link with the actual machine parameters

	private enum Theme 
	{
		Waiting,
		InSafeTime,
		Overloading
	}

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
	public virtual void OnEnterWaiting () { state = MachineState.Waiting; }
	public virtual void OnUpdateWaiting () { }
	public virtual void OnExitUnready () { } 
	#endregion

	#region WORKING
	private float workStartTime;
	public virtual void OnEnterWorking ()
	{
		workStartTime = Time.time;
		state = MachineState.Working;
	}
	public virtual void OnUpdateWorking ()
	{
		if (anim.IsInTransition(0)) return;
		if (Time.time >= workStartTime + bridge.duration)
		{
			// Complete the work if time has finished
			anim.SetTrigger(Work_Completed);
		}
	}
	public virtual void OnExitWorking () { } 
	#endregion

	#region COMPLETION
	private float completionTime;
	public virtual void OnEnterCompleted ()
	{
		completionTime = Time.time;
		state = MachineState.Completed;
		bridge.ProcessObject();
	}
	public virtual void OnUpdateCompleted () 
	{
		if (anim.IsInTransition(0)) return;
		if (Time.time >= completionTime + bridge.safeTime)
		{
			// If time runs out and player hasn't
			// picked up yet, start overheating
			anim.SetTrigger(Start_Overheat);
		}
	}
	public virtual void OnExitCompleted () { }
	#endregion

	#region OVERHEAT
	private float overheatStartTime;
	public virtual void OnEnterOverheat ()
	{
		overheatStartTime = Time.time;
		state = MachineState.Overheating;
	}
	public virtual void OnUpdateOverheat ()
	{
		if (anim.IsInTransition(0)) return;
		if (Time.time >= overheatStartTime + bridge.overheatTime)
		{
			// If time runs out and player hasn't
			// picked up yet, go full overload
			anim.SetTrigger(Start_Overload);
		}
	}
	public virtual void OnExitOverheat () { } 
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
