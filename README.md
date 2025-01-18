# Project Setup Instructions - Plague Runner 🏃
Link to download .zip file: [Plague Runner](https://drive.google.com/file/d/1Fl7VifnTZY3pl28O1tYupwYg8J6-xPBT/view?usp=sharing)

![ScreenRecording2025-01-17at10 38 56PMonline-video-cutter com-ezgif com-video-to-gif-converter-2-2](https://github.com/user-attachments/assets/136d253b-cccc-4d71-8260-d12f96f50953)


The .zip file called PlagueRunner contains the following contents:

```plaintext
Plague Runner/
├── Assets/
├── Library/
├── Logs/
├── obj/
├── Packages/
├── Plague Runner.sln
├── PlagueRunnermacOS
├── PlagueRunnermacOS_B...Information_DoNotShip/
├── PlagueRunnerWindows/
│   ├── Plague Runner.exe
├── ProjectSettings/
├── Temp/
└── UserSettings/

```
## Instructions to run the build

- To run the game on macOS, go to **Plague Runner/** -> **PlagueRunnermacOS**. This would launch the game on your mac in full screen. To quit, press Cmd + Q.
- To run the game on Windows, go to **Plague Runner/** -> **PlagueRunnerWindows/** -> **Plague Runner.exe**. 

## Instructions to set up Plague Runner in Unity Editor

- Unzip the folder and place it in your desired directory.
- Open Unity Hub.
- Click Add or Open, navigate to the unzipped folder (Plague Runner), and select it.
- In the Unity Editor, go to Assets/Assets/Scenes and double-click the main scene file (SampleScene.unity).
- Press the Play button to test the game in the Editor.
- Go to File > Build Settings if you need to modify platform settings or rebuild the project.

## How To Play
- Use left/right arrow keys or A/D to move left/right.
- Use spacebar to jump, down arrow key to duck and shift key to dash.
- There is a cool-off period of 15 seconds after every dash. 
- The goal is to collect all 5 collectibles in the level within time and without losing all three lives. However, the more collectibles you have, the worse it gets!

### Technical Progress

Link to [Project Overview Document](https://docs.google.com/document/d/1H-TXiy2zg03kOYntf0Y8lzOBSOI3wmkT3q0xkXuAJsc/edit?usp=sharing)
- The scripts used in the project can be accessed by the path: Plague Runner/Assets/Assets/Scripts
- The singleton PlayerController class has a comprehensive implementation for various player mechanics, including movement, jumping, dashing, collectibles, and life handling.
- References to Rigidbody2D, BoxCollider2D, Animator, and CanvasGroup components are initialized in Awake and checked for null references.
- Movement is visually represented through animator parameters such as isRunning, isJumping, and isDucking
- Jump Mechanics
  - The player can perform up to maxJumps (including ground and air jumps).
  - Variable Jump Forces: Separate forces are applied for the first jump (jumpForce) and subsequent jumps (doubleJumpForce).
  - Fall Multiplier: Enhances fall speed for better responsiveness.
  - Low Jump Multiplier: Shortens the jump when the jump button is released early.
- Player has 3 lives at the beginning of the game. Colliding with lava reduces lifeCounter, and a method updates the UI accordingly.
- Collectibles
  - Collected items are counted (collectedCount), and the UI is updated.
  - Stat Reduction: Collecting items reduces movement speed and jump force, respecting minimum thresholds (minMoveSpeed and minJumpForce).
  - Fade Animation: Collected item notifications fade out gradually using a coroutine and CanvasGroup.alpha.
- Win Condition: If all collectibles are gathered, the win indication is shown, and the game timer stops 



 






