using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace QWOPNEAT.Behaviours
{
    [DataContract]
    class CameraFollowBehaviour : Behavior
    {

        private const int BaseX = 640;

        private Transform2D Tracked;

        [RequiredComponent]
        private Transform2D trans;

        


        [DataMember]
        public float Speed { get; set; }

        [DataMember]
        public float Threshold { get; set; }



        protected override void DefaultValues()
        {
            base.DefaultValues();
            Speed = 5;
            Threshold = 25;

        }

        protected override void Update(TimeSpan gameTime)
        {

            //get all trackable entities in the scene
            var entities = EntityManager.FindAllByTag("trackable");

            
            if (entities != null)
            { 
                //gets the farthest entity
                foreach (Entity entity in entities)
                {
                    Transform2D _trans = entity.FindComponent<Transform2D>();
                    if (Tracked == null || Tracked.X < _trans.X)
                    {
                        Tracked = _trans;
                    }
                }
                // starts moving to that entity
                if (Tracked != null)
                {
                    var diff = Tracked.X - trans.X;
                    if (Math.Abs(diff) > Threshold)
                    {
                        trans.X += (diff / Math.Abs(diff)) * Speed;
                    }
                   
                }
                else
                {
                    // if no entity is found the go to base pont
                    trans.X = BaseX;
                }
            }
        }
    }
}
