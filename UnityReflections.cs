using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuisHack
{
	public static class UnityReflections
	{
		public static readonly Func<string, float> InputGetAxis =
					(Func<string, float>)Delegate.CreateDelegate(typeof(Func<string, float>), CachedInputGetAxis);
		private static readonly MethodInfo CachedInputGetAxis;

		public static void Initialize()
		{
			InitializeInputGetAxis();
		}

		private static void InitializeInputGetAxis()
		{
			var type = AccessTools.TypeByName("UnityEngine.Input");
			var CachedInputGetAxis = type.GetMethod("GetAxis");
			
		}
	}
}
