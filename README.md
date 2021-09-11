# OVR SmartBridge Client

This software is used as the connection between your gaming machine and the ovrsmartbridge-addon for home assistant. It reads sensor data from OpenVR and displays notification inside your HMD.

Example usecases:
- Turn off your lights when put on your vr headset and turn them on again when you take it off.
- Display notifications inside your HMD when some motion sensor is triggered

## Features

- exposes OpenVR proximity sensor as binary_sensor to home assistant
- adds event listener to display custom notifications from home assistant in your vr headset

## Dependencies

- [benotter/OVRLay](https://github.com/benotter/OVRLay) for rendering the notifications in vr
- [Notification from IENBA](https://freesound.org/people/IENBA/sounds/545495/) as notification sound

## Installation

Make sure you have the [ovrsmartbridge-addon](https://github.com/ovrsmartbridge/ovrsmartbridge-addon) installed in your home assistant setup.

Download the latest setup from the [releases page](https://github.com/ovrsmartbridge/ovrsmartbridge-client/releases) and run it.

OVR Smart Bridge Client will be installed in your programs folder and should be registered in your steamvr overlay autostart list.

## Screenshots

TODO
<!-- ![App Screenshot](https://via.placeholder.com/468x300?text=App+Screenshot+Here) -->

  
<!-- ## FAQ

#### Question 1

Answer 1

#### Question 2

Answer 2 -->
