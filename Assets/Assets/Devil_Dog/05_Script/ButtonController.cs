using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {

	public ButtonType m_buttonType;
	public ButtonManager m_manager;

	private BoxCollider m_collider;
	private MeshRenderer m_render;
	private Transform m_myTransform;
	private bool m_isDown;

	void Awake()
	{
		m_myTransform = transform;
		m_collider = GetComponent<BoxCollider>();
		m_render = GetComponent<MeshRenderer>();
		m_isDown = false;
	}

	void OnMouseDown()
	{
		m_myTransform.localScale = new Vector3 (1.2f, 1.2f, 1.0f);
		m_isDown = true;
	}

	void OnMouseUp()
	{
		m_myTransform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		m_isDown = false;
	}

	void OnMouseExit()
	{
		if (m_isDown) {
			m_isDown = false;
			m_myTransform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}
	}

	void OnMouseUpAsButton()
	{
		m_isDown = false;
		m_manager.OnButtonClick (m_buttonType);
	}

	public void ButtonDisable()
	{
		m_collider.enabled = false;
		m_render.enabled = false;
	}

	public void ButtonEnable()
	{
		m_collider.enabled = true;
		m_render.enabled = true;
	}
}
