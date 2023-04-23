using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 音效模块管理
/// ljs
/// </summary>
namespace BaseFrame
{
    //音效分类
    public enum eAudioType
    {
        NONE = 0,
        MusicBG = 1,//背景音乐
        Sound = 2//音效
    }

    public enum eAudioPlayType
    {
        ONCE = 0,//播放一次
        LOOP = 1,//循环播放
        RANDOM = 2//随机播放
    }

    //UI音效枚举
    public enum SOUND
    {
        UI_TANCHUCAIDAN = 0,        //弹出菜单
        UI_DIANJIFUBEN = 1,         //点击副本按钮
        UI_BUTTONCLICK,             //按钮按下
        UI_GUANBICHUANGKOU,         //关闭窗口，页面
        UI_ANNIUSHIBAI,             //按钮功能失败
        UI_DAKAIBEIBAO,             //打开背包
        UI_ZHUANGBEI ,           //装备 服装或武器
        UI_RENWUWANCHENG,			//任务完成
        UI_OPEN,//打开窗口音效
        UI_CLOSE,//关闭窗口音效
        UI_AWARD,//奖励
        UI_BATTLEWIN,//战斗胜利
        UI_BATTLELOSE,//战斗失败
    }

    public class CoroutineParam
    {
        public object param;
    }

    public class AudioManager : Singleton<AudioManager>,IUpdateComponent
    {

        private Dictionary<string, AudioClip> _sounds = new Dictionary<string, AudioClip>();//记录加载到内存中的音效文件，避免重复加载
        private Dictionary<SOUND, string> _uiSounds = new Dictionary<SOUND, string>();//记录UI常用的音效文件，这里需要预定义
        private AudioSource _backgroundMusic = null;//背景音乐音源
        private List<string> _emptySounds = new List<string>();//记录不存在的音效文件

        public override void InitDataM()
        {
            base.InitDataM();
            UpdateComponentManager.GetInstance().AddUpdateComponent(this, UPDATE_SPACE.SECOND);
            _uiSounds.Clear();
            _uiSounds.Add(SOUND.UI_TANCHUCAIDAN, "UI_anniu01");
            _uiSounds.Add(SOUND.UI_DIANJIFUBEN, "UI_anniu02");
            _uiSounds.Add(SOUND.UI_BUTTONCLICK, "UI_button");
            _uiSounds.Add(SOUND.UI_GUANBICHUANGKOU, "UI_anniu04");
            _uiSounds.Add(SOUND.UI_ANNIUSHIBAI, "UI_anniu_shibai");
            _uiSounds.Add(SOUND.UI_DAKAIBEIBAO, "UI_beibao");
            _uiSounds.Add(SOUND.UI_ZHUANGBEI, "UI_fangzhi");
            _uiSounds.Add(SOUND.UI_OPEN, "UI_open");
            _uiSounds.Add(SOUND.UI_CLOSE, "UI_close");
            _uiSounds.Add(SOUND.UI_AWARD, "UI_award");
            _uiSounds.Add(SOUND.UI_RENWUWANCHENG, "UI_missionOK");

            //音效数据初始化
            if (LocalDBManager.Instance.HasPlayerPerfsKey(PLAYERPERFS.SOUNDVOLUME))
                _soundVolume = LocalDBManager.Instance.GetPlayerFloatPerfs(PLAYERPERFS.SOUNDVOLUME);
            if (LocalDBManager.Instance.HasPlayerPerfsKey(PLAYERPERFS.MUSICVOLUME))
                _musicVolume = LocalDBManager.Instance.GetPlayerFloatPerfs(PLAYERPERFS.MUSICVOLUME);
            if (LocalDBManager.Instance.HasPlayerPerfsKey(PLAYERPERFS.SOUNDENABLE))
                _soundEnable = LocalDBManager.Instance.GetPlayerBoolPerfs(PLAYERPERFS.SOUNDENABLE);
            if (LocalDBManager.Instance.HasPlayerPerfsKey(PLAYERPERFS.MUSICENABLE))
                _musicEnable = LocalDBManager.Instance.GetPlayerBoolPerfs(PLAYERPERFS.MUSICENABLE);
        }

