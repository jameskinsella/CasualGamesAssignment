using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework.Audio;

namespace JKcgMultiplayerAssessment
{
    
    public class Game1 : Game
    {

        #region Misc

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public int GameLength;
        public int GameLengthIncrementer = 0;
        Player Player1;
        Opponent Player2;
        public Messi messi;
        public Ronaldo ronaldo;
        public KeyboardState keyState;
        public KeyboardState PreviouskeyState;
        SpriteFont font;
        Camera camera;
        int PreferredBackBufferWidth = 1200;
        int PreferredBackBufferHeight = 800;
        public List<HidingBox> HidingBoxList = new List<HidingBox>();
        public List<Collectable> CollectableList = new List<Collectable>();
        public Rectangle SelectCharacterBoxPosition;
        public SpecialCollectable specialCollectable;
        public Random rnd = new Random();
        #endregion Misc
        #region SignalR
        public IHubProxy proxy;
        HubConnection connection = new HubConnection("http://localhost:14458/");
        public static Action<string, Vector2, int, bool, float,int> _PlayerPositions;
        public static Action<string, Vector2, Vector2, float> _BallCreated;
        Action<string> _PlayerID;
        Action _BothConnected;
        Action<string, bool, int> _GetOpponentCharacter;
        Action<Vector2> _RecieveCollectablePosition;
        Action<string, bool> _GameOver;
        Action<Point> _GetSuperCollectablePosition;
        string PlayerID;
        static Timer GenerateCollectables;
        public bool GameStarting = false;
        int CheckBothCharactersSelected = 0;
        //static Timer StartGameTimer;
        static TimeSpan StartGameTimer;
        bool StartGameTimerStart = false;
        public bool MessiSelected = true;
        public bool OpponentMessiSelected = true;
        public bool CharacterSelected = false;
        public Vector2 CollectablePosition;
        public bool BothPlayersConnected = false;
        public bool Winner = false;
        public Point SendSpecialCollectablePosition;
        public Point RecievedSpecialCollectablePosition;
        public SoundEffect BallKicked;
        public SoundEffect VoteCollected;
        public SoundEffect PlayerHit;
        #endregion     
        #region Textures
        public List<Texture2D> MessiTex;
        public List<Texture2D> MessiShootingTex;
        public List<Texture2D> RonaldoTex;
        public List<Texture2D> RonaldoShootingTex;
        public Texture2D HealthBar;
        public Texture2D HealthContainer;
        public Texture2D PowerBar;
        public Texture2D PowerContainer;
        public Texture2D FootballTex;
        public Texture2D HidingBoxTex;
        public Texture2D CollectableTex;
        public Texture2D SpecialCollectableTex;
        public Texture2D Background;
        public Texture2D StarTex;
        public Texture2D MenuBackGround;
        public Texture2D WaitPlayer2;
        public Texture2D Player2Connected;
        public Texture2D SelectCharacter;
        public Texture2D CharacterSelectPrompt;
        public Texture2D CharacterSelectConfirm;
        public Texture2D MessiWinTex;
        public Texture2D MessiLoseTex;
        public Texture2D RonaldoWinTex;
        public Texture2D RonaldoLoseTex;
        public int GameLengthCounter = 0;
        public bool SpecialInit = false;
        public bool SpecialCreated = false;
        #endregion Textures
        enum GameState
        {
            titleScreen,
            playingGame,
            endScreen,
        }

