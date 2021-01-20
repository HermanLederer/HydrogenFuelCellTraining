using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HydrogenInteractables
{
	public class HydrogenFilter : HydrogenInteractable
	{
		public MeshRenderer m_Renderer;

		public bool isInGoodCondition = true;

		private void Update()
		{
			if (!isInGoodCondition)
			{
				m_Renderer.material.color = Color.black;
			}
		}

		public void RandomizeCondition()
		{
			isInGoodCondition = Random.Range(0f, 1f) > 0.5f;
		}
	}
}
