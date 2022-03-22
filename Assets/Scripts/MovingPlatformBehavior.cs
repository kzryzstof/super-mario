using System;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario
{
    public class MovingPlatformBehavior : MonoBehaviour
    {
        private const float NoMovement = 0f;
        private float _minPosition;
        private float _maxPosition;

        public float minOffset;

        public float maxOffset;

        public float speed;

        public ScrollingOrientation scrollingOrientation;

        public ScrollingMode scrollingMode;
        
        private float _currentSpeed;

        private Vector3 CurrentPosition => new Vector3(transform.position.x, transform.position.y, 0);
        
        public void Awake()
        {
            _currentSpeed = speed;
            _maxPosition = (scrollingOrientation == ScrollingOrientation.Horizontal ? transform.position.x : transform.position.y) + maxOffset;
            _minPosition = (scrollingOrientation == ScrollingOrientation.Horizontal ? transform.position.x : transform.position.y) + minOffset;

            if (scrollingMode == ScrollingMode.Continuous)
            {
                //  These are absolute values based that describes the
                //  upper and lower boundaries of the screen.
                _minPosition = -8f;
                _maxPosition = 7f;
            }
        }
        
        public void FixedUpdate()
        {
            switch (scrollingMode)
            {
                case ScrollingMode.BackAndForth:
                    HandleBackAndForthMovement();
                    break;
                case ScrollingMode.Continuous:
                    HandleContinueMovement();
                    break;
                default:
                    throw new NotImplementedException();
            }

            Vector3 targetPosition = ComputeTargetPosition();

            AppLogger.Write(LogsLevels.MovingPlatforms, $"Current = ({CurrentPosition.x},{CurrentPosition.y} -> Target = ({targetPosition.x},{targetPosition.y}) at Speed = {_currentSpeed}");
            
            MovePlatformTo(targetPosition);
        }

        private void HandleContinueMovement()
        {
            if (IsMinPositionReached())
                ResetToPosition(_maxPosition);
            if (IsMaxPositionReached())
                ResetToPosition(_minPosition);
        }

        private void ResetToPosition(float offset)
        {
            float targetPositionX = scrollingOrientation == ScrollingOrientation.Horizontal ? offset : CurrentPosition.x;
            float targetPositionY = scrollingOrientation == ScrollingOrientation.Vertical ? offset : CurrentPosition.y;
            transform.position = new Vector3(targetPositionX, targetPositionY, CurrentPosition.z);
            
            AppLogger.Write(LogsLevels.MovingPlatforms, $"Reset to position: ({targetPositionX},{targetPositionY})");
        }        
        
        private void HandleBackAndForthMovement()
        {
            if (IsMinPositionReached())
                ChangeDirection();
            
            if (IsMaxPositionReached())
                ChangeDirection();
        }
        
        private void MovePlatformTo(Vector3 targetPosition)
        {
            transform.position = Vector3.MoveTowards(CurrentPosition, targetPosition, Math.Sign(_currentSpeed) * speed * Time.deltaTime);
        }

        private Vector3 ComputeTargetPosition()
        {
            float targetPositionX = CurrentPosition.x + (scrollingOrientation == ScrollingOrientation.Horizontal ? _currentSpeed : NoMovement);
            float targetPositionY = CurrentPosition.y + (scrollingOrientation == ScrollingOrientation.Vertical ? _currentSpeed : NoMovement);
            
            return new Vector3(targetPositionX, targetPositionY, transform.position.z);
        }

        private void ChangeDirection()
        {
            _currentSpeed = -_currentSpeed;
            AppLogger.Write(LogsLevels.MovingPlatforms, $"Changing direction: ({_currentSpeed})");
        }
        
        private bool IsMinPositionReached()
        {
            if (_currentSpeed > 0)
                return false;
            
            float currentOffset = scrollingOrientation == ScrollingOrientation.Horizontal ? CurrentPosition.x : CurrentPosition.y;

            AppLogger.Write(LogsLevels.MovingPlatforms, $"Current = {currentOffset} - MinPosition = {_minPosition} Is Min position reached? ({currentOffset < _minPosition})");
            
            return currentOffset < _minPosition;
        }
        
        private bool IsMaxPositionReached()
        {
            if (_currentSpeed < 0)
                return false;

            float currentOffset = scrollingOrientation == ScrollingOrientation.Horizontal ? CurrentPosition.x : CurrentPosition.y;
            
            return currentOffset > _maxPosition;
        }
    }
}