/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */
using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// Abstract base class for implementing a cell of <see cref="FancyScrollView{TItemData, TContext}"/>.
    /// If <see cref="FancyCell{TItemData, TContext}.Context"/> is not needed,
    /// use <see cref="FancyCell{TItemData}"/> instead.
    /// </summary>
    /// <typeparam name="TItemData">The data type of the item.</typeparam>
    /// <typeparam name="TContext">The type of <see cref="Context"/>.</typeparam>
    public abstract class FancyCell<TItemData, TContext> : MonoBehaviour where TContext : class, new()
    {
        /// <summary>
        /// The index of the data displayed by this cell.
        /// </summary>
        public int Index { get; set; } = -1;

        public virtual bool IsVisible => gameObject.activeSelf;

        /// <summary>
        /// Reference to <see cref="FancyScrollView{TItemData, TContext}.Context"/>.
        /// The same instance is shared between the cell and the scroll view. Used for passing information and maintaining state.
        /// </summary>
        protected TContext Context { get; private set; }

        public virtual void SetContext(TContext context) => Context = context;

        /// <summary>
        /// Initializes the cell.
        /// </summary>
        public virtual void Initialize()
        {
        }

        public virtual void SetVisible(bool visible) => gameObject.SetActive(visible);

        /// <summary>
        /// Updates the display content of this cell based on the item data.
        /// </summary>
        /// <param name="itemData">Item data.</param>
        public abstract void UpdateContent(TItemData itemData);

        /// <summary>
        /// Updates this cell's scroll position based on a value between <c>0.0f</c> and <c>1.0f</c>.
        /// </summary>
        /// <param name="position">The normalized scroll position within the viewport range.</param>
        public abstract void UpdatePosition(float position);
    }

    /// <summary>
    /// Abstract base class for implementing a cell of <see cref="FancyScrollView{TItemData}"/>.
    /// </summary>
    /// <typeparam name="TItemData">The data type of the item.</typeparam>
    /// <seealso cref="FancyCell{TItemData, TContext}"/>
    public abstract class FancyCell<TItemData> : FancyCell<TItemData, NullContext>
    {
        public sealed override void SetContext(NullContext context) => base.SetContext(context);
    }
}