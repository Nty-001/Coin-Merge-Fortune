Shader "CoinMerge/UI/RecoveredRewardLight"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture",2D)="white"{}
        _FlatTex("Clean colour plate without cash or rays",2D)="white"{}
        _Color("Tint",Color)=(1,1,1,1)
        _GlowTex("Original reward glow",2D)="white"{}
        _GlowAngle("Original rotation",Float)=0
        _GlowCenter("Center",Vector)=(.5,.47,0,0)
        _GlowSize("Size",Vector)=(.85,.85,0,0)
        _BodyRect("Body bounds",Vector)=(.09,.08,.91,.69)
        _SpriteUV("Sprite UV",Vector)=(0,0,1,1)
        _Strength("Strength",Range(0,1))=.8
        _StencilComp("Stencil Comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil Operation",Float)=0
        _StencilWriteMask("Stencil Write Mask",Float)=255
        _StencilReadMask("Stencil Read Mask",Float)=255
        _ColorMask("Color Mask",Float)=15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip",Float)=0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
        Stencil {Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask]}
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata{float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
            struct v2f{float4 vertex:SV_POSITION;fixed4 color:COLOR;float2 uv:TEXCOORD0;float4 local:TEXCOORD1;};
            sampler2D _MainTex,_FlatTex,_GlowTex;fixed4 _Color,_TextureSampleAdd;float4 _ClipRect,_GlowCenter,_GlowSize,_BodyRect,_SpriteUV;float _GlowAngle,_Strength;
            v2f vert(appdata v){v2f o;o.local=v.vertex;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 art=tex2D(_FlatTex,i.uv)+_TextureSampleAdd;
                art.a=tex2D(_MainTex,i.uv).a;
                float2 p=(i.uv-_SpriteUV.xy)/_SpriteUV.zw;
                float2 q=(p-_GlowCenter.xy)/_GlowSize.xy;
                float s=sin(_GlowAngle),c=cos(_GlowAngle);
                // Inverse sampling follows the source RectTransform's clockwise rotation.
                float2 lightUV=float2(c*q.x+s*q.y,-s*q.x+c*q.y)+.5;
                float rays=tex2D(_GlowTex,lightUV).a;
                float edge=min(min(p.x-_BodyRect.x,_BodyRect.z-p.x),min(p.y-_BodyRect.y,_BodyRect.w-p.y));
                float mask=smoothstep(0,.05,edge);
                // Light affects only the clean blue interior; no baked rays remain.
                mask*=smoothstep(.0,.035,art.b-art.g)*smoothstep(.4,.6,art.b)*smoothstep(.0,.035,art.b-art.r);
                mask*=1-smoothstep(.40,.56,length(q));
                art.rgb=lerp(art.rgb,fixed3(.98,1,1),rays*mask*_Strength);
                fixed4 color=art*i.color;
                #ifdef UNITY_UI_CLIP_RECT
                color.a*=UnityGet2DClipping(i.local.xy,_ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a-.001);
                #endif
                return color;
            }
            ENDCG
        }
    }
}
