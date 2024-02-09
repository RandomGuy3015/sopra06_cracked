using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.Options;
using System;
using static Swordsman_Saga.Engine.ObjectManagement.IUnit;

namespace Swordsman_Saga.GameElements.GameObjects.Units;



class Knight : IMelee
{
    public bool JustStartedMoving { get; set; }
    private HashSet<ICollidableObject> _alreadyCollidedWith = new HashSet<ICollidableObject>();

    public int Team { get; set; }
    public string Id { get; set; }
    public FightManager FightManager { get; set; }
    public StatisticsManager StatisticsManager { get; set; }
    public Texture2D Texture { get; set; }
    public Texture2D TextureIsSelected { get; set; }
    public Texture2D TextureAttacking { get; set; }
    public Texture2D TextureIsBeingAttacked { get; set; } 
    public Texture2D TextureIsAttacking { get; set; }
    public Texture2D TextureIsBeingHit { get; set; }
    
    public Rectangle TextureRectangle { get; set; }
    public Vector2 TextureOffset { get; set; }
    public Rectangle HitboxRectangle { get; set; }
    public Vector2 HitboxOffset { get; set; }
    public Vector2 Destination { get; set; }
    public List<Vector2> Path { get; set; }
    public Vector2 Goal { get; set; }

    public Vector2 PathingTo { get; set; }
    public Vector2 MoveOffsetInGroup { get; set; }
    public bool IsSelected { get; set; }
    public int CurrentFrame { get; set; }
    public float TimePassedSinceLastFrame { get; set; }
    public float Speed { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public int AttackRange { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public bool IsMoving { get; set; }
    public bool IsMovingInQueue { get; set; }
    public bool IsColliding { get; set; }
    public SoundManager Sound { get; set; }
    /*
    public bool IsAttacking { get; set; }
    public bool IsBeingAttacked { get; set; }
    public bool IsSelectedAsAttacker { get; set; }
    public bool IsSelectedAsTarget { get; set; }
    public bool IsStriking { get; set; }
    public bool IsBeingHit { get; set; }
    */
    public int Action { get; set; }
    public bool IsDead { get; set; }
    public bool Attacked { get; set; }
    public bool FlipTexture { get; set; }
    public bool PreventCollision { get; set; } = true;
    public int CollisionCount { get; set; }
    public DateTime LastCollisionTime { get; set; }
    public HashSet<ICollidableObject> AlreadyCollidedWith
    {
        get { return _alreadyCollidedWith; }
    }

    public Knight(string id, int x, int y, int player, DynamicContentManager contentManager, FightManager fightManager, StatisticsManager statisticsManager)
    {
        if (id == null)
        {
            ((IGameObject)this).GenerateGuid();
        }
        else
        {
            Id = id;
        }
        FightManager = fightManager;
        StatisticsManager = statisticsManager;
        Action = (int)IUnit.ActionId.Standing;
        Position = new Vector2(x, y);
        ((IGameObject)this).InitializeRectangles(100, 100, 50, 50);
        Destination = new Vector2(x, y);
        Path = new List<Vector2>();
        // nur irgendwelche Beispielwerte
        Speed = 0.2f;
        Damage = 10;
        MaxHealth = 150;
        Health = MaxHealth;
        AttackRange = 100;
        Team = player;
        IsDead = false;
        IsMovingInQueue = false;
        Attacked = true;
        TextureIsSelected = contentManager.Load<Texture2D>("2DAssets/GameObjectAssets/Units/friendly_knight_is_selected");
        Texture = contentManager.Load<Texture2D>("2DAssets/GameObjectAssets/Units/" + (player == 0 ? "friendly" : "enemy") + "_knight");
    }
    void IDataPersistence.SaveData(ref GameData data)
    {
        if (data.mHealth.ContainsKey(Id))
        {
            data.mHealth.Remove(Id);
        }
        if (data.mObjectPosition.ContainsKey(Id))
        {
            data.mObjectPosition.Remove(Id);
        }

        if (data.mGameObjects.ContainsKey(Id))
        {
            data.mGameObjects.Remove(Id);
        }
        if (data.mUnitPlayer.ContainsKey(Id))
        {
            data.mUnitPlayer.Remove(Id);
        }
        if (data.mFlipTexture.ContainsKey(Id))
        {
            data.mFlipTexture.Remove(Id);
        }
        if (data.mAttacked.ContainsKey(Id))
        {
            data.mAttacked.Remove(Id);
        }
        if (data.mIsDead.ContainsKey(Id))
        {
            data.mIsDead.Remove(Id);
        }
        if (data.mAction.ContainsKey(Id))
        {
            data.mAction.Remove(Id);
        }
        if (data.mTimePastSinceLastFrame.ContainsKey(Id))
        {
            data.mTimePastSinceLastFrame.Remove(Id);
        }
        if (data.mCurrentFrame.ContainsKey(Id))
        {
            data.mCurrentFrame.Remove(Id);
        }
        if (data.mGoals.ContainsKey(Id))
        {
            data.mGoals.Remove(Id);
        }
        if (data.mPaths.ContainsKey(Id))
        {
            data.mPaths.Remove(Id);
        }
        if (data.mJustStartedMoving.ContainsKey(Id))
        {
            data.mJustStartedMoving.Remove(Id);
        }
        if (data.mIsColliding.ContainsKey(Id))
        {
            data.mIsColliding.Remove(Id);
        }
        if (data.mIsMoving.ContainsKey(Id))
        {
            data.mIsMoving.Remove(Id);
        }
        if (data.mPathingTo.ContainsKey(Id))
        {
            data.mPathingTo.Remove(Id);
        }

        if (data.mIsMovingInQueue.ContainsKey(Id))
        {
            data.mIsMovingInQueue.Remove(Id);
        }
        if (data.mUnitDestination.ContainsKey(Id))
        {
            data.mUnitDestination.Remove(Id);
        }

        data.mUnitDestination.Add(Id, Destination);
        data.mIsMovingInQueue.Add(Id, IsMovingInQueue);
        data.mIsMoving.Add(Id, IsMoving);
        data.mIsColliding.Add(Id, IsColliding);
        data.mJustStartedMoving.Add(Id, JustStartedMoving);
        data.mPaths.Add(Id, Path);
        data.mGoals.Add(Id, Goal);
        data.mPathingTo.Add(Id, PathingTo);
        data.mCurrentFrame.Add(Id, CurrentFrame);
        data.mTimePastSinceLastFrame.Add(Id, TimePassedSinceLastFrame);
        data.mAction.Add(Id, Action);
        data.mIsDead.Add(Id, IsDead);
        data.mAttacked.Add(Id, Attacked);
        data.mFlipTexture.Add(Id, FlipTexture);
        data.mUnitPlayer.Add(Id, Team);
        data.mGameObjects.Add(Id, this.GetType().Name);
        data.mHealth.Add(Id, Health);
        data.mObjectPosition.Add(Id, Position);
    }

    public void Die()
    {
        if (Action != (int)ActionId.Dying)
        {
            Action = (int)ActionId.Dying;
            CurrentFrame = 0;
            FightManager.RemoveAttacker(this);
        }
    }
}
