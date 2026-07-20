// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Tracks;
using Useful;

namespace StuntCarRacerLib.Cars;

// Player car physics, ported from the original Car Behaviour.cpp.
// All values are integer fixed-point in the original Amiga/PC formats:
// x/z are in PC StuntCarRacer format, y is in Amiga format, and angles
// are unsigned with 65536 = 360 degrees.
public sealed partial class CarPhysics
{
    internal const int CarWidth = 64;

    internal const int CarLength = 128;

    private const int GravityAcceleration = 317;

    private const int RoadWidth = 0x0180;

    // Amiga StuntCarRacer used 256, but 1024 is smoother when interpolating.
    private const int SurfaceSize = 1024;

    private const int LogSurfaceSize = 10;

    private const int OffRoadHeight = 0x1000;

    // Count after which player is put back on track.
    private const int OffTrackLimit = 64;

    // Chain lift-onto-track timing/geometry (see AnimateChainLift).
    private const int ChainLiftStart = 240;
    private const int ChainSwingSettleFrames = 40;
    private const int ChainSwingFullMagnitude = 44;
    private const int ChainSwingRestMagnitude = 16;
    private const int ChainRaiseHeight = 0x600000;

    private const int LocalYFactor = 4;

    private const int Reduction = 238; // (238/256)

    private const int Increase = 276; // (276/256)

    // Track/league dependant in the original (could be added to track data).
    private const int DamagedLimit = 10;

    // Front wheel rotation animation (cockpit display only), from the
    // original SetOneWheelRotationSpeed/WHEEL_ANGLE_MASK.
    private const int WheelSpeedLowThreshold = 0x800;
    private const int WheelSpeedHighOffset = 0x3000;
    private const int WheelSpeedMax = 0xffff;
    private const int WheelSpeedMaxClamped = 0xff00;
    private const int WheelAngleMask = 0xfffff;

    // The damage level at which the original's cockpit crack becomes a hole.
    private const int SmashDamageThreshold = 0x1400;

    // How far below the road a front wheel may show before LimitViewpointY
    // holds the viewpoint up (the original Y_ADJUSTMENT_THRESHOLD).
    private const int YAdjustmentThreshold = 0x480;

    // Used to convert a sin value (0-255) into a cosine value; 128 entries
    // indexed by sin/2. See the original Cosine_Conversion_Table.
    private static readonly int[] s_cosineConversion =
    [
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfe, 0xfe,
        0xfe, 0xfe, 0xfd, 0xfd, 0xfd, 0xfd, 0xfc, 0xfc,
        0xfb, 0xfb, 0xfb, 0xfa, 0xfa, 0xf9, 0xf9, 0xf8,
        0xf8, 0xf7, 0xf7, 0xf6, 0xf6, 0xf5, 0xf4, 0xf4,
        0xf3, 0xf3, 0xf2, 0xf1, 0xf0, 0xf0, 0xef, 0xee,
        0xed, 0xec, 0xec, 0xeb, 0xea, 0xe9, 0xe8, 0xe7,
        0xe6, 0xe5, 0xe4, 0xe3, 0xe2, 0xe1, 0xe0, 0xdf,
        0xde, 0xdd, 0xdb, 0xda, 0xd9, 0xd8, 0xd6, 0xd5,
        0xd4, 0xd2, 0xd1, 0xcf, 0xce, 0xcc, 0xcb, 0xc9,
        0xc8, 0xc6, 0xc5, 0xc3, 0xc1, 0xbf, 0xbe, 0xbc,
        0xba, 0xb8, 0xb6, 0xb4, 0xb2, 0xb0, 0xae, 0xac,
        0xa9, 0xa7, 0xa5, 0xa2, 0xa0, 0x9d, 0x9b, 0x98,
        0x95, 0x92, 0x8f, 0x8c, 0x89, 0x86, 0x83, 0x7f,
        0x7c, 0x78, 0x74, 0x70, 0x6c, 0x68, 0x63, 0x5e,
        0x59, 0x53, 0x4d, 0x47, 0x3f, 0x37, 0x2d, 0x20,
    ];

