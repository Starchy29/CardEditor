using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardEditor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Texture2D Environment;
        public static Texture2D Fortress;
        public static Texture2D Effect;
        public static Texture2D Monster;
        public static Texture2D Structure;
        public static Texture2D Pixel;

        public static SpriteFont Font;

        public const int START_WIDTH = 1600;
        public const int START_HEIGHT = 900;

        private static bool lastClicked = false;

        private Menu currentMenu;

        private Menu main;
        private Menu editor;
        private Menu typeSelector;
        private Menu gallery;
        private Menu deckSelect;

        private bool editCard = true; // allows card type menu to be used for editor and gallery, false is gallery

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = START_WIDTH;
            graphics.PreferredBackBufferHeight = START_HEIGHT;

            Editor.Load("Starchy");

            // set up menus
            main = new Menu();
            main.Add(new Button(new Rectangle(500, 50, 600, 200), "Create", () => { currentMenu = typeSelector; }));
            main.Add(new Button(new Rectangle(500, 350, 600, 200), "Gallery", () => { currentMenu = gallery; }));
            main.Add(new Button(new Rectangle(500, 650, 600, 200), "Testing", () => { currentMenu = deckSelect; }));

            typeSelector = new Menu();
            typeSelector.Add(new Button(new Rectangle(600, 100, 400, 100), "Fortress", () => { 
                if(editCard) {
                    currentMenu = editor; 
                    Editor.New(Card.Type.Fortress); 
                } else {
                    currentMenu = gallery;
                }
            }));
            typeSelector.Add(new Button(new Rectangle(600, 250, 400, 100), "Monster", () => { 
                currentMenu = editor; 
                Editor.New(Card.Type.Monster); 
            }));
            typeSelector.Add(new Button(new Rectangle(600, 400, 400, 100), "Structure", () => { 
                currentMenu = editor; 
                Editor.New(Card.Type.Structure); 
            }));
            typeSelector.Add(new Button(new Rectangle(600, 550, 400, 100), "Environment", () => { 
                currentMenu = editor; 
                Editor.New(Card.Type.Environment); 
            }));
            typeSelector.Add(new Button(new Rectangle(600, 700, 400, 100), "Effect", () => { 
                currentMenu = editor; 
                Editor.New(Card.Type.Effect); 
            }));
            typeSelector.Add(new Button(new Rectangle(200, 350, 200, 200), "Back", () => { currentMenu = main; }));

            editor = new Menu();
            editor.Add(new Button(new Rectangle(200, 200, 200, 100), "Save", () => { Editor.Save(); }));
            editor.Add(new Button(new Rectangle(200, 400, 200, 100), "Clear", () => { Editor.Clear(); }));
            editor.Add(new Button(new Rectangle(200, 600, 200, 100), "Back", () => { currentMenu = typeSelector; }));

            gallery = new Menu();
            editor.Add(new Button(new Rectangle(100, 625, 150, 150), "Back", () => { currentMenu = typeSelector; }));

            currentMenu = main;
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
            Environment = Content.Load<Texture2D>("Images\\environment");
            Effect = Content.Load<Texture2D>("Images\\effect");
            Monster = Content.Load<Texture2D>("Images\\monster");
            Structure = Content.Load<Texture2D>("Images\\structure");
            Fortress = Content.Load<Texture2D>("Images\\fortress");
            Pixel = Content.Load<Texture2D>("Images\\Pixel");

            Font = Content.Load<SpriteFont>("Font");
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
    
            currentMenu.Update();
            if(currentMenu == editor) {
                Editor.Update();
            }

            lastClicked = Mouse.GetState().LeftButton == ButtonState.Pressed;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray * 0.2f);

            spriteBatch.Begin();
            currentMenu.Draw(spriteBatch);
            if(currentMenu == editor) {
                Editor.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static bool JustClicked() {
            return Mouse.GetState().LeftButton == ButtonState.Pressed && !lastClicked;
        }
    }
}
