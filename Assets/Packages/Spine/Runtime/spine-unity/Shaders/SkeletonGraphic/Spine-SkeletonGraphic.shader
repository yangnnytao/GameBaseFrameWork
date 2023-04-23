// This is a premultiply-alpha adaptation of the built-in Unity shader "UI/Default" in Unity 5.6.2 to allow Unity UI stencil masking.

Shader "Spine/SkeletonGraphic"
// Shader "Spine/Skeleton24"

{
    Properties
    {

        [Toggle]_EffectEnabled ("特效开关", int) = 0
        _MainTex ("MainTex", 2D) = "white" { }
        BloomFactor ("BloomFactor", Range(0, 1)) = 0
        MainTex_Color ("MainTex_Color", Color) = (1, 1, 1, 1)
        ADDColor ("ADDColor", Color) = (0, 0, 0, 0)
        discoloration ("Discoloration", Range(0, 1)) = 0 //变成黑白
        [Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput ("Straight Alpha Texture", Int) = 0

        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255

        [HideInInspector]_ColorMask ("Color Mask", Float) = 15

        [HideInInspector][Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    SubShader
    {
        

        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            
            Name "Default"
            
            Blend One OneMinusSrcAlpha
            BlendOp Add, Add
            AlphaToMask Off
            Cull off
            ColorMask RGBA
            ZWrite Off
            ZTest LEqual
            Offset 0, 0
            
            CGPROGRAM

            #pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
            #pragma multi_compile _ _EFFECTENABLED_ON
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            

            struct appdata
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
                float4 color: COLOR;
            };
            
            struct v2f
            {
                float4 vertex: SV_POSITION;
                float2 uv: TEXCOORD0;
                fixed4 color: COLOR;
            };

            float4 MainTex_Color;
            sampler2D _MainTex;
            fixed discoloration;
            float4 ADDColor;
            

            
            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                
                float4 Maintexture1 = tex2D(_MainTex, i.uv)  ;

                
                // MainTex_Color = 1;
                // 特效开关
                #if _EFFECTENABLED_ON
                
                Maintexture1.rgb = lerp(Maintexture1.rgb, Luminance(Maintexture1.rgb), discoloration);

                Maintexture1 = (MainTex_Color * Maintexture1) + ADDColor  ;
                
            #endif


            #if defined(_STRAIGHT_ALPHA_INPUT)
                Maintexture1.rgb *= Maintexture1.a;
            #endif

            
            
            return Maintexture1 * i.color;
        }
        ENDCG

    }

    
    //Pass
    //{
    //    Name "Second"
    //    
    //    CGINCLUDE
    //    #pragma target 2.0
    //    ENDCG

    //    Blend SrcAlpha One
    //    BlendOp Min, Min
    //    AlphaToMask Off
    //    Cull Back
    //    ColorMask A
    //    ZWrite Off
    //    ZTest LEqual
    //    Offset 0, 0



    //    
    //    
    //    CGPROGRAM

    //    #pragma multi_compile _ _EFFECTENABLED_ON
    //    #pragma vertex vert
    //    #pragma fragment frag
    //    #include "UnityCG.cginc"
    //    

    //    struct appdata
    //    {
    //        float4 vertex: POSITION;
    //        float2 uv: TEXCOORD0;
    //    };
    //    
    //    struct v2f
    //    {
    //        float4 vertex: SV_POSITION;
    //        float2 uv: TEXCOORD0;
    //    };

    //    
    //    uniform sampler2D _MainTex;
    //    uniform float BloomFactor;

    //    
    //    v2f vert(appdata v)
    //    {
    //        v2f o;
    //        o.uv = v.uv;
    //        o.vertex = UnityObjectToClipPos(v.vertex);
    //        return o;
    //    }
    //    
    //    fixed4 frag(v2f i): SV_Target
    //    {
    //        
    //        
    //        float4 Maintex_a = tex2D(_MainTex, i.uv);

    //        //这个参数和C#脚本配合有问题，暂时注释掉，问题：加了C#控制后，变体开关失效，并且填充了alpha值导致alpha值有问题。
    //        // Maintex_a = float4(1, 1, 1, (Maintex_a.a + 1));//alpha通道要置灰否则会一直亮者

    //        // #if _EFFECTENABLED_ON // 特效开关
    //        Maintex_a = float4(1, 1, 1, (1.0 - (Maintex_a.a * BloomFactor)));
    //        // #endif

    //        return Maintex_a;
    //    }
    //    ENDCG

    //}
}
}
