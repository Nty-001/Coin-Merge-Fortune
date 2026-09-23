using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Clean colour plate + original silhouette + one source-driven rotating light.
    // Localized banknotes are separate native Images above this material.
    [RequireComponent(typeof(Image))]
    public sealed class RecoveredRewardLight : MonoBehaviour
    {
        public RectTransform sourceGlow;
        public string flatResource;
        public Vector2 lightCenter=new Vector2(.5f,.47f);
        public Vector2 lightSize=new Vector2(.85f,.85f);
        public Vector4 bodyRect=new Vector4(.09f,.08f,.91f,.69f);
        [Range(0,1)] public float strength=.8f;
        Image panel;Material instance,previous;Sprite lastSprite;
        static readonly int Rotation=Shader.PropertyToID("_GlowAngle"),Center=Shader.PropertyToID("_GlowCenter"),Size=Shader.PropertyToID("_GlowSize"),Body=Shader.PropertyToID("_BodyRect"),Strength=Shader.PropertyToID("_Strength"),Uv=Shader.PropertyToID("_SpriteUV");
        public float Angle=>sourceGlow?sourceGlow.localEulerAngles.z:0;
        void Awake(){panel=GetComponent<Image>();}
        void OnEnable()
        {
            if(!panel)panel=GetComponent<Image>();
            if(!instance){var template=Resources.Load<Material>("RewardMotion/Light");if(!template)return;instance=new Material(template){name="Reward light (instance)"};}
            var flat=Resources.Load<Texture2D>(flatResource);
            if(!flat)throw new System.InvalidOperationException("Missing clean reward plate: "+flatResource);
            instance.SetTexture("_FlatTex",flat);
            previous=panel.material;panel.material=instance;lastSprite=null;Apply();
        }
        void LateUpdate(){Apply();}
        public void Apply()
        {
            if(!instance||!panel.sprite)return;
            if(lastSprite!=panel.sprite)
            {
                lastSprite=panel.sprite;var r=lastSprite.textureRect;var t=lastSprite.texture;
                instance.SetVector(Uv,new Vector4(r.x/t.width,r.y/t.height,r.width/t.width,r.height/t.height));
                instance.SetVector(Center,lightCenter);instance.SetVector(Size,lightSize);instance.SetVector(Body,bodyRect);
            }
            instance.SetFloat(Rotation,Angle*Mathf.Deg2Rad);
            instance.SetFloat(Strength,strength);
        }
        void OnDisable(){if(panel&&panel.material==instance)panel.material=previous;}
        void OnDestroy(){if(instance)Destroy(instance);}
    }
}
