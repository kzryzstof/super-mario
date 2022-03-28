// ==========================================================================
// Copyright (C) 2022 by NoSuch Company.
// All rights reserved.
// May be used only in accordance with a valid Source Code License Agreement.
// 
// Last change: 24/03/2022 @ 20:26
// ==========================================================================

using System.Collections.Generic;

namespace NoSuchCompany.Games.SuperMario.Entities
{
    internal sealed class Collisions : ICollisions
    {
        private readonly HashSet<string> _aboveCollisions;
        private readonly HashSet<string> _belowCollisions;
        private readonly HashSet<string> _leftCollisions;
        private readonly HashSet<string> _rightCollisions;

        public bool Above { get; private set; }
        
        public bool Below { get; private set; }
        
        public bool Left { get; private set; }
        
        public bool Right { get; private set; }

        public bool IsCollidingExceptBelow => Left || Above || Right;

        public IEnumerable<string> AboveCollisions => _aboveCollisions;

        public IEnumerable<string> BelowCollisions => _belowCollisions;

        public IEnumerable<string> LeftCollisions => _leftCollisions;

        public IEnumerable<string> RightCollisions => _rightCollisions;
        
        public Collisions()
        {
            _aboveCollisions = new HashSet<string>();
            _belowCollisions = new HashSet<string>();
            _leftCollisions = new HashSet<string>();
            _rightCollisions = new HashSet<string>();
        }
        
        public void ResetAll()
        {
            Above = Below = false;
            Left = Right = false;
            
            _aboveCollisions.Clear();
            _belowCollisions.Clear();
            _leftCollisions.Clear();
            _rightCollisions.Clear();
        }

        public void SetTopCollisions(string tag)
        {
            Above = true;
            _aboveCollisions.Add(tag);
        }
        
        public void SetBottomCollisions(string tag)
        {
            Below = true;
            _belowCollisions.Add(tag);
        }

        public void SetLeftCollisions(string tag)
        {
            Left = true;
            _leftCollisions.Add(tag);
        }

        public void SetRightCollisions(string tag)
        {
            Right = true;
            _rightCollisions.Add(tag);
        }
    }
}