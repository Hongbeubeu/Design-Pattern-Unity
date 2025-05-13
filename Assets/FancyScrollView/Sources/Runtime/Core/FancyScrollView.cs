/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */
using System.Collections.Generic;
using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// Abstract base class for implementing a scroll view.
    /// Supports infinite scrolling and snapping.
    /// If <see cref="FancyScrollView{TItemData, TContext}.Context"/> is not needed,
    /// use <see cref="FancyScrollView{TItemData}"/> instead.
    /// </summary>
    /// <typeparam name="TItemData">The data type of the items.</typeparam>
    /// <typeparam name="TContext">The type of <see cref="Context"/>.</typeparam>
    public abstract class FancyScrollView<TItemData, TContext> : MonoBehaviour where TContext : class, new()
    {
        /// <summary>
        /// Spacing between cells.
        /// </summary>
        [SerializeField, Range(1e-2f, 1f)]
        protected float _cellInterval = 0.2f;
        /// <summary>
        /// Reference point for the scroll position.
        /// </summary>
        /// <remarks>
        /// For example, if <c>0.5</c> is specified and the scroll position is <c>0</c>, the first cell will be positioned in the center.
        /// </remarks>
        [SerializeField, Range(0f, 1f)]
        protected float _scrollOffset = 0.5f;
        /// <summary>
        /// Whether to arrange cells in a circular manner.
        /// </summary>
        /// <remarks>
        /// If set to <c>true</c>, the first cell will appear after the last cell, and the last cell will appear before the first cell.
        /// Specify <c>true</c> to implement infinite scrolling.
        /// </remarks>
        [SerializeField]
        protected bool _loop;
        /// <summary>
        /// The parent <c>Transform</c> for the cells.
        /// </summary>
        [SerializeField]
        protected Transform _cellContainer;
        private readonly IList<FancyCell<TItemData, TContext>> _pool = new List<FancyCell<TItemData, TContext>>();
        /// <summary>
        /// Whether it has been initialized.
        /// </summary>
        protected bool initialized;
        /// <summary>
        /// Current scroll position.
        /// </summary>
        protected float currentPosition;

        /// <summary>
        /// Prefab of the cell.
        /// </summary>
        protected abstract GameObject CellPrefab { get; }

        /// <summary>
        /// List of item data.
        /// </summary>
        protected IList<TItemData> ItemsSource { get; set; } = new List<TItemData>();

        /// <summary>
        /// Instance of <typeparamref name="TContext"/>.
        /// Shared between the cells and the scroll view. Used for passing information and maintaining state.
        /// </summary>
        protected TContext Context { get; } = new();

        /// <summary>
        /// Performs initialization.
        /// </summary>
        /// <remarks>
        /// Called just before the cells are first generated.
        /// </remarks>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Updates the display content based on the provided list of items.
        /// </summary>
        /// <param name="itemsSource">List of items.</param>
        protected virtual void UpdateContents(IList<TItemData> itemsSource)
        {
            ItemsSource = itemsSource;
            Refresh();
        }

        /// <summary>
        /// Forces the layout of the cells to update.
        /// </summary>
        protected virtual void Relayout() => UpdatePosition(currentPosition, false);

        /// <summary>
        /// Forces the layout and content of all cells to update.
        /// </summary>
        protected virtual void Refresh() => UpdatePosition(currentPosition, true);

        /// <summary>
        /// Updates the scroll position.
        /// </summary>
        /// <param name="position">Scroll position.</param>
        protected virtual void UpdatePosition(float position) => UpdatePosition(position, false);

        private void UpdatePosition(float position, bool forceRefresh)
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            currentPosition = position;

            var p = position - _scrollOffset / _cellInterval;
            var firstIndex = Mathf.CeilToInt(p);
            var firstPosition = (Mathf.Ceil(p) - p) * _cellInterval;

            if (firstPosition + _pool.Count * _cellInterval < 1f)
            {
                ResizePool(firstPosition);
            }

            UpdateCells(firstPosition, firstIndex, forceRefresh);
        }

        private void ResizePool(float firstPosition)
        {
            Debug.Assert(CellPrefab != null);
            Debug.Assert(_cellContainer != null);

            var addCount = Mathf.CeilToInt((1f - firstPosition) / _cellInterval) - _pool.Count;

            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellPrefab, _cellContainer).GetComponent<FancyCell<TItemData, TContext>>();

                if (cell == null)
                {
                    throw new MissingComponentException(string.Format(
                        "FancyCell<{0}, {1}> component not found in {2}.",
                        typeof(TItemData).FullName, typeof(TContext).FullName, CellPrefab.name));
                }

                cell.SetContext(Context);
                cell.Initialize();
                cell.SetVisible(false);
                _pool.Add(cell);
            }
        }

        private void UpdateCells(float firstPosition, int firstIndex, bool forceRefresh)
        {
            for (var i = 0; i < _pool.Count; i++)
            {
                var index = firstIndex + i;
                var position = firstPosition + i * _cellInterval;
                var cell = _pool[CircularIndex(index, _pool.Count)];

                if (_loop)
                {
                    index = CircularIndex(index, ItemsSource.Count);
                }

                if (index < 0 || index >= ItemsSource.Count || position > 1f)
                {
                    cell.SetVisible(false);

                    continue;
                }

                if (forceRefresh || cell.Index != index || !cell.IsVisible)
                {
                    cell.Index = index;
                    cell.SetVisible(true);
                    cell.UpdateContent(ItemsSource[index]);
                }

                cell.UpdatePosition(position);
            }
        }

        private int CircularIndex(int i, int size) => size < 1 ? 0 : i < 0 ? size - 1 + (i + 1) % size : i % size;

#if UNITY_EDITOR
        private bool _cachedLoop;
        private float _cachedCellInterval, _cachedScrollOffset;
        private void LateUpdate()
        {
            if (_cachedLoop != _loop ||
                _cachedCellInterval != _cellInterval ||
                _cachedScrollOffset != _scrollOffset)
            {
                _cachedLoop = _loop;
                _cachedCellInterval = _cellInterval;
                _cachedScrollOffset = _scrollOffset;

                UpdatePosition(currentPosition);
            }
        }
#endif
    }

    /// <summary>
    /// Context class for <see cref="FancyScrollView{TItemData}"/>.
    /// </summary>
    public sealed class NullContext
    {
    }

    /// <summary>
    /// Abstract base class for implementing a scroll view.
    /// Supports infinite scrolling and snapping.
    /// </summary>
    /// <typeparam name="TItemData"></typeparam>
    /// <seealso cref="FancyScrollView{TItemData, TContext}"/>
    public abstract class FancyScrollView<TItemData> : FancyScrollView<TItemData, NullContext>
    {
    }
}