﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game_Engine.Components;
using Game_Engine.Managers;
using Game_Engine.Systems;
using System.Linq;
using Assignment3._1.Components;

namespace Assignment3._1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix World = Matrix.Identity;

        private const int shadowMapWidth = 2048;
        private const int shadowMapHeight = 2048;

        const int windowWidth = 800;
        const int windowHeight = 480;

        private Model houseModel;
        private Model blobModel;
        private Model blockModel;
        private Model groundModel;
        private Texture2D houseTexture;

        private CameraSystem cameraSystem;
        private RenderSystem renderSystem;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            cameraSystem = new CameraSystem();
            renderSystem = new RenderSystem(graphics.GraphicsDevice, World);

            SystemManager.Instance.addToUpdateableQueue(cameraSystem);
            SystemManager.Instance.addToDrawableQueue(renderSystem);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // TODO: use this.Content to load your game content here
            houseModel = Content.Load<Model>("farmhouse_obj");
            houseTexture = Content.Load<Texture2D>("farmhouse-texture");
            blobModel = Content.Load<Model>("Blob");
            blockModel = Content.Load<Model>("block2");
            groundModel = Content.Load<Model>("ground");

            CreateCamera(1);
            CreateLighting(2);

            CreateGround(3, new Vector3(50, 0, 20));

            CreateHouse(4, new Vector3(60, 0, 50));
            CreateBlob(5, new Vector3(30, 5, 50));
            CreateBlock(6, new Vector3(35, 10, 10));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            SystemManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            SystemManager.Instance.Draw();
            base.Draw(gameTime);
        }

        #region Entity creation

        private Texture2D CreateTexture(Color colour)
        {
            return CreateTexture(graphics.GraphicsDevice, 1, 1, c => colour);
        }
        private Texture2D CreateTexture(GraphicsDevice device, int width, int height, System.Func<int, Color> paint)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);
            //the array holds the color for each pixel in the texture
            Color[] colorArray = new Color[width * height];
            for (int pixel = 0; pixel < colorArray.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                colorArray[pixel] = paint(pixel);
            }
            //set the color
            texture.SetData(colorArray);
            return texture;
        }


        private void CreateHouse(int houseId, Vector3 position)
        {
            CreateShadedModel(houseId, position, houseModel, houseTexture);
        }
        private void CreateGround(int groundId, Vector3 position)
        {
            CreateShadedModel(groundId, position, groundModel, CreateTexture(Color.Yellow));
        }
        private void CreateBlob(int blobId, Vector3 position)
        {
            CreateShadedModel(blobId, position, blobModel, CreateTexture(Color.PaleVioletRed));
        }
        private void CreateBlock(int blockId, Vector3 position)
        {
            CreateShadedModel(blockId, position, blockModel, CreateTexture(Color.MediumAquamarine));
        }

        private void CreateShadedModel(int entityId, Vector3 position, Model model, Texture2D texture)
        {
            var transformComponent = new TransformComponent(entityId);
            transformComponent.Position = position;
            transformComponent.Scale = 1f;

            var modelComponent = new ModelComponent(entityId);
            modelComponent.Model = model;
            modelComponent.Texture2D = texture;
            modelComponent.ObjectWorld = World 
                * Matrix.CreateScale(new Vector3(transformComponent.Scale)) 
                * Matrix.CreateTranslation(transformComponent.Position);

            var effectSettingsComponent = new GameEffectSettingsComponent(entityId);
            effectSettingsComponent.Effect = Content.Load<Effect>("Shadow");

            ComponentManager.Instance.AddComponent(modelComponent);
            ComponentManager.Instance.AddComponent(effectSettingsComponent, typeof(EffectSettingsComponent));
            ComponentManager.Instance.AddComponent(transformComponent);
        }

        private void CreateCamera(int cameraId)
        {
            var cameraComponent = new CameraComponent(cameraId);
            cameraComponent.CameraPosition = new Vector3(40, 30, -30);
            cameraComponent.CameraForward = new Vector3(0, -0.4472136f, 0.8944272f);
            cameraComponent.BoundingFrustrum = new BoundingFrustum(Matrix.Identity);
            cameraComponent.AspectRatio = (float)windowWidth / (float)windowHeight;
            cameraComponent.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                             cameraComponent.AspectRatio,
                                                             1.0f, 200.0f);
            cameraComponent.CameraTurnSpeed = 0.01f;
            cameraComponent.CameraMoveSpeed = 1f;
            ComponentManager.Instance.AddComponent(cameraComponent);
        }

        private void CreateLighting(int lightID)
        {
            LightSettingsComponent lightComponent = new LightSettingsComponent(lightID);

            lightComponent.LightDirection = new Vector3(-0.3333333f, 0.6666667f, 0.6666667f);
            lightComponent.DiffusColor = Color.White.ToVector4();
            lightComponent.DiffuseIntensity = 0.5f;
            lightComponent.DiffuseLightDirection = lightComponent.LightDirection;
            lightComponent.AmbientColor = Color.White.ToVector4();
            lightComponent.AmbientIntensity = 0.2f;
            lightComponent.RenderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                    shadowMapWidth,
                    shadowMapHeight,
                    true,
                    SurfaceFormat.Single,
                    DepthFormat.Depth24);
            lightComponent.FogColor = Color.CornflowerBlue.ToVector4();
            lightComponent.FogEnabled = true;
            lightComponent.FogStart = 80f;
            lightComponent.FogEnd = 200f;

            ComponentManager.Instance.AddComponent(lightComponent);
        }
        #endregion
    }
}
