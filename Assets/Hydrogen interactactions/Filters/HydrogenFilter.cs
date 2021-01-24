using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HydrogenInteractables
{
	public class HydrogenFilter : HydrogenInteractable
	{
		public Material good;
		public Material broken;

		public MeshRenderer m_Renderer;

		public bool isInGoodCondition { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			SetCondition(true);
		}

		public void RandomizeCondition()
		{
			SetCondition(Random.Range(0f, 1f) > 0.5f);
		}

		public void SetCondition(bool condition)
		{
			isInGoodCondition = condition;

			if (isInGoodCondition)
				m_Renderer.material = good;
			else
				m_Renderer.material = broken;
		}
	}
}
