#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SpeechLib;
using System.Threading;

using XLua;
//namespace XLuaFramework {
/// <summary>
///语音播报状态
/// </summary>

public enum VoiceStatus
{
    /// <summary>
    /// The play
    /// </summary>
    Play,
    /// <summary>
    /// The ready
    /// </summary>
    Ready,
    /// <summary>
    /// The pause
    /// </summary>
    Pause,
}

/// <summary>
///语音播报
/// </summary>

    public class SpeechVoice
    {
        /// <summary>
        /// The _voice
        /// </summary>
        private SpVoice _voice;
        private SpVoiceClass spVoice;
        private readonly SpFileStreamClass spFile;

    private static SpeechVoice _speaker;

    private bool _isLoopAudioFile; // 是否循环播放声音文件
        private bool _isLoopSpeech;  //循环语音
        private VoiceStatus speakStatus = VoiceStatus.Ready;
        private bool IsStopPlayFile;    //是否停止
        private bool IsStopPlayVoice;


        /// <summary>
        /// 播放文件结束
        /// </summary>
        public event EventHandler PlayFileComplete;
        /// <summary>
        /// 播放文件开始
        /// </summary>
        public event EventHandler PlayFileStart;

        /// <summary>
        /// 播放文件结束
        /// </summary>
        public event EventHandler PlayAudioComplete;
        /// <summary>
        /// 播放文件开始
        /// </summary>
        public event EventHandler PlayAudioStart;

    public static SpeechVoice Instance
    {
        get
        {
            if (_speaker != null)
                return _speaker;
             _speaker = new SpeechVoice();
             
             return _speaker;
            
        }
    }
    /// <summary>
    /// 播放语言
    /// </summary>
    /// <param name="voiceContent">Content of the voice.</param>
    public void Speech_Voice(string voiceContent)
        {
            IsStopPlayVoice = false;
            if (speakStatus == VoiceStatus.Play) return; //正在播放就返回
            _speaker.Resume();
            Action invoke = () =>
            {
                OnPlayAudioStart();//触发开始播放事件 
                speakStatus = VoiceStatus.Play;
                _speaker.Speak(voiceContent);
            };
            invoke.BeginInvoke(VoiceCallback, invoke);
        }
        private void VoiceCallback(IAsyncResult ar)
        {
            var ac = ar.AsyncState as Action;
            if (ac != null)
            {
                try//原dll不能多次停止 所以加了try catch 和状态判断
                {
                    if ((_isLoopSpeech) && !IsStopPlayVoice)
                    {
                        //一次播放结束之后如果是循环播放 就继续播放
                        ac.BeginInvoke(VoiceCallback, ac);
                    }
                    else
                    {
                        speakStatus = VoiceStatus.Pause;
                        //触发停止事件
                        OnPlayAudioComplete(this, new EventArgs());
                        
                        ac.EndInvoke(ar);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        //以下同理
        /// <summary>
        /// 暂停播放
        /// </summary>
        public void StopSpeechVoice()
        {
            if (IsStopPlayVoice) return;
            IsStopPlayVoice = true;
            speakStatus = VoiceStatus.Pause;
            Action invoke = () => _speaker.StopVoice();
            invoke.BeginInvoke(null, invoke);
            OnPlayAudioComplete(this, new EventArgs());
        }
        /// <summary>
        /// 停止播放声音文件
        /// </summary>
        public void StopPlayer()
        {
        /*
            if (IsStopPlayVoice) return;
            IsStopPlayFile = true;
            speakStatus = VoiceStatus.Pause;
            Speaker.PauseFile();
            OnPlayFileComplete(this, new EventArgs());
        */
        }

        /// <summary>
        /// 播放声音文件
        /// </summary>
        public void PlayAudioFile()
        {
        /*
            player = new SoundPlayer { SoundLocation = _audioFile.FilePath };
            if (speakStatus == VoiceStatus.Play) return;
            IsStopPlayFile = false;
            if (File.Exists(_audioFile.FilePath))
            {
                Action invoke = () =>
                {
                    OnPlayFileStart();
                    speakStatus = VoiceStatus.Play;
                    Speaker.PlaySound(_audioFile.FilePath);
                };
                invoke.BeginInvoke(Callback, invoke);
            }
        */
        }

        /// <summary>
        /// Called when [play start].
        /// </summary>
        public void OnPlayFileStart()
        {
            var handler = PlayFileStart;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [play start].
        /// </summary>
        public void OnPlayAudioStart()
        {
            var handler = PlayAudioStart;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [play complete].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        
        
        public void OnPlayAudioComplete(object sender, EventArgs e)
        {
            EventHandler handler = PlayAudioComplete;
            if (handler != null) handler(this,e);
        }
        

        /// <summary>
        /// Called when [play complete].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void OnPlayFileComplete(object sender, EventArgs e)
        {
            EventHandler handler = PlayFileComplete;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Callbacks the specified ar.
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void Callback(IAsyncResult ar)
        {
            var ac = ar.AsyncState as Action;
            if (ac != null)
            {
                OnPlayFileComplete(this, new EventArgs());
                try
                {
                    if ((_isLoopSpeech) && !IsStopPlayFile)
                    {
                        ac.BeginInvoke(Callback, ac);
                    }
                    else
                    {
                        speakStatus = VoiceStatus.Pause;
                        ac.EndInvoke(ar);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechVoice"/> class.
    /// </summary>
        SpeechVoice()
        {
            _voice = new SpVoice();
            spVoice = new SpVoiceClass();
            spFile = new SpFileStreamClass();
        }



        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="speakFlag">The speak flag.</param>
        public void Speak(string text, SpeechVoiceSpeakFlags speakFlag = SpeechVoiceSpeakFlags.SVSFDefault)
        {
            _voice.Speak(string.Empty,SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak); _voice.Speak(text, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }


        /// <summary>
        /// 异步播放
        /// </summary>
        /// <param name="text">The text.</param>
        public void SpeakAsync(string text)
        {
            _voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void PlaySound(string fileName)
        {
            //要加载COM组件:Microsoft speech object Library
            if (!System.IO.File.Exists(fileName)) return;
            spFile.Open(fileName, SpeechStreamFileMode.SSFMOpenForRead, true);
            var istream = spFile as ISpeechBaseStream;
            spVoice.SpeakStream(istream, SpeechVoiceSpeakFlags.SVSFIsFilename);
            spFile.Close();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (_voice != null)
                _voice.Pause();
        }

        /// <summary>
        /// Stops the voice.
        /// </summary>
        public void StopVoice()
        {
            _voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        /// <summary>
        /// Pauses the file. 
        /// </summary>
        public void StopFile()
        {
            try
            {
                spFile.ISpStream_Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Resume()
        {
            if (_voice != null)
                _voice.Resume();
        }
    }
//}
#endif