    private readonly Track _track;

    private readonly TrigCoefficients _trig = new();

    // Engine fluctuation is cosmetic (engine sound pitch); seeded for determinism.
    private readonly Random _random = new(0);

    // Speeds.
    private int _playerWorldXSpeed;
    private int _playerWorldYSpeed;
    private int _playerWorldZSpeed;
    private int _playerXSpeed;
    private int _playerYSpeed;

    // Controls.
    private bool _accelerate;
    private bool _brake;
    private bool _accelerating;
    private int _enginePower = 240; // (240 standard, 320 super)
    private int _boostUnitValue = 16; // (16 standard, 12 super)
    private int _leftRightValue;
    private int _engineZAcceleration;

    // Wheel offsets and heights.
    private int _rearWheelXOffset;
    private int _rearWheelZOffset;
    private int _frontLeftWheelXOffset;
    private int _frontLeftWheelZOffset;
    private int _frontRightWheelXOffset;
    private int _frontRightWheelZOffset;
    private int _frontLeftRoadHeight = OffRoadHeight;
    private int _frontRightRoadHeight = OffRoadHeight;
    private int _rearRoadHeight = OffRoadHeight;
    private int _frontLeftActualHeight;
    private int _frontRightActualHeight;
    private int _rearActualHeight;

    // Off road state.
    private bool _offLeft;
    private bool _offRight;
    private bool _wheelOffRoad;
    private int _distanceOffRoad;
    private int _atSideByte;
    private bool _smallerLimitRequired;
    private int _playerDistanceOffRoad;
    private int _offMapStatus;
    private int _offTrackCount;

    // 0x200 if wrecked.
    private int _wreckWheelHeightReduction;

    // Lift-onto-track ("chains") state: after the car has been off the
    // track too long, a crane swings it back over the road and dangles it
    // until the player presses boost/fire, then normal gravity takes over
    // (original car.on.chains.countdown / lift.car.onto.track).
    private int _chainCountdown;
    private int _chainSwingMagnitude;
    private bool _chainSwingFromLeft;
    private int _chainRaisedY;

    // Gravity, collision and damage.
    private int _gravityXAcceleration;
    private int _gravityYAcceleration;
    private int _gravityZAcceleration;
    private int _groundedDelay;
    private int _groundedCount;
    private int _damageValue;
    private int _damagedCount;
    private int _frontLeftAmountBelowRoad;
    private int _frontRightAmountBelowRoad;
    private int _rearAmountBelowRoad;
    private int _oldFrontLeftDifference;
    private int _oldFrontRightDifference;
    private int _oldRearDifference;
    private int _smashedCountdown;
    private int _carCollisionXAcceleration;
    private int _carCollisionYAcceleration;
    private int _carCollisionZAcceleration;
    private int _carToRoadCollisionZAcceleration;
    private int _frontLeftHeightDifference;
    private int _frontRightHeightDifference;
    private int _rearHeightDifference;
    private int _frontDifferenceBelowRoad;
    private int _overallDifferenceBelowRoad;
    private int _frontLeftWheelSpeed;
    private int _frontRightWheelSpeed;

    // Accelerations.
    private int _playerXAcceleration;
    private int _playerYAcceleration;
    private int _playerZAcceleration;
    private int _totalWorldXAcceleration;
    private int _totalWorldYAcceleration;
    private int _totalWorldZAcceleration;

    // Rotation.
    private int _playerXRotationSpeed;
    private int _playerYRotationSpeed;
    private int _playerZRotationSpeed;
    private int _playerFinalXRotationSpeed;
    private int _playerFinalYRotationSpeed;
    private int _playerFinalZRotationSpeed;
    private int _playerXRotationAcceleration;
    private int _playerYRotationAcceleration;
    private int _playerZRotationAcceleration;

