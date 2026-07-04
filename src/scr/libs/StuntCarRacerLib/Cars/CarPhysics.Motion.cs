// 'Stunt Car Racer - The Sharp Kind' - Andy Hawkins 2026.
// 'Stunt Car Racer Remake' - sourceforge.net/projects/stuntcarremake.
// Stunt Car Racer (C) Geoff Crammond / MicroStyle / MicroProse 1989.

using StuntCarRacerLib.Tracks;

namespace StuntCarRacerLib.Cars;

// Movement: speeds, accelerations, collision, steering and rotation
// (from Car Behaviour.cpp).
public sealed partial class CarPhysics
{
    // Calculate the height (y value) of each car wheel.
    private void CalculateActualWheelHeights()
    {
        int sinX = AmigaTrig.Sin(PlayerXAngle);
        int sinZ = AmigaTrig.Sin(PlayerZAngle);

        _rearActualHeight = PlayerY;
        _rearActualHeight -= sinX << (4 + 15 - Track.LogPrecision);
        _rearActualHeight >>= 8;

        _frontRightActualHeight = PlayerY;
        _frontRightActualHeight += sinX << (4 + 15 - Track.LogPrecision);
        _frontRightActualHeight -= sinZ << (3 + 15 - Track.LogPrecision);
        _frontRightActualHeight >>= 8;

        _frontLeftActualHeight = PlayerY;
        _frontLeftActualHeight += sinX << (4 + 15 - Track.LogPrecision);
        _frontLeftActualHeight += sinZ << (3 + 15 - Track.LogPrecision);
        _frontLeftActualHeight >>= 8;
    }

    // Calculate player's actual X/Z speeds by rotating world speed values.
    private void CalculateXZSpeeds()
    {
        _playerXSpeed = (_playerWorldXSpeed * _trig.XX) >> Track.LogPrecision;
        _playerXSpeed += (_playerWorldYSpeed * _trig.XY) >> Track.LogPrecision;
        _playerXSpeed += (_playerWorldZSpeed * _trig.XZ) >> Track.LogPrecision;

        _playerYSpeed = 0; // zero for current implementation

        PlayerZSpeed = (_playerWorldXSpeed * _trig.ZX) >> Track.LogPrecision;
        PlayerZSpeed += (_playerWorldYSpeed * _trig.ZY) >> Track.LogPrecision;
        PlayerZSpeed += (_playerWorldZSpeed * _trig.ZZ) >> Track.LogPrecision;
    }

    // Gravity acts on the Y axis only, therefore only Y components are used.
    private void CalculateGravityAcceleration()
    {
        _gravityXAcceleration = (-GravityAcceleration * _trig.XY) >> Track.LogPrecision;
        _gravityYAcceleration = (-GravityAcceleration * _trig.YY) >> Track.LogPrecision;
        _gravityZAcceleration = (-GravityAcceleration * _trig.ZY) >> Track.LogPrecision;
    }