        GameState state = GameState.titleScreen;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = PreferredBackBufferWidth;
            graphics.PreferredBackBufferHeight = PreferredBackBufferHeight;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            
        }
        protected override void Initialize()
        {


            #region StartProxys
            proxy = connection.CreateHubProxy("GameServer");

            _PlayerID = Get_PlayerID;
            proxy.On("SendPlayerID", _PlayerID);

            _BothConnected = BothConnected;
            proxy.On("BothPlayersConnected", _BothConnected);

            _GetOpponentCharacter = GetOpponentCharacter;
            proxy.On("SendPlayerCharacters", _GetOpponentCharacter);

            _RecieveCollectablePosition = RecieveCollectablePosition;
            proxy.On("SendCollectablePosition", _RecieveCollectablePosition);

            _PlayerPositions = GetOpponentPosition;
            proxy.On("SendPositions", _PlayerPositions);

            _BallCreated = CheckBallCreated;
            proxy.On("BallCreated", _BallCreated);

            _GameOver = GameOver;
            proxy.On("GameEnded", _GameOver);

            _GetSuperCollectablePosition = GetSuperCollectablePosition;
            proxy.On("SendSpecialCollectablePosition", _GetSuperCollectablePosition);
            #endregion proxy


            GameLength = rnd.Next(3,4);
            camera = new Camera(GraphicsDevice.Viewport);
            graphics.ApplyChanges();
            /*
            StartGameTimer = new Timer(5000);
            StartGameTimer.AutoReset = false;
            StartGameTimer.Elapsed += StartGameTimer_T_Elapsed;
            */
            StartGameTimer = TimeSpan.FromMilliseconds(3000);
            
            GenerateCollectables = new Timer(4000);
            GenerateCollectables.AutoReset = true;
            GenerateCollectables.Elapsed += GenerateCollectables_T_Elapsed;
            base.Initialize();
        }

        private void GetSuperCollectablePosition(Point _SuperCollectPosition)
        {
            SpecialCreated = true;
            RecievedSpecialCollectablePosition = _SuperCollectPosition;
        }

        private void GameOver(string _PlayerID, bool _Death)
        {
            if (_Death)
            {
                if (_PlayerID != Player1.PlayerID)
                {
                    Winner = true;
                }
            }
            else
            {
                if (Player1.VotesCollected > Player2.VotesCollected)
                {
                    Winner = true;
                }
                else if (Player1.VotesCollected == Player2.VotesCollected)
                {
                    if (PlayerID == _PlayerID)
                    {
                        Winner = true;
                    }
                }
            }
            state = GameState.endScreen;
        }

        public void CheckIfGameOver()
        {
            if (Player1.Health < 1)
            {
                proxy.Invoke("GameOver", PlayerID,true);
            }
        }
        #region Misc Methods


        public void InitializeGamePlay()
        {

            Vector2 Player1Position;
            Vector2 EnemyPosition;
            if (PlayerID == "Player1")
            {
                Player1Position = new Vector2(-200, 0) - new Vector2((MessiTex[0].Width/3)/2,MessiTex[0].Height/2);
                EnemyPosition = new Vector2(200, 0) - new Vector2((MessiTex[0].Width / 3) / 2, MessiTex[0].Height / 2);
                
            }
            else
            {
                Player1Position = new Vector2(200, 0) - new Vector2((MessiTex[0].Width / 3) / 2, MessiTex[0].Height / 2); ;
                EnemyPosition = new Vector2(-200, 0) - new Vector2((MessiTex[0].Width / 3) / 2, MessiTex[0].Height / 2); ;
            }
            if (MessiSelected)
            {

                Player1 = new Player(MessiTex, Player1Position, 3, "Messi", PlayerID, this);
            }
            else
            {

                Player1 = new Player(RonaldoTex, Player1Position, 3, "Ronaldo", PlayerID, this);
            }
            if (OpponentMessiSelected)
            {
                Player2 = new Opponent(MessiTex, EnemyPosition, 3, CollectableList, this);
            }
            else
            {
                Player2 = new Opponent(RonaldoTex, EnemyPosition, 3, CollectableList, this);
            }
        }
        private void RecieveCollectablePosition(Vector2 _CollectablePosition)
        {
            CollectableList.Add(new Collectable(CollectableTex, _CollectablePosition));
            Player1.FootballsRemaining += 1;
            GameLengthIncrementer++;
            if (GameLengthIncrementer == GameLength)
            {
                
            }
        }

