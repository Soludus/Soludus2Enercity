# Soludus 2 - Enercity

## Platform

A platform that can be used to simulate spatial information and energy use and production.  
Content inside `Assets/Soludus/Energy/`.

## Enercity

A VR game that utilizes the platform.  
Content inside `Assets/Soludus/VR Game/`.  
Many generic components are included that can be reused in different games.  
Most game assets are included in this repository.  
Audio assets are found in [FMOD Banks]. The `FMOD Banks` folder must be placed at the root of the Unity project.

### Asset project sources

Distributed separately under the CC0 license (see License below). [Link to Asset sources].

### 3rd party libraries/assets required by the game

1. [VRTK] (v3.2.1) - VR toolkit
2. VR SDK - see [SDKs supported by VRTK]. During development, [SteamVR Plugin] (v1.2.3) was used.
3. [FMOD] - Audio
4. [Unity Standard Assets] (v2017.1) - Effects & Environment/Water
5. [Water Effect Fits For Lowpoly Style] (v0.1) - Water shaders
6. [Post Processing Stack] (v1.0.4) - Post Effects

### Notes
- **Important!** Some modifications are needed for some 3rd party code. See [3RD-PARTY-FIXES].
- If you can't use FMOD, you can add a "DISABLE_FMOD" define symbol in the Player Settings.

## Other content

`Assets/Soludus/Common/` contains some common components used to implement the Unity integration and are required by the Platform and Game.  
`Assets/Soludus/Legacy/` contains obsolete content created during development.

## Documentation

The documentation for the platform can be found in [DOCUMENTATION.md].

License
-------

The MIT License applies to all files and their contents in this repository (see [LICENSE-MIT]).  
In addition, the CC0 1.0 Universal licence (CC0 1.0) applies to all files and their contents in this repository (see [LICENSE-CC0], also available at https://creativecommons.org/publicdomain/zero/1.0/).  
Content that belongs to the listed 3rd parties have their own respective licenses.


[FMOD Banks]: https://drive.google.com/drive/folders/1sYHXvAd9n3JxAzQCSNVjanpYg5E8a7m4?usp=sharing
[Link to Asset sources]: https://drive.google.com/drive/folders/1sYHXvAd9n3JxAzQCSNVjanpYg5E8a7m4?usp=sharing
[VRTK]: https://vrtoolkit.readme.io/
[SDKs supported by VRTK]: https://github.com/thestonefox/VRTK
[SteamVR Plugin]: https://assetstore.unity.com/packages/templates/systems/steamvr-plugin-32647
[FMOD]: https://www.fmod.com
[Unity Standard Assets]: https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-32351
[Water Effect Fits For Lowpoly Style]: https://assetstore.unity.com/packages/vfx/shaders/water-effect-fits-for-lowpoly-style-87810
[Post Processing Stack]: https://assetstore.unity.com/packages/essentials/post-processing-stack-83912
[3RD-PARTY-FIXES]: 3RD-PARTY-FIXES.txt
[DOCUMENTATION.md]: DOCUMENTATION.md
[LICENSE-MIT]: LICENSE-MIT.txt
[LICENSE-CC0]: LICENSE-CC0.txt
