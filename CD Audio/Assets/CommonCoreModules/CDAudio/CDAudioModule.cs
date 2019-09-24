using CommonCore.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace CommonCore.CDAudio
{

    /// <summary>
    /// CD Audio playback module, using VLC
    /// </summary>
    public class CDAudioModule : CCModule
    {
        private const string HelperPath = @"SandstormMPHelper1.exe";
        private const string PlayerPath = @"vlc/vlc.exe";

        public static CDAudioModule Instance { get; private set; }

        private CDAudioState State;
        private int NextTrackIndex; //for "next" functionality

        //metadata
        private char DriveLetter;
        private int TrackCount;
        private IReadOnlyList<int> NonReservedTracks;

        private Process PlayerProcess;

        public CDAudioModule()
        {
            Instance = this;

            if(!ConfigState.Instance.UseCDAudio)
            {
                Log("Disabled by config setting. Set ConfigState.UseCDAudio to true and restart the game to use CD audio.");
                return;
            }

            //check that files exist
            if(!File.Exists(Path.Combine(CoreParams.GameFolderPath, HelperPath)) || !File.Exists(Path.Combine(CoreParams.GameFolderPath, PlayerPath)))
            {
                LogError("Couldn't find player or helper exe. Aborting.");
                return;
            }

            //get CD status/tracks
            var (count, drive) = GetCDInfo();
            if(count == 0)
            {
                Log("CD drive doesn't exist or is not ready. Aborting.");
                return;
            }
            TrackCount = count;
            DriveLetter = drive;

            Log($"Found CD with {TrackCount} tracks at {DriveLetter}:\\");

            //make a list of non-reserved tracks
            var tracks = new List<int>();
            for(int i = 1; i <= count; i++)
            {
                if (i == 1 || i == 2 || i == count) //reserve tracks 1 and 2 and the last track
                    continue;
                tracks.Add(i);
            }
            NonReservedTracks = tracks;

            State = CDAudioState.Ready;
        }

        public override void Dispose()
        {
            base.Dispose();

            //cleanup CD audio
            KillPlayer();
        }

        private (int count, char drive) GetCDInfo()
        {
            //get the CD info with the helper
            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(CoreParams.GameFolderPath, HelperPath);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] segs = output.Trim().Split(',');

            if (segs.Length <= 1)
                return (0, default);

            return (int.Parse(segs[0]), segs[1][0]);
        }

        private void KillPlayer()
        {
            try
            {
                if (PlayerProcess != null && !PlayerProcess.HasExited)
                {
                    PlayerProcess.Kill();
                    PlayerProcess = null;
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
        }

        private void StartPlayer(int track, bool repeat, float gain)
        {
            try
            {
                string commandLine = $"-I dummy {(repeat ? "repeat" : "")} --gain {gain:F2} cdda:///{DriveLetter}:/ --cdda-track {track}";

                PlayerProcess = new Process();
                PlayerProcess.StartInfo.FileName = Path.Combine(CoreParams.GameFolderPath, PlayerPath);
                PlayerProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(Path.Combine(CoreParams.GameFolderPath, PlayerPath));
                PlayerProcess.StartInfo.Arguments = commandLine;

                PlayerProcess.Start();
            }
            catch(Exception e)
            {
                LogException(e);
            }
        }

        private enum CDAudioState
        {
            NotReady, Ready, Playing
        }

        /// <summary>
        /// Whether the CD audio module is ready
        /// </summary>
        public bool Ready => State > CDAudioState.NotReady;

        /// <summary>
        /// Whether the CD audio module is playing a track
        /// </summary>
        public bool Playing => State == CDAudioState.Playing;

        /// <summary>
        /// Stops the currently playing track
        /// </summary>
        public void Stop()
        {
            KillPlayer();
            if(PlayerProcess == null)
            {
                State = CDAudioState.Ready;
            }
            else
            {
                LogError("Failed to stop CD audio!");
                State = CDAudioState.NotReady; //fault
            }
        }

        /// <summary>
        /// Plays a specific track from the disc
        /// </summary>
        /// <remarks>Use a negative track index to count from the end of the disc</remarks>
        public void Play(int track, bool looping, float volume)
        {
            if (State == CDAudioState.NotReady)
            {
                LogWarning("Tried to play a track with audio player not ready!");
                return;
            }

            int actualTrack;
            if(track > TrackCount || track < -TrackCount)
            {
                throw new ArgumentException("Track must actually be on the CD", nameof(track));
            }
            else if (track > 0)
            {
                //count from start
                actualTrack = track;
            }
            else if (track < 0)
            {
                //count from end
                actualTrack = TrackCount + track + 1; //-1 equals last track and it's one-based
            }
            else
                throw new ArgumentException("track must be non-zero", nameof(track));

            KillPlayer();
            if (PlayerProcess == null)
            {
                StartPlayer(actualTrack, looping, volume);
                if (PlayerProcess != null)
                    State = CDAudioState.Playing;
            }
            else
            {
                LogError("Failed to stop CD audio!");
                State = CDAudioState.NotReady; //fault
            }

        }

        /// <summary>
        /// Plays the next (non-reserved) track
        /// </summary>
        public void PlayNext(bool looping, float volume)
        {
            if (State == CDAudioState.NotReady)
            {
                LogWarning("Tried to play a track with audio player not ready!");
                return;
            }

            
            KillPlayer();
            if(PlayerProcess == null)
            {
                StartPlayer(NonReservedTracks[NextTrackIndex], looping, volume);
            }
            else
            {
                LogError("Failed to stop CD audio!");
                State = CDAudioState.NotReady; //fault
            }
            

            NextTrackIndex++;
            if (NextTrackIndex >= NonReservedTracks.Count)
                NextTrackIndex = 0;
        }

        /// <summary>
        /// Resets the "next track" track counter
        /// </summary>
        public void ResetNext()
        {
            NextTrackIndex = 0;
        }
    }
}