    // Calculate car acceleration caused by collision with the road.
    private void CarCollisionDetection()
    {
        _groundedCount = 0;
        _damageValue = 0;
        Damaged = false;

        int frontLeftDamage = FrontLeftDamage;
        CalculateWheelCollision(
            _frontLeftRoadHeight,
            _frontLeftActualHeight,
            out _frontLeftHeightDifference,
            ref _oldFrontLeftDifference,
            ref _frontLeftAmountBelowRoad,
            ref frontLeftDamage);
        FrontLeftDamage = frontLeftDamage;

        int frontRightDamage = FrontRightDamage;
        CalculateWheelCollision(
            _frontRightRoadHeight,
            _frontRightActualHeight,
            out _frontRightHeightDifference,
            ref _oldFrontRightDifference,
            ref _frontRightAmountBelowRoad,
            ref frontRightDamage);
        FrontRightDamage = frontRightDamage;

        int rearDamage = RearDamage;
        CalculateWheelCollision(
            _rearRoadHeight,
            _rearActualHeight,
            out _rearHeightDifference,
            ref _oldRearDifference,
            ref _rearAmountBelowRoad,
            ref rearDamage);
        RearDamage = rearDamage;

        int averageFrontAmountBelowRoad = (_frontLeftAmountBelowRoad + _frontRightAmountBelowRoad) >> 1;
        int averageAmountBelowRoad = (averageFrontAmountBelowRoad + _rearAmountBelowRoad) >> 1;

        CalculateCarCollisionAcceleration(averageAmountBelowRoad);

        int difference = (_frontLeftAmountBelowRoad - _frontRightAmountBelowRoad) * 3;
        if (difference > 0x1000)
        {
            difference = 0x1000;
        }

        if (difference < -0x1000)
        {
            difference = -0x1000;
        }

        _frontDifferenceBelowRoad = difference;

        difference = averageFrontAmountBelowRoad - _rearAmountBelowRoad;
        _overallDifferenceBelowRoad = difference;

        TouchingRoad = averageAmountBelowRoad != 0;

        if (!TouchingRoad && !_onChains)
        {
            // get angle in Amiga StuntCarRacer format (i.e. correct sign)
            int angle = PlayerXAngle < AmigaTrig.Degrees180 ? PlayerXAngle : PlayerXAngle - AmigaTrig.Degrees360;

            if ((angle < 0 && (_track.Id == TrackId.RollerCoaster || _track.Id == TrackId.SkiJump)) || angle >= 0)
            {
                difference = -128;

                if (angle < 0 && _track.Id == TrackId.SkiJump)
                {
                    difference = -8;
                }

                if (angle >= 0x1000)
                {
                    difference = -256;
                }

                difference -= _overallDifferenceBelowRoad;
                if (difference < 0 && _playerXRotationSpeed >= -256)
                {
                    _overallDifferenceBelowRoad = difference;
                }
            }
        }

        _carToRoadCollisionZAcceleration = _carCollisionZAcceleration;

        CarToCarCollision?.Invoke();

        // Grounded sound (the original also sets the volume from _damageValue).
        if (_groundedDelay > 0)
        {
            --_groundedDelay;
        }

        if (_groundedCount != 0 && _groundedDelay == 0)
        {
            GroundedSoundTriggered = true;
            _groundedDelay = 5;
        }
    }

    private void CalculateWheelCollision(
        int roadHeight,
        int actualHeight,
        out int heightDifference,
        ref int oldDifference,
        ref int amountBelowRoadInOut,
        ref int damageInOut)
    {
        heightDifference = roadHeight - actualHeight - _wreckWheelHeightReduction;

        int newDifference = heightDifference;
        if (newDifference > 0x1400)
        {
            newDifference = 0x1400;
        }
        else if (newDifference < -0x300)
        {
            newDifference = -0x300;
        }

        int amountBelowRoad = newDifference - oldDifference;
        amountBelowRoad = ((amountBelowRoad * Increase) >> 8) + newDifference;

        if (amountBelowRoad >= 0)
        {
            int oldAmountBelowRoad = amountBelowRoadInOut;
            amountBelowRoadInOut = amountBelowRoad;

            if (amountBelowRoad >= 0x400 && oldAmountBelowRoad < 0x200)
            {
                _groundedCount++; // wheel grounded
            }

            int damage = amountBelowRoadInOut - (RoadCushionValue * 256);
            if (damage >= 0x700)
            {
                if (damage > _damageValue)
                {
                    _damageValue = damage;
                }

                damage -= 0x600;

                // The original also checks fourteen_frames_elapsed here, but it
                // is never set (always 0), so the check is dropped.
                _damagedCount++;
                if (_damagedCount < DamagedLimit)
                {
                    damage /= 256;
                    damage &= 0xff;
                    damage += damage / 2;
                    damage += damageInOut;
                    if (damage > 0xff)
                    {
                        damage = 0xff;
                    }

                    damageInOut = damage;
                    Damaged = true;
                }

                if (amountBelowRoadInOut >= 0x1200)
                {
                    amountBelowRoadInOut = 0x11ff;
                }
            }
            else
            {
                _damagedCount = 0;
            }
        }
        else
        {
            amountBelowRoadInOut = 0;
            _damagedCount = 0;
        }

        oldDifference = newDifference;
    }

