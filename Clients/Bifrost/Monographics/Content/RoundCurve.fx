// RoundCurve.fx
// By James R. Anderson
// Version 1.00, Sep 18 2015
//
// Based on RoundLine by Michael Anderson
//

// This shader draws one polyline at a time.
// Each control point occupies an entry in the control point array

#include "LineCurveCommon.fx"
#include "CurveVertexShader.fx"
#include "LineCurvePixelShaders.fx"

technique Standard
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSStandard();
	}
}

technique AlphaGradient
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSAlphaGradient();
	}
}


technique NoBlur
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSNoBlur();
	}
}


technique AnimatedLinear
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSAnimatedLinear();
	}
}

technique AnimatedBidirectional
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSAnimatedBidirectional();
	}
}


technique AnimatedRadial
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSAnimatedRadial();
	}
}


technique Ladder
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSLadder();
	}
}



technique Modern
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSModern();
	}
}


technique Tubular
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSTubular();
	}
}


technique HalfTube
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSHalfTubular();
	}
}


technique Glow
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSGlow();
	}
}


technique Textured
{
	pass P0
	{
		CullMode = CW;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		BlendOp = Add;
		vertexShader = compile vs_4_0 CurveVertexShader();
		pixelShader = compile ps_4_0 MyPSTextured();
	}
}
