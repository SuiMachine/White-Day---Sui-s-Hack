using UnityEngine;

namespace SuisHack.Components.Structs
{
	public struct PositionHistory
	{
		public bool WasActive;
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 LocalScale;
		public float StoredTime;

		public PositionHistory(Transform t)
		{
			WasActive = true;
			this.Position = t.position;
			this.Rotation = t.rotation;
			this.LocalScale = t.localScale;
			this.StoredTime = Time.time;
		}
	}
}