    private void CalculateCarCollisionAcceleration(int averageAmountBelowRoad)
    {
        // averageAmountBelowRoad is the force exerted by the road on the car.
        // Force is directed through the Y axis of the road surface.
        const int logCarLengthFactor = 4;
        const int logCarWidthFactor = 3; // length is twice the width

        // y inclination to road is zero because road exists in X and Z planes only
        int frontHeightDifference = (_frontLeftHeightDifference + _frontRightHeightDifference) >> 1;
        int roadInclinationX = (frontHeightDifference - _rearHeightDifference) >> logCarLengthFactor;

        CalculateInclinationSinCos(roadInclinationX, out int surfaceSinX, out int surfaceCosX);

        int roadInclinationZ = (_frontLeftHeightDifference - _frontRightHeightDifference) >> logCarWidthFactor;

        CalculateInclinationSinCos(roadInclinationZ, out int surfaceSinZ, out int surfaceCosZ);

        int surfaceCosXCosZ = (surfaceCosX * surfaceCosZ) >> 8;
        int surfaceCosXSinZ = (surfaceCosX * surfaceSinZ) >> 8;

        // X acceleration = force * -cosx.sinz
        int surfaceValue = roadInclinationZ < 0 ? -surfaceCosXSinZ : surfaceCosXSinZ;
        _carCollisionXAcceleration = (averageAmountBelowRoad * surfaceValue) >> 8;

        // Y acceleration = force * cosx.cosz (y inclination never negative at the moment)
        _carCollisionYAcceleration = (averageAmountBelowRoad * surfaceCosXCosZ) >> 8;

        // Z acceleration = force * sinx
        surfaceValue = roadInclinationX < 0 ? surfaceSinX : -surfaceSinX;
        _carCollisionZAcceleration = (averageAmountBelowRoad * surfaceValue) >> 8;
    }

    private void CalculateTotalAcceleration()
    {
        _playerYAcceleration = _gravityYAcceleration + _carCollisionYAcceleration;

        // reduce engine z acceleration if car is accelerating and not travelling
        // backwards - simulates wind resistance / reduced ability to accelerate
        int reduction = ((_engineZAcceleration >> 8) | (PlayerZSpeed >> 8)) & 0xff;
        if ((reduction & 0x80) != 0x80 && (_engineZAcceleration & 0xff) != 0)
        {
            _engineZAcceleration -= reduction;
        }

        // limit engine z acceleration to (2 * car collision y acceleration) -
        // possibly prevents accelerating without enough grip
        int twiceY = GetTwiceCollisionYAcceleration();
        if (Math.Abs(_engineZAcceleration) >= twiceY)
        {
            if (_engineZAcceleration < 0)
            {
                twiceY = -twiceY;
            }

            _engineZAcceleration = twiceY;
        }

        _playerZAcceleration = _engineZAcceleration + _gravityZAcceleration + _carCollisionZAcceleration;

        CalculateXAcceleration();
    }

    private int GetTwiceCollisionYAcceleration()
        => TouchingRoad ? _carCollisionYAcceleration * 2 : 0;

    private void CalculateXAcceleration()
    {
        int acceleration = _gravityXAcceleration + _carCollisionXAcceleration;
        int speedDiff = acceleration - _playerXSpeed;

        int twiceY = GetTwiceCollisionYAcceleration();
        if (Math.Abs(speedDiff) >= twiceY)
        {
            if (_playerXSpeed < 0)
            {
                twiceY = -twiceY;
            }

            acceleration -= twiceY;
            _playerXAcceleration = acceleration;
        }
        else
        {
            _playerXAcceleration = _carCollisionXAcceleration - _playerXSpeed;
        }
    }