        //音量开关
        private bool _soundEnable = true;
        public bool mSoundEnable
        {
            get { return _soundEnable; }
            set
            {
                if (_soundEnable == value) return;
                _soundEnable = value;
                LocalDBManager.Instance.SetPlayerPerfs(PLAYERPERFS.SOUNDENABLE, _soundEnable);
            }
        }

        //音量大小
        private float _soundVolume = 1f;
        public float mSoundVolume
        {
            get { return _soundVolume; }
            set
            {
                _soundVolume = value;
                if (_soundVolume > 1) _soundVolume = 1;
                else if (_soundVolume < 0) _soundVolume = 0;
                LocalDBManager.Instance.SetPlayerPerfs(PLAYERPERFS.SOUNDVOLUME, _soundVolume);
            }
        }

        //背景音乐开关
        private bool _musicEnable = true;
        public bool mMusicEnable
        {
            get { return _musicEnable; }
            set
            {
                if (_musicEnable == value) return;
                _musicEnable = value;
                if (_backgroundMusic != null) _backgroundMusic.mute = !_musicEnable;
                LocalDBManager.Instance.SetPlayerPerfs(PLAYERPERFS.MUSICENABLE, _musicEnable);
            }
        }

        //背景音乐音量
        private float _musicVolume = 1f;
        public float mMusicVolume
        {
            get { return _musicVolume; }
            set
            {
                if (_musicVolume == value) return;
                _musicVolume = value;
                if (_musicVolume > 1) _musicVolume = 1;
                else if (_musicVolume < 0) _musicVolume = 0;
                if (_backgroundMusic != null) _backgroundMusic.volume = _musicVolume;
                LocalDBManager.Instance.SetPlayerPerfs(PLAYERPERFS.MUSICVOLUME, _musicVolume);
            }
        }

        private bool _lastPlayStatus = true;
        public void UpdateM(float deltaTime_, float fixedDeltaTime_, float realDeltaTime_)
        {
            if (_backgroundMusic == null) return;
           
            if (eAudioPlayType.RANDOM == _bgMusicPlayType)//随机播放
            {
                if (true == _lastPlayStatus && false == _backgroundMusic.isPlaying)//播放完毕
                {
                    ResetSceneBgMusic(true);
                }
                _lastPlayStatus = _backgroundMusic.isPlaying;
            }
            
        }
        public override void DestroyM()
        {
            UpdateComponentManager.GetInstance().DelUpdateComponent(this);
        }

        public bool IsCanPlay(eAudioType type_)
        {
            switch (type_)
            {
                case eAudioType.MusicBG:
                    return _musicEnable;
                case eAudioType.Sound:
                    return _soundEnable;
                default:
                    break;
            }
            return false;
        }

        //重置场景背景音乐
        public void ResetSceneBgMusic(bool isRandom_ = false)
        {
            string tempSceneName = SingleSceneManager.Instance.GetSceneName();
            //eAudioPlayType tempType = (eAudioPlayType)(UnitData.GetMapData(tempSceneName).randomType);
            //this.SetBackgroundMusic(UnitData.GetMapBgMusic(tempSceneName, isRandom_), tempType);
        }

