// Toony Colors Pro+Mobile 2
// (c) 2014-2024 Jean Moreno

using UnityEngine;
using UnityEngine.Rendering;
#if TCP2_UNIVERSAL_RP
using UnityEngine.Rendering.Universal;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

// Use this script to generate the Reflection Render Texture when using the "Planar Reflection" mode from the Shader Generator

// Usage:
// - generate a water shader with "Planar Reflection"
// - assign this shader to a planar mesh's material
// - add this script on the same GameObject

// Based on: http://wiki.unity3d.com/index.php/MirrorReflection4

namespace ToonyColorsPro
{
	namespace Runtime
	{
		[ExecuteInEditMode]
		public class TCP2_PlanarReflection : MonoBehaviour
		{
			public int textureSize = 1024;
			public RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			public LayerMask reflectLayers = -1;
			public float clipPlaneOffset = 0.07f;
			[Space]
			public bool useCustomBackgroundColor;
			public Color backgroundColor = Color.black;
			[Space]
			public bool applyBlur;
			[Range(1,4)] public int blurIterations = 1;
			[Range(0.01f,10)] public float blurDistance = 1;

			Camera reflectionCamera;
			RenderTexture reflectionRenderTexture;
#if TCP2_UNIVERSAL_RP && TCP2_RECURSIVE_RENDER_REQUEST
			UniversalRenderPipeline.SingleCameraRequest renderReflectionCameraRequest;
#endif
			Shader blurShader;
			Material blurMaterial;
			CommandBuffer commandBufferBlur;
			Shader reflectionDepthShader;
			RenderTexture reflectionDepthRenderTexture;
			bool isURP;

			static bool s_InsideRendering;

			static readonly int _SamplingDistance = Shader.PropertyToID("_SamplingDistance");
			static readonly int _PlanarReflectionTempRT = Shader.PropertyToID("_PlanarReflectionTempRT");
			static readonly int _ReflectionTex = Shader.PropertyToID("_ReflectionTex");

			// --------------------------------------------------------------------------------------------------------------------------------
			// Unity Events

			void OnValidate()
			{
				UpdateRenderTexture();
				UpdateCommandBuffer();
			}

			void OnEnable()
			{
				isURP  = GraphicsSettings.currentRenderPipeline != null && GraphicsSettings.currentRenderPipeline.GetType().Name.Contains("Universal");

#if TCP2_UNIVERSAL_RP
				if (isURP)
					RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
				else
#endif
					Camera.onPreRender += BeginCameraRendering;

				UpdateRenderTexture();
				UpdateCommandBuffer();
			}

			void OnDisable()
			{
				if (isURP)
					RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
				else
					Camera.onPreRender -= BeginCameraRendering;

				ClearCommandBuffer();
				ClearRenderTexture();
			}

			// --------------------------------------------------------------------------------------------------------------------------------
			// Render Texture

			void UpdateRenderTexture()
			{
				if (reflectionRenderTexture == null
				    || reflectionRenderTexture.width != textureSize
				    || reflectionRenderTexture.format != renderTextureFormat
				    )
				{
					ClearRenderTexture();
					reflectionRenderTexture = new RenderTexture(textureSize, textureSize, 16, renderTextureFormat, RenderTextureReadWrite.sRGB)
					{
						name = $"Planar Reflection {Random.Range(1000,9999)}",
						hideFlags = HideFlags.HideAndDontSave
					};
#if TCP2_UNIVERSAL_RP && TCP2_RECURSIVE_RENDER_REQUEST
					renderReflectionCameraRequest = new UniversalRenderPipeline.SingleCameraRequest
					{
						destination = reflectionRenderTexture
					};
#endif
					AssignRenderTextureToMaterials();
				}
			}

