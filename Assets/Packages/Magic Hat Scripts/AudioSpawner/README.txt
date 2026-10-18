UNITY AUDIO SPAWNER

A lightweight, static audio utility for Unity that makes playing sound effects from anywhere in your code simple. Automatically manages and recycles AudioSources using an internal pooling system.


USAGE

Play an audio clip with a single line:

AudioSpawner.Play(audioClip);

A parameterized overload lets you customize playback. See the method signatures in AudioSpawner.cs for available options.

The Play methods return the AudioSource used for playback, allowing you to access it when needed. AudioSources are automatically recycled after playback finishes.


INSTILLATION

Extract AudioSpawner.cs from the downloaded package.

Add the script to your Unity project's Assets folder.

Call AudioSpawner.Play(audioClip) from your code.



LICENSE

See LICENSE.txt for usage terms.


ABOUT

Created by Magic Hat Games.

https://magichatgames.com

https://magic-hat-games.itch.io