    // Steering working values (shared between steering functions, as in the original).
    private int _yAngleDifference;
    private int _differenceAngle;
    private int _posDifferenceAngle;

    // Current surface coordinates (CalculateWorldRoadHeight state).
    private int _sx1;
    private int _sy1;
    private int _sz1;
    private int _sx2;
    private int _sy2;
    private int _sz2;
    private int _sx3;
    private int _sy3;
    private int _sz3;
    private int _sx4;
    private int _sy4;
    private int _sz4;
    private int _surfacePiece = -1;
    private int _surfaceSegment = -1;
    private bool _surfaceFirstTime = true;

    // Current piece coordinates (IdentifyPiece state).
    private int _px1;
    private int _pz1;
    private int _px2;
    private int _pz2;
    private int _px3;
    private int _pz3;
    private int _px4;
    private int _pz4;
    private int _steeringPiece;

    // Engine revs change (used for the engine sound pitch).
    private int _engineRevsChange;

    // Lap data.
    private bool _carOnFirstHalfOfLap;

    public CarPhysics(Track track)
    {
        Guard.ArgumentNull(track);
        _track = track;
    }

    private enum WheelPosition
    {
        FrontLeft = 0,
        FrontRight = 1,
        Rear = 2,

        // Not a wheel position; used by CalculatePlayersRoadPosition.
        Centre = 3,
    }

    // Render-space outputs (as output by the original CarBehaviour).
    public int X => PlayerX;

    public int Y => -(PlayerY * LocalYFactor);

    public int Z => PlayerZ;

    // X and Z angles are reversed because the drawing code rotates around
    // x and z in the opposite direction to the physics.
    public int XAngle => -PlayerXAngle & (Track.MaxAngle - 1);

    public int YAngle => PlayerYAngle & (Track.MaxAngle - 1);

    public int ZAngle => -PlayerZAngle & (Track.MaxAngle - 1);

    public bool TouchingRoad { get; private set; }

    public bool DropStartDone { get; private set; } = true;

    // True while a crane is lifting/swinging/dangling the car back over the
    // track after it went off for too long.
    public bool OnChains => _chainCountdown != 0;

    // True while the car is dangling on chains waiting for the player to
    // press boost/fire to be released (drives a HUD prompt).
    public bool WaitingToReleaseChains { get; private set; }

    public int CurrentPiece { get; private set; }

    public int CurrentSegment { get; private set; }

    public int DistanceIntoSection { get; private set; }

    public int RoadXPosition { get; private set; }

    public int BoostReserve { get; set; }

    public int BoostUnit { get; set; }

    public int LapNumber { get; private set; }

    // Elapsed 50Hz ticks (see ApplyEngineRevs) since the current lap
    // started; the fastest completed lap so far, or null before the first
    // lap finishes (original print.lap.time/show.best.lap.time, dashboard
    // M:SS.CC read-outs - see the lap-times backlog item for the caveats
    // around this simplified, real-time-based port of the original's BCD
    // stopwatch).
    public int CurrentLapTicks { get; private set; }

    public int? BestLapTicks { get; private set; }

    public bool RaceFinished { get; private set; }

    public int FrontLeftDamage { get; private set; }

    public int FrontRightDamage { get; private set; }

    public int RearDamage { get; private set; }

    public bool Damaged { get; private set; }

    public int NewDamage { get; private set; }

    public bool Wrecked => _wreckWheelHeightReduction != 0;

    // Speed value for display, using player z speed (original
    // CalculateDisplaySpeed). ptitSeb's remake raised the dead zone from
    // "< 0" to "< 0x1100" (the first few values aren't shown) and rescaled
    // the result by 200/128 to fill the new cockpit gauge's range (full at
    // 240, matching Rendering/HudRenderer's speed bar).
    public int DisplaySpeed
    {
        get
        {
            int speed = PlayerZSpeed;
            if (speed < 0x1100)
            {
                speed = 0;
            }

            speed = (speed * 183) >> 15;
            return (speed * 200) >> 7;
        }
    }

