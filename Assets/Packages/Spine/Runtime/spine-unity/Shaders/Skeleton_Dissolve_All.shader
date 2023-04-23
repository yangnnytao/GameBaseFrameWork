Shader "Spine/Skeleton_Dissolve_All"
{
    Properties
    {
        [Header(Base)]
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("MainTex", 2D) = "white" { }
        
        

        [Space(60)]
        [Header(Noise)]
        _Noise ("Noise", 2D) = "white" { }
        _Scale ("Scale", Range(0, 5)) = 1

        [Space]
        [Header(Dissolve)]
        _DisAmount ("DisAmount", Range(-2, 2)) = 0.0
        [Space(30)]

        [Toggle] _NoiseEnabled ("NoiseEnabled", int) = 0
        [Toggle(INVERT)] _INVERT ("INVERT", Float) = 1
        
        _DissolveColorWidth ("DissolveColorWidth", Range(0, 2)) = 0.01
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 2)) = 0.05
        [Space(30)]
        _DisColor ("DisColor", Color) = (1, 1, 0, 1)
        _BloomFactor ("BloomFactor", Range(0, 1)) = 0.0
    }


    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }

        LOD 600
        Pass
        {
            Cull Off
            ZWrite Off
            ColorMask RGB
            Blend One OneMinusSrcAlpha
            Lighting Off
            CGPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _NOISEENABLED_ON
            #pragma multi_compile _ INVERT
            #include "UnityCG.cginc"
            


            float _DisAmount, _Scale;
            sampler2D _MainTex, _Noise;
            float _DissolveColorWidth, _Cutoff, _Smoothness;
            fixed4 _Color, _DisColor;
            
            struct appdata
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
            };

            struct v2f
            {
                float4 pos: SV_POSITION;
                float2 uv: TEXCOORD0; // _MainTex
                float4 worldPos: TEXCOORD1;
            };


            // vertex shader
            v2f vert(appdata v)
            {
                
                v2f o = (v2f)0;
                // 本地空间转换到世界空间
                #if _NOISEENABLED_ON
                o.pos = mul(unity_ObjectToWorld, v.vertex.xyz);//上下溶解使用的空间变换，如果关闭那么将使用普通溶解

            #endif
            //美术上直观说法： O.Pos.y获得了一张世界空间黑白图通过加上_DisAmount可以Y轴上的黑白区域，上下游走
            float dispPos = (o.pos.y + _DisAmount);

            #if INVERT //同上，进行上下翻转
                
                dispPos = 1 - (o.pos.y + _DisAmount);

            #endif
            //因为本地空间转换到世界空间最有用的其实是Y轴，因为Y轴要做上下cilp
            o.worldPos.w = o.pos.y;
            
            o.pos = UnityObjectToClipPos(v.vertex);//转换到其次裁剪空间，主贴图使用
            o.uv = v.uv;//UV传出去
            
            //模型本地空间转换到世界空间
            o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;//溶解贴图使用
            
            return o;
        }



        // fragment shader
        fixed4 frag(v2f i): SV_Target
        {
            
            //采样主纹理贴图
            fixed4 c = tex2D(_MainTex, i.uv) * _Color;

            float3 adjustedworldpos = i.worldPos.xyz;//产生的效果类似渐变

            
            float3 zn = tex2D(_Noise, adjustedworldpos.xy * _Scale);

            // // 将所有侧面拼接在一起，形成niose纹理
            float3 noisetexture = zn;
            
            //noise贴图的R通道
            float noise = noisetexture.r;

            // 沿着Y轴进行上升或者下降,custompack0.y是锁渐变的一张图，上下左右移动是渐变是锁住的，会根据你的移动而移动
            float MovingPosOnModel = _DisAmount + i.worldPos.w;
            // 乘上noise贴图
            MovingPosOnModel *= noise;

            // 发光长度
            float maintexturePart = smoothstep(0, _Smoothness, MovingPosOnModel - _DissolveColorWidth);
            maintexturePart = step(_Cutoff, maintexturePart);

            // normal贴图部分
            float glowingPart = smoothstep(0, _Smoothness, MovingPosOnModel);
            glowingPart = step(_Cutoff, glowingPart);
            // 取出正常纹理部分
            glowingPart *= (1 - maintexturePart);

            #if INVERT   //上下翻转

                // 发光贴贴图长度
                maintexturePart = 1 - smoothstep(0, _Smoothness, MovingPosOnModel + _DissolveColorWidth);
                maintexturePart = step(_Cutoff, maintexturePart);

                // normal贴图部分
                glowingPart = 1 - smoothstep(0, _Smoothness, MovingPosOnModel);
                glowingPart = step(_Cutoff, glowingPart);
                // 取出正常纹理部分
                glowingPart *= (1 - maintexturePart);
            #endif
            

            // cilp的边缘是白色的，然后乘上_DisColor，获得乘上后的颜色
            float4 glowingColored = glowingPart * _DisColor  ;

            // 丢弃像素而不是溶解
            clip((maintexturePart + glowingPart) - 0.01);

            // // 主贴图*颜色贴图
            float4 mainTexture = lerp(c, glowingColored, glowingPart);
            //预乘,因为我们战斗小人spine是没有预乘的。
            mainTexture.rgb *= mainTexture.a;
            return mainTexture * c.a;
        }

        ENDCG

    }
    
    Pass
    {
        ColorMask A
        Blend SrcAlpha One
        BlendOp Min, Min
        AlphaToMask Off
        Cull Back
        ZWrite Off
        ZTest LEqual
        Offset 0, 0

        CGPROGRAM

        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile _ _NOISEENABLED_ON
        #pragma multi_compile _ INVERT
        #include "UnityCG.cginc"
        


        float _DisAmount, _Scale;
        sampler2D _MainTex, _Noise;
        float _DissolveColorWidth, _Cutoff, _Smoothness, _BloomFactor;
        fixed4 _Color, _DisColor;
        
        struct appdata
        {
            float4 vertex: POSITION;
            float2 uv: TEXCOORD0;
        };

        struct v2f
        {
            float4 pos: SV_POSITION;
            float2 uv: TEXCOORD0; // _MainTex
            float4 worldPos: TEXCOORD1;
        };


        // vertex shader
        v2f vert(appdata v)
        {
            
            v2f o = (v2f)0;
            // 本地空间转换到世界空间
            #if _NOISEENABLED_ON
                o.pos = mul(unity_ObjectToWorld, v.vertex.xyz);//上下溶解使用的空间变换，如果关闭那么将使用普通溶解
            #endif

            //美术上直观说法： O.Pos.y获得了一张世界空间黑白图通过加上_DisAmount可以Y轴上的黑白区域，上下游走
            float dispPos = (o.pos.y + _DisAmount);

            #if INVERT //同上，进行上下翻转
                dispPos = 1 - (o.pos.y + _DisAmount);
            #endif
            //因为本地空间转换到世界空间最有用的其实是Y轴，因为Y轴要做上下cilp
            o.worldPos.w = o.pos.y;
            
            o.pos = UnityObjectToClipPos(v.vertex);//转换到其次裁剪空间，主贴图使用
            o.uv = v.uv;//UV传出去
            
            //模型本地空间转换到世界空间
            o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;//溶解贴图使用
            
            return o;
        }



        // fragment shader
        fixed4 frag(v2f i): SV_Target
        {
            
            //采样主纹理贴图
            fixed4 c = tex2D(_MainTex, i.uv) * _Color;

            float3 adjustedworldpos = i.worldPos.xyz;//产生的效果类似渐变

            
            float3 zn = tex2D(_Noise, adjustedworldpos.xy * _Scale);

            // // 将所有侧面拼接在一起，形成niose纹理
            float3 noisetexture = zn;
            
            //noise贴图的R通道
            float noise = noisetexture.r;

            // 沿着Y轴进行上升或者下降,custompack0.y是锁渐变的一张图，上下左右移动是渐变是锁住的，会根据你的移动而移动
            float MovingPosOnModel = _DisAmount + i.worldPos.w;
            // 乘上noise贴图
            MovingPosOnModel *= noise;

            // 发光长度
            float maintexturePart = smoothstep(0, _Smoothness, MovingPosOnModel - _DissolveColorWidth);
            maintexturePart = step(_Cutoff, maintexturePart);

            // normal贴图部分
            float glowingPart = smoothstep(0, _Smoothness, MovingPosOnModel);
            glowingPart = step(_Cutoff, glowingPart);
            // 取出正常纹理部分
            glowingPart *= (1 - maintexturePart);

            #if INVERT   //上下翻转

                // 发光贴贴图长度
                maintexturePart = 1 - smoothstep(0, _Smoothness, MovingPosOnModel + _DissolveColorWidth);
                maintexturePart = step(_Cutoff, maintexturePart);

                // normal贴图部分
                glowingPart = 1 - smoothstep(0, _Smoothness, MovingPosOnModel);
                glowingPart = step(_Cutoff, glowingPart);
                // 取出正常纹理部分
                glowingPart *= (1 - maintexturePart);
            #endif
            

            // cilp的边缘是白色的，然后乘上_DisColor，获得乘上后的颜色
            float4 glowingColored = glowingPart * _DisColor * _BloomFactor  ;
            float4 glowingColoredfull = 1 - float4(glowingColored.rgb, glowingColored.r * c.a);
            
            return glowingColoredfull;
        }

        ENDCG

    }
}

