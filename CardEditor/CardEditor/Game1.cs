using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        private static Game1 instance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = START_WIDTH;
            graphics.PreferredBackBufferHeight = START_HEIGHT;

            instance = this;

            // set up menus
            main = new Menu();
            main.Add(new Button(new Rectangle(500, 200, 600, 200), "Gallery", () => { currentMenu = typeSelector; }));
            main.Add(new Button(new Rectangle(500, 500, 600, 200), "Testing", () => { 
                currentMenu = deckSelect; 

                // load decks
                deckSelect.Clear();
                deckSelect.Add(new Button(new Rectangle(700, 100, 200, 100), "New", () => { Editor.Save(); }));


            }));

            typeSelector = new Menu();
            typeSelector.Add(new Button(new Rectangle(600, 100, 400, 100), "Fortress", () => { 
                currentMenu = gallery;
                Gallery.Load(Card.Type.Fortress);
            }));
            typeSelector.Add(new Button(new Rectangle(600, 250, 400, 100), "Monster", () => { 
                currentMenu = gallery;
                Gallery.Load(Card.Type.Monster);
            }));
            typeSelector.Add(new Button(new Rectangle(600, 400, 400, 100), "Structure", () => { 
                currentMenu = gallery;
                Gallery.Load(Card.Type.Structure);
            }));
            typeSelector.Add(new Button(new Rectangle(600, 550, 400, 100), "Environment", () => { 
                currentMenu = gallery;
                Gallery.Load(Card.Type.Environment);
            }));
            typeSelector.Add(new Button(new Rectangle(600, 700, 400, 100), "Effect", () => { 
                currentMenu = gallery;
                Gallery.Load(Card.Type.Effect);
            }));
            typeSelector.Add(new Button(new Rectangle(200, 350, 200, 200), "Back", () => { currentMenu = main; }));

            editor = new Menu();
            editor.Add(new Button(new Rectangle(200, 150, 200, 100), "Save", () => { Editor.Save(); }));
            editor.Add(new Button(new Rectangle(200, 300, 200, 100), "Clear", () => { Editor.Clear(); }));
            editor.Add(new Button(new Rectangle(200, 450, 200, 100), "Back", () => { currentMenu = gallery; }));
            editor.Add(new Button(new Rectangle(200, 600, 200, 100), "Delete", () => { Editor.Delete(); currentMenu = gallery; }));

            gallery = new Menu();
            gallery.Add(new Button(new Rectangle(100, 250, 150, 150), "Back", () => { currentMenu = typeSelector; }));
            gallery.Add(new Button(new Rectangle(100, 500, 150, 150), "Create", () => { currentMenu = editor; Editor.New(Gallery.GetCurrentType()); }));

            deckSelect = new Menu(); // buttons created by main menu

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
    
            if(currentMenu == editor) {
                Editor.Update();
            }
            else if(currentMenu == gallery) {
                Gallery.Update();
            }
            currentMenu.Update();
            
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
            else if(currentMenu == gallery) {
                Gallery.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static bool JustClicked() {
            return Mouse.GetState().LeftButton == ButtonState.Pressed && !lastClicked;
        }

        public static void Edit(String name) {
            instance.currentMenu = instance.editor;
            Editor.Load(name);
        }
    }
}
