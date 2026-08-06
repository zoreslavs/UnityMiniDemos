# Unity Mini Demos

A small Unity project with three mini demos. Open them from the main menu.

Play it in your browser: [Unity Mini Demos on itch.io](https://zoreslav.itch.io/unity-mini-demos)

## Demos

### Ace Of Shadows

Moves 144 cards between two stacks. It has stack counters, a completion message plus a speed slider from `0.1x` to `10x`.

### Magic Words

Loads a dialogue plus avatars from an endpoint, then shows the messages one by one as chat bubbles. It supports inline emoji, an empty avatar fallback plus retrying when the request fails.

### Phoenix Flame

A layered fire particle effect: smoke, sparks, embers, glow. Press `Next Color` to cycle the fire smoothly through orange, green and blue using an Animator Controller.

## Project Structure

```
Assets/
├── Animations/PhoenixFlame/  Animator Controller plus clips driving the fire colour cycle
├── Prefabs/                  Card, Dialogue Item, Background, FPS Counter
├── Scenes/                   MainMenu plus one scene per demo
├── Scripts/
│   ├── Core/                 FPS counter plus scene navigation, shared by every scene
│   └── Features/             One folder per demo, with no references between them
│       ├── AceOfShadows/
│       ├── MagicWords/
│       └── PhoenixFlame/
├── Settings/                 URP pipeline assets, input actions
├── Sprites/                  Art per demo; the Twemoji sheet lives in MagicWords/Emoji
└── WebGLTemplates/           Custom WebGL template with a loading screen
```

Scene components focus on orchestration plus UI. Feature-specific logic is kept in focused classes: `CardDeck`, `EmojiTextProcessor`, `DialogueLoader`, `AvatarLoader`.

## Implementation Notes

- Ace Of Shadows shifts deck roots as cards move, avoiding a full layout update of both stacks after every transfer. A nested canvas limits UI rebuilds to the card hierarchy. Moving cards are re-parented to the top of that hierarchy so they draw above both stacks.
- Magic Words starts avatar requests in parallel. Each message only waits for its own avatar, so one slow request does not delay unrelated messages.
- Magic Words shows messages with randomized delays to imitate a live chat. Each speaker receives a consistent, deterministic bubble color from the blue-purple palette.
- Emoji are packed into a custom sprite atlas. A TMP sprite asset registered as the default asset renders them in text.
- Phoenix Flame combines separate flame, glow, smoke, spark, ember particle layers. The Animator cycles `Orange → Green → Blue → Orange` through the `NextColor` trigger. Its animated `Tint` value updates the particle color-over-lifetime gradient.

## Requirements

- Unity `6000.3.20f1`
- WebGL Build Support module for a WebGL build

## Run

1. Open the project in Unity Hub.
2. Open `Assets/Scenes/MainMenu.unity`.
3. Enter Play Mode.

Every scene is included in Build Settings, uses responsive UI scaling and displays an FPS counter in the top-left corner.

## WebGL Build

Create a WebGL build from Unity's Build Profiles window with the Web platform selected. The `LoadingScreen` Web Template is already selected in Player Settings and adds the branded loading screen.

`Decompression Fallback` is enabled in Player Settings. Keep it enabled when building with Brotli or Gzip for a host whose `Content-Encoding` headers you do not control.