    // Allow the car to steer (affects player y angle and y rotation acceleration).
    private void CalculateSteering()
    {
        bool backwards = false;

        // find the piece that the car is currently on
        IdentifyPiece(PlayerX, PlayerZ, ref _steeringPiece);
        CurrentPiece = _steeringPiece;
        TrackPiece piece = _track.Pieces[_steeringPiece];

        int sectionSteeringAmount = piece.SteeringAmount;

        // calculate car x/z position relative to the piece
        CalcXZRelativeToPiece(PlayerX, PlayerZ, _steeringPiece, out int rx, out int rz);

        // calculate y angle of piece at the point where the centre of the car lies
        int sectionYAngle = CalcSectionYAngle(_steeringPiece, rx, rz);

        // temporarily reverse section y angle
        sectionYAngle = -sectionYAngle & (Track.MaxAngle - 1);

        // difference between the section and player's y angle:
        // increasingly negative turning right, increasingly positive turning left
        _yAngleDifference = sectionYAngle - PlayerYAngle;

        // make the difference range from -180 to 180 degrees
        if (_yAngleDifference > AmigaTrig.Degrees180)
        {
            _yAngleDifference -= AmigaTrig.Degrees360;
        }

        if (_yAngleDifference < -AmigaTrig.Degrees180)
        {
            _yAngleDifference += AmigaTrig.Degrees360;
        }

        // extra logic to allow car to drive round track in either direction
        // (backwards flag only applies to curves)
        if (_yAngleDifference > AmigaTrig.Degrees90)
        {
            _yAngleDifference -= AmigaTrig.Degrees180;
            backwards = true;
        }

        if (_yAngleDifference < -AmigaTrig.Degrees90)
        {
            _yAngleDifference += AmigaTrig.Degrees180;
            backwards = true;
        }

        // if player is on a curved section then adjust the difference angle
        bool leftHandBend = false;
        if (piece.Type is 0x80 or 0xc0)
        {
            // correctly identify left/right hand bend when driving backwards
            if ((piece.Type == 0x80) ^ piece.OppositeDirection ^ backwards)
            {
                // right hand bend
                _yAngleDifference += 217;
            }
            else
            {
                // left hand bend
                _yAngleDifference -= 217;
                leftHandBend = true;
            }
        }

        _differenceAngle = _yAngleDifference;
        _posDifferenceAngle = Math.Abs(_yAngleDifference);

        // save a scaled positive difference angle ranging from 0 to 0x7fff
        int scaledPosDifferenceAngle = _posDifferenceAngle < 0x800 ? _posDifferenceAngle << 4 : 0x7fff;

        if (_leftRightValue != 0)
        {
            // player is steering

            // work out if the positive difference angle is going to increase,
            // i.e. whether the car is trying to keep in line with the track
            bool increasing = (_differenceAngle < 0) ^ (_leftRightValue < 0);

            int steeringAmount;
            if (piece.Type is 0x80 or 0xc0)
            {
                // curve
                if ((_leftRightValue >= 0) ^ leftHandBend)
                {
                    // steering into the bend
                    steeringAmount = sectionSteeringAmount + 45;
                }
                else
                {
                    // steering away from bend
                    steeringAmount = sectionSteeringAmount - 35;

                    // left/right value below just used as +'ve/-'ve flag
                    _leftRightValue = leftHandBend ? -1 : 1;

                    // ensure steering assistance is not done
                    increasing = true;
                }
            }
            else
            {
                // straight
                steeringAmount = sectionSteeringAmount;
            }

            if (!increasing)
            {
                // add current difference (between player and road) onto steering
                // amount to assist steering when keeping in line with track
                steeringAmount += scaledPosDifferenceAngle >> 8;
            }

            CalculateSteeringAcceleration(steeringAmount);
        }
        else
        {
            // player is not steering
            _yAngleDifference = 0; // zero steering acceleration

            if (piece.Type is 0x00 or 0x40)
            {
                // straight
                AlignCarWithRoad();
                AdjustSteeringAcceleration();
            }
            else
            {
                // curve: left/right value below just used as +'ve/-'ve flag
                _leftRightValue = leftHandBend ? -1 : 1;

                // give effect of centrifugal force?
                CalculateSteeringAcceleration(sectionSteeringAmount);
            }
        }
    }

