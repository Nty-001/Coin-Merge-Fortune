using UnityEngine;
namespace CoinMerge.Recovery
{
    // World-space prefab, cached components, pooled by the board.
    public sealed class NativeMergeCoin : MonoBehaviour
    {
        public Rigidbody2D body;
        public CircleCollider2D solid,sensor;
        public SpriteRenderer visual;
        public int value {get;private set;}
        public bool IsPreview {get;private set;}
        public bool IsMerging {get;internal set;}
        public bool IsAlive {get;private set;}
        public int Generation {get;private set;}
        bool collisionPlayed;
        public float Radius=>definition.visualWidthPixels*.5f/board.Units;
        public float HalfHeight=>definition.visualHeightPixels*.5f/board.Units;
        public bool SpawnLocked=>spawnElapsed<spawnDuration;
        public Vector2 Position=>body.position;
        public NativeMergeBoard Board=>board;
        NativeMergeBoard board;
        CoinDefinition definition;
        int contacts;
        bool relaxOnContact,settling;
        float settleElapsed,spawnElapsed,spawnDuration;
        bool mergeAnimation;
        public void Initialize(NativeMergeBoard owner){board=owner;}
        public void Configure(int newValue, NativeMergeBoard owner)
        {Initialize(owner);Configure(newValue,Vector2.zero,false,false);}
        public void Configure(int newValue,Vector2 position,bool preview,bool animate,bool merged=false)
        {
            Generation++;collisionPlayed=false;
            definition=RecoveredGameRules.Coin(board.Config.rules,newValue);value=newValue;
            IsPreview=preview;IsMerging=false;IsAlive=true;contacts=0;settling=relaxOnContact=false;
            body.simulated=false;transform.localScale=Vector3.one;
            transform.position=new Vector3(position.x,position.y,board.transform.position.z);
            body.position=position;body.rotation=0;
            visual.sprite=board.SpriteFor(newValue);visual.color=Color.white;
            float radius=Mathf.Min(definition.radiusPixels,definition.visualWidthPixels*.5f);
            solid.radius=radius/board.Units;sensor.radius=(radius+board.Config.rules.physics.mergeSensorExtraRadius)/board.Units;
            var p=board.Config.rules.physics;
            solid.density=Mathf.Clamp(p.density*p.densityRadiusBase/Mathf.Max(1,radius),p.densityMin,p.densityMax);
            sensor.density=0;body.useAutoMass=true;
            spawnElapsed=0;mergeAnimation=merged;
            spawnDuration=animate?(merged?.2f:board.Config.rules.flow.spawnDuration):0;
            SetVisualScale(animate?board.Config.rules.flow.spawnInitialScale:1);
            gameObject.SetActive(true);
            if(!preview&&!(merged&&definition.upgrade==value))SetupPhysics(false);
        }
        public void Release(float distancePixels){IsPreview=false;SetupPhysics(true,distancePixels);}
        public void SetupPhysics(bool dropping,float distancePixels=-1)
        {
            var p=board.Config.rules.physics;
            bool effective=distancePixels>=0;
            float normalized=effective?Mathf.Min(1,Mathf.Max(distancePixels,p.minEffectiveDropDistance)/p.dropDistanceRange):0;
            contacts=0;settleElapsed=0;settling=false;relaxOnContact=dropping&&effective;
            body.bodyType=RigidbodyType2D.Dynamic;body.simulated=true;
            solid.enabled=true;sensor.enabled=true;
            body.gravityScale=relaxOnContact?p.dropGravityBase+normalized*p.dropGravityDistanceBonus:p.settleGravity;
            body.drag=relaxOnContact?Mathf.Max(p.minimumDropDamping,definition.dropDamping*p.dropDampingMultiplier):p.settleLinearDamping;
            body.angularDrag=p.angularDamping;
            body.collisionDetectionMode=relaxOnContact?CollisionDetectionMode2D.Continuous:CollisionDetectionMode2D.Discrete;
            body.velocity=dropping?new Vector2(0,-(effective?p.dropSpeedBase+normalized*p.dropSpeedDistanceBonus:p.dropSpeedFallback)/board.Units):Vector2.zero;
            if(!dropping)body.angularVelocity=0;
            body.WakeUp();
        }
        public void Tick(float dt)
        {
            if(SpawnLocked)
            {
                spawnElapsed+=dt;
                float t=Mathf.Clamp01(spawnElapsed/spawnDuration),start=board.Config.rules.flow.spawnInitialScale;
                if(mergeAnimation)
                    SetVisualScale(t<.8f?Mathf.LerpUnclamped(start,1.1f,BackOut(t/.8f)):Mathf.Lerp(1.1f,1,Mathf.Sin((t-.8f)*5*Mathf.PI*.5f)));
                else SetVisualScale(Mathf.LerpUnclamped(start,1,BackOut(t)));
                if(!SpawnLocked)SetVisualScale(1);
            }
            if(!settling)return;
            if(IsPreview||IsMerging||board.GameOver||contacts<=0){settleElapsed=0;return;}
            settleElapsed+=dt;
            if(settleElapsed<board.Config.rules.physics.contactSettleDelay)return;
            settling=false;settleElapsed=0;
            body.gravityScale=board.Config.rules.physics.settleGravity;body.drag=board.Config.rules.physics.settleLinearDamping;
        }
        static float BackOut(float t){float v=t-1;return 1+2.70158f*v*v*v+1.70158f*v*v;}
        void SetVisualScale(float scale){visual.transform.localScale=new Vector3(scale,scale,1);}
        void OnCollisionEnter2D(Collision2D contact)
        {
            if(!IsAlive||IsPreview||IsMerging||board.GameOver)return;
            contacts++;
            if(!collisionPlayed&&contacts==1){collisionPlayed=true;board.NotifyCoinContact(this);}
            if(relaxOnContact&&!board.IsSideWall(contact.collider))
            {relaxOnContact=false;settling=true;settleElapsed=0;body.collisionDetectionMode=CollisionDetectionMode2D.Discrete;}
            board.TryMergeContact(this,contact.collider);
        }
        void OnCollisionExit2D(Collision2D contact){contacts=Mathf.Max(0,contacts-1);}
        public bool IsStill(float velocityPixels,float angular)
        {
            if(!body.simulated||body.IsSleeping())return true;
            Vector2 v=body.velocity*board.Units;
            return Mathf.Abs(v.x)<=velocityPixels&&Mathf.Abs(v.y)<=velocityPixels&&Mathf.Abs(body.angularVelocity)<=angular;
        }
        public void Freeze(){body.simulated=false;IsMerging=false;settling=relaxOnContact=false;SetVisualScale(1);}
        public void Recycle()
        {IsAlive=false;IsMerging=false;IsPreview=false;body.simulated=false;gameObject.SetActive(false);}
    }
}
