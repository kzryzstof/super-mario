using System;
using NoSuchCompany.Games.SuperMario.Diagnostics;
using NoSuchCompany.Games.SuperMario.Entities;
using UnityEngine;

namespace NoSuchCompany.Games.SuperMario
{
    public class MovingPlatformBehavior : MonoBehaviour
    {
        private const float NoMovement = 0f;
        
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
        }
        
        public void FixedUpdate()
        {
            switch (scrollingMode)
            {
                case ScrollingMode.BackAndForth:
                    HandleBackAndForthMovement();
                    break;
                default:
                    throw new NotImplementedException();
            }
            

            Vector3 targetPosition = ComputeTargetPosition();

            AppLogger.Write(LogsLevels.MovingPlatforms, $"Current = ({CurrentPosition.x},{CurrentPosition.y} -> Target = ({targetPosition.x},{targetPosition.y}) at Speed = {_currentSpeed}");
            
            MovePlatformTo(targetPosition);
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
            transform.position = Vector3.MoveTowards(CurrentPosition, targetPosition, speed * Time.deltaTime);
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

            return currentOffset < minOffset;
        }
        
        private bool IsMaxPositionReached()
        {
            if (_currentSpeed < 0)
                return false;

            float currentOffset = scrollingOrientation == ScrollingOrientation.Horizontal ? CurrentPosition.x : CurrentPosition.y;
            
            return currentOffset > maxOffset;
        }
    }
}