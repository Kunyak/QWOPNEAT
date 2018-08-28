using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace QWOPNEAT.Behaviours
{


    [DataContract]
    class PlayerTestHuman : Behavior
    {

        private ControllerType ctrl = ControllerType.Player;

        [RequiredComponent]
        private Transform2D transform;

        private List<RevoluteJoint2D> joints;

        private Keys[] keys;

        [DataMember]
        public float MotorSpeed;

        [DataMember]
        public float MotorTorque;

        [DataMember]
        public float MaximumAngle;


        protected override void DefaultValues()
        {
            base.DefaultValues();
            MotorTorque = 500f;
            MotorSpeed = 50f;
            MaximumAngle = 15f;
        }

        protected override void Initialize()
        {
            base.Initialize();
            joints = new List<RevoluteJoint2D>();

            keys = new Keys[] { Keys.Q, Keys.A, Keys.W, Keys.S, Keys.E, Keys.D, Keys.R, Keys.F, Keys.T, Keys.G, Keys.Z, Keys.H, Keys.U, Keys.J, Keys.I, Keys.K }; // qwertzui -> moves legs clockwise; asdfghjk -> moves legs counterclovkw..

            getJoints(Owner);


        }

        private void getJoints(Entity _entity)
        {
            var parentJoints = _entity.FindComponents<RevoluteJoint2D>();
            joints.AddRange(parentJoints);

            foreach (RevoluteJoint2D joint in parentJoints)
            {
                Debug.WriteLine("Found joint", "jointfinder");
            }

            var childEntities = _entity.ChildEntities;

            foreach (Entity child in childEntities)
            {
                getJoints(child);
            }
          
        }

        protected override void Update(TimeSpan gameTime)
        {

            if (ctrl == ControllerType.Player)
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
        }
    }
}
