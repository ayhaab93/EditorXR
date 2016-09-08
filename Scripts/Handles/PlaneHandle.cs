﻿using UnityEngine.EventSystems;
using UnityEngine.VR.Modules;
using UnityEngine.VR.Utilities;

namespace UnityEngine.VR.Handles
{
	public class PlaneHandle : BaseHandle
	{
		private class PlaneHandleEventData : HandleEventData
		{
			public Vector3 raycastHitWorldPosition;

			public PlaneHandleEventData(Transform rayOrigin, bool direct) : base(rayOrigin, direct) { }
		}

		[SerializeField]
		private Material m_PlaneMaterial;

		private const float kMaxDragDistance = 1000f;

		private Plane m_Plane;
		private Vector3 m_LastPosition;

		protected override HandleEventData GetHandleEventData(RayEventData eventData)
		{
			return new PlaneHandleEventData(eventData.rayOrigin, U.Input.IsDirectEvent(eventData)) { raycastHitWorldPosition = eventData.pointerCurrentRaycast.worldPosition };
		}

		protected override void OnHandleBeginDrag(HandleEventData eventData)
		{
			var planeEventData = eventData as PlaneHandleEventData;
			m_LastPosition = planeEventData.raycastHitWorldPosition;

			m_Plane.SetNormalAndPosition(transform.forward, transform.position);

			base.OnHandleBeginDrag(eventData);
		}

		protected override void OnHandleDrag(HandleEventData eventData)
		{
			Transform rayOrigin = eventData.rayOrigin;

			var worldPosition = m_LastPosition;

			float distance;
			Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
			if (m_Plane.Raycast(ray, out distance))
				worldPosition = ray.GetPoint(Mathf.Min(Mathf.Abs(distance), kMaxDragDistance));

			var deltaPosition = worldPosition - m_LastPosition;
			m_LastPosition = worldPosition;

			deltaPosition = transform.InverseTransformVector(deltaPosition);
			deltaPosition.z = 0;
			deltaPosition = transform.TransformVector(deltaPosition);
			eventData.deltaPosition = deltaPosition;

			base.OnHandleDrag(eventData);
		}
	}
}