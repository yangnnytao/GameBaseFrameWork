Shader "Spine/Skeleton_battle"
{
    
    //必须是以上路径才能实现叠加效果，具体原因不明，也许是这个版本运行库的一个bug
    // Shader "MGame/3D/Sprites/Default"
    // {
        Properties
        {
            [NoScaleOffset] _MainTex ("Sprite Texture", 2D) = "white" { }
            _Color ("基础颜色叠加属性", Color) = (1, 1, 1, 1)
            //因为是通过SetGlobal控制变量的，由于用了setGloal，所以shader里面Properties初始值会被注释掉并且不会在shader面板式显示，初始值的控制权移交给了class的public，
            //但游戏没有挂这个脚本的场景因为没有加载对应C#，没有值会变成系统默认值会变成0，模型会变透明或者颜色不正常,所以加了_DissolveEnabled，让其初始值就是我们想要的值，故加了"颜色曝光属性2"。
            [Toggle]_DissolveEnabled ("Dissolve Enabled", int) = 0
            // FlickMultColor ("颜色曝光属性", Color) = (0,0,0,1)
            _FlickMultColor2 ("颜色曝光属性2", Color) = (0, 0, 0, 1)
            [Space(30)]
            _FlickerColor ("闪白的颜色", Color) = (0, 0, 0, 1)
            _FlickValue ("闪白", Range(0, 1)) = 0
            [Space(20)]
            [Header(Depth FOG)]
            [Space(8)]
            // jinshengColor ("景深雾的颜色", Color) = (1,1,1,0)
            [HideInInspector]_jinshengrotate ("景深雾的旋转", Float) = 0.5
            
            // [Space(20)]
            // [Toggle]_PcossessedColorEnabled ("正常乐姬打勾_魔化乐姬关闭", int) = 0
            // _PcossessedColor ("魔化乐姬叠加属性", Color) = (0.78,0.79,0.87,1)
            // jinshengx ("景深雾X轴", Float) = 0
            // jinshengy ("景深雾Y轴", Float) = 0
            // [Space(20)]
            // [Header(Height FOG)]
            // [Space(8)]
            // gaoduColor ("高度雾气颜色", Color) = (1,1,1,0)
            // gaodux ("高度雾X轴", Float) = 0
            // gaoduy ("高度雾Y轴", Float) = 0

        }

        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }

            Fog
            {
                Mode Off
            }
            Cull Off
            ZWrite Off
            Blend One OneMinusSrcAlpha
            Lighting Off

            Pass
            {
                CGPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag
                #pragma target 2.0
                #pragma multi_compile _ _DISSOLVEENABLED_ON
                
                #include "UnityCG.cginc"


                struct appdata
                {
                    float4 vertex: POSITION;
                    float4 color: COLOR;
                    float2 uv: TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos: SV_POSITION;
                    fixed4 color: COLOR;
                    float2 uv: TEXCOORD0;
                    float4 worldPosition: TEXCOORD1;
                };

                sampler2D _MainTex;
                fixed4 _FlickerColor;
                fixed _FlickValue;
                fixed4 _Color;
                fixed4 jinshenMColor;
                fixed4 jinshenAColor;
                fixed4 gaoduMColor;
                fixed4 range;

                v2f Vert(appdata v)
                {
                    v2f o = (v2f) 0;
                    o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;

                    return o;
                }

                fixed4 Frag(v2f i): SV_Target
                {
                    //     fixed4 _FlickMultColorMix = _FlickMultColor2;
                    float4 rawColor = tex2D(_MainTex, i.uv) * _Color ;

                    #if _DISSOLVEENABLED_ON
                    // 前后渐变算法，worldposition.z值减去worldposition.y值获得一个斜坡渐变然后通过_range和_aroud控制渐变位置和虚化层度
                    float mask = saturate((i.worldPosition.z * 1.73 - i.worldPosition.y) * range.r + range.g);
                    //mask加上景深颜色得到景深控制的颜色。
                    float3 maskcolor = saturate(mask + jinshenMColor.a + jinshenMColor.rgb);
                    //高度雾算法
                    float mask1 = saturate(i.worldPosition.y + range.b) ;
                    // mask加上高度雾颜色得到叫上控制的颜色。
                    float3 mask1color = saturate(mask1 + gaoduMColor.a + gaoduMColor.rgb);

                    rawColor = float4(rawColor.rgb * maskcolor * mask1color + jinshenAColor.rgb, rawColor.a);
                #endif
                
                //闪白算法
                fixed3 shanbai = lerp(rawColor.rgb, _FlickerColor.rgb, _FlickValue);


                fixed4 shanbaicolor2 = fixed4(shanbai, rawColor.a) ;

                shanbaicolor2.rgb *= shanbaicolor2.a ;


                return shanbaicolor2 * i.color;
            }


            ENDCG

        }
    }
}
