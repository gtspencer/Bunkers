using UnityEngine;
using System.Collections;

public enum ButtonType
{
	None,
	Prev,
	Next,
	Color_Red,
	Color_Blue,
	Color_Black,
	Color_Yellow
}

public class ButtonManager : MonoBehaviour {

	public Animation m_objAnimation;
	public Material m_objMaterial;
	public AnimationClip[] m_aniClips;
	public Texture[] m_textures;
	public ButtonController[] m_buttons;

	public int m_nowIndex;

	void Awake()
	{
		m_nowIndex = 0;
	}
	
	public void OnButtonClick(ButtonType type)
	{
		switch(type)
		{
		case ButtonType.Prev:
			m_objAnimation.clip = m_aniClips[--m_nowIndex];
			break;
		case ButtonType.Next:
			m_objAnimation.clip = m_aniClips[++m_nowIndex];
			break;
		case ButtonType.Color_Red:
			m_objMaterial.mainTexture = m_textures[0];
			break;
		case ButtonType.Color_Blue:
			m_objMaterial.mainTexture = m_textures[1];
			break;
		case ButtonType.Color_Black:
			m_objMaterial.mainTexture = m_textures[2];
			break;
		case ButtonType.Color_Yellow:
			m_objMaterial.mainTexture = m_textures[3];
			break;
		}
		m_objAnimation.Play ();

		if (m_nowIndex == 0)
			m_buttons [0].ButtonDisable ();
		else if (m_nowIndex == m_aniClips.Length - 1)
			m_buttons [1].ButtonDisable ();

		if (m_nowIndex == 1)
			m_buttons [0].ButtonEnable ();
		else if (m_nowIndex == m_aniClips.Length - 2)
			m_buttons [1].ButtonEnable ();
	}
}
