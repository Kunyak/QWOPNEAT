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

    enum ControllerType
    {
        Player,
        Neat
    }

    [DataContract]
    class PlayerBehaviourBasic : Behavior
    {

        private ControllerType ctrl = ControllerType.Player;

        [RequiredComponent]
        private Transform2D transform;

        [RequiredComponent]
        private RevoluteJoint2D[] joints;

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

            keys = new Keys[] {Keys.Q, Keys.A, Keys.W, Keys.S, Keys.E, Keys.D, Keys.R, Keys.F, Keys.T, Keys.G, Keys.Z, Keys.H }; // qwertz -> moves legs clockwise; asdfgh -> moves legs counterclovkw..

            foreach (RevoluteJoint2D joint in joints)
            {
                joint.MaxMotorTorque = MotorTorque;
                joint.UpperAngle = (float)(Math.PI / 180) * MaximumAngle; // conversion to RAD
                joint.LowerAngle = (float)(Math.PI / 180) * (-MaximumAngle);
                joint.EnableLimits = true;
            }


        }

        protected override void Update(TimeSpan gameTime)
        {

            if (ctrl == ControllerType.Player)
            {
                var keyboard = WaveServices.Input.KeyboardState;

                if (keyboard.IsConnected)
                {
                    for (int i = 0; i < joints.Length; i++) // goint through the joints and assigning new keys
                    {
                        if (keyboard.IsKeyPressed(keys[2*i]) && keyboard.IsKeyReleased(keys[2*i+1])) // one assigned key is active
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
