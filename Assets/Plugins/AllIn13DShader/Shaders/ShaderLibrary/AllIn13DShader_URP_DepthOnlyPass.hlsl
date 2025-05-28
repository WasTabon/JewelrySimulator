#ifndef ALLIN13DSHADER_URP_DEPTHONLYPASS
#define ALLIN13DSHADER_URP_DEPTHONLYPASS

struct DepthOnlyVertexData
{
    float4 positionOS     : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct DepthOnlyFragmentData
{
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

DepthOnlyFragmentData DepthOnlyVertex(DepthOnlyVertexData input)
{
	DepthOnlyFragmentData res;

	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, res);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(res); 

	res.positionCS = TransformObjectToHClip(input.positionOS.xyz);

	return res;
}

float DepthOnlyFragment(DepthOnlyFragmentData input) : SV_TARGET
{
	UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	return input.positionCS.z;
}

#endif