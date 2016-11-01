using UnityEngine;
using System.Collections;

public class ExampleEnemy : MonoBehaviour {

	public int maxHp = 10;
	public float resetTime = 5;
	public Vector3 upRotation;
	public Vector3 downRotation;
	public Transform forwardTransform;

	private int hp;

	void Start()
	{
		hp = maxHp;
	}

	public void Damage(DamageInfo info)
	{
		if (hp <= 0) return;
		hp -= info.damage;
		if (hp <= 0) Die();
	}

	void Die()
	{
		StartCoroutine(ToggleRotation(true));
		StartCoroutine(Reset(resetTime));
	}

	IEnumerator ToggleRotation(bool fall)
	{
		float t = 0;
		Quaternion startRotation = transform.rotation;
		//Quaternion targetRotation = fall ? Quaternion.Euler(downRotation) : Quaternion.Euler(upRotation);
		Quaternion targetRotation = fall ? Quaternion.LookRotation(forwardTransform.up, -forwardTransform.forward) : Quaternion.LookRotation(forwardTransform.forward, forwardTransform.up);
		while(t<=1)
		{
			if ((fall && hp > 0) || (!fall && hp <= 0)) yield break;
			transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
			t += 0.01f;
			yield return null;
		}
	}

	IEnumerator Reset(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		hp = maxHp;
		StartCoroutine(ToggleRotation(false));
	}
}
