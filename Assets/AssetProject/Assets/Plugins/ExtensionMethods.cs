using System;
using UnityEngine;

namespace QModManager.Utility
{
    /// <summary>
    /// Various extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Like <see cref="Version.ToString()"/>, but it removes any trailing zeroes
        /// </summary>
        /// <param name="version">The <see cref="Version"/> to parse</param>
        /// <returns>The parsed <see cref="Version"/> as a <see cref="string"/>, with no trailing zeroes</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="version"/> is null</exception>
        public static string ToStringParsed(this Version version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            if (version.Revision == 0)
                if (version.Build == 0)
                    if (version.Minor == 0)
                        return version.ToString(1);
                    else
                        return version.ToString(2);
                else
                    return version.ToString(3);
            else
                return version.ToString(4);
        }

        /// <summary> </summary>
        [Obsolete("Use GameObject.EnsureComponent instead")]
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject)
                throw new ArgumentNullException(nameof(gameObject));
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary> </summary>
        [Obsolete("Use GameObject.EnsureComponent instead")]
        public static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
        {
            if (!behaviour)
                throw new ArgumentNullException(nameof(behaviour));
            if (!behaviour.gameObject)
                throw new NullReferenceException($"The provided component is not attached to a GameObject!");
            return behaviour.gameObject.GetOrAddComponent<T>();
        }

        /// <summary>
        /// Modifies the edge of a <see cref="RectTransform"/>
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="edge">The edge to modify</param>
        /// <param name="amount">The amount to set the edge to</param>
        /// <exception cref="ArgumentException">If <paramref name="edge"/> is not <see cref="RectTransform.Edge.Left"/>, <see cref="RectTransform.Edge.Right"/>, <see cref="RectTransform.Edge.Top"/>, or <see cref="RectTransform.Edge.Bottom"/></exception>
        public static void SetEdge(this RectTransform rt, RectTransform.Edge edge, float amount)
        {
            switch (edge)
            {
                case RectTransform.Edge.Left:
                    rt.offsetMin = new Vector2(amount, rt.offsetMin.y);
                    break;
                case RectTransform.Edge.Right:
                    rt.offsetMax = new Vector2(-amount, rt.offsetMax.y);
                    break;
                case RectTransform.Edge.Top:
                    rt.offsetMax = new Vector2(rt.offsetMax.x, -amount);
                    break;
                case RectTransform.Edge.Bottom:
                    rt.offsetMin = new Vector2(rt.offsetMin.x, amount);
                    break;
                default:
                    throw new ArgumentException("Invalid edge");
            }
        }
    }
}