    private void CalculateSteeringAcceleration(int steeringAmount)
    {
        // steering acceleration increases with speed and is
        // calculated in a slightly odd way, to match Amiga StuntCarRacer
        int steeringAcceleration = (PlayerZSpeed * steeringAmount) >> 8;

        if (_leftRightValue < 0)
        {
            steeringAcceleration = -steeringAcceleration; // steering left
        }

        steeringAcceleration >>= 3;

        _yAngleDifference = steeringAcceleration;

        if (_posDifferenceAngle >= 30 * 256)
        {
            AlignCarWithRoad();
        }

        AdjustSteeringAcceleration();
    }

    private void AlignCarWithRoad()
    {
        // gradually bring the car back in line with the road
        int adjust = _posDifferenceAngle;

        if (adjust >= 256)
        {
            adjust -= 30 * 256;
            if (adjust >= 0)
            {
                // large adjustment for when the car is very out of line
                // (e.g. sideways with respect to road)
                if (_differenceAngle >= 0)
                {
                    PlayerYAngle += adjust;
                }
                else
                {
                    PlayerYAngle -= adjust;
                }

                return;
            }

            adjust = 255; // set adjustment amount to maximum
        }

        // adjustment of player's Y angle increases as player's speed increases
        int speed = Math.Abs(PlayerZSpeed) + 0xa00;
        if (speed > 0x7f00)
        {
            speed = 0x7f00;
        }

        adjust = (adjust * speed) >> 15;

        if (adjust == 0)
        {
            adjust = 1; // at least do some adjusting
        }

        if (_differenceAngle >= 0)
        {
            PlayerYAngle += adjust;
        }
        else
        {
            PlayerYAngle -= adjust;
        }
    }

    private void AdjustSteeringAcceleration()
    {
        int acceleration = _yAngleDifference - _playerYRotationSpeed;

        // steering is disabled when not touching the road
        _playerYRotationAcceleration = TouchingRoad ? acceleration : 0;
    }

    // Adds components of the player's (rotated) X, Y and Z accelerations
    // to give world acceleration values.
    private void CalculateWorldAcceleration()
    {
        _totalWorldXAcceleration = (_playerXAcceleration * _trig.XX) >> Track.LogPrecision;
        _totalWorldXAcceleration += (_playerYAcceleration * _trig.YX) >> Track.LogPrecision;
        _totalWorldXAcceleration += (_playerZAcceleration * _trig.ZX) >> Track.LogPrecision;

        _totalWorldYAcceleration = (_playerXAcceleration * _trig.XY) >> Track.LogPrecision;
        _totalWorldYAcceleration += (_playerYAcceleration * _trig.YY) >> Track.LogPrecision;
        _totalWorldYAcceleration += (_playerZAcceleration * _trig.ZY) >> Track.LogPrecision;

        _totalWorldZAcceleration = (_playerXAcceleration * _trig.XZ) >> Track.LogPrecision;
        _totalWorldZAcceleration += (_playerYAcceleration * _trig.YZ) >> Track.LogPrecision;
        _totalWorldZAcceleration += (_playerZAcceleration * _trig.ZZ) >> Track.LogPrecision;
    }

