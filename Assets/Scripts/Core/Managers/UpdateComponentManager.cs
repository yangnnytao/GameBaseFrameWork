using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BaseFrame
{
    /// <summary>
    /// 更新组件：驱动之力
    /// ljs
    /// </summary>
    public enum UPDATE_SPACE
    {
        NONE,           //每帧更新
        TWENTIETH,      //每二十分之一秒更新一次
        TENTH,          //每十分之一秒更新一次
        HALF_SECOND,    //半秒更新一次
        SECOND,         //一秒更新一次
        MINUTE,         //一分钟更新一次
    }

    public interface IUpdateComponent
    {
        void UpdateM(float deltaTime_, float fixedDeltaTime_, float realDeltaTime_);
    }
    public class UpdateComponentManager
    {
        private static UpdateComponentManager _instance = null;
        public static UpdateComponentManager GetInstance()
        {
            if (_instance == null)
                _instance = new UpdateComponentManager();
            return _instance;
        }
        List<IUpdateComponent> listNoneComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> listTwentiethComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> listTenthComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> listHalfSecondComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> listSecondComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> listMinuteComponents = new List<IUpdateComponent>();

        List<IUpdateComponent> alwayslistNoneComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> alwayslistTwentiethComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> alwayslistTenthComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> alwayslistHalfSecondComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> alwayslistSecondComponents = new List<IUpdateComponent>();
        List<IUpdateComponent> alwayslistMinuteComponents = new List<IUpdateComponent>();

        List<IUpdateComponent> listRemoveComponents = new List<IUpdateComponent>();
        const float noneSpace = 0.01f;
        const float twentiethSpace = 0.05f;
        const float tenthSpace = 0.1f;
        const float halfSecondSpace = 0.5f;
        const float secondSpace = 1f;
        const float minuteSpace = 60f;
        float noneTimes = 0f;
        float twentiethTimes = 0f;
        float tenthTimes = 0f;
        float halfSecondTimes = 0f;
        float secondTimes = 0;
        float minuteTimes = 0;

        public void AddUpdateComponent(IUpdateComponent component, UPDATE_SPACE space)
        {
            if (space == UPDATE_SPACE.NONE)
                listNoneComponents.Add(component);
            else if (space == UPDATE_SPACE.TWENTIETH)
                listTwentiethComponents.Add(component);
            else if (space == UPDATE_SPACE.TENTH)
                listTenthComponents.Add(component);
            else if (space == UPDATE_SPACE.HALF_SECOND)
                listHalfSecondComponents.Add(component);
            else if (space == UPDATE_SPACE.SECOND)
                listSecondComponents.Add(component);
            else if (space == UPDATE_SPACE.MINUTE)
                listMinuteComponents.Add(component);
        }
        public void AddAlwaysUpdateComponent(IUpdateComponent component, UPDATE_SPACE space)
        {
            if (space == UPDATE_SPACE.NONE)
                alwayslistNoneComponents.Add(component);
            else if (space == UPDATE_SPACE.TWENTIETH)
                alwayslistTwentiethComponents.Add(component);
            else if (space == UPDATE_SPACE.TENTH)
                alwayslistTenthComponents.Add(component);
            else if (space == UPDATE_SPACE.HALF_SECOND)
                alwayslistHalfSecondComponents.Add(component);
            else if (space == UPDATE_SPACE.SECOND)
                alwayslistSecondComponents.Add(component);
            else if (space == UPDATE_SPACE.MINUTE)
                alwayslistMinuteComponents.Add(component);
        }
        public void DelUpdateComponent(IUpdateComponent component)
        {
            listRemoveComponents.Add(component);
        }
        public void Clear()
        {
            listNoneComponents.Clear();
            listTwentiethComponents.Clear();
            listTenthComponents.Clear();
            listHalfSecondComponents.Clear();
            listSecondComponents.Clear();
            listMinuteComponents.Clear();
        }
        public void update(float deltaTime, float fixedDeltaTime, float realDeltaTime)
        {
            if (listRemoveComponents.Count > 0)
            {
                for (int i = 0; i < listRemoveComponents.Count; ++i)
                {
                    listNoneComponents.Remove(listRemoveComponents[i]);
                    listTwentiethComponents.Remove(listRemoveComponents[i]);
                    listTenthComponents.Remove(listRemoveComponents[i]);
                    listHalfSecondComponents.Remove(listRemoveComponents[i]);
                    listSecondComponents.Remove(listRemoveComponents[i]);
                    listMinuteComponents.Remove(listRemoveComponents[i]);

                    alwayslistNoneComponents.Remove(listRemoveComponents[i]);
                    alwayslistTwentiethComponents.Remove(listRemoveComponents[i]);
                    alwayslistTenthComponents.Remove(listRemoveComponents[i]);
                    alwayslistHalfSecondComponents.Remove(listRemoveComponents[i]);
                    alwayslistSecondComponents.Remove(listRemoveComponents[i]);
                    alwayslistMinuteComponents.Remove(listRemoveComponents[i]);
                }
                listRemoveComponents.Clear();
            }
            noneTimes += realDeltaTime;
            if (noneTimes > noneSpace)
            {
                while (alwayslistNoneComponents.Remove(null)) { }
                for (int i = 0; i < alwayslistNoneComponents.Count; ++i)
                {
                    alwayslistNoneComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }
                while (listNoneComponents.Remove(null)) { }
               // listNoneComponents.removeAll(Collections.singleton(null));
                for (int i = 0; i < listNoneComponents.Count; ++i)
                {
                    if(null != listNoneComponents[i])
                        listNoneComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }
                noneTimes %= noneSpace;
            }

            twentiethTimes += realDeltaTime;
            if (twentiethTimes > twentiethSpace)
            {
                while (alwayslistTwentiethComponents.Remove(null)) { }
                for (int i = 0; i < alwayslistTwentiethComponents.Count; ++i)
                {
                    alwayslistTwentiethComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }

                while (listTwentiethComponents.Remove(null)) { };
                for (int i = 0; i < listTwentiethComponents.Count; ++i)
                {
                    listTwentiethComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }
                twentiethTimes %= twentiethSpace;
            }
            tenthTimes += realDeltaTime;
            if (tenthTimes > tenthSpace)
            {
                while (alwayslistTenthComponents.Remove(null)) { }
                for (int i = 0; i < alwayslistTenthComponents.Count; ++i)
                {
                    alwayslistTenthComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }

                while (listTenthComponents.Remove(null)) { };
                for (int i = 0; i < listTenthComponents.Count; ++i)
                {
                    listTenthComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }
                tenthTimes %= tenthSpace;
            }
            halfSecondTimes += realDeltaTime;
            if (halfSecondTimes > halfSecondSpace)
            {
                while (alwayslistHalfSecondComponents.Remove(null)) { }
                for (int i = 0; i < alwayslistHalfSecondComponents.Count; ++i)
                {
                    alwayslistHalfSecondComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }

                while (listHalfSecondComponents.Remove(null)) { };
                for (int i = 0; i < listHalfSecondComponents.Count; ++i)
                {
                    listHalfSecondComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }
                halfSecondTimes %= halfSecondSpace;
            }
            secondTimes += realDeltaTime;
            if (secondTimes > secondSpace)
            {
                while (alwayslistSecondComponents.Remove(null)) { }
                for (int i = 0; i < alwayslistSecondComponents.Count; ++i)
                {
                    alwayslistSecondComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }

                while (listSecondComponents.Remove(null)) { };
                for (int i = 0; i < listSecondComponents.Count; ++i)
                {
                    listSecondComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }
                secondTimes %= secondSpace;
            }
            minuteTimes += realDeltaTime;
            if (minuteTimes > minuteSpace)
            {
                while (alwayslistMinuteComponents.Remove(null)) { }
                for (int i = 0; i < alwayslistMinuteComponents.Count; ++i)
                {
                    alwayslistMinuteComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }

                while (listMinuteComponents.Remove(null)) { };
                for (int i = 0; i < listMinuteComponents.Count; ++i)
                {
                    listMinuteComponents[i].UpdateM(deltaTime, fixedDeltaTime, realDeltaTime);
                }
                minuteTimes %= minuteSpace;
            }
        }
    }
}

