using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace _Emulator
{

    public sealed class RegMapQuickFilter
    {
        public string Text = "";
        private string _lastText = "";
        private string[] _tokens = new string[0];
        private int[] _indices = new int[0];

        public int[] Indices
        {
            get { return _indices; }
        }

        /// <summary>
        /// Draws label + textfield.
        /// Returns true if text changed this frame.
        /// </summary>
        public bool Draw(
            Vector2 labelPos,
            Rect fieldRect)
        {
            // Uses your project’s label util (same as title field)
            LabelUtil.TextOut(labelPos, "Filter Maps", "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);

            string prev = Text;
            Text = GUI.TextField(fieldRect, Text);
            bool changed = (prev != Text);

            if (changed)
            {
                // Update tokens immediately so caller can rebuild indices right away
                UpdateTokensIfNeeded(force: true);
            }

            return changed;
        }

        /// <summary>
        /// Ensure tokens are up to date (call before rebuilding).
        /// </summary>
        public void UpdateTokensIfNeeded(bool force = false)
        {
            if (!force && _lastText == Text)
                return;

            _lastText = Text;

            if (string.IsNullOrEmpty(Text))
            {
                _tokens = new string[0];
                return;
            }

            // Split on spaces, AND semantics
            _tokens = Text
                .ToLowerInvariant()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Rebuild filtered indices for a given RegMap array (keeps indices into the original array).
        /// </summary>
        public void Rebuild(RegMap[] reg)
        {
            UpdateTokensIfNeeded();

            if (reg == null || reg.Length == 0)
            {
                _indices = new int[0];
                return;
            }

            // No filter => all indices
            if (_tokens == null || _tokens.Length == 0)
            {
                _indices = new int[reg.Length];
                for (int i = 0; i < reg.Length; i++)
                    _indices[i] = i;
                return;
            }

            List<int> list = new List<int>(reg.Length);
            for (int i = 0; i < reg.Length; i++)
            {
                if (Passes(reg[i]))
                    list.Add(i);
            }

            _indices = list.ToArray();
        }

        /// <summary>
        /// If selection is not visible, pick a safe one.
        /// Returns the possibly-updated selection index into reg[].
        /// </summary>
        public int ClampSelection(RegMap[] reg, int currentRegIndex)
        {
            if (reg == null || reg.Length == 0)
                return 0;

            if (_indices == null || _indices.Length == 0)
                return 0;

            // If current is visible, keep it
            for (int i = 0; i < _indices.Length; i++)
            {
                if (_indices[i] == currentRegIndex)
                    return currentRegIndex;
            }

            // Otherwise select first visible
            return _indices[0];
        }

        private bool Passes(RegMap m)
        {
            if (_tokens == null || _tokens.Length == 0)
                return true;

            string alias = (m != null && m.Alias != null) ? m.Alias : "";
            string dev = (m != null && m.Developer != null) ? m.Developer : "";

            string haystack = (alias + " " + dev).ToLowerInvariant();

            // AND across tokens
            for (int i = 0; i < _tokens.Length; i++)
            {
                if (!haystack.Contains(_tokens[i]))
                    return false;
            }

            return true;
        }
    }

}
