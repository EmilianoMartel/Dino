using Raylib_cs;
using System.Net.NetworkInformation;
using System.Numerics;

public class Program
{
    //Screen
    const int SCREEN_WIDTH = 1240;
    const int SCREEN_HEIGTH = 720;

    static Random random = new Random();

    //CONST
    const int FLOOR_HEIGHT = 600;
    const int FLOOR_LENGTH = 2400;
    const int CLOUD_LENGTH = 95;
    const float ORIGINAL_SPEED = 700f;
    const float JUMP_IMPULSE = 900f;
    const float GRAVITY = 2100f;
    const float RISE_SPEED = 100f;
    const float MAX_SPEED = 1400f;
    const int SCORE_SPEED_CHANGE = 20;

    //Images
    static Image imagePlayer = Raylib.LoadImage("assets/dinosaur.png");
    static Image imageCactusLarge = Raylib.LoadImage("assets/cacti1.png");
    static Image imageCactus2 = Raylib.LoadImage("assets/cacti2.png");
    static Image imageCactus3 = Raylib.LoadImage("assets/cacti3.png");
    static Image imageFloor = Raylib.LoadImage("assets/floor.png");
    static Image imageFloor2 = Raylib.LoadImage("assets/floor.png");
    static Image imageCloud1 = Raylib.LoadImage("assets/cloud.png");
    static Image imageCloud2 = Raylib.LoadImage("assets/cloud.png");

    //Textures
    static List<Texture2D> cactusTextureList = new List<Texture2D>();

    //Player view position
    static Vector2 playerPosition = new Vector2(150, FLOOR_HEIGHT);

    //Gameplay
    static float gameSpeed = ORIGINAL_SPEED;
    static bool canChange = true;

    //Score
    static string scoreText = "000000";
    static float score = 0;
    static float timerScore = 0;
    static float highScore = 0;
    static int hp = 1;
    static bool gameStart = false;
    static bool isJumping = false;
    static float playerSpeed = 0f;
    static float delta = 0;

    //Cactus variables
    static float timeBetweenCactus;
    const float MIN_TIME = 0.9f;
    const float MAX_TIME = 2.7f;
    static float timerCactus;


    //Cactus Position
    static Vector2[] cactusPositionArray = new Vector2[10];
    static bool[] isMovingArray = new bool[10];
    static int indexArray = 0;
    static Texture2D[] cactusActiveTextureArray = new Texture2D[10];
    static Rectangle[] boundArray = new Rectangle[10];

    //Floor position
    static Vector2 floorPosition = new Vector2(0, FLOOR_HEIGHT);
    static Vector2 floorPosition2 = new Vector2(0, FLOOR_HEIGHT);

    //Cloud Position
    static Vector2 cloudPosition = new Vector2(SCREEN_WIDTH, SCREEN_HEIGTH / 2);
    static Vector2 cloudPosition2 = new Vector2(SCREEN_WIDTH * 2, cloudPosition.Y - 50);

    //UI variables
    static int gameOverTextPositionY = 1000;

    public static void Main()
    {
        timeBetweenCactus = (float)(random.NextDouble() * (MAX_TIME - MIN_TIME) + MIN_TIME);

        Color currentColor = Color.WHITE;

        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGTH, "Dino Game");

        //Textures
        Texture2D texturePlayer = Raylib.LoadTextureFromImage(imagePlayer);
        Raylib.UnloadImage(imagePlayer);

        cactusTextureList.Add(Raylib.LoadTextureFromImage(imageCactusLarge));
        Raylib.UnloadImage(imageCactusLarge);

        cactusTextureList.Add(Raylib.LoadTextureFromImage(imageCactus2));
        Raylib.UnloadImage(imageCactus2);

        cactusTextureList.Add(Raylib.LoadTextureFromImage(imageCactus2));
        Raylib.UnloadImage(imageCactus3);

        Texture2D textureFloor = Raylib.LoadTextureFromImage(imageFloor);
        Raylib.UnloadImage(imageFloor);

        Texture2D textureFloor2 = Raylib.LoadTextureFromImage(imageFloor2);
        Raylib.UnloadImage(imageFloor2);

        Texture2D textureCloud1 = Raylib.LoadTextureFromImage(imageCloud1);
        Raylib.UnloadImage(imageCloud1);

        Texture2D textureCloud2 = Raylib.LoadTextureFromImage(imageCloud2);
        Raylib.UnloadImage(imageCloud2);

        floorPosition2.X = floorPosition.X + FLOOR_LENGTH;

        while (!Raylib.WindowShouldClose())
        {
            delta = Raylib.GetFrameTime();

            //Gameplay
            if (gameStart == true)
            {
                UpdateGameplay();
                UpdatePlayer();
                UpdateFloor();

                timerScore += Raylib.GetFrameTime();

                if (timerScore >= 1f)
                {
                    canChange = true;
                    score += 1;
                    timerScore = 0f;
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_A))
                {
                    NewCactusLogic();
                }

                NewCactusLogic();
                UpdateCactus();
            }