    // Number of cockpit crack "holes" (the original nholes), one per smash
    // since the crack reached the smash threshold.
    internal int SmashHoles { get; private set; }

    // Front wheel animation frame (0-5), for the cockpit display.
    internal int LeftWheelFrame => (LeftWheelAngle >> 16) % 6;

    internal int RightWheelFrame => (RightWheelAngle >> 16) % 6;

    // Suspension compression, in cockpit display pixels (the original
    // old_leftwheel/old_rightwheel): how far the wheel sprite rises into
    // the wheel well.
    internal int LeftWheelBounce => _frontLeftAmountBelowRoad >> 6;

    internal int RightWheelBounce => _frontRightAmountBelowRoad >> 6;

    // Player position in PC StuntCarRacer format where X and Z equal the render
    // outputs. PlayerY is in Amiga format, positive upwards.
    internal int PlayerX { get; private set; }

    internal int PlayerY { get; private set; }

    internal int PlayerZ { get; private set; }

    // Raw physics angles (unsigned, 65536 = 360 degrees); the public
    // XAngle/ZAngle properties are the reversed rendering outputs.
    internal int PlayerXAngle { get; private set; }

    internal int PlayerYAngle { get; private set; }

    internal int PlayerZAngle { get; private set; }

    internal int PlayerZSpeed { get; private set; }

    internal int EngineRevs { get; private set; }

    // Random fluctuation applied to the engine sound period.
    internal int EngineFluctuation { get; private set; }

    // 0x80 while boost is being applied (used for the boost gauge).
    internal int BoostActivated { get; private set; }

    // 0x80 when falling off the left road edge, 0x40 for the right (sparks side).
    internal int WhichSideByte { get; private set; }

    // Rear wheel x position across the road surface, reduced to 0-255.
    internal int RearWheelSurfaceX { get; private set; }

    // 0 for standard league, 1 for super league.
    internal int RoadCushionValue { get; set; }

    // Set by the opponent's collision detection; invoked during the player's
    // collision detection to transfer car-to-car accelerations (the original
    // CarToCarCollision call).
    internal Action? CarToCarCollision { get; set; }

    // Slipstream/proximity flags maintained by the opponent each frame.
    internal bool PlayerCloseToOpponent { get; set; }

    internal bool OpponentBehindPlayer { get; set; }

    // Sound triggers, set for one frame when the effect should play.
    internal bool GroundedSoundTriggered { get; private set; }

    internal bool CreakSoundTriggered { get; private set; }

    internal bool SmashSoundTriggered { get; private set; }

    internal bool OffRoadSoundTriggered { get; private set; }

    internal bool WreckSoundTriggered { get; private set; }

    // Front wheel rotation angles (cockpit display only).
    private int LeftWheelAngle { get; set; }

    private int RightWheelAngle { get; set; }

