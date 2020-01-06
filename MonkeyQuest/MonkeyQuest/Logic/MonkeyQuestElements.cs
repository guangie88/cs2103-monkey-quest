// Main Contributors: Everyone (fair share)

using System;
using System.Windows.Input;
using System.Diagnostics;
using System.Media;
using System.Collections.Generic;

using MonkeyQuest.Framework.Logic;
using MonkeyQuest.Framework.Utility;
using System.Windows.Media;

namespace MonkeyQuest.MonkeyQuest.Logic
{
    public class BorderTile : Tile
    {
        public override bool SetPosition(Position nextPosition)
        {
            // overrides the fact that you can't pass the border
            // because this tile represents border itself
            SetPositionDirectly(nextPosition);
            return true;
        }

        public BorderTile()
            : base("")
        {
            CanGroundJumpOn = true;
            CanWallJumpOn = false;
        }
    }

    #region Weapons

    public abstract class Rocket : BasicWeapon
    {

        public Rocket(RectangularMask rectangularMask = default(RectangularMask))
            : base(rectangularMask)
        {
            HomingSound = new SoundPlayer(@"Resources/Sfx/weapon_homing.wav");
            AIKilledSound = new SoundPlayer(@"Resources/Sfx/explosion.wav");
            HomingSound.PlayLooping();
        }
    }

    public class RocketLeft : Rocket
    {
        protected override void InitializeMovement()
        {
            base.InitializeMovement();
            AddMovement(new SlideLeftRight(Board.CalculateFineness(-0.1)));
        }

        public RocketLeft(RectangularMask rectangularMask = default(RectangularMask))
            : base(rectangularMask)
        {
        }
    }

    public class RocketRight : Rocket
    {
        protected override void InitializeMovement()
        {
            base.InitializeMovement();
            AddMovement(new SlideLeftRight(Board.CalculateFineness(0.1)));
        }

        public RocketRight(RectangularMask rectangularMask = default(RectangularMask))
            : base(rectangularMask)
        {
        }
    }

    #endregion

    #region AI

