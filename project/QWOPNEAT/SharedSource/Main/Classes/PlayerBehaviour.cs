using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace QWOPNEAT.Classes
{
    enum ControllerType
    {
        Player,
        Neat
    }

    [DataContract]
    class PlayerBehaviour : Behavior
    {

        private ControllerType ctrl = ControllerType.Player;

        [RequiredComponent]
        private Transform2D transform;

        [RequiredComponent]
        private PolygonCollider2D baseCollider;

        protected List<RevoluteJoint2D> joints;

        protected Keys[] keys;

        [DataMember]
        public float MotorSpeed;

        public bool isAlive; 

        [DataMember]
        public Boolean GroundDeath;

        protected override void DefaultValues()
        {
            base.DefaultValues();
            MotorSpeed = 45f;
            GroundDeath = true;
        }

        private void Die()
        {
            Owner.IsActive = false;
            isAlive = false;
           // EntityManager.Remove(Owner);
        }

        protected override void Initialize()
        {
            base.Initialize();
            isAlive = true;
            joints = new List<RevoluteJoint2D>();

            keys = new Keys[] { Keys.Q, Keys.A, Keys.W, Keys.S, Keys.E, Keys.D, Keys.R, Keys.F, Keys.T, Keys.G, Keys.Z, Keys.H, Keys.U, Keys.J, Keys.I, Keys.K }; // qwertzui -> moves legs clockwise; asdfghjk -> moves legs counterclovkw..

            getJoints(Owner);

            baseCollider.BeginCollision += BaseCollider_BeginCollision;


        }


        protected virtual void getJoints(Entity _entity)
        {
            var parentJoints = _entity.FindComponents<RevoluteJoint2D>();
            joints.AddRange(parentJoints);

            var childEntities = _entity.ChildEntities;

            foreach (Entity child in childEntities)
            {
                getJoints(child);
            }

        }

        protected virtual void PlayerControll()
        {
            var keyboard = WaveServices.Input.KeyboardState;

            if (keyboard.IsConnected)
            {
                for (int i = 0; i < joints.Count; i++) // goint through the joints and assigning new keys
                {
                    if (keyboard.IsKeyPressed(keys[2 * i]) && keyboard.IsKeyReleased(keys[2 * i + 1])) // one assigned key is active
                    {
                        joints[i].MotorSpeed = MotorSpeed;
                        joints[i].EnableMotor = true;
                    }
                    else if (keyboard.IsKeyPressed(keys[2 * i + 1]) && keyboard.IsKeyReleased(keys[2 * i]))  // the other key is active
                    {
                        joints[i].MotorSpeed = -MotorSpeed;
                        joints[i].EnableMotor = true;
                    }
                    else // both or neither is active = dont move
                    {
                        joints[i].EnableMotor = false;
                    }
                }
            }
        }

        protected virtual void NeuralControll()
        {
            throw new NotImplementedException();
        }

        protected override void Update(TimeSpan gameTime)
        {

            switch (ctrl)
            {
                case ControllerType.Neat:
                    NeuralControll();
                    break;
                case ControllerType.Player:
                    PlayerControll();
                    break;
            }
        }



        private void BaseCollider_BeginCollision(WaveEngine.Common.Physics2D.ICollisionInfo2D contact)
        {

            //check hitting ground
            if (contact.ColliderA.CollisionCategories == WaveEngine.Common.Physics2D.ColliderCategory2D.Cat1 ||
                contact.ColliderB.CollisionCategories == WaveEngine.Common.Physics2D.ColliderCategory2D.Cat1)
            {
                if (GroundDeath)
                {

                    Die();
                }
            }

            // check finishing game

            if (contact.ColliderA.CollisionCategories == WaveEngine.Common.Physics2D.ColliderCategory2D.Cat3 ||
               contact.ColliderB.CollisionCategories == WaveEngine.Common.Physics2D.ColliderCategory2D.Cat3)
            {
                throw new NotImplementedException();
            }


        }


        private void CheckFall()
        {
            if (transform.Y > 1000)
            {
                Die();
            }
        }


    }
}