    // Reset all car behaviour variables to their initial state (original ResetPlayer).
    public void Reset()
    {
        PlayerX = 0;
        PlayerY = 0;
        PlayerZ = 0;
        PlayerXAngle = 0;
        PlayerYAngle = 0;
        PlayerZAngle = 0;

        _playerWorldXSpeed = 0;
        _playerWorldYSpeed = 0;
        _playerWorldZSpeed = 0;
        _playerXSpeed = 0;
        _playerYSpeed = 0;
        PlayerZSpeed = 0;

        _accelerating = false;
        _enginePower = 240;
        _boostUnitValue = 16;
        _leftRightValue = 0;
        _engineZAcceleration = 0;
        BoostActivated = 0;

        _rearWheelXOffset = 0;
        _rearWheelZOffset = 0;
        _frontLeftWheelXOffset = 0;
        _frontLeftWheelZOffset = 0;
        _frontRightWheelXOffset = 0;
        _frontRightWheelZOffset = 0;

        _frontLeftRoadHeight = OffRoadHeight;
        _frontRightRoadHeight = OffRoadHeight;
        _rearRoadHeight = OffRoadHeight;
        _frontLeftActualHeight = 0;
        _frontRightActualHeight = 0;
        _rearActualHeight = 0;

        _offLeft = false;
        _offRight = false;
        _wheelOffRoad = false;
        _distanceOffRoad = 0;
        _atSideByte = 0;
        WhichSideByte = 0;
        _smallerLimitRequired = false;

        _wreckWheelHeightReduction = 0;

        DropStartDone = true;
        TouchingRoad = false;

        _chainCountdown = 0;
        _chainSwingMagnitude = 0;
        WaitingToReleaseChains = false;

        _playerDistanceOffRoad = 0;
        _offMapStatus = 0;

        _gravityXAcceleration = 0;
        _gravityYAcceleration = 0;
        _gravityZAcceleration = 0;

        _groundedDelay = 0;
        _groundedCount = 0;
        _damageValue = 0;
        _damagedCount = 0;
        Damaged = false;

        _frontLeftAmountBelowRoad = 0;
        _frontRightAmountBelowRoad = 0;
        _rearAmountBelowRoad = 0;
        _oldFrontLeftDifference = 0;
        _oldFrontRightDifference = 0;
        _oldRearDifference = 0;

        FrontLeftDamage = 0;
        FrontRightDamage = 0;
        RearDamage = 0;
        NewDamage = 0;
        _smashedCountdown = 0;
        SmashHoles = 0;
        _frontLeftWheelSpeed = 0;
        _frontRightWheelSpeed = 0;
        LeftWheelAngle = 0;
        RightWheelAngle = 0;

        _carCollisionXAcceleration = 0;
        _carCollisionYAcceleration = 0;
        _carCollisionZAcceleration = 0;
        _carToRoadCollisionZAcceleration = 0;

        _playerXAcceleration = 0;
        _playerYAcceleration = 0;
        _playerZAcceleration = 0;

        _totalWorldXAcceleration = 0;
        _totalWorldYAcceleration = 0;
        _totalWorldZAcceleration = 0;

        _playerXRotationSpeed = 0;
        _playerYRotationSpeed = 0;
        _playerZRotationSpeed = 0;
        _playerFinalXRotationSpeed = 0;
        _playerFinalYRotationSpeed = 0;
        _playerFinalZRotationSpeed = 0;
        _playerXRotationAcceleration = 0;
        _playerYRotationAcceleration = 0;
        _playerZRotationAcceleration = 0;
    }

    // Start a new race: reset the player onto the track's start piece.
    public void StartRace()
    {
        Reset();
        PositionCarAbovePiece(_track.PlayersStartPiece);
        DropStartDone = false;
        _offTrackCount = 0;
        ResetLapData();
    }

    public void ResetLapData()
    {
        RaceFinished = false;
        LapNumber = 0;
        _carOnFirstHalfOfLap = false;
        CurrentLapTicks = 0;
        BestLapTicks = null;
    }

    // One physics frame (original CarBehaviour).
    public void Update(CarInput input)
    {
        GroundedSoundTriggered = false;
        OffRoadSoundTriggered = false;
        WreckSoundTriggered = false;

        // Put the car back on the track after it has been off for too long.
        if (_offTrackCount > OffTrackLimit && _chainCountdown == 0)
        {
            BeginChainRecovery();
        }

        CarControl(input);
        CarMovement();
        UpdateEngineRevs();

        if (_chainCountdown > 0)
        {
            AnimateChainLift(input);
        }
        else if (TouchingRoad)
        {
            DropStartDone = true;
        }

        UpdateEffectSounds();
    }

    // Apply the revs change calculated by the last physics frame. The
    // original did this in FramesWheelsEngine at the full 50Hz frame rate,
    // not the physics rate, so the revs ramp between physics frames.
    public void ApplyEngineRevs()
    {
        int revs = EngineRevs + _engineRevsChange;
        if (revs < 0)
        {
            revs = 0;
        }

        EngineRevs = revs;

        AdvanceWheelAngles();

        if (!RaceFinished)
        {
            CurrentLapTicks++;
        }
    }

