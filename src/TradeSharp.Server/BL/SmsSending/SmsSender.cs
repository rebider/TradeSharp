﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradeSharp.Contract.Util.Proxy;
using TradeSharp.Server.Contract;
using TradeSharp.Util;

namespace TradeSharp.Server.BL
{
    public class SmsSender
    {
        const String sendUrl = "http://sms.ru/sms/send";

        protected Thread routineThread;
        protected volatile bool isStopped;

        private ISmsSenderSettings settings;

        public void Start()
        {
            isStopped = false;
            routineThread = new Thread(SmsSendRoutine);
            routineThread.Start();
        }

        public virtual void Stop()
        {
            isStopped = true;
            routineThread.Join();
        }

        private void SmsSendRoutine()
        {
            while (!isStopped)
            {
                DateTime now = DateTime.Now;
                DateTime tomorrow = now.AddDays(1).Date.AddHours(settings.SendHour);
                int nextSendTimeSpan = (int)(tomorrow - now).TotalMilliseconds;

                if (nextSendTimeSpan < 0) //Эта ошибка возможна, если значение в TotalMilliseconds слишком большой для Int
                {
                    Logger.Error("SmsSender. Рассылка SMS уведомлений: время до следующей рассылки меньше нуля.");
                    isStopped = true;
                    return;
                }

                Thread.Sleep(nextSendTimeSpan);

                if (settings.IsPaused)
                    continue;

                int accountId = 951;//GetUserFullInfo

                //var f = TradeSharp.Contract.Util.Proxy.PlatformManager.Instance.proxy.GetUsersFullInfo()
                //PlatformManager pm = new PlatformManager();
               // TradeSharpDictionary.Instance.proxy.GetAllPlatformUsers();
                //ManagerAccount.Instance.GetAccountInfo
            }
        }

        private void Send(String from, Dictionary<String, String> to)
        {
            if (to.Count < 1)
                throw new ArgumentNullException("to", "Неверные входные данные - массив пуст.");
            if (to.Count > 100)
                throw new ArgumentOutOfRangeException("to", "Неверные входные данные - слишком много элементов (больше 100) в массиве.");

            String auth = String.Empty;
            String parameters = String.Empty;
            String answer = String.Empty;

            try
            {
                auth = String.Format("api_id={0}", settings.ApiId);

                foreach (var item in to)
                {
                    String recipient = item.Key;
                    parameters = String.Format("{0}&to={1}&text={2}&from={3}", auth, recipient, item.Value, from);

                    WebRequest request = WebRequest.Create(sendUrl);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Method = "POST";
                    Byte[] bytes = Encoding.UTF8.GetBytes(parameters);
                    request.ContentLength = bytes.Length;
                    Stream os = request.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);
                    os.Close();

                    using (WebResponse resp = request.GetResponse())
                    {
                        if (resp == null)
                            return;

                        using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                        {
                            answer = sr.ReadToEnd().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка SMS рассылки", ex);
            }
        }
    }
}