        private void GenerateCollectables_T_Elapsed(object sender, ElapsedEventArgs e)
        {
            CollectablePosition.X = rnd.Next(-1500, 1250);
            CollectablePosition.Y = rnd.Next(-720, 545);
            proxy.Invoke("SendCollectablePosition", CollectablePosition);
            GameLengthCounter++;
            
            if (GameLength == GameLengthCounter)
            {
                if (!SpecialCreated)
                {
                    CollectablePosition.X = rnd.Next(-1500, 1250);
                    CollectablePosition.Y = rnd.Next(-720, 545);
                    specialCollectable = new SpecialCollectable(SpecialCollectableTex, new Point((int)CollectablePosition.X,(int)CollectablePosition.Y), Player1, Player2, 3000, 1400);
                    SpecialCreated = true;
                }
            }
        }
        private void GetOpponentCharacter(string _PlayerID, bool _OpponentMessiSelected, int _BothSelected)
        {
            if (_PlayerID == PlayerID)
            {
                if (_OpponentMessiSelected)
                {
                    OpponentMessiSelected = true;
                }
                else
                {
                    OpponentMessiSelected = false;
                }
                CheckBothCharactersSelected++;
            }

            if (CheckBothCharactersSelected > 2)
            {
                StartGameTimerStart = true;
                GameStarting = true;
            }
        }