    private void ReduceWorldAcceleration()
    {
        int amount = 0;
        int factor = 1; // set maximum reduction factor
        bool normalSituation = true;

        if (TouchingRoad || _onChains)
        {
            amount = Math.Abs(_carToRoadCollisionZAcceleration >> 8);

            if (amount >= 3 || _offMapStatus != 0 || Wrecked || _onChains)
            {
                // collision z acceleration large, off map, wrecked or on chains
                if (Wrecked || _onChains)
                {
                    factor = 3; // set medium reduction factor
                }

                amount = 0x6000;
                normalSituation = false;
            }
        }

        // normal case - car not on chains, little Z collision with road
        if (normalSituation)
        {
            // reduce accelerations depending upon car speed:
            // get greatest of player's x, y and z speeds
            amount = Math.Abs(_playerXSpeed);

            int speedY = Math.Abs(_playerYSpeed);
            if (speedY > amount)
            {
                amount = speedY;
            }

            int speedZ = Math.Abs(PlayerZSpeed);
            if (speedZ > amount)
            {
                amount = speedZ;
            }

            factor = 5; // set minimum reduction factor

            // Slipstream: if player and opponent are in line left to right
            // and the opponent is in front, there is less drag on the player.
            if (PlayerCloseToOpponent && !OpponentBehindPlayer)
            {
                amount -= 20 * 128;
                if (amount < 0)
                {
                    amount = 0;
                }
            }
        }

        // reduce acceleration values using current speed values
        int reductionX = (_playerWorldXSpeed * amount) >> 16;
        int reductionY = (_playerWorldYSpeed * amount) >> 16;
        int reductionZ = (_playerWorldZSpeed * amount) >> 16;
        _totalWorldXAcceleration -= reductionX >> factor;
        _totalWorldYAcceleration -= reductionY >> factor;
        _totalWorldZAcceleration -= reductionZ >> factor;
    }

    // Damp the car X and Z angles to keep the car level with the road.
    private void CalculateXZRotationAcceleration()
    {
        // overall difference below road is effectively car X inclination;
        // front difference below road is effectively car Z inclination
        _playerXRotationAcceleration = _overallDifferenceBelowRoad - (_playerXRotationSpeed >> 4);
        if (TouchingRoad)
        {
            // lifts the car up at the front during forwards acceleration and,
            // vice versa, dips the front during backwards acceleration
            _playerXRotationAcceleration += _playerZAcceleration >> 2;
        }

        _playerZRotationAcceleration = _frontDifferenceBelowRoad - (_playerZRotationSpeed >> 4);
    }

    private void UpdatePlayersRotationSpeed()
    {
        _playerXRotationSpeed += (_playerXRotationAcceleration * Reduction) >> 8;
        _playerYRotationSpeed += (_playerYRotationAcceleration * Reduction) >> 8;
        _playerZRotationSpeed += (_playerZRotationAcceleration * Reduction) >> 8;
    }

    private void CalculateFinalRotationSpeed()
    {
        int sinX = AmigaTrig.Sin(PlayerXAngle);
        int sinZ = AmigaTrig.Sin(PlayerZAngle);
        int cosZ = AmigaTrig.Cos(PlayerZAngle);

        _playerFinalXRotationSpeed = (_playerXRotationSpeed * cosZ) >> Track.LogPrecision;
        _playerFinalXRotationSpeed += (_playerYRotationSpeed * -sinZ) >> Track.LogPrecision;

        _playerFinalYRotationSpeed = (_playerXRotationSpeed * sinZ) >> Track.LogPrecision;
        _playerFinalYRotationSpeed += (_playerYRotationSpeed * cosZ) >> Track.LogPrecision;

        // final Z rotation speed: rotate Y rotation speed about the X axis
        // and add it onto the Z rotation speed
        _playerFinalZRotationSpeed = _playerZRotationSpeed;
        _playerFinalZRotationSpeed += (_playerFinalYRotationSpeed * sinX) >> Track.LogPrecision;
    }

    private void UpdatePlayersWorldSpeed()
    {
        _playerWorldXSpeed += (_totalWorldXAcceleration * Reduction) >> 8;
        _playerWorldYSpeed += (_totalWorldYAcceleration * Reduction) >> 8;
        _playerWorldZSpeed += (_totalWorldZAcceleration * Reduction) >> 8;
    }

