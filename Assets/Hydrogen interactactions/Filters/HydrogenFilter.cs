using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HydrogenInteractables
{
	public class HydrogenFilter : HydrogenInteractable
	{
		public MeshRenderer m_Renderer;

		[HideInInspector] public float condition;

		private void Start()
		{
			RandomizeCondition();
		}

		private void Update()
		{
			if (condition < 0.5f)
			{
				m_Renderer.material.color = Color.black;
			}
		}

		public void RandomizeCondition()
		{
			condition = Random.Range(0f, 1f);
		}
	}
}