    // Calculate player position values required for opponent behaviour
    // (original CalculatePlayersRoadPosition).
    public void CalculateRoadPosition()
        => CalculateWorldRoadHeight(WheelPosition.Centre, PlayerX, PlayerZ, out _);

    public void UpdateLapData()
    {
        int startFinishPiece = _track.StartLinePiece + 1 < _track.NumPieces ? _track.StartLinePiece + 1 : 0;

        if (_carOnFirstHalfOfLap)
        {
            if (CurrentPiece == _track.HalfALapPiece)
            {
                _carOnFirstHalfOfLap = false;
            }
        }
        else if (CurrentPiece == startFinishPiece)
        {
            _carOnFirstHalfOfLap = true;
            LapNumber++;

            // the car starts on the start line itself, so every lap
            // (including the first) is a genuine full circuit worth timing.
            if (BestLapTicks is null || CurrentLapTicks < BestLapTicks.Value)
            {
                BestLapTicks = CurrentLapTicks;
            }

            CurrentLapTicks = 0;
        }

        const int lapThatFinishesRace = 4;
        if (!RaceFinished && LapNumber == lapThatFinishesRace)
        {
            RaceFinished = true;
        }
    }

    public void UpdateDamage()
    {
        CreakSoundTriggered = false;
        SmashSoundTriggered = false;

        if (Damaged)
        {
            int d = (FrontLeftDamage + FrontRightDamage) / 2;
            NewDamage = (d + RearDamage) / 2;

            // original car.is.wrecked, reached when the HUD crack (damage.line)
            // hits the end of its beam.
            if (NewDamage >= 240)
            {
                _wreckWheelHeightReduction = 0x200;
            }
        }

        if (_smashedCountdown > 0)
        {
            --_smashedCountdown;
            if (Damaged)
            {
                CreakSoundTriggered = true;
            }

            return;
        }

        if (!Damaged)
        {
            return;
        }

        if (_damageValue >= SmashDamageThreshold)
        {
            _smashedCountdown = 69;
            SmashSoundTriggered = true;
            SmashHoles++;
            return;
        }

        // the original also sets the creak volume from _damageValue
        CreakSoundTriggered = true;
    }

    // Adds car-to-car collision accelerations from the opponent.
    internal void AddCollisionAcceleration(int x, int y, int z)
    {
        _carCollisionXAcceleration += x;
        _carCollisionYAcceleration += y;
        _carCollisionZAcceleration += z;
    }

    // Adds car-to-car collision damage to all three damage areas.
    internal void AddCollisionDamage(int amount)
    {
        RearDamage = Math.Min(255, RearDamage + amount);
        FrontRightDamage = Math.Min(255, FrontRightDamage + amount);
        FrontLeftDamage = Math.Min(255, FrontLeftDamage + amount);
        Damaged = true;
    }

    private static void CalculateInclinationSinCos(int inclination, out int sin, out int cos)
    {
        // inclination is effectively the sin of the inclination angle,
        // sign-removed and limited to 255 to enable table indexing.
        inclination = Math.Abs(inclination);

        sin = inclination < 256 ? inclination : 255;

        // note only 128 values in table
        cos = s_cosineConversion[sin / 2];
    }

    private static int CalcDistanceOffRoad(int x, int z, int ox, int oz, int ux, int uz, int vx, int vz, out int ex, out int ez)
    {
        // ox, oz - origin point

        // z vector
        ux -= ox;
        uz -= oz;

        // x vector
        vx -= ox;
        vz -= oz;

        // calculate the (perpendicular) distance from the left or right edge
        // method is similar to that used when texture mapping
        int v = ((x - ox) * uz) + ((oz - z) * ux);
        int denominator = (uz * vx) - (ux * vz);

        if (denominator == 0)
        {
            ex = x;
            ez = z;
            return 0;
        }

        ex = x - (v * vx / denominator);
        ez = z - (v * vz / denominator);
        return v * RoadWidth / denominator;
    }