        private void BothConnected()
        {
            BothPlayersConnected = true;
        }
        private void GetOpponentPosition(string _PlayerID, Vector2 _OppositionPosition, int _SpriteDirection, bool _Moving, float _Health, int _EnemyVotesCollected)
        {
            if (PlayerID == _PlayerID)
            {
                Player2.Position = _OppositionPosition;
                Player2.Direction = _SpriteDirection;
                Player2.Moving = _Moving;
                Player2.Health = _Health;
                Player2.VotesCollected = _EnemyVotesCollected;
            }
        }
        private void CheckBallCreated(string _PlayerID, Vector2 _Position, Vector2 _Direction, float _ShotPower)
        {
            if (_PlayerID == Player1.PlayerID)
            {
                Player1.OpponentFootballList.Add(new Football(FootballTex, _Position, _ShotPower, _Direction, HidingBoxList, CollectableList, false));
                BallKicked.Play();
            }
        }
        public void Get_PlayerID(string ID)
        {
            if (PlayerID == null)
            {
                PlayerID = ID;
            }
        }
        #endregion Misc Methods
        protected override void LoadContent()
        {
            #region LoadTextures
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>(@"font");


            MessiTex = new List<Texture2D> { Content.Load<Texture2D>(@"Tex_Messi_Down"),
                                            (Content.Load<Texture2D>(@"Tex_Messi_Up")),
                                            (Content.Load<Texture2D>(@"Tex_Messi_Right")),
                                            (Content.Load<Texture2D>(@"Tex_Messi_Left")),
                                            };

            RonaldoTex = new List<Texture2D> { Content.Load<Texture2D>(@"Tex_Ronaldo_Down"),
                                            (Content.Load<Texture2D>(@"Tex_Ronaldo_Up")),
                                            (Content.Load<Texture2D>(@"Tex_Ronaldo_Right")),
                                            (Content.Load<Texture2D>(@"Tex_Ronaldo_Left"))
                                            };

            MessiShootingTex = new List<Texture2D> { Content.Load<Texture2D>(@"Tex_Messi_Down_Shoot"),
                                            (Content.Load<Texture2D>(@"Tex_Messi_Up_Shoot")),
                                            (Content.Load<Texture2D>(@"Tex_Messi_Right_Shoot")),
                                            (Content.Load<Texture2D>(@"Tex_Messi_Left_Shoot")),
                                            };
            RonaldoShootingTex = new List<Texture2D> { Content.Load<Texture2D>(@"Tex_Ronaldo_Down_Shoot"),
                                            (Content.Load<Texture2D>(@"Tex_Ronaldo_Up_Shoot")),
                                            (Content.Load<Texture2D>(@"Tex_Ronaldo_Right_Shoot")),
                                            (Content.Load<Texture2D>(@"Tex_Ronaldo_Left_Shoot")),
                                            };
            FootballTex = Content.Load<Texture2D>(@"Tex_Football");
            StarTex = Content.Load<Texture2D>(@"Star");
            HealthContainer = Content.Load<Texture2D>(@"Tex_HealthbarContainer");
            HealthBar = Content.Load<Texture2D>(@"Tex_GreenHealthBar");
            PowerContainer = Content.Load<Texture2D>(@"Tex_PowerBarContainer");
            PowerBar = Content.Load<Texture2D>(@"Tex_PowerBar");
            CollectableTex = Content.Load<Texture2D>(@"Tex_Collectable");
            SpecialCollectableTex = Content.Load<Texture2D>(@"Tex_SpecialCollectable");
            Background = Content.Load<Texture2D>(@"Pitch");
            HidingBoxTex = Content.Load<Texture2D>(@"Tex_HidingBox");
            MenuBackGround = Content.Load<Texture2D>(@"MessiRonaldoBG");
            WaitPlayer2 = Content.Load<Texture2D>(@"WaitTex");
            Player2Connected = Content.Load<Texture2D>(@"Player2Connected");
            SelectCharacter = Content.Load<Texture2D>(@"SelectCharacter");
            CharacterSelectPrompt = Content.Load<Texture2D>(@"SelectYourCharacterPrompt");
            CharacterSelectConfirm = Content.Load<Texture2D>(@"CharacterSelectConfirm");
            RonaldoWinTex = Content.Load<Texture2D>(@"RonaldoWinTex");
            RonaldoLoseTex = Content.Load<Texture2D>(@"RonaldoLoseTex");
            MessiWinTex = Content.Load<Texture2D>(@"MessiWinTex");
            MessiLoseTex = Content.Load<Texture2D>(@"MessiLoseTex");

            #endregion LoadTextures
            BallKicked = Content.Load<SoundEffect>(@"BallKicked");
            VoteCollected = Content.Load<SoundEffect>(@"VoteCollected");
            PlayerHit = Content.Load<SoundEffect>(@"PlayerHit");


            messi = new Messi(this);
            ronaldo = new Ronaldo(this);

            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(485, 485)));
            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(1200, 300)));

            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(-485, 485)));
            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(-1200, 300)));

            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(485, -485)));
            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(1200, -300)));

            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(-485, -485)));
            HidingBoxList.Add(new HidingBox(HidingBoxTex, new Vector2(-1200, -300)));


            connection.Start().Wait();

        }
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        #region DrawSections

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            switch (state)
            {
                case GameState.titleScreen:
                    DrawTitleScreen();
                    break;
                case GameState.playingGame:
                    DrawGamePlay();
                    break;
                case GameState.endScreen:
                    DrawEndScreen();
                    break;
            }

            base.Draw(gameTime);
        }
        public void DrawTitleScreen()
        {

            spriteBatch.Begin();
            spriteBatch.Draw(MenuBackGround, new Rectangle(GraphicsDevice.Viewport.Width / 2 - (MenuBackGround.Width / 2), GraphicsDevice.Viewport.Height - MenuBackGround.Height, MenuBackGround.Width, MenuBackGround.Height), Color.White);

            if (BothPlayersConnected)
            {
                spriteBatch.Draw(Player2Connected, new Rectangle(GraphicsDevice.Viewport.Width / 2 - (Player2Connected.Width / 2), 100, Player2Connected.Width, Player2Connected.Height), Color.White);

                if (CharacterSelected == false)
                {
                    spriteBatch.Draw(SelectCharacter, SelectCharacterBoxPosition, Color.White * 0.1f);
                    spriteBatch.Draw(CharacterSelectPrompt, new Rectangle(GraphicsDevice.Viewport.Width / 2 - (CharacterSelectPrompt.Width / 2), GraphicsDevice.Viewport.Height / 4, CharacterSelectPrompt.Width, CharacterSelectPrompt.Height), Color.White);
                }
                else
                {
                    spriteBatch.Draw(CharacterSelectConfirm, new Rectangle(GraphicsDevice.Viewport.Width / 2 - (CharacterSelectConfirm.Width / 2), GraphicsDevice.Viewport.Height / 4, CharacterSelectConfirm.Width, CharacterSelectConfirm.Height), Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(WaitPlayer2, new Rectangle(GraphicsDevice.Viewport.Width / 2 - (WaitPlayer2.Width / 2), 100, WaitPlayer2.Width, WaitPlayer2.Height), Color.White);
            }
            if (StartGameTimerStart)
            {
                spriteBatch.DrawString(font, "Starting Game", new Vector2((GraphicsDevice.Viewport.Width / 2) - 20, GraphicsDevice.Viewport.Height - 300), Color.White);
                int CountdownTimer = int.Parse(StartGameTimer.Seconds.ToString());
                spriteBatch.DrawString(font,"       " + (CountdownTimer+1).ToString(), new Vector2((GraphicsDevice.Viewport.Width / 2), GraphicsDevice.Viewport.Height - 260), Color.White);
            }
            spriteBatch.End();
        }
        public void DrawGamePlay()
        {
            proxy.Invoke("RecievePlayerPositions", PlayerID, Player1.Position, Player1.SpriteDirection, Player1.Moving, Player1.Health, Player1.VotesCollected).Wait();
            if (Player1.PlayerID == "Player1")
                if (specialCollectable != null)
                    proxy.Invoke("SendSpecialCollectablePosition", SendSpecialCollectablePosition);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
            spriteBatch.Draw(Background, new Rectangle(-2400, -1200, Background.Width, Background.Height), Color.White);
            Player1.Draw(spriteBatch, font);
            Player2.Draw(spriteBatch);
            foreach (HidingBox HidingBox in HidingBoxList)
            {
                HidingBox.Draw(spriteBatch);
            }
            foreach (Collectable Collectable in CollectableList)
            {
                Collectable.Draw(spriteBatch);
            }
            if (specialCollectable != null)
                specialCollectable.Draw(spriteBatch,RecievedSpecialCollectablePosition);
            spriteBatch.End();
        }
        public void DrawEndScreen()
        {
            spriteBatch.Begin();
            Vector2 PlayerEndScreenPosition;
            Vector2 EnemyEndScreenPosition;
            Vector2 EndTexSize = new Vector2(MessiWinTex.Width, MessiWinTex.Height);

            if (Player1.PlayerID == "Player1")
            {
                 PlayerEndScreenPosition = new Vector2(20, PreferredBackBufferHeight/2-(EndTexSize.Y/2));
                 EnemyEndScreenPosition = new Vector2(PreferredBackBufferWidth - EndTexSize.X - 20, PreferredBackBufferHeight / 2 - (EndTexSize.Y / 2));
            }
            else
            {
                PlayerEndScreenPosition = new Vector2(PreferredBackBufferWidth - EndTexSize.X - 20, PreferredBackBufferHeight / 2 - (EndTexSize.Y / 2));
                EnemyEndScreenPosition = new Vector2(20, PreferredBackBufferHeight / 2 - (EndTexSize.Y / 2));
            }
            Vector2 EndScreenWinnerTextPosition = new Vector2(PreferredBackBufferWidth/2-20, PreferredBackBufferHeight/2);

            if (Winner)
                {
                    if (MessiSelected)
                    {
                        spriteBatch.Draw(MessiWinTex, PlayerEndScreenPosition, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(RonaldoWinTex, PlayerEndScreenPosition, Color.White);
                    }
                    if (OpponentMessiSelected)
                    {
                        spriteBatch.Draw(MessiLoseTex, EnemyEndScreenPosition, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(RonaldoLoseTex, EnemyEndScreenPosition, Color.White);
                    }
                    spriteBatch.DrawString(font,"You Win", EndScreenWinnerTextPosition, Color.White);
                }
                else
                {
                    if (MessiSelected)
                    {
                        spriteBatch.Draw(MessiLoseTex, PlayerEndScreenPosition, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(RonaldoLoseTex, PlayerEndScreenPosition, Color.White);
                    }
                    if (OpponentMessiSelected)
                    {
                        spriteBatch.Draw(MessiWinTex, EnemyEndScreenPosition, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(RonaldoWinTex, EnemyEndScreenPosition, Color.White);
                    }
                    spriteBatch.DrawString(font,"You Lose", EndScreenWinnerTextPosition, Color.White);
                

            }
            spriteBatch.End();
        }
        #endregion DrawSections
        #region UpdateSections

        protected override void Update(GameTime gameTime)
        {
            keyState = Keyboard.GetState();

            switch (state)
            {
                case GameState.titleScreen:
                    UpdateTitleScreen(gameTime);
                    break;
                case GameState.playingGame:
                    UpdateGamePlay(gameTime);
                    break;
                case GameState.endScreen:
                    UpdateEndScreen();
                    break;
            }
            PreviouskeyState = keyState;
            base.Update(gameTime);
        }
        public void UpdateTitleScreen(GameTime gameTime)
        {
            if (BothPlayersConnected)
            {
                if (CharacterSelected == false)
                {
                    if (keyState != PreviouskeyState)
                        if (keyState.IsKeyDown(Keys.D))
                            MessiSelected = false;
                    if (keyState != PreviouskeyState)
                        if (keyState.IsKeyDown(Keys.A))
                            MessiSelected = true;
                    if (MessiSelected)
                    {
                        SelectCharacterBoxPosition = new Rectangle(GraphicsDevice.Viewport.Width / 2 - (MenuBackGround.Width / 2), GraphicsDevice.Viewport.Height - MenuBackGround.Height, SelectCharacter.Width, SelectCharacter.Height);
                    }
                    else
                    {
                        SelectCharacterBoxPosition = new Rectangle(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - MenuBackGround.Height, SelectCharacter.Width, SelectCharacter.Height);
                    }
                    if (keyState != PreviouskeyState)
                        if (keyState.IsKeyDown(Keys.Enter))
                        {
                            CharacterSelected = true;
                            CheckBothCharactersSelected++;
                            proxy.Invoke("SendCharacterSelected", PlayerID, MessiSelected, CheckBothCharactersSelected);
                        }
                }
            }
            if (StartGameTimerStart)
            {
                StartGameTimer -= gameTime.ElapsedGameTime;
            }
            if (StartGameTimer < TimeSpan.Zero)
            {
                InitializeGamePlay();
                state = GameState.playingGame;
                if (PlayerID == "Player1")
                {
                    GenerateCollectables.Start();
                }
            }
            if (keyState != PreviouskeyState)
                if (keyState.IsKeyDown(Keys.T))
                    state = GameState.playingGame;
        }
        public void UpdateGamePlay(GameTime gameTime)
        {
            /*if (TurnOnProxy == false)
            {
                
            }*/
            

            if (Player1.PlayerID == "Player1")
                if (specialCollectable != null)
                    SendSpecialCollectablePosition = specialCollectable.GetPosition();
            if (specialCollectable != null)
                specialCollectable.Update(RecievedSpecialCollectablePosition);
            Player1.Update(gameTime);
            Player2.Update(gameTime);
            camera.Update(gameTime, Player1, PreferredBackBufferWidth, PreferredBackBufferHeight);
            CheckIfGameOver();
            if (SpecialInit == false)
            {
                if (SpecialCreated == true)
                {
                    specialCollectable = new SpecialCollectable(SpecialCollectableTex, RecievedSpecialCollectablePosition, Player1, Player2, 3000, 1400);
                    SpecialInit = true;
                }
            }
        }
        
        public void UpdateEndScreen()
        {
            
        }

        #endregion UpdateSections
    }
}