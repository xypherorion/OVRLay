# FeVRDeck (0.0.1)

An Overlay Deck Engine designed to be used with https://Streamer.bot/user/decks built on OVRLay

---
FeVRDeck is a companion to Nate1280's Streamer.Bot and streamer.bot decks website
Its like a StreamDeck or LoupeDeck in VR, connected with the most powerful stream bot on the planet
=
FeVRDeck is based on OVRLay, a small toolkit for creating Overlays for Open/Steam VR which
provides the neccassary OpenVR API's, Plugins, and Unity Prefabs to enable a sort of drag-and-drop ease of use.

## OVRLay Features
- Implements and Supports most OpenVR Overlay Features / Settings / Flags!
- Create and Manipulate Both In-Game, and Dashboard Menu Overlays!
- Load in OBJ (WaveFront) files as RenderModels, and attach them to overlays!
- Support for the lastest Stable Release of Unity (2017.1)!
- Does not use Unity's Built-In OpenVR support, but has its own OpenVR Handler that deals with getting a connection to SteamVR, getting the HMD and Right/Left controller positions, and updating / handling Overlay Relevant OpenVR events!
- Has drop-in support for interaction with unmodified UnityUI, by simulating mouse screen cords via a in-scene camera! (Just make sure to position camera to look at WorldSpace UI!)


## FeVRDeck Features
- Relies on https://streamer.bot/user/decks
- 5 Individual Decks visible at once
- Currently Interactable ONLY in the Dashboard

## Planned Features
- Unlimited Decks
- Deck Transform Configuration
- Space Positioned Decks
- Deck Visibility Triggers
- Pads, Knobs, Sliders, and Points
- "touch" interaction when not in Dashboard
- Streamer.Bot OAuth for Private Decks and network Interop

https://fevr.gg/discord

Download:
===
https://github.com/xypherorion/OVRLay/tree/FeVRDeck/releases


Gigantic thank you to Ben Otter (user:benotter) for the original OVRLay code!

Credits
Xypher Orion
Ben Otter (Original OVRLay Source)