    // Sound triggers for the off-road dust clouds and edge sparks, from the
    // original DrawOtherGraphics/DrawDustClouds/DrawSparks (which only played
    // the sound effects in the remake).
    private void UpdateEffectSounds()
    {
        if (!OnChains && _offMapStatus != 0 && TouchingRoad)
        {
            OffRoadSoundTriggered = true;
        }

        if ((WhichSideByte != 0 || Wrecked) && _offMapStatus == 0)
        {
            int p = Math.Abs(PlayerZSpeed) >> 8;
            if (p >= 1 && TouchingRoad)
            {
                WreckSoundTriggered = true;
            }
        }

        // the original cleared this in update.wheel.positions
        WhichSideByte = 0;
    }

    private void CarControl(CarInput input)
    {
        bool left = input.HasFlag(CarInput.Left);
        bool right = input.HasFlag(CarInput.Right);
        bool boost = input.HasFlag(CarInput.Boost);

        _accelerate = input.HasFlag(CarInput.Accelerate);
        _brake = input.HasFlag(CarInput.Brake);

        _leftRightValue = 0;
        if (TouchingRoad && !OnChains)
        {
            if (left)
            {
                _leftRightValue = -15;
            }

            if (right)
            {
                _leftRightValue = 15;
            }
        }

        bool boostFlag = !boost; // active low

        if (PlayerZSpeed < 120 * 256 && !OnChains && !Wrecked)
        {
            if (_accelerate)
            {
                _engineZAcceleration = _enginePower;
                _accelerating = true;
            }
            else if (_brake)
            {
                _engineZAcceleration = -240;
                _accelerating = false;
            }
            else if (_accelerating)
            {
                // car keeps accelerating even when control released
                _engineZAcceleration = _enginePower;
            }
            else
            {
                _engineZAcceleration = 0;
            }
        }
        else
        {
            _engineZAcceleration = 0;
        }

        BoostPower(boostFlag);
    }

    private void BoostPower(bool boostFlag)
    {
        BoostActivated = 0;

        if (!boostFlag && !Wrecked && (_accelerating || _accelerate || _brake) && BoostReserve > 0)
        {
            --BoostUnit;
            if (BoostUnit < 0)
            {
                BoostUnit = _boostUnitValue;
                --BoostReserve;
            }

            BoostActivated = 0x80;
            _engineZAcceleration *= 2;
        }
    }

    private void CarMovement()
    {
        // calculate required sin/cos values using player x, y and z angles
        _trig.CalculateYXZ(PlayerXAngle, PlayerYAngle, PlayerZAngle);

        CalculateWheelXZOffsets();
        CalculateRoadWheelHeights();
        CalculateActualWheelHeights();

        CalculateXZSpeeds();

        SetWheelRotationSpeed();
        CalculateGravityAcceleration();
        CarCollisionDetection();

        CalculateTotalAcceleration();
        CalculateSteering();

        CalculateWorldAcceleration();
        ReduceWorldAcceleration();

        CalculateXZRotationAcceleration();
        UpdatePlayersRotationSpeed();
        CalculateFinalRotationSpeed();

        UpdatePlayersWorldSpeed();
        UpdatePlayersPosition();

        // extra bit to set flags
        if (_playerDistanceOffRoad >= 256 - (RoadWidth / 2))
        {
            _offMapStatus = 0x80;
        }
        else
        {
            _offMapStatus = 0;
            _offTrackCount = 0;
            _smallerLimitRequired = false;
        }

        if (_offMapStatus != 0 && TouchingRoad && PlayerY < 0x1000000)
        {
            _offTrackCount++;
            _smallerLimitRequired = true;
        }
    }
}
