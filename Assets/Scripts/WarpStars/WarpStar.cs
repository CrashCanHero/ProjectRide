using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project: Ride/Warp Star")]
public class WarpStar : ScriptableObject {
    public float DistanceToGround = 1;
    public float Weight = 1;
    public float Acceleration = 1;
    public float MaxSpeed = 1;
    public float Traction = 1;
    public float ChargeTime = 1;
    public float BoostAmount = 1;
    public float TurnSpeed = 1;
    [HideInInspector] public float WeightMultiplier = 1;
    [HideInInspector] public float AccelerationMultiplier = 1;
    [HideInInspector] public float MaxSpeedMultiplier = 1;
    [HideInInspector] public float TractionMultiplier = 1;
    [HideInInspector] public float ChargeTimeMultiplier = 1;
    [HideInInspector] public float BoostAmountMultiplier = 1;
    [HideInInspector] public float TurnSpeedMultiplier = 1;

    public float NewWeight {
        get {
            return Weight * WeightMultiplier;
        }
    }
    public float NewAcceleration {
        get {
            return Acceleration * AccelerationMultiplier;
        }
    }
    public float NewMaxSpeed {
        get {
            return MaxSpeed * MaxSpeedMultiplier;
        }
    }
    public float NewTraction {
        get {
            return Traction * TractionMultiplier;
        }
    }
    public float NewChargeTime {
        get {
            return ChargeTime / ChargeTimeMultiplier;
        }
    }
    public float NewBoostAmount {
        get {
            return BoostAmount * BoostAmountMultiplier;
        }
    }

    public float NewTurnSpeed {
        get {
            return TurnSpeed * TurnSpeedMultiplier;
        }
    }
}