    public class Spike : NonKillableNonMovingAI
    {
        public Spike()
            : base(new RectangularMask(0.2, 0.2, 0.4, 0))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Enemy Killed.wav");
        }
    }

    public class SpikeCeiling : NonKillableNonMovingAI
    {
        public SpikeCeiling()
            : base(new RectangularMask(0.2, 0.2, 0, 0.4))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Enemy Killed.wav");
        }
    }

    public class Spider : BasicMovingAI
    {
        public override double MovementSpeedProportion
        {
            get { return 0.03; }
        }

        public Spider()
            : base(new RectangularMask(0.08, 0.08, 0.4, 0.1))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Die.wav");
        }
    }

    public class Crab : BasicMovingAI
    {
        public override double MovementSpeedProportion
        {
            get { return 0.04; }
        }

        public Crab()
            : base(new RectangularMask(0.08, 0.08, 0.5, 0.1))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Die.wav");
        }
    }

    public class Octopus : BasicMovingAI
    {
        public override double MovementSpeedProportion
        {
            get { return 0.04; }
        }

        public Octopus()
            : base(new RectangularMask(0.04, 0.04, 0.05, 0.05))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Die.wav");
        }
    }

    public class Tortoise : NonMovingGravityAffectedAI
    {
        public Tortoise()
            : base(new RectangularMask(0.04, 0.04, 0.5, 0.05))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Die.wav");
        }
    }

    public class TortoiseLeft : Tortoise
    {
    }

    public class Gaga : BasicMovingAI
    {
        public Gaga()
            : base(new RectangularMask(0.04, 0.04, 0.1, 0.0))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Gaga.wav");
        }
    }

    public class Ghost : BasicMovingAI
    {
        public Ghost()
            : base(new RectangularMask(0.04, 0.04, 0.1, 0.00))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Pacman.wav");
        }
    }

    public class Goomba : BasicMovingAI
    {
        public Goomba(MonkeyBoard board = null)
            : base(new RectangularMask(0.04, 0.04, 0.1, 0.00))
        {
            MonkeyKilledSound = new SoundPlayer(@"Resources/Sfx/Die.wav");
        }
    }

    #endregion

    public class Monkey : Controllable<MonkeyBoard>
    {
        private const int MoveInAirCooldownLimit = ReboundOffWall.StopLimit;
        private const int LeftWallJumpCooldownLimit = ReboundOffWall.StopLimit * 5;
        private const int RightWallJumpCooldownLimit = ReboundOffWall.StopLimit * 5;
        private const int WallFrictionCooldownLimit = 55;

        private const double DecayFactorLateralWallJump = 0.65;
        private const double DecayFactorVerticalWallJump = 0.90;

        private const double WallClingFactor = 0.6;
        private const double JumpHeightValue = 0.21;
        private const double LateralWallJumpHeightValue = 0.19;
        private const double VerticalWallJumpHeightValue = 0.22;
        private const double GravityValue = 0.02;
        private const double LateralReboundValue = 0.5;
        private const double VerticalReboundValue = 0.07;
        private const double MovementValue = 0.04;

        private static SoundPlayer jumpSound = new SoundPlayer(@"Resources/Sfx/Jump.wav");

        private int moveInAirCooldown = 0;
        private int leftWallJumpCooldown = 0;
        private int rightWallJumpCooldown = 0;

        private int weaponCooldown = 0;
        private const int weaponCooldownLimit = 50; //1sec
        private int weaponsLimit = 3;

        private bool isWallClinging = false;
        private bool isWallJumping = false;

        private bool hasLeftWallJumped = false;
        private bool hasRightWallJumped = false;

        private CollisionStatus lastCrateCollisionDirection;

        public bool IsWallClinging
        {
            get { return isWallClinging; }
            protected set { isWallClinging = value; }
        }

        public bool IsWallJumping
        {
            get { return isWallJumping; }
            protected set { isWallJumping = value; }
        }

        public bool HasLeftWallJumped
        {
            get { return hasLeftWallJumped; }
            protected set { hasLeftWallJumped = value; }
        }

        public bool HasRightWallJumped
        {
            get { return hasRightWallJumped; }
            protected set { hasRightWallJumped = value; }
        }

        public bool IsTryingToMoveLeft
        {
            get
            {
                foreach (IMovement movement in MovementList)
                    if (movement is MoveLeftRight && (movement as MoveLeftRight).MoveFinenessX < 0)
                        return true;

                return false;
            }
        }

        public bool IsTryingToMoveRight
        {
            get
            {
                foreach (IMovement movement in MovementList)
                    if (movement is MoveLeftRight && (movement as MoveLeftRight).MoveFinenessX > 0)
                        return true;

                return false;
            }
        }

        public CollisionStatus LastCrateCollisionDirection
        {
            get { return lastCrateCollisionDirection; }
            set { lastCrateCollisionDirection = value; }
        }

        protected internal override void BoardKeyDownMapperBind(object sender, BoardKeyEventArgs<MonkeyBoard> e)
        {
            // mechanism to make wall jumping smoother
            IsWallClinging = false;

            // left wall jumped cooldown mechanism
            if (HasLeftWallJumped)
                leftWallJumpCooldown++;

            if (leftWallJumpCooldown == LeftWallJumpCooldownLimit)
            {
                HasLeftWallJumped = false;
                leftWallJumpCooldown = 0;
            }

            // right wall jumped cooldown mechanism
            if (HasRightWallJumped)
                rightWallJumpCooldown++;

            if (rightWallJumpCooldown == RightWallJumpCooldownLimit)
            {
                HasRightWallJumped = false;
                rightWallJumpCooldown = 0;
            }

            if (IsOnGround())
            {
                // resets all wall jumping
                IsWallJumping = false;
                HasLeftWallJumped = false;
                HasRightWallJumped = false;

                moveInAirCooldown = 0;
                leftWallJumpCooldown = 0;
                rightWallJumpCooldown = 0;
            }
            else if (IsWallJumping)
            {
                moveInAirCooldown++;

                if (moveInAirCooldown == MoveInAirCooldownLimit)
                {
                    IsWallJumping = false;
                    moveInAirCooldown = 0;
                }
            }

            if (e.IsKeyDown(Key.Right) && !IsWallJumping)
            {
                //System.Diagnostics.Debug.WriteLine("Right key is pressed.");
                AddMovement(new MoveLeftRight(Board.CalculateFineness(MovementValue)));
            }
            if (e.IsKeyDown(Key.Left) && !IsWallJumping)
            {
                //System.Diagnostics.Debug.WriteLine("Left key is pressed.");
                AddMovement(new MoveLeftRight(-Board.CalculateFineness(MovementValue)));
            }
            if (e.IsKeyDown(Key.B))
            {
                if (weaponCooldown == 0)
                {
                    if (weaponsLimit-- < 1)
                        return;
                    weaponCooldown = weaponCooldownLimit;
                    Rocket r = new RocketRight();
                    r.SetPosition(this.X + 1, this.Y);
                    r.Board = Board;
                }
            }
            if (e.IsKeyDown(Key.V))
            {
                if (weaponCooldown == 0)
                {
                    if (weaponsLimit-- < 1)
                        return;
                    weaponCooldown = weaponCooldownLimit;
                    Rocket r = new RocketLeft();
                    r.SetPosition(this.X - 1, this.Y);
                    r.Board = Board;
                }
            }
            // beginner form
            #region Beginner Form

            if (e.IsKeyPressedOnce(Key.LeftShift) || e.IsKeyPressedOnce(Key.Space))
            {
                if (IsOnGround())
                {
                    // ground jumping
                    jumpSound.Play();
                    AddMovement(new Jump(-Board.CalculateFineness(JumpHeightValue)));
                }
                else if (!IsOnGround() && !HasLeftWallJumped && IsNextToWallJumpTile() == AdjacentStatus.TileOnLeft)
                {
                    if (e.IsKeyDown(Key.Up))
                    {
                        AddMovement(new Jump(-Board.CalculateFineness(VerticalWallJumpHeightValue)));
                        AddMovement(new ReboundOffWall(Board.CalculateFineness(VerticalReboundValue), DecayFactorVerticalWallJump));
                    }
                    else
                    {
                        AddMovement(new Jump(-Board.CalculateFineness(LateralWallJumpHeightValue)));
                        AddMovement(new ReboundOffWall(Board.CalculateFineness(LateralReboundValue), DecayFactorLateralWallJump));
                    }

                    // wall jumping, rebounding left wall
                    jumpSound.Play();

                    IsWallJumping = true;
                    moveInAirCooldown = 0;

                    HasLeftWallJumped = true;
                    HasRightWallJumped = false;
                }
                else if (!IsOnGround() && !HasRightWallJumped && IsNextToWallJumpTile() == AdjacentStatus.TileOnRight)
                {
                    if (e.IsKeyDown(Key.Up))
                    {
                        AddMovement(new Jump(-Board.CalculateFineness(VerticalWallJumpHeightValue)));
                        AddMovement(new ReboundOffWall(-Board.CalculateFineness(VerticalReboundValue), DecayFactorVerticalWallJump));
                    }
                    else
                    {
                        AddMovement(new Jump(-Board.CalculateFineness(LateralWallJumpHeightValue)));
                        AddMovement(new ReboundOffWall(-Board.CalculateFineness(LateralReboundValue), DecayFactorLateralWallJump));
                    }

                    // wall jumping, rebounding right wall
                    jumpSound.Play();

                    IsWallJumping = true;
                    moveInAirCooldown = 0;

                    HasRightWallJumped = true;
                    HasLeftWallJumped = false;
                }
            }

            #endregion

            if ((e.IsKeyDown(Key.Right) && IsNextToWallJumpTile() == AdjacentStatus.TileOnRight) ||
                (e.IsKeyDown(Key.Left) && IsNextToWallJumpTile() == AdjacentStatus.TileOnLeft))
            {
                IsWallClinging = true;
            }
        }

        private void BindToBoardTick(object sender, EventArgs e)
        {
            if (weaponCooldown > 0)
                weaponCooldown--;
        }

        // checks if monkey is on ground; used for implementing its jump functionality
        protected internal bool IsOnGround()
        {
            List<Collidable<MonkeyBoard>> collidableList = new List<Collidable<MonkeyBoard>>(FindCollidablesWithinRegion());

            foreach (Collidable<MonkeyBoard> collidable in collidableList)
            {
                // checks for things that it was supposed to be able to jump on
                if (collidable is Tile && (collidable as Tile).CanGroundJumpOn || collidable is Crate)
                {
                    int dummy;
                    Position groundPosition = Position.AddFineness(0, 1);
                    
                    if (IsCollidingInto(Position, groundPosition, RectangularMask, collidable, out dummy) != CollisionStatus.NoCollision)
                        return true;
                }
            }
            
            return false;
        }

        private enum AdjacentStatus { TileOnRight, TileOnLeft, None };

        private AdjacentStatus IsNextToWallJumpTile()
        {
            List<Collidable<MonkeyBoard>> collidableList = new List<Collidable<MonkeyBoard>>(FindCollidablesWithinRegion());

            foreach (Collidable<MonkeyBoard> collidable in collidableList)
            {
                // temporarily check for crate
                // we should create one interface/class for this special type of ground collision

                // No crates, since the crate wouldn't be able to provide requisite opposing force to kickstart the jump. Instead, the crate itself would move.
                if (collidable is Tile && (collidable as Tile).CanWallJumpOn)    
                {
                    int dummy;
                    
                    Position leftOfCurrentPosition = Position.AddFineness(-1, 0);
                    Position rightOfCurrentPosition = Position.AddFineness(1, 0);

                    if (PerformCollidingInto(Position, leftOfCurrentPosition, RectangularMask, collidable, out dummy) != CollisionStatus.NoCollision)
                        return AdjacentStatus.TileOnLeft;
                    else if (PerformCollidingInto(Position, rightOfCurrentPosition, RectangularMask, collidable, out dummy) != CollisionStatus.NoCollision)
                        return AdjacentStatus.TileOnRight;
                }
            }

            return AdjacentStatus.None ;
        }

        private void AllLivesUsed(object sender, MonkeyLivesEventArgs e)
        {
            Board.RemovePositional(this);
        }

        protected override void InitializeBindings()
        {
            base.InitializeBindings();

            Board.AllLivesUsed += AllLivesUsed;
            AddMovement(new Gravity(Board.CalculateFineness(GravityValue)));
            Board.SetNumberOfLives(3);
            Board.BoardTick += BindToBoardTick;
        }

        protected override void RemoveBindings()
        {
            Board.AllLivesUsed -= AllLivesUsed;
            base.RemoveBindings();
        }

        protected override void CollisionAfter(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (otherCollidable is Crate)
            {
                if (direction == CollisionStatus.CollisionFromLeft || direction == CollisionStatus.CollisionFromRight)
                    LastCrateCollisionDirection = direction;
                else if (direction == CollisionStatus.CollisionFromTop)
                {
                    Board.TempRemovePositional(this);

                    if (LastCrateCollisionDirection == CollisionStatus.CollisionFromLeft)
                        SetPosition(otherCollidable.X + 1, Y, 0, 0);
                    else
                        SetPosition(otherCollidable.X - 1, Y, 0, 0);

                    Board.RestorePositional(this);
                    Board.Dispatcher.Invoke((Action)(() => Board.DecreaseNumberOfLives()));
                }
            }
        }

        public override int MaxVelocityFinenessX
        {
            get { return Board.CalculateFineness(0.12); }
        }

        public override int MinVelocityFinenessX
        {
            get { return -Board.CalculateFineness(0.12); }
        }

        public override int MaxVelocityFinenessY
        {
            get { return Board.CalculateFineness(0.12); }
        }

        public override int MinVelocityFinenessY
        {
            get { return -Board.CalculateFineness(0.22); }
        }

        public Monkey()
            : base(new RectangularMask(0.12, 0.12, 0.10, 0.06))
        {
            // creates resistance to object's motion, which is friction
            AfterMotion += (object sender, MovableEventArgs<MonkeyBoard> e) =>
            {
                e.Movable.Velocity = new Velocity(0, e.Movable.Velocity.FinenessY);

                if (IsWallClinging && e.Movable.Velocity.FinenessY > 0)
                    e.Movable.Velocity = new Velocity(e.Movable.Velocity.FinenessX, (int)(e.Movable.Velocity.FinenessY * WallClingFactor));
            };
        }
    }

    public class Banana : Collidable<MonkeyBoard>
    {
        private static SoundPlayer eatSound = new SoundPlayer(@"Resources/Sfx/Banana Take.wav");

        public override MonkeyBoard Board
        {
            get { return base.Board; }
            protected internal set
            {
                base.Board = value;
                // increase the total number of bananas detected by board
                if (Board != null)
                    Board.IncrementBananaTotal();
            }
        }

        protected override CollisionStatus BeforeCollision(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            // display some debug message
            if (otherCollidable is Monkey)
            {
                // this will get board to remove positional, unbind its tick from given positional, and emit an event to boardUI to remove other aspects
                eatSound.Play();

                // this works because MonkeyBanana must take in only MonkeyBoard
                Board.Dispatcher.Invoke((Action)(() => Board.IncrementBananaCollected()));
                Board.RemovePositional(this);

                return CollisionStatus.NoCollision;
            }

            return direction;
        }

        protected override void CollisionAfter(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
        }

        public Banana()
            : base(new RectangularMask(0.12, 0.12, 0.30, 0.16))
        {
        }
    }

    public class Crate : Movable<MonkeyBoard>
    {
        private const int DelayTicks = 15;
        private const double CrateMoveValue = 0.05;

        private int maxDisplacement;

        private static SoundPlayer soundPlayer = new SoundPlayer("Resources/Sfx/Crate Move.wav");

        private bool startedPush = false;
        private int pushTick = 0;

        protected int PushTick
        {
            get { return pushTick; }
            set { pushTick = value; }
        }

        protected bool StartedPush
        {
            get { return startedPush; }
            set { startedPush = value; }
        }

        protected int MaxDisplacement
        {
            get { return maxDisplacement; }
            set { maxDisplacement = value; }
        }

        public override int MaxVelocityFinenessY
        {
            get { return Board.CalculateFineness(0.12); }
        }

        public override int MinVelocityFinenessY
        {
            get { return -Board.CalculateFineness(0.22); }
        }

        protected override void InitializeBindings()
        {
            base.InitializeBindings();

            Board.BoardTick += IncreaseTick;

            AddMovement(new Gravity(Board.CalculateFineness(0.02)));
            MaxDisplacement = Board.FinenessLimit * 2;
        }

        protected void IncreaseTick(object sender, EventArgs e)
        {
            if (StartedPush == true)
            {
                PushTick++;
                //Debug.WriteLine("lefttick: " + PushTick);
            }
            if (PushTick > DelayTicks)
            {
                StartedPush = false;
                PushTick = 0;
                //Debug.WriteLine("stop timer");
            }
        }

        private void SlideLeft()
        {
            AddMovement(new CrateMove(true, MaxDisplacement, Board.CalculateFineness(CrateMoveValue)));
        }

        private void SlideRight()
        {
            AddMovement(new CrateMove(false, MaxDisplacement, Board.CalculateFineness(CrateMoveValue)));
        }

        protected override void CollisionAfter(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (otherCollidable is Monkey)
            {
                Monkey monkey = otherCollidable as Monkey;

                if (!IsCurrentlySliding() && monkey.IsOnGround())
                {
                    if (direction == CollisionStatus.CollisionFromLeft && monkey.IsTryingToMoveRight ||
                        direction == CollisionStatus.CollisionFromRight && monkey.IsTryingToMoveLeft)
                    {
                        if (!StartedPush)
                        {
                            StartedPush = true;
                            //Debug.WriteLine("started push");
                        }
                        else if (PushTick == DelayTicks)
                        {
                            // this means that there is enough "push" on the crate
                            // and it should start moving

                            if (direction == CollisionStatus.CollisionFromLeft)
                                SlideRight();
                            else
                                SlideLeft();

                            // resets push tick
                            StartedPush = false;
                            PushTick = 0;

                            soundPlayer.Play();
                        }
                    }
                }
            }
        }

        public bool IsCurrentlySliding()
        {
            foreach (IMovement movement in MovementList)
                if (movement.GetType() == typeof(CrateMove))
                    return true;

            return false;
        }

        public Crate()
            : base(new RectangularMask())
        {
            // no need to create friction for it
        }
    }

    public class Door : Collidable<MonkeyBoard>
    {
        private static MediaPlayer doorOpenPlayer = new MediaPlayer();

        public delegate void DoorHandler(object sender, EventArgs e);

        public event DoorHandler DoorOpen;
        public event DoorHandler DoorClose;

        public event EventHandler MonkeyReachedDoor;

        public enum DoorState
        {
            Closed, Opened
        }

        private DoorState doorState;
        private bool isMonkeyatDoor = false;

        public DoorState CurrentDoorState
        {
            get { return doorState; }
            protected set { doorState = value; }
        }

        public bool IsMonkeyAtDoor
        {
            get { return isMonkeyatDoor; }
            protected set
            {
                isMonkeyatDoor = value;

                // triggers monkey reached door event so that the board package knows it's time to change map
                if (IsMonkeyAtDoor)
                    OnMonkeyReachedDoor(new EventArgs());
            }
        }

        protected override void InitializeBindings()
        {
            Board.BananaTotalCollect += OpenDoorBinding;
            MonkeyReachedDoor += Board.MonkeyReachedDoorBinding;

            base.InitializeBindings();
        }
        
        internal protected void OpenDoorBinding(object sender, BananasEventArgs e)
        {
            switch (CurrentDoorState)
            {
                case DoorState.Closed:

                    CurrentDoorState = DoorState.Opened;
                    CollidingInclusionSet.Add(typeof(Monkey));

                    // forces the sound file to be played at the start again
                    // by stopping it
                    doorOpenPlayer.Stop();
                    doorOpenPlayer.Play();

                    OnDoorOpen(new EventArgs());

                    //Debug.WriteLine("door opened!");
                    break;

                case DoorState.Opened:
                    break;
            }
        }

        internal protected void CloseDoorBinding(object sender, BananasEventArgs e)
        {
            switch (CurrentDoorState)
            {
                case DoorState.Closed:
                    break;

                case DoorState.Opened:

                    CurrentDoorState = DoorState.Closed;
                    CollidingInclusionSet.Remove(typeof(Monkey));

                    OnDoorClose(new EventArgs());

                    //Debug.WriteLine("door closed!");
                    break;
            }
        }

        protected virtual void OnDoorOpen(EventArgs e)
        {
            if (DoorOpen != null)
                DoorOpen(this, e);
        }

        protected virtual void OnDoorClose(EventArgs e)
        {
            if (DoorClose != null)
                DoorClose(this, e);
        }

        protected virtual void OnMonkeyReachedDoor(EventArgs e)
        {
            if (MonkeyReachedDoor != null)
                MonkeyReachedDoor(this, e);
        }

        protected override void CollisionAfter(Collidable<MonkeyBoard> otherCollidable, CollisionStatus direction)
        {
            if (otherCollidable is Monkey && !IsMonkeyAtDoor)
            {
                // culprit
                // can't fade out here and don't know why
                // should be something to do with threading
                // can't go past stage 2 also

                Board.Dispatcher.Invoke((Action)(() =>
                {
                    Board.OnBoardFadeOut(this);
                    IsMonkeyAtDoor = true;
                }));
            }
        }

        static Door()
        {
            doorOpenPlayer.Open(new Uri("Resources/Sfx/Door Open.wav", UriKind.Relative));
        }

        public Door()
            : base(new RectangularMask(0.49, 0.49, 0.49, 0.49))
        {
            CollidingMode = CollidingMode.Inclusion;
            CurrentDoorState = DoorState.Closed;
        }
    }
} 
            