			void AssignRenderTextureToMaterials()
			{
				var materials = this.GetComponent<Renderer>().sharedMaterials;
				foreach (Material mat in materials)
				{
					if (mat.HasProperty(_ReflectionTex))
					{
						mat.SetTexture(_ReflectionTex, reflectionRenderTexture);
					}
				}
			}

			void ClearRenderTexture()
			{
				if (reflectionRenderTexture != null)
				{
					reflectionRenderTexture.Release();
					DestroyImmediate(reflectionRenderTexture);
				}
			}

			// --------------------------------------------------------------------------------------------------------------------------------
			// Command Buffer (blur)

			void UpdateCommandBuffer()
			{
				if (commandBufferBlur != null)
				{
					ClearCommandBuffer();
				}

				if (!applyBlur)
				{
					return;
				}

				if (blurMaterial == null)
				{
					if (blurShader == null)
					{
						blurShader = Shader.Find("Hidden/TCP2 Gaussian Blur Filter");

						if (blurShader == null)
						{
							Debug.LogError("[TCP2 Planar Reflection] Can't find Gaussian Blur Filter shader!", this.gameObject);
							return;
						}
					}

					blurMaterial = new Material(blurShader);
					blurMaterial.name = "Planar Reflection Blur";
				}

				float resolutionRatio = Screen.width * (textureSize / 1024f);
				blurMaterial.SetFloat(_SamplingDistance, blurDistance * (resolutionRatio / 1080f));

				if (reflectionRenderTexture == null)
				{
					return;
				}

				if (reflectionCamera == null)
				{
					return;
				}

				commandBufferBlur = new CommandBuffer();
				{
					// Create temp render texture
					commandBufferBlur.GetTemporaryRT(_PlanarReflectionTempRT, textureSize, textureSize, 16, FilterMode.Bilinear, reflectionRenderTexture.format, RenderTextureReadWrite.sRGB);

					// Down sample
					commandBufferBlur.CopyTexture(reflectionRenderTexture, _PlanarReflectionTempRT); // copy reflection to temp
					commandBufferBlur.Blit(_PlanarReflectionTempRT, reflectionRenderTexture, blurMaterial, 0); // down sample

					// Blur passes
					for (int i = 0; i < blurIterations; i++)
					{
						commandBufferBlur.Blit(reflectionRenderTexture, _PlanarReflectionTempRT, blurMaterial, 1); // blur 1st pass
						commandBufferBlur.Blit(_PlanarReflectionTempRT, reflectionRenderTexture, blurMaterial, 2); // blur 2nd pass
					}

					// Release temp render texture
					commandBufferBlur.ReleaseTemporaryRT(_PlanarReflectionTempRT);
				}

				// Add command buffer
				if (!isURP)
					reflectionCamera.AddCommandBuffer(CameraEvent.AfterEverything, commandBufferBlur);
			}

			void ClearCommandBuffer()
			{
				if (reflectionCamera != null && reflectionCamera.commandBufferCount > 0 && !isURP)
				{
					reflectionCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, commandBufferBlur);
				}
				if (commandBufferBlur != null)
				{
					commandBufferBlur.Clear();
					commandBufferBlur.Release();
					commandBufferBlur = null;
				}
			}

			// --------------------------------------------------------------------------------------------------------------------------------
			// Reflection Camera Rendering

			void BeginCameraRendering(Camera cam)
			{
				BeginCameraRendering(default, cam);
			}

			void BeginCameraRendering(ScriptableRenderContext context, Camera cam)
			{
				if ((cam.cameraType & (CameraType.Game | CameraType.SceneView)) == 0)
				{
					return;
				}

#if UNITY_EDITOR
				// texture ref can be lost in Editor because of hide flags, but this shouldn't happen at play mode/runtime
				if (!Application.isPlaying)
				{
					AssignRenderTextureToMaterials();
				}
#endif

				if (reflectionCamera == null)
				{
					var go = new GameObject("Planar Reflection Camera", typeof(Camera));
					reflectionCamera = go.GetComponent<Camera>();
					reflectionCamera.enabled = false;
					go.hideFlags = HideFlags.HideAndDontSave;

					UpdateRenderTexture();
					UpdateCommandBuffer();
					reflectionCamera.targetTexture = reflectionRenderTexture;
				}

				RenderPlanarReflection(context, cam);
			}

