using UnityEngine;
using UnityEngine.UIElements;

namespace Mimizh.UnityUtilities
{
    public static class VisualElementExtensions
    {
        public static VisualElement CreateChild(this VisualElement parent, params string[] classes)
        {
            var child = new VisualElement();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        public static T CreateChild<T>(this VisualElement parent, params string[] classes)
            where T : VisualElement, new()
        {
            var child = new T();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement
        {
            parent.Add(child);
            return child;
        }
        
        /// <remarks>
        /// See <see cref="AddTo{T}(T, VisualElement)"/> for adding a child to a parent.
        /// </remarks>
        public static void RemoveFrom<T>(this T child, VisualElement parent)
            where T : VisualElement => parent.Remove(child);

        public static T AddClass<T>(this T visualElement, params string[] classes) where T : VisualElement
        {
            foreach (var @class in classes)
            {
                if (!string.IsNullOrEmpty(@class))
                {
                    visualElement.AddToClassList(@class);
                }
            }
            return visualElement;
        }
        
        public static void RemoveClass<T>(this T visualElement, params string[] classes) where T : VisualElement {
            foreach (string cls in classes) {
                if (!string.IsNullOrEmpty(cls)) {
                    visualElement.RemoveFromClassList(cls);
                }
            }
        }

        public static T WithManipulator<T>(this T visualElement, IManipulator manipulator) where T : VisualElement
        {
            visualElement.AddManipulator(manipulator);
            return visualElement;
        }
        
        /// <summary>
        /// Sets the background image of a VisualElement using a given Sprite.
        /// </summary>
        /// <param name="imageContainer">The VisualElement whose background image will be set.</param>
        /// <param name="sprite">The Sprite to use as the background image.</param>
        public static void SetImageFromSprite(this VisualElement imageContainer, Sprite sprite) {
            var texture = sprite.texture;
            if (texture) {
                imageContainer.style.backgroundImage = new StyleBackground(texture);
            }
        }
    }
}