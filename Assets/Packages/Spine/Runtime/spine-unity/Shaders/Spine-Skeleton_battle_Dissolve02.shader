Shader "Spine/Skeleton_battle_Dissolve02"
{
    
    //必须是以上路径才能实现叠加效果，具体原因不明，也许是这个版本运行库的一个bug
    // Shader "MGame/3D/Sprites/Default"
    // {
        Properties
        {
            [NoScaleOffset] _MainTex ("Sprite Texture", 2D) = "white" { }
            _Color ("主颜色", Color) = (1, 1, 1, 1)
            [Space(30)]
            _FlickerColor ("闪白的颜色", Color) = (1, 1, 1, 1)
            _FlickValue ("闪白", Range(0, 1)) = 0
            //因为是通过SetGlobal控制变量的，由于用了setGloal，所以shader里面Properties初始值会被注释掉并且不会在shader面板式显示，初始值的控制权移交给了class的public，
            //但游戏没有挂这个脚本的场景因为没有加载对应C#，没有值会变成系统默认值会变成0，模型会变透明或者颜色不正常,所以加了_DissolveEnabled，让其初始值就是我们想要的值，故加了"颜色曝光属性2"。
            
            [Space(30)]
            [Toggle]_DissolveEnabled ("Dissolve Enabled", int) = 0
            // FlickMultColor ("颜色曝光属性", Color) = (0, 0, 0, 1)
            // [Space(10)]
            // _range ("R(景虚范围0-5),G(景深深度),B(高度雾），A()", vector) = (0, 0, 0, 0)//控制宽度
            
            // // [Space(8)]
            // jinshenMColor ("景深雾气叠加颜色", Color) = (1, 1, 1, 0)
            // jinshenAColor ("景深雾气相加颜色", Color) = (0, 0, 0, 1)
            // gaoduMColor ("高度雾叠加颜色", Color) = (1, 1, 1, 1)

            [Space(30)]
            [Header(Noise)]
            _Noise ("Noise", 2D) = "white" { }
            // _Scale ("Scale", Range(0, 5)) = 1
            [Space(30)]
            [Toggle(INVERT)] _INVERT ("INVERT", Float) = 1
            _Dissolve ("Dissolve", vector) = (1, 0, 0.01, 0.5)//溶解贴图
            
            // _DisAmount ("DisAmount", Range(-2, 2)) = 0.0
            
            // _DissolveColorWidth ("DissolveColorWidth", Range(0, 2)) = 0.01
            // _Cutoff ("Cutoff", Range(0, 1)) = 0.5
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
                // ColorMask RGB
                Blend One OneMinusSrcAlpha
                Lighting Off
                CGPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag
                #pragma target 2.0
                #pragma multi_compile _ _DISSOLVEENABLED_ON
                #pragma multi_compile _ INVERT
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

                sampler2D _MainTex, _Noise;//基本贴图和噪波贴图
                fixed4 _FlickerColor; // 闪白颜色
                fixed _FlickValue; //闪白范围
                fixed4 _Color;//全局颜色
                fixed4 jinshenMColor; // 景深雾气相加值
                fixed4 jinshenAColor; // 景深雾气相加值
                fixed4 gaoduMColor;//高度雾气乘法值
                fixed4 range;//高度雾气，景深雾气范围值
                //溶解
                // float _Dissolve.g, _Dissolve.r;
                // float _Dissolve.b, _Dissolve.a;
                fixed4 _Dissolve; // 溶解
                fixed4 _DisColor;//溶解边缘颜色



                v2f Vert(appdata v)
                {
                    v2f o = (v2f) 0;
                    // 本地空间转换到世界空间
                    
                    o.pos = mul(unity_ObjectToWorld, v.vertex.xyz);//上下溶解使用的空间变换，如果关闭那么将使用普通溶解
                    
                    //美术上直观说法： O.Pos.y获得了一张世界空间黑白图通过加上_DisAmount可以Y轴上的黑白区域，上下游走
                    float dispPos = (o.pos.y + _Dissolve.g);

                    #if INVERT //同上，进行上下翻转
                    
                    dispPos = 1 - (o.pos.y + _Dissolve.g);

                #endif
                //因为本地空间转换到世界空间最有用的其实是Y轴，因为Y轴要做上下cilp
                o.worldPosition.w = o.pos.y;
                //转换到齐次裁剪空间，主贴图使用
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;//UV传出

                o.color = v.color;//顶点色传出去
                // 模型本地空间转换到世界空间
                o.worldPosition.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 Frag(v2f i): SV_Target
            {
                //     fixed4 _FlickMultColorMix = _FlickMultColor2;
                float4 rawColor = tex2D(_MainTex, i.uv) * _Color ;

                float3 adjustedworldpos = i.worldPosition.xyz; // 产生的效果类似渐变

                float3 zn = tex2D(_Noise, adjustedworldpos.xy * _Dissolve.r);//采样溶解贴图并让其有个缩放效果
                // // 将所有侧面拼接在一起，形成niose纹理
                float3 noisetexture = zn;
                
                //noise贴图的R通道
                float noise = noisetexture.r;

                // 沿着Y轴进行上升或者下降,custompack0.y是锁渐变的一张图，上下左右移动是渐变是锁住的，会根据你的移动而移动
                float MovingPosOnModel = _Dissolve.g + i.worldPosition.w;
                // 乘上noise贴图
                MovingPosOnModel *= noise;

                // 发光长度
                float maintexturePart = MovingPosOnModel - _Dissolve.b;
                maintexturePart = step(_Dissolve.a, maintexturePart);

                // normal贴图部分
                float glowingPart = MovingPosOnModel;
                glowingPart = step(_Dissolve.a, glowingPart);
                // 取出正常纹理部分
                glowingPart *= (1 - maintexturePart);

                #if INVERT   //上下翻转

                    // 发光贴贴图长度
                    maintexturePart = 1 - (MovingPosOnModel + _Dissolve.b);
                    maintexturePart = step(_Dissolve.a, maintexturePart);

                    // normal贴图部分
                    glowingPart = 1 - MovingPosOnModel;
                    glowingPart = step(_Dissolve.a, glowingPart);
                    // 取出正常纹理部分
                    glowingPart *= (1 - maintexturePart);
                #endif


                // cilp的边缘是白色的，然后乘上_DisColor，获得乘上后的颜色
                float4 glowingColored = glowingPart * _DisColor  ;

                // 丢弃像素而不是溶解
                clip((maintexturePart + glowingPart) - 0.01);


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
                //闪白的结果在和glowingcolor做差值，其实说白了就是闪白和溶解的边缘部分做差值
                float4 mainTexture = lerp(shanbaicolor2, glowingColored, glowingPart);
                //预乘，因为spine导出来的时候没有勾选预乘，所以这边需要做
                mainTexture.rgb *= mainTexture.a;
                return mainTexture * i.color;
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

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 2.0
            #pragma multi_compile _ _DISSOLVEENABLED_ON
            #pragma multi_compile _ INVERT
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

            sampler2D _MainTex, _Noise;//基本贴图和噪波贴图
            // fixed4 _FlickerColor; // 闪白颜色
            // fixed _FlickValue; //闪白范围
            // fixed4 _Color;//全局颜色
            // fixed4 jinshenMColor; // 景深雾气相加值
            // fixed4 jinshenAColor; // 景深雾气相加值
            // fixed4 gaoduMColor;//高度雾气乘法值
            // fixed4 range;//高度雾气，景深雾气范围值
            //溶解
            // float _Dissolve.g, _Dissolve.r;
            // float _Dissolve.b, _Dissolve.a;
            fixed4 _Dissolve; // 溶解
            fixed4 _DisColor;//溶解边缘颜色
            fixed _BloomFactor; // 溶解边缘颜色



            v2f Vert(appdata v)
            {
                v2f o = (v2f) 0;
                // 本地空间转换到世界空间
                
                o.pos = mul(unity_ObjectToWorld, v.vertex.xyz);//上下溶解使用的空间变换，如果关闭那么将使用普通溶解
                
                //美术上直观说法： O.Pos.y获得了一张世界空间黑白图通过加上_DisAmount可以Y轴上的黑白区域，上下游走
                float dispPos = (o.pos.y + _Dissolve.g);

                #if INVERT //同上，进行上下翻转
                    
                    dispPos = 1 - (o.pos.y + _Dissolve.g);

                #endif
                //因为本地空间转换到世界空间最有用的其实是Y轴，因为Y轴要做上下cilp
                o.worldPosition.w = o.pos.y;
                //转换到齐次裁剪空间，主贴图使用
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;//UV传出

                o.color = v.color;//顶点色传出去
                // 模型本地空间转换到世界空间
                o.worldPosition.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 Frag(v2f i): SV_Target
            {
                //     fixed4 _FlickMultColorMix = _FlickMultColor2;
                float4 rawColor = tex2D(_MainTex, i.uv) ;

                float3 adjustedworldpos = i.worldPosition.xyz; // 产生的效果类似渐变

                float3 zn = tex2D(_Noise, adjustedworldpos.xy * _Dissolve.r);//采样溶解贴图并让其有个缩放效果
                // // 将所有侧面拼接在一起，形成niose纹理
                float3 noisetexture = zn;
                
                //noise贴图的R通道
                float noise = noisetexture.r;

                // 沿着Y轴进行上升或者下降,custompack0.y是锁渐变的一张图，上下左右移动是渐变是锁住的，会根据你的移动而移动
                float MovingPosOnModel = _Dissolve.g + i.worldPosition.w;
                // 乘上noise贴图
                MovingPosOnModel *= noise;

                // 发光长度
                float maintexturePart = MovingPosOnModel - _Dissolve.b;
                maintexturePart = step(_Dissolve.a, maintexturePart);

                // normal贴图部分
                float glowingPart = MovingPosOnModel;
                glowingPart = step(_Dissolve.a, glowingPart);
                // 取出正常纹理部分
                glowingPart *= (1 - maintexturePart);

                #if INVERT   //上下翻转

                    // 发光贴贴图长度
                    maintexturePart = 1 - (MovingPosOnModel + _Dissolve.b);
                    maintexturePart = step(_Dissolve.a, maintexturePart);

                    // normal贴图部分
                    glowingPart = 1 - MovingPosOnModel;
                    glowingPart = step(_Dissolve.a, glowingPart);
                    // 取出正常纹理部分
                    glowingPart *= (1 - maintexturePart);
                #endif


                float4 glowingColored = glowingPart * _DisColor * _BloomFactor  ;
                float4 glowingColoredfull = 1 - float4(glowingColored.rgb, glowingColored.r * rawColor.a);
                return glowingColoredfull;
            }


            ENDCG

        }
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
        LOD 400
        Cull Off
        ZWrite Off
        // ColorMask RGB
        Blend One OneMinusSrcAlpha
        Lighting Off
        Pass
        {

            
            CGPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 2.0
            #pragma multi_compile _ _DISSOLVEENABLED_ON
            #pragma multi_compile _ INVERT
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

            sampler2D _MainTex, _Noise;//基本贴图和噪波贴图
            fixed4 _FlickerColor; // 闪白颜色
            fixed _FlickValue; //闪白范围
            fixed4 _Color;//全局颜色
            fixed4 jinshenMColor; // 景深雾气相加值
            fixed4 jinshenAColor; // 景深雾气相加值
            fixed4 gaoduMColor;//高度雾气乘法值
            fixed4 range;//高度雾气，景深雾气范围值
            //溶解
            // float _Dissolve.g, _Dissolve.r;
            // float _Dissolve.b, _Dissolve.a;
            fixed4 _Dissolve; // 溶解
            fixed4 _DisColor;//溶解边缘颜色



            v2f Vert(appdata v)
            {
                v2f o = (v2f) 0;
                // 本地空间转换到世界空间
                
                o.pos = mul(unity_ObjectToWorld, v.vertex.xyz);//上下溶解使用的空间变换，如果关闭那么将使用普通溶解
                
                //美术上直观说法： O.Pos.y获得了一张世界空间黑白图通过加上_DisAmount可以Y轴上的黑白区域，上下游走
                float dispPos = (o.pos.y + _Dissolve.g);

                #if INVERT //同上，进行上下翻转
                    
                    dispPos = 1 - (o.pos.y + _Dissolve.g);

                #endif
                //因为本地空间转换到世界空间最有用的其实是Y轴，因为Y轴要做上下cilp
                o.worldPosition.w = o.pos.y;
                //转换到齐次裁剪空间，主贴图使用
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;//UV传出

                o.color = v.color;//顶点色传出去
                // 模型本地空间转换到世界空间
                o.worldPosition.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 Frag(v2f i): SV_Target
            {
                //     fixed4 _FlickMultColorMix = _FlickMultColor2;
                float4 rawColor = tex2D(_MainTex, i.uv) * _Color ;

                float3 adjustedworldpos = i.worldPosition.xyz; // 产生的效果类似渐变

                float3 zn = tex2D(_Noise, adjustedworldpos.xy * _Dissolve.r);//采样溶解贴图并让其有个缩放效果
                // // 将所有侧面拼接在一起，形成niose纹理
                float3 noisetexture = zn;
                
                //noise贴图的R通道
                float noise = noisetexture.r;

                // 沿着Y轴进行上升或者下降,custompack0.y是锁渐变的一张图，上下左右移动是渐变是锁住的，会根据你的移动而移动
                float MovingPosOnModel = _Dissolve.g + i.worldPosition.w;
                // 乘上noise贴图
                MovingPosOnModel *= noise;

                // 发光长度
                float maintexturePart = MovingPosOnModel - _Dissolve.b;
                maintexturePart = step(_Dissolve.a, maintexturePart);

                // normal贴图部分
                float glowingPart = MovingPosOnModel;
                glowingPart = step(_Dissolve.a, glowingPart);
                // 取出正常纹理部分
                glowingPart *= (1 - maintexturePart);

                #if INVERT   //上下翻转

                    // 发光贴贴图长度
                    maintexturePart = 1 - (MovingPosOnModel + _Dissolve.b);
                    maintexturePart = step(_Dissolve.a, maintexturePart);

                    // normal贴图部分
                    glowingPart = 1 - MovingPosOnModel;
                    glowingPart = step(_Dissolve.a, glowingPart);
                    // 取出正常纹理部分
                    glowingPart *= (1 - maintexturePart);
                #endif


                // cilp的边缘是白色的，然后乘上_DisColor，获得乘上后的颜色
                float4 glowingColored = glowingPart * _DisColor  ;

                // 丢弃像素而不是溶解
                clip((maintexturePart + glowingPart) - 0.01);


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
                //闪白的结果在和glowingcolor做差值，其实说白了就是闪白和溶解的边缘部分做差值
                float4 mainTexture = lerp(shanbaicolor2, glowingColored, glowingPart);
                //预乘，因为spine导出来的时候没有勾选预乘，所以这边需要做
                mainTexture.rgb *= mainTexture.a;
                return mainTexture * i.color;
            }


            ENDCG

        }
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
        LOD 200
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
            };

            sampler2D _MainTex;
            fixed4 _Color;
            
            v2f Vert(appdata v)
            {
                v2f o = (v2f) 0;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            fixed4 Frag(v2f i): SV_Target
            {
                //     fixed4 _FlickMultColorMix = _FlickMultColor2;
                float4 rawColor = tex2D(_MainTex, i.uv) * _Color ;
                rawColor.rgb *= rawColor.a  ;
                return rawColor * i.color;
            }


            ENDCG

        }
    }



    CustomEditor"ShaderGUI_Skeleton_battle_Dissolve02"
}
