//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

// <code>
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

namespace CognitiveServicesGenerateVTT
{
    class Program
    {
        private static string SubscriptionKey = ConfigurationManager.AppSettings.Get("SubscriptionKey");
        private static string Region = ConfigurationManager.AppSettings.Get("Region");

        private static string InputFilePath;
        private static string VttOutputPath;

        public static async Task TranslationContinuousRecognitionAsync()
        {
            // Creates an instance of a speech translation config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechTranslationConfig.FromSubscription(SubscriptionKey, Region);

            InputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SourceVideo", "ignite.wav");
            VttOutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Transcript_{0}.vtt");

            // Sets source and target languages.
            string fromLanguage = "en-US";
            config.SpeechRecognitionLanguage = fromLanguage;
            config.AddTargetLanguage("de");
            config.AddTargetLanguage("ar");
            config.AddTargetLanguage("fr");

            //Create VTT files
            CreateTranscriptFile(fromLanguage);
            foreach (var targetLang in config.TargetLanguages)
            {
                CreateTranscriptFile(targetLang);
            }

            using var audioConfig = GetWavAudioConfig();
            //using var audioConfig = GetBytesAudioConfig();
            
            using (var recognizer = new TranslationRecognizer(config, audioConfig))
            {
                // Subscribes to events.
                recognizer.Recognizing += (s, e) =>
                {
                    Console.WriteLine($"RECOGNIZING in '{fromLanguage}': Text={e.Result.Text}");

                    foreach (var element in e.Result.Translations)
                    {
                        Console.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");
                    }
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.TranslatedSpeech)
                    {
                        WriteVttLine(fromLanguage, FormatTime(e.Result.OffsetInTicks) + " --> " + FormatTime(e.Result.OffsetInTicks + e.Result.Duration.Ticks) + Environment.NewLine);
                        WriteVttLine(fromLanguage, e.Result.Text + Environment.NewLine + Environment.NewLine);

                        Console.WriteLine($"\nFinal result: Reason: {e.Result.Reason.ToString()}, recognized text in {fromLanguage}: {e.Result.Text}.");
                        foreach (var element in e.Result.Translations)
                        {
                            Console.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");

                            WriteVttLine(element.Key, FormatTime(e.Result.OffsetInTicks) + " --> " + FormatTime(e.Result.OffsetInTicks + e.Result.Duration.Ticks) + Environment.NewLine);
                            WriteVttLine(element.Key, element.Value + Environment.NewLine + Environment.NewLine);
                        }
                    }
                };

                recognizer.Synthesizing += (s, e) =>
                {
                    var audio = e.Result.GetAudio();
                    Console.WriteLine(audio.Length != 0
                        ? $"AudioSize: {audio.Length}"
                        : $"AudioSize: {audio.Length} (end of synthesis data)");
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"\nRecognition canceled. Reason: {e.Reason}; ErrorDetails: {e.ErrorDetails}");
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\nSession started event.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\nSession stopped event.");
                };

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                //Console.WriteLine("Say something...");

                await recognizer.StartContinuousRecognitionAsync();

                do
                {
                    Console.WriteLine("Press Enter to stop");
                } while (Console.ReadKey().Key != ConsoleKey.Enter);

                // Stops continuous recognition.
                await recognizer.StopContinuousRecognitionAsync();
            }
        }

        private static AudioConfig GetWavAudioConfig()
        {
            return AudioConfig.FromWavFileInput(InputFilePath); //wav file
        }

        private static AudioConfig GetBytesAudioConfig()
        {
            //Custom wav file when sending the byte stream
            AudioStreamFormat audioStreamFormat = AudioStreamFormat.GetWaveFormatPCM(48000, 16, 2);

            //mp4 file
            //AudioStreamFormat audioStreamFormat = AudioStreamFormat.GetCompressedFormat(AudioStreamContainerFormat.ANY);

            var reader = new BinaryReader(File.OpenRead(InputFilePath));
            using var audioInputStream = AudioInputStream.CreatePushStream(audioStreamFormat);
            using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);

            byte[] readBytes;
            do
            {
                readBytes = reader.ReadBytes(1024);
                audioInputStream.Write(readBytes, readBytes.Length);
            } while (readBytes.Length > 0);

            return audioConfig;
        }

        static async Task Main(string[] args)
        {
            await TranslationContinuousRecognitionAsync();
        }

        static string FormatTime(long nano)
        {
            var hour = Math.Floor(Convert.ToDecimal(nano / 36000000000));
            var temp = nano % 36000000000;
            var minute = Math.Floor(Convert.ToDecimal(temp / 600000000));
            var temp2 = temp % 600000000;
            var second = Math.Floor(Convert.ToDecimal(temp2 / 10000000));
            var mil = temp2 % 10000000;
            return $"{hour.ToString("00")}:{minute.ToString("00")}:{second.ToString("00")}.{(mil.ToString().Length > 3 ? mil.ToString().Substring(0, 3) : mil.ToString())}";
        }

        static void CreateTranscriptFile(string language)
        {
            string filePath = string.Format(VttOutputPath, language);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var file = File.Create(filePath)) { }
            WriteVttLine(language, "WEBVTT" + Environment.NewLine + Environment.NewLine);
            WriteVttLine(language, "NOTE" + Environment.NewLine);
            WriteVttLine(language, "Language:" + language + Environment.NewLine + Environment.NewLine);
        }

        static void WriteVttLine(string language, string line)
        {
            string filePath = string.Format(VttOutputPath, language);
            File.AppendAllText(filePath, line);
        }
    }
}
// </code>
