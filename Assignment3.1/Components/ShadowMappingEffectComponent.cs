﻿using Game_Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment3._1.Components
{
    public class ShadowMappingEffectComponent : EntityComponent
    {

        public ShadowMappingEffectComponent(int entityID) : base(entityID)
        {
        }

        public LightComponent LightComponet { get; set; }
        public FogComponent FogComponent { get; set; }
        public AmbientComponent AmbientComponent { get; set; }
        public CameraComponent CameraComponent { get; set; }
        public Effect effect { get; set; }
        public RenderTarget2D shadowRenderTarget { get; set; }
        public Matrix world{ get; set; }      
        public bool createShadowMap { get; set; }
        public string Techniquename { get; set; }
        public void AddEffect(Effect effectIN, Texture2D texture2D)
        {
            effect.Parameters["Texture"].SetValue(effectIN.Parameters["Texture"].GetValueTexture2D());
            if(effect.Parameters["Texture"] == null || effect.Parameters["Texture"].GetValueTexture2D() == null)
            {
                effect.Parameters["Texture"].SetValue(texture2D);
            }

        }
        public void Apply()
        {
            effect.CurrentTechnique = effect.Techniques[Techniquename];
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(CameraComponent.View);
            effect.Parameters["Projection"].SetValue(CameraComponent.Projection);
            effect.Parameters["LightDirection"].SetValue(LightComponet.LightDir);
            effect.Parameters["LightViewProj"].SetValue(LightComponet.LightViewProjection);
            effect.Parameters["ShadowStrenght"].SetValue(1f);
            effect.Parameters["DepthBias"].SetValue(0.001f);

            if (!createShadowMap)
            {
                effect.Parameters["ShadowMap"].SetValue(shadowRenderTarget);
            }

            effect.Parameters["AmbientColor"].SetValue(AmbientComponent.AmbientColor);
            effect.Parameters["AmbientIntensity"].SetValue(AmbientComponent.AmbientIntensity);
            
            effect.Parameters["ViewVector"].SetValue(Vector3.One);
            //effect.Parameters["DiffuseLightDirection"].SetValue(shader.DiffuseLightDirection);
            effect.Parameters["DiffuseLightDirection"].SetValue(LightComponet.DiffuseLightDirection); // todo
            effect.Parameters["DiffuseColor"].SetValue(LightComponet.DiffusColor);
            effect.Parameters["DiffuseIntensity"].SetValue(LightComponet.DiffuseIntensity);

    
  
            effect.Parameters["CameraPosition"].SetValue(CameraComponent.CameraPosition);
   

            effect.Parameters["FogStart"].SetValue(FogComponent.FogStart);
            effect.Parameters["FogEnd"].SetValue(FogComponent.FogEnd);
            effect.Parameters["FogColor"].SetValue(FogComponent.FogColor);
            effect.Parameters["FogEnabled"].SetValue(FogComponent.FogEnabled);



            effect.Parameters["Shininess"].SetValue(0.9f);
            effect.Parameters["SpecularColor"].SetValue(Color.MediumVioletRed.ToVector4());
            effect.Parameters["SpecularIntensity"].SetValue(0.1f);
            foreach(var effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
            }
        }
    }
}