    private void UpdatePlayersPosition()
    {
        // set player's new position
        PlayerX += _playerWorldXSpeed * Reduction * Track.PcFactor;
        PlayerY += (_playerWorldYSpeed * Reduction) >> 1;
        PlayerZ += _playerWorldZSpeed * Reduction * Track.PcFactor;

        if (PlayerY >= 0x10000000)
        {
            PlayerY = 0x10000000;
        }

        // set player's new angles
        PlayerXAngle += (_playerFinalXRotationSpeed * Reduction) >> 8;
        PlayerYAngle += (_playerFinalYRotationSpeed * Reduction) >> 8;
        PlayerZAngle += (_playerFinalZRotationSpeed * Reduction) >> 8;

        // limit to valid range as no longer stored as words
        PlayerXAngle &= Track.MaxAngle - 1;
        PlayerYAngle &= Track.MaxAngle - 1;
        PlayerZAngle &= Track.MaxAngle - 1;

        // check player's X angle
        int limit;
        if (_atSideByte == 0xe0 && _smallerLimitRequired)
        {
            // all wheels off road and car on ground
            limit = 11 * 256;
        }
        else
        {
            limit = 45 * 256;
        }

        // get angle in Amiga StuntCarRacer format (i.e. correct sign)
        int angle = PlayerXAngle < AmigaTrig.Degrees180 ? PlayerXAngle : PlayerXAngle - AmigaTrig.Degrees360;

        if (Math.Abs(angle) > limit)
        {
            angle = angle >= 0 ? limit : -limit;

            // get player's x angle in PC StuntCarRacer format (i.e. correct sign)
            PlayerXAngle = angle > 0 ? angle : angle + AmigaTrig.Degrees360;

            if ((_playerXRotationSpeed >= 0 && angle < 0) || (_playerXRotationSpeed < 0 && angle >= 0))
            {
                _playerXRotationSpeed = 0; // values have different signs
            }
        }

        // check player's Z angle
        angle = PlayerZAngle < AmigaTrig.Degrees180 ? PlayerZAngle : PlayerZAngle - AmigaTrig.Degrees360;

        if (Math.Abs(angle) > limit)
        {
            angle = angle >= 0 ? limit : -limit;

            PlayerZAngle = angle > 0 ? angle : angle + AmigaTrig.Degrees360;

            if ((_playerZRotationSpeed >= 0 && angle < 0) || (_playerZRotationSpeed < 0 && angle >= 0))
            {
                _playerZRotationSpeed = 0; // values have different signs
            }
        }
    }

    // Engine revs calculation, used for the engine sound pitch (tested
    // against Amiga in the original).
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Security",
        "CA5394:Do not use insecure randomness",
        Justification = "Random fluctuation of the engine sound pitch only.")]
    private void UpdateEngineRevs()
    {
        int c;

        if (!TouchingRoad)
        {
            // not touching road, so test if joystick is held forwards or backwards
            c = 0;
            if (_accelerate || _brake)
            {
                c = 0x9000;
            }
        }
        else
        {
            c = PlayerZSpeed & ~0xf; // zero low four bits
            if (c < 0)
            {
                c = -c;
            }
        }

        c += 0x580;
        c >>= 3;
        if (EngineRevs < 192)
        {
            // if engine revs are low then increase them slowly (e.g. at race start)
            c = 2;
        }
        else
        {
            // otherwise calculate revs change depending on current engine revs
            c -= EngineRevs;
            c >>= 3;
        }

        _engineRevsChange = c;

        // now adjust revs change
        if (_engineRevsChange >= 0x100)
        {
            _engineRevsChange = 0x100;
        }
        else if (_engineRevsChange < 0)
        {
            // minimum change is -0x100 when touching the road, -0x20 otherwise
            int minimum = TouchingRoad ? -0x100 : -0x20;
            if (_engineRevsChange < minimum)
            {
                _engineRevsChange = minimum;
            }
        }

        EngineFluctuation = _random.Next() & 0xf;

        // apply the revs change (done by FramesWheelsEngine in the original)
        int revs = EngineRevs + _engineRevsChange;
        if (revs < 0)
        {
            revs = 0;
        }

        EngineRevs = revs;
    }
}
