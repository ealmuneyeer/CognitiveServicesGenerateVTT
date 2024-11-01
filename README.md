# CognitiveServicesGenerateVTT
Generate transcription and translation using Azure Cognitive Services

This project help you in generating VTT files (Transcriptoin and Translation), then you can use those generated VTT files to provide subtitle for your video streams


By default, Cognitive Services supports only WAV files (16 kHz or 8 kHz, 16-bit, and mono PCM), but it can support compressed files (i.e. mp4) or other WAV formats by using GStreamer. For more information, you can check the documentation here [GStreamer Configuration](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-use-codec-compressed-audio-input-streams?tabs=windows%2Cdebian%2Cjava-android%2Cterminal&pivots=programming-language-csharp#gstreamer-configuration).

You can find the latest GStreamer version in [GStreamer official website](https://gstreamer.freedesktop.org/)

Alternatively, if you have an mp4 video file and wish to generate the wav file without using the GStreamer, you can run the below ffmpeg command "ffmpeg -i SourceVideo.mp4 OutputAudio.wav"

# Note:-
+ Please make sure to update the values in the `App.Config`
   + `SubscriptionKey`: Cognitive Services key
   + `Region`: Cognitive Services resource location
   + `InputFilePath`: Input file location
   + `UseGStreamerWithWav`: Force to use GStreamer with Wav files or not

