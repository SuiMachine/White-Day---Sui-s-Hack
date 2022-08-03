using System.Collections.Generic;
using UnityEngine;

namespace SuisHack.Components
{
	public class CameraRenderInterpolation : MonoBehaviour
	{
		private static readonly List<Interfaces.IInterpolateGameObject> InterpolatedObjects = new List<Interfaces.IInterpolateGameObject>();

		public static void RegisterObject(Interfaces.IInterpolateGameObject obj)
		{
			if (!InterpolatedObjects.Contains(obj))
				InterpolatedObjects.Add(obj);
		}

		public static void UnregisterObject(Interfaces.IInterpolateGameObject obj)
		{
			if (InterpolatedObjects.Contains(obj))
				InterpolatedObjects.Remove(obj);
		}

		void OnPreRender()
		{
			bool containsNulls = false;
			foreach(var element in InterpolatedObjects)
			{
				if (element == null)
				{
					containsNulls = true;
					continue;
				}

				element.SetInterpolatedPosition();
			}

			if(containsNulls)
			{
				for(int i=InterpolatedObjects.Count-1; i>=0; i--)
				{
					if (InterpolatedObjects[i] == null)
						InterpolatedObjects.RemoveAt(i);
				}
			}
		}

		void OnPostRender()
		{
			foreach (var element in InterpolatedObjects)
			{
				element.RestoreOriginal();
			}
		}
	}
}
