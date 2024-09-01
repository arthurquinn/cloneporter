using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ICarryable
{
    void StartCarry();
    void StopCarry();
    void UpdateCarryPosition(Vector2 position);
}
