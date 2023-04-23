using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 本地数据库管理
///  ljs
/// </summary>
namespace BaseFrame
{
    public enum PLAYERPERFS
    {
        ACCOUNT,          //账号
        PASSWORD,       //密码
        LASTSERVER,      //服务器地址
        PLAYERNAME,    //玩家名称
        VISITOR,        //游客账号
        MUSICVOLUME,       //背景音乐音量
        MUSICENABLE,        //背景音乐开关
        SOUNDVOLUME,    //音效音量
        SOUNDENABLE,     //音效开关
        CLIENTVER              //前端版本号
        //需要的在下面继续枚举

    }

    public class LocalDBManager : Singleton<LocalDBManager>
    {
        private  Dictionary<PLAYERPERFS, string> _playerPerfs = new Dictionary<PLAYERPERFS, string>();

        public override void InitDataM()
        {
            _playerPerfs.Add(PLAYERPERFS.ACCOUNT,"AT");
            _playerPerfs.Add(PLAYERPERFS.PASSWORD, "PW");
            _playerPerfs.Add(PLAYERPERFS.LASTSERVER, "LS");
            _playerPerfs.Add(PLAYERPERFS.MUSICVOLUME, "MV");
            _playerPerfs.Add(PLAYERPERFS.MUSICENABLE, "ME");
            _playerPerfs.Add(PLAYERPERFS.SOUNDVOLUME, "SV");
            _playerPerfs.Add(PLAYERPERFS.SOUNDENABLE, "SE");
            _playerPerfs.Add(PLAYERPERFS.CLIENTVER, "CV");
            _playerPerfs.Add(PLAYERPERFS.VISITOR, "VR");
        }

        public  bool HasPlayerPerfsKey(PLAYERPERFS index)
        {
            if (_playerPerfs.ContainsKey(index))
                return PlayerPrefs.HasKey(_playerPerfs[index]);
            return false;
        }
        public  string GetPlayerStringPerfs(PLAYERPERFS index)
        {
            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                if (PlayerPrefs.HasKey(key))
                    return PlayerPrefs.GetString(key);
            }
            return "";
        }
        public  int GetPlayerIntPerfs(PLAYERPERFS index)
        {
            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                if (PlayerPrefs.HasKey(key))
                    return PlayerPrefs.GetInt(key);

            }
            return 0;
        }
        public  float GetPlayerFloatPerfs(PLAYERPERFS index)
        {
            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                if (PlayerPrefs.HasKey(key))
                    return PlayerPrefs.GetFloat(key);
            }
            return 0;
        }
        public  bool GetPlayerBoolPerfs(PLAYERPERFS index)
        {

            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                if (PlayerPrefs.HasKey(key))
                    return (PlayerPrefs.GetInt(key) == 1);
            }
            return false;
        }

        public bool GetPlayerBoolPerfs(string key_)
        {
            if ((PlayerPrefs.GetInt(key_) == 1))
                return true;
            return false;
        }

        public  void SetPlayerPerfs(PLAYERPERFS index, string val)
        {
            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                PlayerPrefs.SetString(key, val);
                PlayerPrefs.Save();
            }
        }

        public void DeletePlayerPrefs(PLAYERPERFS index_)
        {
            if (_playerPerfs.ContainsKey(index_))
            {
                string key = _playerPerfs[index_];
                PlayerPrefs.DeleteKey(key);
            }
        }

        public void SetPlayerPerfs(string key_, int value_)
        {
            PlayerPrefs.SetInt(key_, value_);
            PlayerPrefs.Save();
        }



        public  void SetPlayerPerfs(PLAYERPERFS index, int val)
        {
            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                PlayerPrefs.SetInt(key, val);
                PlayerPrefs.Save();
            }
        }

        public  void SetPlayerPerfs(PLAYERPERFS index, float val)
        {
            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                PlayerPrefs.SetFloat(key, val);
                PlayerPrefs.Save();
            }
        }
        public  void SetPlayerPerfs(PLAYERPERFS index, bool val)
        {
            if (_playerPerfs.ContainsKey(index))
            {
                string key = _playerPerfs[index];
                PlayerPrefs.SetInt(key, val ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

    }
}