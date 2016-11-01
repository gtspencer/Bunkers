using UnityEngine;
using System.Collections;

public class VRGunSlide : VRInteractableItem 
{
	public VRGunHandler gunHandler;
	public Vector3 defaultPosition;
	public Vector3 pulledPosition;
	public Quaternion defaultRotation;
	public bool useAsSecondHeld;
	public bool animateSlide = true;

	private bool _pulled = false;
	public bool pulled
	{
		get { return _pulled; }
	}
	private Vector3 orignalControllerPos;
	private bool _active = false;

	void Start () 
	{
		Init();
	}

	void Update()
	{
		Step();
	}

	override protected void Init()
	{
		canBeHeld = false;
		if (hover != null)
		{
			switch(hoverMode)
			{
			case HoverMode.SHADER:
				if (hoverShader == null) hoverShader = Shader.Find("Unlit/Texture");
				if (defaultShader == null) defaultShader = hover.material.shader;
				break;
			case HoverMode.MATERIAL:
				if (hoverMat == null)
				{
					hoverMat = new Material(hover.material);
					hoverMat.shader = Shader.Find("Unlit/Texture");
				}
				if (defaultMat == null) hoverMat = hover.material;
				break;
			}
		}
		if (gunHandler == null)
		{
			Debug.LogError("Gun Handler is null - " + name, gameObject);
			enabled = false;
		}
		item.SetParent(gunHandler.item);
		item.localPosition = defaultPosition;
		item.localRotation = defaultRotation;
	}

	override protected void Step()
	{
		if (!_active) return;
		if (heldBy != null)
		{
			if (!heldBy.ActionPressed("ACTION") && !heldBy.ActionPressed("PICKUP_DROP") && !heldBy.ActionPressed("EJECT"))
			{
				heldBy.Drop();
				return;
			}

			Vector3 rotatedDefaultOffset = defaultRotation*defaultPosition;
			Vector3 scaledDefaultOffset = rotatedDefaultOffset;
			Vector3 heldByLocal = (gunHandler.item.InverseTransformPoint(heldBy.transform.position)-scaledDefaultOffset)-orignalControllerPos;
			item.localPosition = VRUtils.ClosestPointOnLine(defaultPosition, pulledPosition, heldByLocal);
			if (item.localPosition == pulledPosition) Pulled();
			else _pulled = false;
		} else
		{
			if (!_pulled)
			{
				item.localPosition = Vector3.MoveTowards(item.localPosition, defaultPosition, 0.005f/gunHandler.item.localScale.magnitude);
				if (item.localPosition == defaultPosition) _active = false;
			}
		}
	}

	public void Shoot(bool hasBullet)
	{
		if (animateSlide) StartCoroutine(ShootAnimation(hasBullet));
	}

	IEnumerator ShootAnimation(bool hasBullet)
	{
		float t = 0;
		while(item.localPosition != pulledPosition)
		{
			item.localPosition = Vector3.Lerp(defaultPosition, pulledPosition, t);
			t+=0.1f;
			yield return null;
		}

		if (!hasBullet)
		{
			_pulled = true;
			yield break;
		}

		t = 0;
		while(item.localPosition != defaultPosition)
		{
			item.localPosition = Vector3.Lerp(pulledPosition, defaultPosition, t);
			t+=0.1f;
			yield return null;
		}
	}

	private void Pulled()
	{
		if (_pulled) return;
		_pulled = true;
		if (gunHandler != null && gunHandler.slidePulled != null) gunHandler.PlaySound(gunHandler.slidePulled);
		gunHandler.SlidePulled();
	}

	public void Release()
	{
		_pulled = false;
		if (item.localPosition != defaultPosition) _active = true;
		if (gunHandler != null && gunHandler.slideRelease != null) gunHandler.PlaySound(gunHandler.slideRelease);
	}

	override public bool Pickup(VRInteractor hand)
	{
		if (gunHandler.heldBy == null) return false;
		if (useAsSecondHeld) gunHandler.usingSecondHeld = true;
		heldBy = hand;
		Vector3 rotatedDefaultOffset = defaultRotation*defaultPosition;
		Vector3 scaledDefaultOffset = rotatedDefaultOffset;
		orignalControllerPos = gunHandler.item.InverseTransformPoint(heldBy.transform.position)-(scaledDefaultOffset*2);
		_active = true;
		return true;
	}

	override public void Drop(SteamVR_TrackedObject trackedObj)
	{
		if (gunHandler.hasBullet || item.localPosition != pulledPosition)
			Release();
		if (useAsSecondHeld) Drop();
		heldBy = null;
	}

	override public void Drop()
	{
		if (useAsSecondHeld) gunHandler.usingSecondHeld = false;
		base.Drop();
	}
}