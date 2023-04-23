using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager {


    public static void Register()
    {
#if UNITY_IOS
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert);
#elif UNITY_ANDROID

#endif
    }
    public static void NotificationMessage(string message, int hour, bool isRepeatDay)
    {
        System.DateTime cd = System.DateTime.Now;
        
        if (hour <= System.DateTime.Now.Hour)
        {
            cd = System.DateTime.Now.AddDays(1);
        }
        System.DateTime newDate = new System.DateTime(cd.Year, cd.Month, cd.Day, hour, 0, 0);
        NotificationMessage(message, newDate, isRepeatDay);
    }

    public static void NotificationMessage(string message, int addSeconds)
    {
        System.DateTime cd = System.DateTime.Now.AddSeconds(addSeconds);
        System.DateTime newDate = new System.DateTime(cd.Year, cd.Month, cd.Day, cd.Hour, cd.Minute, cd.Second);
        NotificationMessage(message, newDate, false);
    }
    //本地推送 你可以传入一个固定的推送时间
    public static void NotificationMessage(string message, System.DateTime newDate, bool isRepeatDay)
    {
        //推送时间需要大于当前时间
        if (newDate > System.DateTime.Now)
        {
#if UNITY_IOS
            UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
            localNotification.fireDate = newDate;
            localNotification.alertBody = message;
            localNotification.applicationIconBadgeNumber = 1;
            localNotification.hasAction = true;
            if (isRepeatDay)
            {
                //是否每天定期循环
                localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.GregorianCalendar;
                localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
            }
            localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
#elif UNITY_ANDROID
            if (InitNotificator())
            {
                Debug.Log("InitNotificator true:"+ (newDate - System.DateTime.Now).TotalSeconds);
                m_ANObj.CallStatic(
                    "ShowNotification",
                    Application.productName,
                    "",
                    message,
                    (int)(newDate -System.DateTime.Now).TotalSeconds,
                    isRepeatDay);
            }
#endif
        }
    }

    //清空所有本地消息
    public static void CleanNotification()
    {
        Debug.Log("CleanNotification");
#if UNITY_IOS
        UnityEngine.iOS.LocalNotification l = new UnityEngine.iOS.LocalNotification();
        l.applicationIconBadgeNumber = -1;
        UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(l);
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#elif UNITY_ANDROID
        if (InitNotificator())
        {
            m_ANObj.CallStatic("ClearNotification");
        }
#endif
    }


#if UNITY_ANDROID
    private static AndroidJavaObject m_ANObj = null;
    private static bool InitNotificator()
    {
        if (m_ANObj == null)
        {
            try
            {
                m_ANObj = new AndroidJavaObject("com.sdk.AndroidNotificator");
            }
            catch
            {
                return false;
            }
        }
        return m_ANObj != null;
    }
#endif
}