            else if (hp == 0)
            {
                gameOverTextPositionY = SCREEN_HEIGTH / 2;
            }

            //Start game Logic
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                gameStart = true;
            }

            if (hp <= 0)
            {
                gameStart = false;
            }

            //RestartOption
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_R) && !gameStart)
            {
                ResetGame();
            }

            UpdateClouds();

            //Collision
            Rectangle boundsPlayer = new Rectangle((int)playerPosition.X - texturePlayer.width / 2, (int)playerPosition.Y - texturePlayer.height, texturePlayer.width, texturePlayer.height);

            CheckCollissions(boundsPlayer);

            Raylib.BeginDrawing();

            //Drawing gameplay
            Raylib.ClearBackground(currentColor);

            Raylib.DrawTexture(textureCloud1, (int)cloudPosition.X - textureCloud1.width / 2, (int)cloudPosition.Y - textureCloud1.height, Color.WHITE);
            Raylib.DrawTexture(textureCloud2, (int)cloudPosition2.X - textureCloud1.width / 2, (int)cloudPosition2.Y - textureCloud2.height, Color.WHITE);
            Raylib.DrawTexture(texturePlayer, (int)playerPosition.X - texturePlayer.width / 2, (int)playerPosition.Y - texturePlayer.height, Color.WHITE);
            Raylib.DrawTexture(textureFloor, (int)floorPosition.X, FLOOR_HEIGHT, Color.WHITE);
            Raylib.DrawTexture(textureFloor2, (int)floorPosition2.X, FLOOR_HEIGHT, Color.WHITE);

            DrawUI();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private static void DrawUI()
    {
        int delete;
        string addText = score.ToString();
        delete = 6 - addText.Length;
        scoreText = scoreText.Substring(0, delete) + addText;
        Raylib.DrawText("Score: " + scoreText, 700, 30, 30, Color.DARKGRAY);
        Raylib.DrawText("HighScore: " + highScore, 1000, 30, 30, Color.DARKGRAY);

        Raylib.DrawText("press the SPACEBAR to JUMP", 30, 30, 20, Color.DARKGRAY);
        if (!gameStart)
        {
            Raylib.DrawText("JUMP TO START", 30, 60, 20, Color.DARKGRAY);
        }

        Raylib.DrawRectangle(0, 0, (int)SCREEN_WIDTH, 5, Color.DARKGRAY);
        Raylib.DrawRectangle(0, 5, 5, (int)SCREEN_HEIGTH - 10, Color.DARKGRAY);
        Raylib.DrawRectangle((int)SCREEN_WIDTH - 5, 5, 5, (int)SCREEN_HEIGTH - 10, Color.DARKGRAY);
        Raylib.DrawRectangle(0, (int)SCREEN_HEIGTH - 5, (int)SCREEN_WIDTH, 5, Color.DARKGRAY);

        Raylib.DrawText("GAME OVER", (SCREEN_WIDTH / 2) - 150, gameOverTextPositionY, 50, Color.DARKGRAY);
        Raylib.DrawText("press R to RESTART", (SCREEN_WIDTH / 2) - 160, gameOverTextPositionY + 50, 30, Color.DARKGRAY);
    }

    private static Vector2 UpdatePlayer()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) && !isJumping)
        {
            playerSpeed = -JUMP_IMPULSE;
            isJumping = true;
        }

        if (isJumping)
        {
            playerPosition.Y += playerSpeed * delta + GRAVITY / 2 * delta * delta;
            playerSpeed += GRAVITY * delta;

            if (playerPosition.Y >= FLOOR_HEIGHT)
            {
                playerSpeed = 0;
                playerPosition.Y = FLOOR_HEIGHT;
                isJumping = false;
            }
        }

        return playerPosition;
    }

    private static void UpdateFloor()
    {
        floorPosition.X -= gameSpeed * delta;
        floorPosition2.X -= gameSpeed * delta;

        if (floorPosition.X + FLOOR_LENGTH <= 0)
        {
            floorPosition.X = SCREEN_WIDTH;
        }

        if (floorPosition2.X + FLOOR_LENGTH <= 0)
        {
            floorPosition2.X = floorPosition.X + FLOOR_LENGTH;
        }
    }

    private static void UpdateClouds()
    {
        cloudPosition.X -= (gameSpeed / 10) * delta;
        cloudPosition2.X -= (gameSpeed / 10) * delta;

        if (cloudPosition.X + CLOUD_LENGTH <= 0)
        {
            cloudPosition.X = SCREEN_WIDTH;
        }

        if (cloudPosition2.X + CLOUD_LENGTH <= 0)
        {
            cloudPosition2.X = SCREEN_WIDTH * 2;
        }
    }

    private static void CheckCollissions(Rectangle boundsPlayer)
    {
        for (int i = 0; i < boundArray.Length; i++)
        {
            if (Raylib.CheckCollisionRecs(boundsPlayer, boundArray[i]))
            {
                if (hp > 0)
                    hp--;
            }
        }
    }

    private static void ResetGame()
    {
        hp = 1;

        if (score > highScore)
            highScore = score;

        score = 0;
        scoreText = "000000";
        timerScore = 0;
        timerCactus = 0;

        playerPosition.Y = FLOOR_HEIGHT;
        isJumping = false;
        playerSpeed = 0;

        floorPosition.X = 0;
        floorPosition2.X = floorPosition.X + FLOOR_LENGTH;

        cloudPosition = new Vector2(SCREEN_WIDTH, SCREEN_HEIGTH / 2);
        cloudPosition2 = new Vector2(SCREEN_WIDTH * 2, cloudPosition.Y - 50);

        gameOverTextPositionY = 1000;

        indexArray = 0;

        for (int i = 0; i < isMovingArray.Length; i++)
        {
            cactusPositionArray[i] = new Vector2(0, 0);
            isMovingArray[i] = false;
            cactusActiveTextureArray[i] = cactusTextureList[0];
            boundArray[i] = new Rectangle(0, 0, 0, 0);
        }

        gameStart = true;
    }

    private static void UpdateCactus()
    {
        for (int i = 0; i < isMovingArray.Length; i++)
        {
            if (isMovingArray[i])
            {
                cactusPositionArray[i].X -= gameSpeed * delta;
                Raylib.DrawTexture(cactusActiveTextureArray[i], (int)cactusPositionArray[i].X - cactusActiveTextureArray[i].width / 2, (int)cactusPositionArray[i].Y - cactusActiveTextureArray[i].height / 2, Color.WHITE);
                boundArray[i] = new Rectangle(cactusPositionArray[i].X - cactusActiveTextureArray[i].width / 2, cactusPositionArray[i].Y - cactusActiveTextureArray[i].height / 2, cactusActiveTextureArray[i].width, cactusActiveTextureArray[i].height);
            }
            if (cactusPositionArray[i].X + cactusActiveTextureArray[i].width <= 0)
            {
                isMovingArray[i] = false;
                cactusPositionArray[i].X = SCREEN_WIDTH + cactusActiveTextureArray[i].width;
                Raylib.DrawTexture(cactusActiveTextureArray[i], (int)cactusPositionArray[i].X - cactusActiveTextureArray[i].width / 2, (int)cactusPositionArray[i].Y - cactusActiveTextureArray[i].height / 2, Color.WHITE);
                boundArray[i] = new Rectangle(cactusPositionArray[i].X - cactusActiveTextureArray[i].width / 2, cactusPositionArray[i].Y - cactusActiveTextureArray[i].height / 2, cactusActiveTextureArray[i].width, cactusActiveTextureArray[i].height);
            }
        }
    }

    private static void CactusRandomizer()
    {
        int i = random.Next(0, cactusTextureList.Count);
        Texture2D cactusActive = cactusTextureList[i];
        if (indexArray >= cactusPositionArray.Length)
        {
            indexArray = 0;
        }
        try
        {
            cactusActiveTextureArray[indexArray] = cactusActive;
            cactusPositionArray[indexArray].X = SCREEN_WIDTH;
            cactusPositionArray[indexArray].Y = FLOOR_HEIGHT - cactusActiveTextureArray[indexArray].height / 2;
            isMovingArray[indexArray] = true;
            boundArray[indexArray] = new Rectangle(cactusPositionArray[indexArray].X - cactusActiveTextureArray[indexArray].width / 2, cactusPositionArray[indexArray].Y - cactusActiveTextureArray[indexArray].height, cactusActiveTextureArray[indexArray].width, cactusActiveTextureArray[indexArray].height);
        }
        catch (Exception)
        {
            Console.WriteLine("Out of range");
            throw;
        }
    }

    private static void NewCactusLogic()
    {
        timerCactus += Raylib.GetFrameTime();
        if (timerCactus >= timeBetweenCactus)
        {
            timeBetweenCactus = (float)(random.NextDouble() * (MAX_TIME - MIN_TIME) + MIN_TIME);
            timerCactus = 0f;
            CactusRandomizer();
            indexArray++;
        }
    }

    private static void UpdateGameplay()
    {
        if (gameSpeed < MAX_SPEED && score != 0 && score % SCORE_SPEED_CHANGE == 0 && canChange == true)
        {
            canChange = false;
            gameSpeed += RISE_SPEED;
        }
    }
}