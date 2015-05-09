Shader "Masked/FlatMultiply" {
     Properties {
         _Color ("Main Color", Color) = (1,1,1,1)
     }
     SubShader {
         Tags { "RenderType" = "Transparent"
                "Queue" = "Transparent" }
         Pass { ColorMask 0 }
         Pass {
             Blend SrcAlpha OneMinusSrcAlpha
             ColorMask RGB
             Color [_Color]
         }
     }
 }