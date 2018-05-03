﻿using Assignment2.Models;
using Assignment2.Systems;
using Game_Engine.Components;
using Game_Engine.Managers;
using Game_Engine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Robot;
using System;
using System.Collections.Generic;

namespace Assignment2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Laboration2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D mapTexture;
        Texture2D mapTextureImage;
        Texture2D houseTexture;
        Texture2D treeTexture;
        Model playerModel;
        Model treeModel;
        Model houseModel;

        WorldTerrain worldTerrain;
        PlayerCameraSystem cameraSystem;
        WorldDrawSystem worldDrawSystem;
        WorldObjectsDrawSystem worldObjectsDrawSystem;

        BasicEffect Effect;
        Player torso;
        RobotArm robotArm;

        List<Vector3> modelPositions;

        public Laboration2()
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
            mapTexture = Content.Load<Texture2D>("US_Canyon");
            mapTextureImage = Content.Load<Texture2D>("grass");
            houseModel = Content.Load<Model>("farmhouse_obj");
            houseTexture = Content.Load<Texture2D>("farmhouse-texture");
            treeModel = Content.Load<Model>("farmhouse_obj");
            treeTexture = Content.Load<Texture2D>("Tree");

            worldTerrain = new WorldTerrain(GraphicsDevice, mapTexture,
                mapTextureImage, new Vector3(0,0,0));
            worldTerrain.HeightmapWorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 1080));
            List<House> houses = CreateHouses(houseModel, 100);
            

            //systems
            worldDrawSystem = new WorldDrawSystem(worldTerrain, GraphicsDevice);
            worldObjectsDrawSystem = new WorldObjectsDrawSystem();
            worldObjectsDrawSystem.Objects = houses;

            SystemManager.Instance.addToDrawableQueue(worldDrawSystem, worldObjectsDrawSystem);


            //Create engine components
            int id = 1;
            var view = Matrix.CreateLookAt(new Vector3(200, 50, 50), new Vector3(0, 0, 0), Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);
            CameraComponent cameraComponent = new CameraComponent(id,view,  projection, true);
            ComponentManager.Instance.AddComponent(cameraComponent);

            Effect = new BasicEffect(GraphicsDevice);

            robotArm = new RobotArm(GraphicsDevice);
            robotArm.GetHeightMap(worldTerrain.GetHeightmapData());

            torso = new Player(Vector3.Zero, Vector3.Zero, new Vector3(50, 50, 50), GraphicsDevice);
            torso.HeightMap = worldTerrain.GetHeightmapData();
            Effect.VertexColorEnabled = true;
            Effect.Projection = cameraComponent.projection;
            Effect.View = cameraComponent.view;

            cameraSystem = new PlayerCameraSystem(torso);

            SystemManager.Instance.addToUpdateableQueue(cameraSystem);
        }

        private void CreateRobotArm(int entityId)
        {
            RobotArmComponent robotArmComponent = new RobotArmComponent(entityId);
            LowerArmComponent lowerArmComponent = new LowerArmComponent(entityId);
            CuboidMeshComponent cuboidMeshComponent = new CuboidMeshComponent(entityId, GraphicsDevice, 2, 1, 2, Effect);

            ComponentManager.Instance.AddComponent(robotArmComponent);
            ComponentManager.Instance.AddComponent(lowerArmComponent);
            ComponentManager.Instance.AddComponent(cuboidMeshComponent);
        }

        private List<House> CreateHouses(Model houseModel, int nModels)
        {
            List<House> houses = new List<House>();
            var heightmapData = worldTerrain.GetHeightmapData();
            modelPositions = GeneratePositions(heightmapData, nModels);
            houses.Add(new House(houseModel, modelPositions[0], treeTexture));
            for (int i = 1; i < nModels; i++)
            {
                var house = new House(houseModel, modelPositions[i], houseTexture);
                houses.Add(house);
            }
            
            return houses;
        }

        private List<Vector3> GeneratePositions(float[,] heightmapData, int nPositions)
        {
            List<Vector3> positions = new List<Vector3>();

            Random rnd = new Random();
            int x = -10;
            int z = -10;
            for (int i = 0; i < nPositions; i++)
            {
                x += 10;
                z += 10;
                positions.Add(new Vector3(x, heightmapData[Math.Abs(x) , Math.Abs(z)] + (houseModel.Meshes[0].BoundingSphere.Radius), z));
                //Console.WriteLine(x.ToString() + " " + z.ToString());
            }

             return positions;
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
            torso.Update();
            robotArm.Update(gameTime);
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
            torso.Draw(Effect, Matrix.Identity);
            robotArm.Draw(Effect, Matrix.Identity);
            SystemManager.Instance.Draw();

            //Console.WriteLine(torso.Position.ToString());

            base.Draw(gameTime);
        }
    }
}