SubShader
{
    Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }

    LOD 200
    Pass
    {
        Cull Off
        ZWrite Off
        ColorMask RGB
        Blend One OneMinusSrcAlpha
        Lighting Off
        CGPROGRAM

        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile _ _NOISEENABLED_ON
        #pragma multi_compile _ INVERT
        #include "UnityCG.cginc"
        


        float _DisAmount, _Scale;
        sampler2D _MainTex, _Noise;
        float _DissolveColorWidth, _Cutoff, _Smoothness;
        fixed4 _Color, _DisColor;
        
        struct appdata
        {
            float4 vertex: POSITION;
            float2 uv: TEXCOORD0;
        };

        struct v2f
        {
            float4 pos: SV_POSITION;
            float2 uv: TEXCOORD0; // _MainTex
            float4 worldPos: TEXCOORD1;
        };


        // vertex shader
        v2f vert(appdata v)
        {
            
            v2f o = (v2f)0;
            // 本地空间转换到世界空间
            #if _NOISEENABLED_ON
                o.pos = mul(unity_ObjectToWorld, v.vertex.xyz);//上下溶解使用的空间变换，如果关闭那么将使用普通溶解

            #endif
            //美术上直观说法： O.Pos.y获得了一张世界空间黑白图通过加上_DisAmount可以Y轴上的黑白区域，上下游走
            float dispPos = (o.pos.y + _DisAmount);

            #if INVERT //同上，进行上下翻转
                
                dispPos = 1 - (o.pos.y + _DisAmount);

            #endif
            //因为本地空间转换到世界空间最有用的其实是Y轴，因为Y轴要做上下cilp
            o.worldPos.w = o.pos.y;
            
            o.pos = UnityObjectToClipPos(v.vertex);//转换到其次裁剪空间，主贴图使用
            o.uv = v.uv;//UV传出去
            
            //模型本地空间转换到世界空间
            o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;//溶解贴图使用
            
            return o;
        }



        // fragment shader
        fixed4 frag(v2f i): SV_Target
        {
            
            //采样主纹理贴图
            fixed4 c = tex2D(_MainTex, i.uv) * _Color;

            float3 adjustedworldpos = i.worldPos.xyz;//产生的效果类似渐变

            
            float3 zn = tex2D(_Noise, adjustedworldpos.xy * _Scale);

            // // 将所有侧面拼接在一起，形成niose纹理
            float3 noisetexture = zn;
            
            //noise贴图的R通道
            float noise = noisetexture.r;

            // 沿着Y轴进行上升或者下降,custompack0.y是锁渐变的一张图，上下左右移动是渐变是锁住的，会根据你的移动而移动
            float MovingPosOnModel = _DisAmount + i.worldPos.w;
            // 乘上noise贴图
            MovingPosOnModel *= noise;

            // 发光长度
            float maintexturePart = smoothstep(0, _Smoothness, MovingPosOnModel - _DissolveColorWidth);
            maintexturePart = step(_Cutoff, maintexturePart);

            // normal贴图部分
            float glowingPart = smoothstep(0, _Smoothness, MovingPosOnModel);
            glowingPart = step(_Cutoff, glowingPart);
            // 取出正常纹理部分
            glowingPart *= (1 - maintexturePart);

            #if INVERT   //上下翻转

                // 发光贴贴图长度
                maintexturePart = 1 - smoothstep(0, _Smoothness, MovingPosOnModel + _DissolveColorWidth);
                maintexturePart = step(_Cutoff, maintexturePart);

                // normal贴图部分
                glowingPart = 1 - smoothstep(0, _Smoothness, MovingPosOnModel);
                glowingPart = step(_Cutoff, glowingPart);
                // 取出正常纹理部分
                glowingPart *= (1 - maintexturePart);
            #endif
            

            // cilp的边缘是白色的，然后乘上_DisColor，获得乘上后的颜色
            float4 glowingColored = glowingPart * _DisColor  ;

            // 丢弃像素而不是溶解
            clip((maintexturePart + glowingPart) - 0.01);

            // // 主贴图*颜色贴图
            float4 mainTexture = lerp(c, glowingColored, glowingPart);
            //预乘,因为我们战斗小人spine是没有预乘的。
            mainTexture.rgb *= mainTexture.a;
            return mainTexture * c.a;
        }

        ENDCG

    }
}
}