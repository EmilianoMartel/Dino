using Raylib_cs;
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
    const float ORIGINAL_SPEED = 500f;
    const float JUMP_IMPULSE = 350f;
    const float GRAVITY = 335f;

    //Images
    static Image imagePlayer = Raylib.LoadImage("assets/dinosaur.png");
    static Image imageCactusLarge = Raylib.LoadImage("assets/cacti1.png");
    static Image imageCactus2 = Raylib.LoadImage("assets/cacti2.png");
    static Image imageCactus3 = Raylib.LoadImage("assets/cacti3.png");
    static Image imageFloor = Raylib.LoadImage("assets/floor.png");
    static Image imageFloor2 = Raylib.LoadImage("assets/floor.png");
    static Image imageCloud1 = Raylib.LoadImage("assets/cloud.png");
    static Image imageCloud2 = Raylib.LoadImage("assets/cloud.png");

    //Player view position
    static Vector2 playerPosition = new Vector2(150, FLOOR_HEIGHT);

    //Gameplay
    static float gameSpeed = ORIGINAL_SPEED;

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
    const float MIN_TIME = 0.7f;
    const float MAX_TIME = 2.5f;
    static float timerCactus;
    static bool newCactus = true;

    static bool startMovingCactus1 = false;
    static bool startMovingCactus2 = false;
    static bool startMovingCactus3 = false;

    //Cactus Position
    static Vector2 cactusPosition = new Vector2(SCREEN_WIDTH, FLOOR_HEIGHT);
    static Vector2 cactusPosition2 = new Vector2(SCREEN_WIDTH, FLOOR_HEIGHT);
    static Vector2 cactusPosition3 = new Vector2(SCREEN_WIDTH, FLOOR_HEIGHT);

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

        Texture2D textureCactus = Raylib.LoadTextureFromImage(imageCactusLarge);
        Raylib.UnloadImage(imageCactusLarge);

        Texture2D textureCactus2 = Raylib.LoadTextureFromImage(imageCactus2);
        Raylib.UnloadImage(imageCactus2);

        Texture2D textureCactus3 = Raylib.LoadTextureFromImage(imageCactus3);
        Raylib.UnloadImage(imageCactus3);

        Texture2D textureFloor = Raylib.LoadTextureFromImage(imageFloor);
        Raylib.UnloadImage(imageFloor);

        Texture2D textureFloor2 = Raylib.LoadTextureFromImage(imageFloor2);
        Raylib.UnloadImage(imageFloor2);

        Texture2D textureCloud1 = Raylib.LoadTextureFromImage(imageCloud1);
        Raylib.UnloadImage(imageCloud1);

        Texture2D textureCloud2 = Raylib.LoadTextureFromImage(imageCloud2);
        Raylib.UnloadImage(imageCloud2);

        cactusPosition.X = SCREEN_WIDTH + textureCactus.width;
        cactusPosition2.X = SCREEN_WIDTH + textureCactus2.width;
        cactusPosition3.X = SCREEN_WIDTH + textureCactus3.width;
        floorPosition2.X = floorPosition.X + FLOOR_LENGTH;

        while (!Raylib.WindowShouldClose())
        {
            delta = Raylib.GetFrameTime();

            //Gameplay
            if (gameStart == true)
            {
                UpdatePlayer();
                UpdateFloor();

                UpdateCactus(textureCactus, ref cactusPosition, ref startMovingCactus1);
                UpdateCactus(textureCactus2, ref cactusPosition2, ref startMovingCactus2);
                UpdateCactus(textureCactus3, ref cactusPosition3, ref startMovingCactus3);

                CactusRandomizer(textureCactus, textureCactus2, textureCactus3);

                timerScore += Raylib.GetFrameTime();

                if (timerScore >= 1f)
                {
                    score += 1;
                    timerScore = 0f;
                }

                if (!newCactus)
                {
                    timerCactus += Raylib.GetFrameTime();
                    if (timerCactus >= timeBetweenCactus)
                    {
                        timeBetweenCactus = (float)(random.NextDouble() * (MAX_TIME - MIN_TIME) + MIN_TIME);
                        newCactus = true;
                        timerCactus = 0f;
                    }
                }
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
                cactusPosition.X = SCREEN_WIDTH + textureCactus.width;
                cactusPosition2.X = SCREEN_WIDTH + textureCactus2.width;
                cactusPosition3.X = SCREEN_WIDTH + textureCactus3.width;

                ResetGame();
            }

            UpdateClouds();

            

            //COLLISSION
            Rectangle boundsPlayer = new Rectangle((int)playerPosition.X - texturePlayer.width / 2, (int)playerPosition.Y - texturePlayer.height, texturePlayer.width, texturePlayer.height);

            Rectangle boundsCactus = new Rectangle((int)cactusPosition.X - textureCactus.width / 2, (int)cactusPosition.Y - textureCactus.height, textureCactus.width, textureCactus.height);
            Rectangle boundsCactus2 = new Rectangle((int)cactusPosition2.X - textureCactus2.width / 2, (int)cactusPosition2.Y - textureCactus2.height, textureCactus2.width, textureCactus2.height);
            Rectangle boundsCactus3 = new Rectangle((int)cactusPosition3.X - textureCactus3.width / 2, (int)cactusPosition3.Y - textureCactus3.height, textureCactus3.width, textureCactus3.height);

            CheckCollissions(boundsPlayer, boundsCactus);
            CheckCollissions(boundsPlayer, boundsCactus2);
            CheckCollissions(boundsPlayer, boundsCactus3);

            Raylib.BeginDrawing();

            //DRAWING GAMEPLAY
            Raylib.ClearBackground(currentColor);

            //CLOUDS
            Raylib.DrawTexture(textureCloud1, (int)cloudPosition.X - textureCloud1.width / 2, (int)cloudPosition.Y - textureCloud1.height, Color.WHITE);
            Raylib.DrawTexture(textureCloud2, (int)cloudPosition2.X - textureCloud1.width / 2, (int)cloudPosition2.Y - textureCloud2.height, Color.WHITE);

            //CACTI
            Raylib.DrawTexture(textureCactus, (int)cactusPosition.X - textureCactus.width / 2, (int)cactusPosition.Y - textureCactus.height, Color.WHITE);
            Raylib.DrawTexture(textureCactus2, (int)cactusPosition2.X - textureCactus2.width / 2, (int)cactusPosition2.Y - textureCactus2.height, Color.WHITE);
            Raylib.DrawTexture(textureCactus3, (int)cactusPosition3.X - textureCactus3.width / 2, (int)cactusPosition3.Y - textureCactus3.height, Color.WHITE);

            //PLAYER
            Raylib.DrawTexture(texturePlayer, (int)playerPosition.X - texturePlayer.width / 2, (int)playerPosition.Y - texturePlayer.height, Color.WHITE);

            //FLOOR
            Raylib.DrawTexture(textureFloor, (int)floorPosition.X, FLOOR_HEIGHT, Color.WHITE);
            Raylib.DrawTexture(textureFloor2, (int)floorPosition2.X, FLOOR_HEIGHT, Color.WHITE);

            //DRAWING INTERFACE 
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

    private static void UpdateCactus(Texture2D textureCactus, ref Vector2 positionCactus, ref bool isMoving)
    {
        if (isMoving)
        {
            positionCactus.X -= gameSpeed * delta;
        }

        if (positionCactus.X + textureCactus.width <= 0)
        {
            isMoving = false;
            positionCactus.X = SCREEN_WIDTH + textureCactus.width;
        }
    }

    private static void CactusRandomizer(Texture2D textureCactus, Texture2D textureCactus2, Texture2D textureCactus3)
    {
        if (!newCactus) return;

        Random rnd = new Random();
        int random = rnd.Next(1, 4);

        switch (random)
        {
            case 1:
                startMovingCactus1 = true;
                break;
            case 2:
                startMovingCactus2 = true;
                break;
            case 3:
                startMovingCactus3 = true;
                break;
        }

        newCactus = false;

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

    private static void CheckCollissions(Rectangle boundsPlayer, Rectangle boundsCactus)
    {
        if (Raylib.CheckCollisionRecs(boundsPlayer, boundsCactus))
        {
            if (hp > 0)
                hp--;
        }
    }

    private static void ResetGame()
    {
        hp = 1;

        if (score > highScore)
            highScore = score;

        score = 0;
        timerScore = 0;
        timerCactus = 0;

        playerPosition.Y = FLOOR_HEIGHT;
        isJumping = false;
        playerSpeed = 0;

        floorPosition.X = 0;
        floorPosition2.X = floorPosition.X + FLOOR_LENGTH;

        cloudPosition = new Vector2(SCREEN_WIDTH, SCREEN_HEIGTH / 2);
        cloudPosition2 = new Vector2(SCREEN_WIDTH * 2, cloudPosition.Y - 50);

        startMovingCactus1 = false;
        startMovingCactus2 = false;
        startMovingCactus3 = false;

        gameOverTextPositionY = 1000;

        gameStart = true;
    }
}