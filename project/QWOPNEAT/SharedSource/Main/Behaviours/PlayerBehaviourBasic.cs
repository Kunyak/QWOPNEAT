using QWOPNEAT.Classes;
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
    class PlayerBehaviourBasic : PlayerBehaviour
    {


        [DataMember]
        public float MotorTorque;

        [DataMember]
        public float MaximumAngle;


        protected override void DefaultValues()
        {
            base.DefaultValues();
            MotorTorque = 500f;
            MaximumAngle = 15f;
        }

        protected override void Initialize()
        {
            base.Initialize();

            foreach (RevoluteJoint2D joint in joints)
            {
                joint.MaxMotorTorque = MotorTorque;
                joint.UpperAngle = (float)(Math.PI / 180) * MaximumAngle; // conversion to RAD
                joint.LowerAngle = (float)(Math.PI / 180) * (-MaximumAngle);
                joint.EnableLimits = true;
            }


        }

 
    }
}
