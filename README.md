# <div align="center"> MOBILE COMPUTING PROJECT: BROKEN DREAMS </div>
The following project is a small mobile game taken as a delivery for the Mobile Computing exam. It is the result of work carried out by myself and my colleague [Antonio Lanza](https://github.com/AntonioSouls) who worked with me in the entire realization of this project

## Game Plot
The game catapults you into a shattered dream, a dream world where your mind is a prisoner. You are overwhelmed by emotions: *Anger*, *Fear*, *Sadness* and *Depression* have annihilated all rationality, leaving you helpless, at the mercy of their attacks.

**BUT A SPARK STILL BURNS WITHIN YOU**: you want to return to your life. You want to come out of this nightmare.

To do so, you will have to face a challenge: explore the labyrinth of your mind, find where every emotion is hiding and confront the intrusive thoughts that try to stop you.

Only by retrieving the **magic key** can you access the core of emotion. Only by finding the **potion of courage** will you have the strength to face it and overcome it.

*Anger*. *Fear*. *Sadness*. *Depression*. One by one, you will face them. Only by overcoming them all will you be able to wake up and take charge of your life again.

<p align="center">
  <img src="BrokenDreams_DemoVideo.gif" alt="Demo" />
</p>

## How to Install
Clone the repository by running the following commands in your terminal:
```
git clone https://github.com/adducec03/BrokenDreams.git
cd BrokenDreams
```
Then follow the appropriate procedure for your operating system

### Android

Open the project in Unity and select **file -> build profiles -> Android**. If you don't have it install the Android module in the Unity Hub. Then select the **Build** option and chose a directory where the .apk file will be saved.

At this point when you have your .apk file install it on your Android device.

### iOS
Open the project in Unity and select **file -> build profiles -> iOS**. If you don't have it install the iOS module in the Unity Hub. Then select the **Build** option and chose a directory where the iOS build files will be saved.

At this point connect your iPhone to your MacOS device and make sure you have an Apple account connected to XCode. To do so go to **XCode -> Settings -> Accounts** and login with your Apple ID.

Then open XCode and in **Window -> Devices and Simulators** connect your iPhone to XCode.

After that open the **Unity-iPhone.xcworkspace** file in XCode. You will see the Window with the project settings. Go to **Signing & Capabilities** and select your Apple ID in the filed **Team**. Then set the **Bundle Identifier** as **com.yourname.gamename**.

At this point select the **Build** button at the top left of the window and start the build with your iPhone connected to the Mac via cable.

Once the installation is complete go to your iPhone, unlock it and go to **Settings -> Generals -> VPN and Device Management** and authorize the developer of the app. Now you can open the game on your home screen and play it!

## Authors
<a href="https://github.com/AntonioSouls">
  <img src="https://github.com/AntonioSouls.png" width="80">
</a>
<a href="https://github.com/adducec03">
  <img src="https://github.com/adducec03.png" width="80">
</a>