			void RenderPlanarReflection(ScriptableRenderContext context, Camera worldCamera)
			{
				if (worldCamera == null || !enabled)
					return;

				// Safeguard from recursive reflections.      
				if (s_InsideRendering) return;
				s_InsideRendering = true;

				// find out the reflection plane: position and normal in world space
				Transform thisTransform = transform;
				Vector3 pos = thisTransform.position;
				Vector3 normal = thisTransform.up;

				reflectionCamera.CopyFrom(worldCamera);
				if (useCustomBackgroundColor)
				{
					reflectionCamera.clearFlags = CameraClearFlags.Color;
					reflectionCamera.backgroundColor = backgroundColor;
				}

				// Reflect camera around reflection plane
				float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
				Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
				Matrix4x4 reflectionMatrix = Matrix4x4.zero;
				CalculateReflectionMatrix(ref reflectionMatrix, reflectionPlane);
				reflectionCamera.worldToCameraMatrix = worldCamera.worldToCameraMatrix * reflectionMatrix;

				// Setup oblique projection matrix so that near plane is our reflection plane. This way we clip everything below/above it for free.
				Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f, clipPlaneOffset);
				Matrix4x4 projection = worldCamera.CalculateObliqueMatrix(clipPlane);
				reflectionCamera.projectionMatrix = projection;

				reflectionCamera.targetTexture = reflectionRenderTexture;
				reflectionCamera.cullingMask = ~(1 << this.gameObject.layer) & reflectLayers.value; // never render this object's layer

				Transform reflectionCamTransform = reflectionCamera.transform;
				reflectionCamTransform.position = reflectionMatrix.MultiplyPoint(worldCamera.transform.position);
				Vector3 euler = worldCamera.transform.eulerAngles;
				reflectionCamTransform.eulerAngles = new Vector3(0, euler.y, euler.z);

				GL.invertCulling = true;
				if (isURP)
				{
	#if TCP2_UNIVERSAL_RP && TCP2_RECURSIVE_RENDER_REQUEST
					RenderPipeline.SubmitRenderRequest(reflectionCamera, renderReflectionCameraRequest);
	#elif TCP2_UNIVERSAL_RP
#pragma warning disable CS0618
					// Recursive render request is needed (Unity 6+) for the replacement method to work, so we continue using the deprecated one until then
					UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
#pragma warning restore CS0618
	#endif
					if (applyBlur && commandBufferBlur != null)
					{
						context.ExecuteCommandBuffer(commandBufferBlur);
					}
				}
				else
				{
					reflectionCamera.Render();
				}
				GL.invertCulling = false;


				s_InsideRendering = false;
			}

