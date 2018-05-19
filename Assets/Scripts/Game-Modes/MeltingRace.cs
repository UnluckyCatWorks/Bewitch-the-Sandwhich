using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Span = System.TimeSpan;

public class MeltingRace : Game 
{
	#region DATA
	[Header ("Game settings")]
	public float roundDuration;
	public Transform isle;
	public Text timer;

	[Header ("Boats settings")]
	public Vector2 boatSpeed;
	public Transform drivers;

	internal Span time;
	public static List<Grabbable> spawnedIngredients = new List<Grabbable> ();
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic () 
	{
		// Start cauldron
		MRCauldron.GetStarted ();

		time = Span.FromSeconds (roundDuration);
		while (time.TotalSeconds > 0f)
		{
			float value = (float)time.TotalSeconds / roundDuration;
			value = Mathf.Pow (value, 0.7f);

			// Hacer la islita pequeña
			float scale = Mathf.Lerp (0.43f, 1f, value);
			isle.localScale = new Vector3 (scale, 1f, scale);

			// Make boats rotate
			float boatSpeed = Mathf.Lerp (this.boatSpeed.x, this.boatSpeed.y, 1-value);
			drivers.Rotate (Vector3.up, -boatSpeed * Time.deltaTime);
			drivers.localScale = isle.localScale;

			yield return null;
			time -= Span.FromSeconds (Time.deltaTime);
			timer.text = string.Format ("{0:0}:{1:00}", time.Minutes, time.Seconds);
		}

		// Declare winner
		int winner = MRCauldron.GetWinner ();
		DeclareWinner (winner);
	}

	public override IEnumerator ResetStage () 
	{
		MRCauldron.scores = new int[2];
		spawnedIngredients.ForEach(i=> { if (i) i.Destroy (); });

		// Turn isle to it's original size
		float factor = 0f;
		while (factor <= 1.1f) 
		{
			float value = Mathf.Pow (factor, 0.6f);
			value = Mathf.Lerp (0.43f, 1f, value);
			isle.localScale = new Vector3 (value, 1f, value);

			// Make boats go and put them in place again
			drivers.localScale = isle.localScale;
			drivers.Rotate (Vector3.up, -boatSpeed.y * (1-value) * Time.deltaTime);

			yield return null;
			factor += Time.deltaTime / /*duration*/ 2f;
		}
		yield return new WaitForSeconds (1f);
	}
	#endregion
}