        private eAudioPlayType _bgMusicPlayType;
        //播放背景音乐
        public void SetBackgroundMusic(string sound_, eAudioPlayType type_)
        {
            
            if (false == this.IsCanPlay(eAudioType.MusicBG)) return;

            if (!string.IsNullOrEmpty(sound_))
            {
                _bgMusicPlayType = type_;
                bool tempLoop = false;
                if (eAudioPlayType.LOOP == type_)
                    tempLoop = true;
                Common.StartCoroutine(SetBackgroundMusic_impl(sound_, tempLoop));
            }
        }
        private AudioListener _audioListener;
        IEnumerator SetBackgroundMusic_impl(string sound_,bool loop_)
        {
            CoroutineParam tempParam = new CoroutineParam();
            yield return GetSound(sound_, eAudioType.MusicBG,tempParam);
            AudioClip tempAudio = (AudioClip)tempParam.param;
            if (tempAudio == null)
                yield break;
            if (_backgroundMusic != null)
                UnityEngine.Object.Destroy(_backgroundMusic.gameObject);
            GameObject tempGameObject = new GameObject("BackgroundMusic");
            _audioListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;
            if (!_audioListener)
                _audioListener = Common.AddComponent<AudioListener>(tempGameObject);
            _backgroundMusic = Common.AddComponent<AudioSource>(tempGameObject);
            _backgroundMusic.rolloffMode = AudioRolloffMode.Linear;
            _backgroundMusic.clip = tempAudio;
            _backgroundMusic.volume = _musicVolume;
            _backgroundMusic.loop = loop_;
            _backgroundMusic.mute = !_musicEnable;
            _backgroundMusic.Play();
        }
        public void PlayUISound(SOUND sound_)
        {
            if (false == this.IsCanPlay(eAudioType.Sound)) return;

            if (!_uiSounds.ContainsKey(sound_)) return;

            PlaySound(_uiSounds[sound_], null);
        }

        //播放音效
        public void PlaySound(string sound_, GameObject gameObject_)
        {
            if (false == this.IsCanPlay(eAudioType.Sound)) return;
            Common.StartCoroutine(playSound_impl(sound_, gameObject_));
        }

        IEnumerator playSound_impl(string sound, GameObject gameObject)
        {
            CoroutineParam param = new CoroutineParam();
            yield return GetSound(sound, eAudioType.Sound, param);
            AudioClip tempAudio = (AudioClip)param.param;
            if (tempAudio == null || _soundEnable == false)
                yield break;
            if (gameObject != null)
            {
                GameObject soundObject = new GameObject("Sound");
                Common.AddChild(gameObject, soundObject);
                AudioSource audioSource = Common.AddComponent<AudioSource>(soundObject);
                audioSource.loop = false;
                audioSource.clip = tempAudio;
                audioSource.volume = _soundVolume;
                audioSource.Play();
                UnityEngine.Object.Destroy(soundObject, tempAudio.length + 0.1f);
            }
            else
            {
                if(null != _audioListener)
                    AudioSource.PlayClipAtPoint(tempAudio, _audioListener.transform.position, _soundVolume);
            }
        }

        Coroutine GetSound(string sound_, eAudioType type_, CoroutineParam param_)
        {
            return Common.StartCoroutine(getSound_impl(sound_, type_, param_));
        }

        IEnumerator getSound_impl(string sound_, eAudioType type_,CoroutineParam param_)
        {
            if (string.IsNullOrEmpty(sound_))
                yield break;
            if (_emptySounds.Contains(sound_))
            {
                //LogManager.GetInstance().error("Sound [{0}] is null", sound_);
                yield break;
            }
            AudioClip tempAudio = null;
            if (_sounds.ContainsKey(sound_))
            {
                tempAudio = _sounds[sound_];
            }
            else
            {
                CoroutineParam audioParam = new CoroutineParam();
                // yield return ResourceManager.GetInstance().LoadResource(sound, audioParam);
                //tempAudio = (AudioClip)audioParam.param;
                string tempAudioPath = Common.resPath_Sound;
                if (eAudioType.MusicBG == type_)
                    tempAudioPath = Common.resPath_Music;
                tempAudio = Common.GetGameAsset<AudioClip>(sound_, tempAudioPath);
                
                if (tempAudio == null)
                {
                    _emptySounds.Add(sound_);
                    yield break;
                }
                if (!_sounds.ContainsKey(sound_))
                    _sounds.Add(sound_, tempAudio);
            }
            param_.param = tempAudio;
        }

    }
}
