# CognitiveServicesGenerateVTT
Generate transcription and translation using Azure Cognitive Services

This project help you in generating VTT files (Transcriptoin and Translation), then you can use those generated VTT files to provide subtitle for your video streams


By default, Cognitive Services supports only WAV files, but it can support compressed files (i.e. mp4) by dusing GStreamer. For more information, you can check the documentation here [GStreamer Configuration](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-use-codec-compressed-audio-input-streams?tabs=windows%2Cdebian%2Cjava-android%2Cterminal&pivots=programming-language-csharp#gstreamer-configuration).

You can find the latest GStreamer version in [GStreamer official website](https://gstreamer.freedesktop.org/)

Alternatively, if you have an mp4 video file and wish to generate the wav file without using the GStreamer, you can run the below ffmpeg command "ffmpeg -i SourceVideo.mp4 OutputAudio.wav"

## How to configure application to use stream rather than WAV file (to process other file types than WAV):
1- Modify the file extenstion. The application has 2 sample files WAV and MP4.
![image](https://user-images.githubusercontent.com/36260446/184531387-9e28dd33-609e-4e6d-9e17-802d692446eb.png)

2- There is 2 function to upload the sample `GetWavAudioConfig` which works with WAV files only, and `GetBytesAudioConfig` which works with other types, but it required `GStreamer` to be downloaded into the machine
![image](https://user-images.githubusercontent.com/36260446/184531483-4c32899e-4940-4845-ba66-9ca60e731bb9.png)

3- In `GetBytesAudioConfig` you need to modify the `AudioStreamFormat` as follows, by commenting the first box and uncomment the second one
![image](https://user-images.githubusercontent.com/36260446/184531607-595701dd-627e-4785-bcea-a77811d11591.png)



# Note:-
Please make sure to replace your Azure congnitive services key and location in the application App.Config
![image](https://user-images.githubusercontent.com/36260446/184531945-9bb69212-bed8-487b-899a-a2d0a2e1ae4a.png)

