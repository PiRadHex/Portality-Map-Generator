# Portality Map Generator
Welcome to Portality Map Generator, a Unity toolkit that harnesses the power of procedural algorithms to create random levels. Inspired by the concept of portals, this toolkit allows you to generate intricate and physically unfeasible structures that challenge traditional level design.

## Table of Contents
- [Concept](#concept)
- [How to Experience](#how-to-experience)
- [Features](#features)

## Concept
Imagine being in a house with just one door. You can walk through this door to step outside, and then seamlessly return inside. In reality, both the outside and inside are in the same physical space, with just one door facilitating movement between them. Now, imagine if these spaces were separated, each with its own door that links to another. When you walk through the first door, you're instantly transported to the second door, and vice versa. From your perspective, nothing changes - you experience the same environment. However, from a higher viewpoint, these places can be vastly different, perhaps far apart, stacked on top of each other, adjacent, or in any configuration imaginable. The possibilities are endless.

https://github.com/PiRadHex/Portality-Map-Generator/assets/124064917/c79fc073-8573-4fd0-96e4-43282a473461

Now, picture numerous random rooms arranged in a straight line, with no connections between them. Each room contains potential positions for placing portals. By randomly assigning linked portals to these positions, we create a procedural and unpredictable structure. Rooms connect to each other via portals, resulting in a physically unfeasible yet fascinating and dynamic environment. This concept allows for the creation of unique and engaging levels that challenge traditional notions of spatial design and navigation.

![SceneView](https://github.com/PiRadHex/Portality-Map-Generator/assets/124064917/07c6faae-7d9b-4a92-b67e-710342ffb20a)

https://github.com/PiRadHex/Portality-Map-Generator/assets/124064917/151d3259-708c-4c06-96ba-56d9171a19e1

https://github.com/PiRadHex/Portality-Map-Generator/assets/124064917/d5680323-8182-4e87-94a4-66c91da0c901

## How to Experience
- **Demo**: Download the demo for [PC]() and [Android]() or test it on [WebGL]().
- **Unity**: Clone the repository and open it in Unity.
- **Unity Package**: Download the [package]() and import it into Unity.

### Demo Scene
1. Enter/Exit edit mode by pressing "E" (or tap on the setting icon for touch screen devices).
2. Set desired values in the UI (seed, number of rooms, etc.).
3. Click the "Generate" button.
4. View the generated paths and connections by pressing the arrow icon in the UI.

https://github.com/PiRadHex/Portality-Map-Generator/assets/124064917/688da65a-80b4-484a-95ed-aee6478ff9f9

## Features
- Ready-to-use first-person controller.
- Touch screen support.
- 20+ pre-built room prefabs.
- User-friendly and animated UI/UX.

### Advanced Features
- Smart disable/enable animator for optimization.
- Use of gizmos to display portal candidate positions in the Unity editor.

### Challenges
- **Portal**: Implementing a new portal shader was time-consuming, leading us to use SebLague's Portals repository.
- **Doors in Front of Portals**: To address the issue of viewing through multiple portals, we implemented doors that open when entering a room and close when exiting.
- **Eternal Portals**: Due to shader-related runtime issues, we pre-create a set of linked portals in the hierarchy to ensure seamless functionality.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.

Explore the limitless possibilities of Portality Map Generator and create unique worlds. Feel free to contribute and enhance the toolkit for even more exciting experiences!

SOON!

https://github.com/PiRadHex/Portality-Map-Generator/assets/124064917/c237303c-08a5-4c2d-979a-275fef8a5536