			// Given position/normal of the plane, calculates plane in camera space.
			static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign, float clipPlaneOffset)
			{
				Vector3 offsetPos = pos + normal * clipPlaneOffset;
				Matrix4x4 matrix = cam.worldToCameraMatrix;
				Vector3 camPosition = matrix.MultiplyPoint(offsetPos);
				Vector3 camNormal = matrix.MultiplyVector(normal).normalized * sideSign;
				return new Vector4(camNormal.x, camNormal.y, camNormal.z, -Vector3.Dot(camPosition, camNormal));
			}

			// Calculates reflection matrix around the given plane
			static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
			{
				reflectionMat.m00 = (1F - 2F*plane[0]*plane[0]);
				reflectionMat.m01 = (-2F*plane[0]*plane[1]);
				reflectionMat.m02 = (-2F*plane[0]*plane[2]);
				reflectionMat.m03 = (-2F*plane[3]*plane[0]);

				reflectionMat.m10 = (-2F*plane[1]*plane[0]);
				reflectionMat.m11 = (1F - 2F*plane[1]*plane[1]);
				reflectionMat.m12 = (-2F*plane[1]*plane[2]);
				reflectionMat.m13 = (-2F*plane[3]*plane[1]);

				reflectionMat.m20 = (-2F*plane[2]*plane[0]);
				reflectionMat.m21 = (-2F*plane[2]*plane[1]);
				reflectionMat.m22 = (1F - 2F*plane[2]*plane[2]);
				reflectionMat.m23 = (-2F*plane[3]*plane[2]);

				reflectionMat.m30 = 0F;
				reflectionMat.m31 = 0F;
				reflectionMat.m32 = 0F;
				reflectionMat.m33 = 1F;
			}
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(Runtime.TCP2_PlanarReflection))]
	class TCP2_PlanarReflectionEditor : Editor
	{
		static readonly GUIContent[] textureSizeLabels = new GUIContent[]
		{
			new GUIContent("64"),
			new GUIContent("128"),
			new GUIContent("256"),
			new GUIContent("512"),
			new GUIContent("1024"),
			new GUIContent("2048"),
			new GUIContent("4096"),
			new GUIContent("8192")
		};
		static readonly int[] textureSizeValues = new int[]
		{
			64,
			128,
			256,
			512,
			1024,
			2048,
			4096,
			8192
		};

		public override void OnInspectorGUI()
		{
			// base.OnInspectorGUI();

			// Customized 'DrawDefaultInspector'
			serializedObject.UpdateIfRequiredOrScript();
			SerializedProperty iterator = serializedObject.GetIterator();
			bool enterChildren = true;
			bool guiEnabled = GUI.enabled;
			while (iterator.NextVisible(enterChildren))
			{
				if (iterator.propertyPath == "m_Script")
				{
					using (new EditorGUI.DisabledScope(true))
					{
						EditorGUILayout.PropertyField(iterator, true);
					}

					GUILayout.Space(4);
					EditorGUILayout.HelpBox("This scripts will render planar reflections, it needs to be used with a generated shader with the \"Planar Reflections\" feature enabled.", MessageType.Info);
					GUILayout.Space(4);
					EditorGUILayout.HelpBox("This script only works with axis-aligned meshes on the XZ plane in Unity space. (e.g. it will work with the \"Plane\" built-in mesh, but not with the \"Quad\" one).", MessageType.Warning);
					GUILayout.Space(4);
				}
				else if (iterator.propertyPath == "textureSize")
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.IntPopup(iterator, textureSizeLabels, textureSizeValues);
						if (GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
						{
							iterator.intValue = textureSizeValues[Mathf.Clamp(System.Array.IndexOf(textureSizeValues, iterator.intValue) - 1, 0, textureSizeValues.Length - 1)];
						}
						if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(20)))
						{
							iterator.intValue = textureSizeValues[Mathf.Clamp(System.Array.IndexOf(textureSizeValues, iterator.intValue) + 1, 0, textureSizeValues.Length - 1)];
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				else if (iterator.propertyPath == "reflectLayers")
				{
					EditorGUILayout.PropertyField(iterator, true);
					EditorGUILayout.HelpBox("The layer this GameObjects is assigned to will be ignored to prevent the reflective object from reflecting itself!\nAny other object in that layer won't be rendered in the reflection as well.", MessageType.Info);
					GUILayout.Space(4);
				}
				else
				{
					EditorGUILayout.PropertyField(iterator, true);
					
					if (iterator.propertyPath == "applyBlur")
					{
						GUI.enabled &= iterator.boolValue;
					}
					else if (iterator.propertyPath == "useBlurDepth")
					{
						GUI.enabled &= iterator.boolValue;
					}
				}

				enterChildren = false;
			}
			
			GUI.enabled = guiEnabled;
			serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}