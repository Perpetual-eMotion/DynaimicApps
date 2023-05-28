# Dynaimic Apps
This repository is meant to hold the code of the Dynaimic Apps Engine, a system that allows Unity applications to have their own logic modified at runtime by generative AI, and a sample of its use, called CubicMusic.

The purpose is to have artificial intelligence write scripts based on certain (direct or indirect) user inputs, and have those scripts loaded at runtime into the application. It is like the application mods itself at runtime using the power of artificial intelligence.
The long term vision is having applications that are completely managed by artificial intelligence, that code their behaviour at runtime on the fly, so that to be able to adapt to the user that is experiencing them (e.g. a horror game may create a new type of enemy that scares in particular the user that is playing the game)

You can watch a video that explains the high-level vision of the project, together with some in-game footage, here:  
[![DynaimicApps Project](https://img.youtube.com/vi/yEaqyvl8A1U/0.jpg)](https://www.youtube.com/watch?v=yEaqyvl8A1U)

# Project Status
The provided project is a prototype meant to verify that the long term vision of applications that generate their logic on the fly with the power of artificial intelligence is possible. Arriving to a full implementation would require much more work. But I guess that in 5 to 10 years we can get there. ([Here a link to get the joke about the 5 to 10 years](https://skarredghost.com/2022/11/11/the-7-laws-present-metaverse/))

# Project Structure
The project is structured in a modular way and includes both the Dynaimic Apps Engine and a sample application that uses it, called CubicMusic. Currently the repository contains both the engine and the demo in a single Unity project, for simplicity. But the project is structed in such a way that you can easily detach the engine from the rest of the experience.
In the future, if the project grows, I will probably split the two things in two different repositories (with the sample CubicMusic adding the Dynaimic Engine as a Git submodule), or at least two different projects in the same repository.

All the most relevant files are in folders that begin with underscore "_", while the other folders are mostly dependencies and settings. The wo most important folders are:
- _DynaimicEngine: contains the Dynaimic Apps Engine, which is the core of the project. It contains the scripts that make you create runtime logic powered by AI, that let an application mod itself on the fly. You can export this folder as a Unity package and use it in other projects.
- _CubicMusic: contains the sources of the CubicMusic application, which is a sample application that uses the Dynaimic Apps Engine to let you spawn cubes and assign them a custom logic. Inside this folder there are also the scenes that you can build to test the application.

Both folders correspond to two Assembly Definitions, so that you can easily detach them from the rest of the project, and better dependency management is guaranteed.

The scenes that you can use to test the application are all in the folder _CubicMusic/Scenes.

## Architecture and deep dive on the project technicalities
I have made a draw.io architecture of the project, that you can find at this link: [Architecture](https://vrroomproject-my.sharepoint.com/:u:/g/personal/tony_vrroom_world/Ea-55DTSc3tPrjwlDUvtEi8BUPLLWoPlp3d83EokGh2kGA?e=f2W5he) (You can use app.diagrams.net to open it)

If you want a detailed explanation of how the project is designed, you can watch a very long technical deep dive here:
[![DynaimicApps Configuration](https://img.youtube.com/vi/fN6QuwTwoOM/0.jpg)](https://www.youtube.com/watch?v=fN6QuwTwoOM)

I decided to make a video because writing everything that I explained there would require a huge wall of text, that no one would read anyway :P.

# Known Configuration (and dependencies)
The project has been tested on the following configuration:
- Unity 2021.3.25f1
- Roslyn C# Runtime Compiler 1.7.4
- Dotnow Interpreter 0.1.3-beta
- Com.openai.unity 4.8.7
- Oculus Integration 53.1

It should work also with different versions of the same packages, but I cannot guarantee it. If you have problems, try to use the same versions of the packages that I used.

## Roslyn C# Runtime Compiler
Notwithstanding this project being opensource, it uses a paid asset that is called Roslyn C# Runtime Compiler, by Trivial Interactive. This tool is very well made, and lets you compile at runtime some Unity scripts and then load them into memory. It works well both on Windows an Android. I think they are €20 very well spent (also to support the devs).

Considering that Roslyn (the .NET compiler) is opensource, it is possible to create a free version of it, and actually I've found some people already using Roslyn into Unity, as long as only PC-compatibility was required (e.g. this video https://www.youtube.com/watch?v=FuZHnpgntEI). Android compatibility, though, is more complex, and I needed it to add support for VR headsets. I've done this project in my free time and I wanted to develop it in a short time, so for the prototype I preferred using an off-the-shelf solution instead of spending months to code my custom version of the C# compiler. 

Anyway, if you have the time, you are free to try to create a custom version of the C# compiler for Unity, and then replace the Roslyn C# Runtime Compiler with your custom version. Since the application is quite modular, it shouldn't be that difficult to replace one Compiler library with another one.

# Installation Instructions
The project has been stripped away from all the dependencies so that it can be uploaded on GitHub. This means that you have to install them manually. Here are the required steps:

(I have already added some of the depencies via the package manager. For instance, you don't need to import Com.openai.unity, which is a tool that you can find at this repository: https://github.com/RageAgainstThePixel/com.openai.unity)
1. Open the project in Unity. Unity will tell you there are compilation errors. This is because the project is missing a lot of needed dependencies. Just Ignore the warning and go on. We are brave, after all :)
2. The project opens, and a popup should ask you to import the TextMeshPro package. Click on "Import TMP Essentials". If you don't see this popup, open a scene of the project to make it appear. For instance, you can open the scene Assets/_CubicMusic/Scenes/CubicMusicQuestMR to make it appear
3. Import the Roslyn C# Compiler. This is the only paid asset that you need. You can buy it here https://assetstore.unity.com/packages/tools/integration/roslyn-c-runtime-compiler-142753 for around €20. Then import it inside your project. If you want you can skip importing the Examples folder, we don't need it.
4. Import dotnow. This is an opensource asset that you can find here https://github.com/scottyboy805/dotnow-interpreter/releases. Import it inside the project.
5. Import the Oculus Integration plugin. Even if you don't use VR, it is needed to make the project to compile (in future versions, I can make things more modular, but for now bear with my laziness). You can find it here: https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022 . When importing it, import only the VR directory of the package, plus the OculusProjectConfig.asset file, to avoid cluttering your project.
6. If Oculus says there is something to Update in a dialog box, say Yes to update it. If it asks to reboot the editor, reboot it
7. If everything went correctly, now your project should take a while to load and then compile. If it doesn't, try to reboot Unity. If it still doesn't... there is a problem :(
8. If a new popup asks you to enable OpenXR Feature Groups say Enable
9. Set up your OpenAI credentials in the file Assets/Configurations/Resources/OpenAIConfigurationSettings.asset . You must have an OpenAI account and then you can get an API key for yourself or your organization at this page: https://platform.openai.com/account/api-keys
10. Remove all the DLL references from Roslyn C# Compiler. Open the scriptable asset at Assets/RoslynCSharp/Resources/RoslynCSharpSettings.asset and remove all the DLL references by selecting them one by one and using the "-" button
11. On the same file, disable the checkmarks for Allow Concurrent Compile and Generate Symbols. Activate the one about Generate In Memory, instead

If you plan to use Mixed Reality on Quest, there are additional steps to perform:

12. Open the scene CubicMusicQuestMR and select the OVRCameraRig gameobject. In the inspector, look for Quest Features/General, check the entry "Requires System Keyboard", and then select "Supported" for the entry "Passthrough Support".
13. On the same gameobject, look for Insight Passthrough (it's down the options of the OVRCameraRig) and check Enable Passthrough
14. Switch the build target to Android, which is the platform used by the Quest

At this point, the project should be ready to be built. Congratulations, you survived the long installation process!

If something is not clear in this instructions or you prefer watching this procedure made live for you, you can watch this installation video here:

[![DynaimicApps Configuration](https://img.youtube.com/vi/Kzt_uL7-8_U/0.jpg)](https://www.youtube.com/watch?v=Kzt_uL7-8_U)

## Exporting the Dynaimics Engine
If you want to export the dynamics engine to use it in your own project, you can do it by following these steps:
- Create your own Unity project
- Export the folder _DynaimicEngine from this project to your project
- Add the OpenAI bridge to your project (you can find the installation instructions here: https://github.com/RageAgainstThePixel/com.openai.unity)
- Add to your project the same dependencies as specified above, with the exact same procedure, with the exception of TextMeshPro and Oculus Integration (you don't need them)
- Use the GenerativeLogicManager class to query the AI and have a script generated at runtime depending on your prompt
- If you need them, take with you also some scripts from the _CubicMusic folder that you may use in your project (e.g. to add sound-reactiveness). My suggestion is to take the CubicMusicManager and modify it according to your needs. Don't forget in your scene to add to the logic generation class (the CubicMusicManager in the sample) the Assembly references that are needed by the generated scripts, otherwise the generated scripts won't have the dependencies to run. See the documentation of the Roslyn C# Compiler for more info, or take inspiration from the code of the sample scenes for that
- For now, because of a bug in Unity 2021, you may need to set your IL2CPP runtime to Debug in the Project Settings, or the build process of the project may take all your RAM and then fail

In general, if something is not working in your project, check how things have been done in the CubicMusic one (either in the Project Settings or in the sample scenes).

It may be also interesting for you to read the Roslyn C# package documentation (which is inside the package itself) and the integration guide between dotnow and Roslyn C# Compiler, which is available at this [link](https://github.com/scottyboy805/dotnow-interpreter/wiki/Roslyn-C%23-Integration).

# Build platforms & settings
The project can be made to work both on flatscreen PC and on VR/MR headsets. Let me tell you how to choose the settings that you prefer. Be careful that the default settings are meant to build for the Mixed Reality mode on Meta Quest Pro, which may not be what you want (but it's the coolest of the options!).

## Flatscreen VS VR/MR
The project can be built for both flatscreen devices and VR/MR headsets. To choose between the two, go to the Project Settings (Edit/Project Settings) and look for the XR Plug-in Management section. There, select the tab related to your build platform and then choose between the following options:
- PC platform: select OpenXR to build for PC VR, do not select anything to build for flatscreen PC
- Android platform: select Oculus to build for Quest, do not select anything to build for flatscreen Android

Scenes to build:
- On flatscreen devices (e.g. PC, smartphone), the scenes you can build are CubicMusicFlat and CubicMusicEmotionalFlat, because they are the one that have been designed to work on a flatscreen device. 
- For Virtual Reality devices (e.g. Oculus Quest 2, Valve Index), the scenes are CubicMusicVR and CubicMusicEmotionalVR. They have been made with Unity.XR so should be compatible with every virtual reality device that is compatible with this middleware.
- If you want the special mixed reality experience, you can build the CubicMusicQuestMR scene. But be careful that this is made specifically for Quest devices (Quest 2 and Quest Pro), especially for the part related to passthrough AR. It should be rather trivial to convert it to work to another headset, though

Notice that you can build only one scene at a time, because I've not implemented a scene selection menu yet. I told you I'm lazy.

## PC vs Android 
- If you plan to run the application on PC, of course, you have to select "Windows, Mac, Linux" as the target platform, while for Android you should select "Android". Yeah, I'm a master scientist.
- Notice that the Android mode is mainly implemented for XR devices, because I have not made the interface for spawning cubes compatible for touchscreens. It shouldn't be difficult to implement it, it's just that I didn't have the time.

## Mono vs IL2CPP
This is probably the most important choice to take, because it has deep consequences not only on the final CubicMusic application, but also in all the workings of the Dynaimic Apps Engine. I will explain that in a dedicated chapter below. For now let me just tell you how to build for the two backends:

- You can build for Mono by selecting the "Mono" option in the "Scripting Backend" dropdown menu in the Project Settings window (Player tab). Also be sure to remove "ROSLYNCSHARP" in the Scripting Define Symbols of your project.
- You can bulid for IL2CPP by selecting the "IL2CPP" option in the "Scripting Backend" dropdown menu in the Project Settings window (Player tab). Also be sure to add the "ROSLYNCSHARP" in the Scripting Define Symbols of your project. This is the default option when you dowload the project from GitHub.

If you want to build for the Quest, you can use both modes, but for the special passthrough mode, you must use IL2CPP. This is because the passthrough mode uses the OpenXR plugin, which is not compatible with Mono.

### In-depth explanation of the Mono vs IL2CPP choice
Mono is the preferred runtime for this system, because it exploits completely the "runtime generation" capabilities of the engine. Mono is a managed-runtime, it is pure .NET with Just-In-Time compilation, so it's easy for the engine to create a new assembly on the fly including the script generated by the AI and load it in the application memory.

IL2CPP is a new way of building your application, that basically compiles it into native code. It goes from your source codes to managed artifacts that are then built into native code. It is as the application was written in C++, so the code is harder to decompile, it is faster, it is safer. IL2CPP has become the preferred way for the stores (Android Play Store, Meta Quest Store) to have applications built for mobile devices. In fact now the Meta Quest SDK lets you still build for Mono, but some advanced features (like the passthrough MR which is used in one of the sample scenes) are only available with IL2CPP backend.

The problem of IL2CPP is that it is an AOT (Ahead-Of-Time) compilation, so all the sources of the application should be available when it is built. It is not made for adding code at runtime, and for sure it is not made to add managed .NET assemblies like the ones generated by the Roslyn compiler. The solution that the guys at Trivial Interactive suggested me is to use the dotnow runtime, which is an opensource project to let you run managed scripts in applications built with IL2CPP. dotnow basically creates an emulation of the .NET runtime that runs on top of the native code of the application, so managed assemblies can still be loaded.

While dotnow works, it is still in alpha, and some things are not working for me with it (e.g. I can't receive the OnTriggerEnter callbacks on my MonoBehaviours). Furthermore an emulation layer is not optimized at all. Probably it would be possible to create a system to build native DLLs to be loaded at runtime, but my limited time available to spend on this project prevented me from even trying to do that.

There is a final problem with IL2CPP: the compilation always strips away unused classes and functions. Since this application is all about generating logic at runtime, there is no logic present at compilation time, and the linker may strip away almost everything. So for instance, it may strip away the RigidBody class, but then if in your prompt, you ask to add some physics to the cube, the Rigidbody class won't be found and the scripts will fail, potentially crashing the application. The solution for this problem is to edit the link.xml file in the project and add to it the assemblies you plan to use. I've already added a few of the most important ones.

My final suggestion is to use Mono for your tests, unless you don't need to use IL2CPP for specific reasons (e.g. for passthrough MR on Quest). Since the main scene of the application is the mixed reality one for Quest (it is where the experience is the coolest), I've pre-selected IL2CPP. I know, I don't sound very coherent.

# How to use the application
The application offers two different experiences, that can be enjoyed both on flatscreen PC or on MR/VR devices.

## CubicMusic
The scenes CubicMusicFlat, CubicMusicVR, and CubicMusicQuestMR all revolve around the original Cubic Music scene. The idea is to have a sandbox where you can generate cubes and give them whatever logic you want. The application doesn't have any scripts to assign to the cube. It's the user that says a sentence (or writes it) about what he/she wants the cube to do, and then the system will generate automatically the logic to make it happen through generative Artificial Intelligence (OpenAI to be exact) and load it in the application runtime. This means that the application modifies itself to add the logic that the user wants.

There is a preview cube that shows where to spawn the next cube and you can move it around using the Input described below. When you are satisfied with that location, you can press the confirmation button and spawn a cube there. You can then move the preview cursor and spawn as many cubes as you want whenever you want. When you're done spawning this first group of cubes, you can input a query to send to the the AI engine, to tell what is the logic that you want to apply to these cubes.

After the AI has elaborated your request, it will generate a script that will be loaded in the application runtime and will be assigned to each one of the cubes you created. If the AI did a good job, you should see the cubes doing what you requested. At this point, you can generate new cubes, that will be part of the next group. They will get the logic you will ask at the next prompt. And so on.

### Accepted AI commands
The system expects written or vocal command about what the cubes should do. These are the rules:
- Do not say to generate a Unity script, just say what you want to obtain. For instance, you can say "make it blue"
- You can mention commands related to "volume of the music" and "volume of the microphone" to introduce sound-reactiveness in the cubes behaviour. For instance, you can say "make the cube go from red to green smoothly following the volume of the music, where red is for low volume"
- You can mention commands about "touching the hand", but only in the CubicMusicVR scene. For instance, you can say "make the cube go from red to green when the hand touches it"
- In general, try to give the instructions in a clear and precise way, because you are still talking to a machine. Do not try to make queries that are too long, because the longer the requests, the higher the possibility that there is an error in the generated script, and so it doesn't compile

#### Example prompts
If in doubt, you can try with these prompts:
- "Make it blue" 
- "Give it a random color"
- "Make it go from blue to yellow smoothly every 2 seconds"
- "Make it go from red to green smoothly following the volume of the music, where red is for low volume"
- "Make it scale from 0.2 to 0.5 following the volume of the microphone, where 0.2 is for low volume"
- "Give it gravity and make it fall. Add a rigidbody if needed"

### Controls
- On flatscreen PC, you move the preview cube with the WASDQE keys, and you spawn a cube with the Spacebar or mouse click. You can then do a camera view by pressing the right mouse button and use again the WASDQE keys to move the camera around
- On VR/MR devices, you have the preview cube attached to your right hand, and you spawn a cube by pressing the trigger of the right controller.

### UI
On the left you have the panel showing the status of the AI queries, while on the right you can input your prompts.

The left panel contains:
- The "AI Status" label, that tells you if the AI is busy or not
- The "AI Transcription" label, that tells you the latest transcription of your voice command
- The "AI Response" label, that tells you the latest response of the AI to your command, that is the latest script generated

The right panel contains:
- A microphone icon, that you can press to start recording your voice prompt and press again to stop recording. Voice prompts are automatically sent and do not require you to press the "Send" button
- The "Input" field, where you can write your prompt
- The "Send" button, that sends the prompt to the AI engine

## CubicMusicEmotional
The scenes CubicMusicEmotionalFlat and CubicMusicEmotionalVR revolve around a more high-level concept: the possibility of letting the AI generate both the cubes and their logic depeding on your mood. This is a step forward towards the vision of having a fully reactive application that auto-codes itself depending on the status of the user.

It works as follow:
- You, the user, tell the system your current mood
- The AI engine interprets the mood and translates it in what types of cubes should be generated (e.g. happyness: more colourful and fast cubes)
- Knowing that it has to spawn 7 cubes (the number is hardcoded), the system generates for every cube a position and an AI query to implement a behaviour that should be implemented. Notice that in this first step the AI doesn't generate directly the cube logic, but an AI query for it. This is to simulate a hierarchical AI work (a bit like in AgentGPT), where different layers of AI do different things. The top-level AI cares about mood interpretation, and spawns various AI agents to care about the actual cube logic generation
- A loop goes for the 7 cubes, and for each one of them instantiates it, then performs a query to OpenAI to obtain the actual script to apply to it. The generation is pretty slow, so let it take its time

In the end, in the scene, there should be 7 cubes representing the mood of the user. They feel a bit like an abstract artwork, and for sure most of the time they are a bit weird. But be kind and compliment the AI for its work, because when the Terminators will conquer the world, they will remember when you appreciated their art.

the original Cubic Music scene. The idea is to have a sandbox where you can generate cubes and give them whatever logic you want. The application doesn't have any scripts to assign to the cube. It's the user that says a sentence (or writes it) about what he/she wants the cube to do, and then the system will generate automatically the logic to make it happen through generative Artificial Intelligence (OpenAI to be exact) and load it in the application runtime. This means that the application modifies itself to add the logic that the user wants.

### Accepted AI commands
The system expects written or vocal command about mood. Be concise and just say your mood, like "I'm happy" or "I'm sad". The system will then generate the cubes and their logic depending on your mood.

#### Example prompts
If in doubt, you can try with these prompts:
- "I'm very very happy" 
- "I'm sad"

### Controls
- On flatscreen PC, you can then do a camera view by pressing the right mouse button and use  the WASDQE keys to move the camera around
- On VR/MR devices, you can just look around to explore the generation made by the AI. No movement is currently implemented (but it should be trivial to do)

### UI
On the left you have the panel showing the status of the AI queries, while on the right you can input your prompts.

The left panel contains:
- The "AI Status" label, that tells you if the AI is busy or not
- The "AI Transcription" label, that tells you the latest transcription of your voice command
- The "AI Response" label, that tells you the latest response of the AI to a command, that is the latest script generated

The right panel contains:
- A microphone icon, that you can press to start recording your voice prompt and press again to stop recording. Voice prompts are automatically sent and do not require you to press the "Send" button
- The "Input" field, where you can write your prompt
- The "Send" button, that sends the prompt to the AI engine

# Known issues
There are some known issues with the application:
- There is no way to reset a scene, the only way is to restart the application
- Cubes are all generated directly into the scene, while probably they should be grouped in groups not to make the scene to cluttered at runtime

There are some known problems with the AI in the experience:
- I am not an expert with AI prompts (to be honest, I've just started!), so the prompts I've written may not be optimal. If you have suggestions, please let me know!
- The AI may generate scripts that are wrong, or don't compile, so the cubes in the demo may not do anything after you have input your query. This is pretty normal with the current status of the technology. If a prompt fails, go on making another one
- I had to code the classes for volume detection on the microphone and the music because when I asked the AI to do them, I obtained always buggy results. Hopefully in the future they could be removed and the AI could generate this logic on the fly when asked to develop sound-reactiveness of the cubes
- If you need to add physics to the cubes, you have to specify in your query to "add a rigidbody if needed", or the application may crash if the AI doesn't generate it

The build for IL2CPP has many issues:
- The application must be built with the Debug version of the IL2CPP runtime (I already set this up for you). For some reason, with the current LTS version of Unity 2021, if you try to build with the Release version, the build will saturate all your RAM and then fail. This should be solved with Unity 2022.2 (see https://forum.unity.com/threads/il2cpp-android-build-llvm-error-out-of-memory.801198/). The solution I've found (https://stackoverflow.com/questions/70764203/unity-il2cpp-build-llvm-error-out-of-memory) is setting the runtime to Debug for now.
- The application won't be able to launch some MonoBehaviour events. Start and Update work, but for instance OnTriggerEnter does not. This is why any command related to "hands touching" do not work when building with IL2CPP
- The application will freeze for a few seconds when the logic is loaded into memory to apply it to the cubes. This is because dotnow offers a function called CompileAndLoadSourceInterpreted which must be called by the main thread. In VR/MR freezes are very annoying, and I advice you to close your eyes when they are happening not to feel discomfort. This problem may be fixed when dotnow provides async methods for compiling and loading scripts
- ILC2PP may strip away some methods/classes that look unused by the application. If the logic written by the AI is correct, but you see nothing happening, it may be because Unity has stripped some classes or methods used by the logic generated at runtime. Add the assemblies with the logic stripped away to the link.xml file in the project

The build for Mixed Reality has an additional issue:
- When the Meta Quest system keyboard appears on screen, there is no way to make it disappear. This is a bug related to the interaction between Unity and the Quest runtime. Do not use the written prompt text field, but only vocal commands. If by chance you have clicked on the text field, press the power button on the headset and make it go in standby, then press it again and make it on again. The keyboard should have disappeared
- When the application runs, it seems that the Quest runtime is not that happy because some weird "tearing" effect happens in front of your eyes from time to time

In general, this project is still a prototype, and there are many things that are broken or that can be improved. Take it as a starting point and not a final destination.

# Safety warning
There is also a final safety concern I have.

The application is made to load custom logic at runtime... a cracker could exploit it to make it load malicious code. If the script that OpenAI returns you is about deleting your hard drive, the program will delete your hard drive. Use it with caution. I'm not responsible for any misuse of this prototype.

# Authors

* **Antony Vitillo (Skarredghost)** - [Blog](http://skarredghost.com) - [Company](https://vrroom.world) - [Patreon](https://www.patreon.com/skarredghost) - [Contacts](https://skarredghost.com/contact/) 

# License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

# Acknowledgments

I have developed this plugin in my free time, but still as part of my efforts inside my company [VRROOM](https://vrroom.world), in which I'm on a mission to create next-gen events and concerts using immersive realities. If you would like to collaborate with us, invest in us, or just say hello, you are more than welcome to do that.

We're releasing this for free, to be helpful for the community, to build something together with the community. The value of sharing is strong in us.

We'll try to maintain this plugin as much as we can and to answer your questions, but we have a lot of work to do for our product, so we cannot guarantee that we'll be able to continue the project or fix all the issues that may arise. We'll try to do our best.

Have fun :)







