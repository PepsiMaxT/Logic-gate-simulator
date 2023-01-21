using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace Farming
{
    public class Game1 : Game
    {
        private static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        RenderTarget2D renderTarget;
        public float scale = 0.44444f;
        bool F11PressedLastFrame = false;

        //Gates
        Texture2D[] gateTexture;
        GateType gateSelected = GateType.OR;
        int parsedGate = 0;
        Rectangle potentialLocation;
        bool willIntersect;

        List<Gate> gateList = new List<Gate>();

        Texture2D whiteRectangle;
        SpriteFont arial;

        MouseState lastMouse;
        MouseState mouse;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            mouse = Mouse.GetState();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = (int)(graphics.PreferredBackBufferWidth / (16f / 9f));
            graphics.ApplyChanges();
            
            scale = 1f / (1080f / graphics.GraphicsDevice.Viewport.Height);

            gateList.Add(new Gate(Vector2.Zero, GateType.OR, gateTexture));
            gateList.Add(new Gate(new Vector2(64, 0), GateType.AND, gateTexture));
            gateList.Add(new Gate(new Vector2(128, 0), GateType.NOT, gateTexture));
            gateList.Add(new Gate(new Vector2(192, 0), GateType.XOR, gateTexture));
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, 1920, 1080);

            // TODO: use this.Content to load your game content here
            whiteRectangle = new Texture2D(graphics.GraphicsDevice, 1, 1);
            whiteRectangle.SetData<Color>(new[] { Color.White });

            arial = Content.Load<SpriteFont>("Arial");

            //gates
            gateTexture = new Texture2D[4] {
                Content.Load<Texture2D>("Gates/OR"),
                Content.Load<Texture2D>("Gates/AND"),
                Content.Load<Texture2D>("Gates/NOT"),
                Content.Load<Texture2D>("Gates/XOR")
            };
        }

        protected override void Update(GameTime gameTime)
        {
            lastMouse = mouse;
            mouse = Mouse.GetState();

            //Full screening
            if (Keyboard.GetState().IsKeyDown(Keys.F11) && !F11PressedLastFrame)
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
                scale = 1f / (1080f / graphics.GraphicsDevice.Viewport.Height);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F11)) F11PressedLastFrame = true;
            else F11PressedLastFrame = false;


            // TODO: Add your update logic here
            if (Keyboard.GetState().GetPressedKeyCount() > 0 && int.TryParse(Keyboard.GetState().GetPressedKeys()[0].ToString(), out parsedGate) && parsedGate > 0 && parsedGate <= 4)
            {
                gateSelected = (GateType)(parsedGate - 1);
            }

            potentialLocation = new Rectangle((int)(mouse.X / scale) - (Gate.scale * gateTexture[(int)gateSelected].Width / 2), (int)(mouse.Y / scale) - (Gate.scale * gateTexture[(int)gateSelected].Width / 2), gateTexture[(int)gateSelected].Width * Gate.scale, gateTexture[(int)gateSelected].Height * Gate.scale);
            willIntersect = false;
            foreach (Gate gate in gateList)
            {
                if (gate.rectangle.Intersects(potentialLocation)) willIntersect = true;
            }
            if (mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton != ButtonState.Pressed)
            {
                if (!(willIntersect)) {
                    gateList.Add(new Gate(potentialLocation, gateSelected, gateTexture));
                }
            }            

            if (mouse.RightButton == ButtonState.Pressed && lastMouse.RightButton != ButtonState.Pressed)
            {
#nullable enable
                Gate? gateToBeRemoved = null;
                foreach (Gate gate in gateList)
                {
                    if (gate.rectangle.Contains(new Vector2(mouse.X / scale, mouse.Y / scale)))
                    {
                        Point positionInGate = new Point((int)(mouse.X / scale - gate.rectangle.Left) / Gate.scale, (int)(mouse.Y/scale - gate.rectangle.Top) / Gate.scale);
                        Color[] textureData = new Color[gate.texture.Width * gate.texture.Height];
                        gate.texture.GetData<Color>(textureData);
                        if (textureData[gate.texture.Width * positionInGate.Y + positionInGate.X] != new Color(0, 0, 0, 0))gateToBeRemoved = gate;
                        break;
                    }
                }
                if (gateToBeRemoved != null)gateList.Remove(gateToBeRemoved);
#nullable disable
            }
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            foreach (Gate gate in gateList)
            {
                spriteBatch.Draw(gate.texture, gate.rectangle, Color.White);
            }
            if (willIntersect) spriteBatch.Draw(gateTexture[(int)gateSelected], potentialLocation, null, Color.Red * 0.3f, 0f, Vector2.Zero, SpriteEffects.None, 0);
            else spriteBatch.Draw(gateTexture[(int)gateSelected], potentialLocation, null, Color.White * 0.3f, 0f, Vector2.Zero, SpriteEffects.None, 0);

            for (int i = 0; i < 64; i++)
            {
                if (i % 2 == 0) spriteBatch.Draw(whiteRectangle, new Rectangle(i * 30, 0, 30, 0), Color.White);
                else spriteBatch.Draw(whiteRectangle, new Vector2(i * 60, 0), Color.Orange);
            }

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(renderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

            spriteBatch.End();



            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}