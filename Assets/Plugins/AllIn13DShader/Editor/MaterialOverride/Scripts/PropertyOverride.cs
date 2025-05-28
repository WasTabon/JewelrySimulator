using UnityEngine;
using UnityEngine.Rendering;

namespace AllIn13DShader
{
	public class PropertyOverride
	{
		public string propertyName
		{
			get; private set;
		}

		public string displayName
		{
			get; private set;
		}

		private AbstractEffectOverride effectOverride;

		public ShaderPropertyType shaderPropertyType;
		public PropertyOverrideType propertyOverrideType;

		public float floatValue;
		public int intValue;
		public Color colorValue;
		public Vector2 rangeLimits;
		public Vector4 vectorValue;
		public Texture texValue;

		public bool isHDR;

		public string[] keywords;


		public PropertyOverride(AbstractEffectOverride effectOverride, int propertyIndex, Shader shader)
		{
			this.effectOverride = effectOverride;

			string propDescription = shader.GetPropertyName(propertyIndex);

			if(propDescription == "_FogOn")
			{
				this.propertyOverrideType = PropertyOverrideType.TOGGLE;
				this.keywords = new string[1] { "_FOG_ON" };
			}
			else
			{
				this.propertyOverrideType = PropertyOverrideType.BASIC;
				this.keywords = new string[0];
			}

			Initialize(propertyIndex, shader);
		}

		public PropertyOverride(AbstractEffectOverride effectOverride, EffectProperty effectProperty, Shader shader) 
		{
			this.effectOverride = effectOverride;

			if (effectProperty.IsToggleProperty())
			{
				this.propertyOverrideType = PropertyOverrideType.TOGGLE;
			}
			else if (effectProperty.IsEnumProperty())
			{
				this.propertyOverrideType = PropertyOverrideType.ENUM;
			}

			if (this.propertyOverrideType != PropertyOverrideType.BASIC)
			{
				keywords = new string[effectProperty.propertyKeywords.Count];
				for (int i = 0; i < effectProperty.propertyKeywords.Count; i++)
				{
					keywords[i] = effectProperty.propertyKeywords[i];
				}
			}

			this.isHDR = effectProperty.isHDR;

			Initialize(effectProperty.propertyIndex, shader);
		}

		private void Initialize(int propertyIndex, Shader shader)
		{
			this.propertyName = shader.GetPropertyName(propertyIndex);
			this.displayName = shader.GetPropertyDescription(propertyIndex);

			this.shaderPropertyType = shader.GetPropertyType(propertyIndex);

			if (this.shaderPropertyType == ShaderPropertyType.Range)
			{
				this.rangeLimits = shader.GetPropertyRangeLimits(propertyIndex);
			}

			InitializeDefaultValues(propertyIndex, shader);
		}

		private void InitializeDefaultValues(int propertyIndex, Shader shader)
		{
			switch (shaderPropertyType)
			{
				case ShaderPropertyType.Range:
				case ShaderPropertyType.Float:
					this.floatValue = shader.GetPropertyDefaultFloatValue(propertyIndex);
					break;
				case ShaderPropertyType.Vector:
					this.vectorValue = shader.GetPropertyDefaultVectorValue(propertyIndex);
					break;
				case ShaderPropertyType.Color:
					this.colorValue = shader.GetPropertyDefaultVectorValue(propertyIndex);
					break;
				case ShaderPropertyType.Int:
					this.intValue = (int)shader.GetPropertyDefaultFloatValue(propertyIndex);
					break;
			}
		}

		public override bool Equals(object obj)
		{
			bool res = false;

			if (obj is PropertyOverride)
			{
				PropertyOverride propertyOverride = (PropertyOverride)obj;
				res = (propertyName == propertyOverride.propertyName);
			}

			return res;
		}

		public override int GetHashCode()
		{
			int res = propertyName.GetHashCode();

			return res;
		}

		public void ApplyChangesToMaterial(Material mat)
		{
			if(mat == null) { return; }

			if (this.propertyOverrideType == PropertyOverrideType.ENUM)
			{
				for(int i = 0; i < keywords.Length; i++)
				{
					string kw = (propertyName + "_" + keywords[i]).ToUpper();
					mat.DisableKeyword(kw);

					if(i == floatValue)
					{
						mat.EnableKeyword(kw);
					}
				}
			}
			else if(this.propertyOverrideType == PropertyOverrideType.TOGGLE)
			{
				string kw = keywords[0];

				if (floatValue > 0)
				{
					mat.EnableKeyword(kw);
				}
				else
				{
					mat.DisableKeyword(kw);
				}
			}

			switch (shaderPropertyType)
			{
				case ShaderPropertyType.Float:
				case ShaderPropertyType.Range:
					mat.SetFloat(propertyName, floatValue);
					break;
				case ShaderPropertyType.Color:
					mat.SetColor(propertyName, colorValue);
					break;
				case ShaderPropertyType.Texture:
					mat.SetTexture(propertyName, texValue);
					break;
				case ShaderPropertyType.Vector:
					mat.SetVector(propertyName, vectorValue);
					break;
				case ShaderPropertyType.Int:
					mat.SetInt(propertyName, intValue);
					break;
			}
		}

		public bool Remove()
		{
			bool res = false;

			if(effectOverride != null)
			{
				res = effectOverride.RemovePropertyOverride(this);
			}

			return res;
		}
	}
}