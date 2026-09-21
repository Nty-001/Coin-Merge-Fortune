using UnityEngine;
namespace CoinMerge.Recovery
{
    public sealed class PackagedCoin : MonoBehaviour
    {
        public Rigidbody2D body;
        public PolygonCollider2D polygon;
        public SpriteRenderer visual;
        public PackagedCoinDefinition Definition {get;private set;}
        public int Type=>Definition.type;
        public bool alive,preview,merging;
        public int overFrames;
        public float animationLeft;
        PackagedBoard owner;int contacts;bool born;
        public void Configure(PackagedBoard board,int type,Vector2 position,bool isPreview,bool animate)
        {
            owner=board;Definition=board.balance.coins[type-1];alive=true;preview=isPreview;merging=false;overFrames=contacts=0;
            body.simulated=false;body.bodyType=RigidbodyType2D.Dynamic;transform.position=position;body.position=position;body.rotation=0;
            body.velocity=Vector2.zero;body.angularVelocity=0;body.gravityScale=board.balance.dropGravity;body.drag=0;body.angularDrag=0;
            polygon.SetPath(0,Definition.polygon);polygon.sharedMaterial=board.freshMaterial;body.useAutoMass=true;
            visual.sprite=board.SpriteFor(type);visual.enabled=true;
            born=animate;animationLeft=animate?.2f:0;
            SetScale(animate?0:1);gameObject.SetActive(true);body.simulated=!isPreview;
        }
        void SetScale(float scale)
        {
            var size=visual.sprite.bounds.size;
            visual.transform.localScale=new Vector3(Definition.size.x/owner.balance.units/size.x*scale,Definition.size.y/owner.balance.units/size.y*scale,1);
        }
        public void Tick(float dt)
        {
            if(animationLeft<=0)return;animationLeft=Mathf.Max(0,animationLeft-dt);
            float t=.2f-animationLeft;
            SetScale(!born?1:t<.1f?t*10:t<.15f?1+(t-.1f)*4:1.2f-(t-.15f)*4);
        }
        public bool CanMerge=>alive&&!preview&&!merging&&animationLeft<=.1f;
        void OnCollisionEnter2D(Collision2D collision)
        {
            if(!CanMerge||owner.GameOver)return;
            if(++contacts>3){polygon.sharedMaterial=owner.settledMaterial;body.drag=owner.balance.settledDrag;body.gravityScale=owner.balance.settledGravity;}
            owner.Contact(this,collision.collider);
        }
        public void Recycle(){alive=false;body.simulated=false;gameObject.SetActive(false);}
    }
}
