using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using MGF.QOLMS.QolmsJotoWebView.Workers;
using MGF.QOLMS.QolmsKaradaKaruteApiCoreV1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView
{
    internal sealed class IntegrationTanitaConnectionWorker
    {

        IKaradaKaruteApiRepository _karadaKaruteApiRepo;
        ITanitaConnectionRepository _tanitaRepo;

        #region Constructor

        /// <summary>
        /// デフォルト コンストラクタは使用できません。
        /// </summary>
        public IntegrationTanitaConnectionWorker(IKaradaKaruteApiRepository karadaKaruteApiRepository, ITanitaConnectionRepository tanitaConnectionRepository)
        {
            _karadaKaruteApiRepo = karadaKaruteApiRepository;
            _tanitaRepo = tanitaConnectionRepository;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// JOBを実行します。
        /// </summary>
        public bool ExecuteQolmsKaradaKaruteFirstJob(string jobUri, string jobUser, string jobPassword, string paramString)
        {
            try
            {
                var webreq = (HttpWebRequest)WebRequest.Create(jobUri + "?arguments=" + paramString);
                webreq.PreAuthenticate = true;
                webreq.Credentials = new NetworkCredential(jobUser, jobPassword);
                webreq.Method = "POST";
                webreq.ContentLength = 0;

                var webres = (HttpWebResponse)webreq.GetResponse();
                webres.Dispose();
            }
            catch (WebException exh)
            {
                DebugLog(exh.Message);

                var str = new StringBuilder();
                str.AppendLine(EncriptString(exh.Message));
                str.AppendLine(paramString);

                this.SendMail(str.ToString());
                AccessLogWorker.WriteAccessLog(null, string.Empty,
                    AccessLogWorker.AccessTypeEnum.Error, string.Format(exh.Message));

                return false;
            }
            catch (Exception ex)
            {
                DebugLog(ex.Message);

                var str = new StringBuilder();
                str.AppendLine(EncriptString(ex.Message));
                str.AppendLine(paramString);

                this.SendMail(str.ToString());
                AccessLogWorker.WriteAccessLog(null, string.Empty,
                    AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));

                return false;
            }

            return true;
        }

        private string EncriptString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                return crypt.EncryptString(str);
            }
        }

        private string DecriptString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            using (var crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                return crypt.DecryptString(str);
            }
        }

        private void SendMail(string message)
        {
            var br = new StringBuilder();
            br.AppendLine(string.Format("タニタ接続のエラーです。"));
            br.AppendLine(message);
            var task = NoticeMailWorker.SendAsync(br.ToString());
        }

        [Conditional("DEBUG")]
        public void DebugLog(string message)
        {
            try
            {
                string log = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "tanita.txt");
                File.AppendAllText(log, string.Format("{0}:{1}\r\n", DateTime.Now, message));
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Public Method

        public bool GetConnected(QolmsJotoModel mainModel, ref string id)
        {
            var apiResult = _tanitaRepo.ExecuteTanitaConnectionReadApi(
                mainModel, new List<string>() { QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString() });

            if (apiResult.LinkageItems.Count > 0)
            {
                id = apiResult.LinkageItems.First().LinkageSystemId;
                if (apiResult.LinkageItems.First().LinkageSystemNo == QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString() &&
                    apiResult.LinkageItems.First().StatusType == "2")
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetConnected(QolmsJotoModel mainModel)
        {
            var apiResult = _tanitaRepo.ExecuteTanitaConnectionReadApi(
                mainModel, new List<string>() { QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString() });

            if (apiResult.LinkageItems.Count == 0 ||
                string.IsNullOrWhiteSpace(apiResult.LinkageItems.First().LinkageSystemId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<DeviceItem> GetConnectedDevice(QolmsJotoModel mainModel)
        {
            var result = new List<DeviceItem>();
            var apiResult = _tanitaRepo.ExecuteTanitaConnectionReadApi(
                mainModel, new List<string>() { QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString() });

            result.Add(new DeviceItem() { DevicePropertyName = "BodyCompositionMeter", DeviceName = "体重" });
            result.Add(new DeviceItem() { DevicePropertyName = "Sphygmomanometer", DeviceName = "血圧" });
            result.Add(new DeviceItem() { DevicePropertyName = "Pedometer", DeviceName = "歩数" });

            foreach (var linkageItem in apiResult.LinkageItems)
            {
                if (linkageItem.LinkageSystemNo == QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString())
                {
                    if (!string.IsNullOrWhiteSpace(linkageItem.LinkageSystemId))
                    {
                        var linked = linkageItem.Devices.ConvertAll(i => byte.Parse(i));
                        foreach (var item in result)
                        {
                            var device = (QsKaradaKaruteApiDeviceTypeEnum)Enum.Parse(
                                typeof(QsKaradaKaruteApiDeviceTypeEnum), item.DevicePropertyName);
                            if (linked.Contains((byte)device))
                            {
                                item.Checked = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 連携している デバイス 情報の リスト を取得します。
        /// </summary>
        [Obsolete("実装中")]
        public List<DeviceItem> GetConnectedDevice(QolmsJotoModel mainModel, ref string refMemberNo)
        {
            refMemberNo = string.Empty;
            var result = new List<DeviceItem>();

            try
            {
                var apiResult = _tanitaRepo.ExecuteTanitaConnectionReadApi(
                    mainModel, new List<string>() { QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString() });

                result.AddRange(new[]
                {
                new DeviceItem()
                {
                    DeviceName = "体重",
                    DevicePropertyName = QsKaradaKaruteApiDeviceTypeEnum.BodyCompositionMeter.ToString(),
                    Checked = false
                },
                new DeviceItem()
                {
                    DeviceName = "血圧",
                    DevicePropertyName = QsKaradaKaruteApiDeviceTypeEnum.Sphygmomanometer.ToString(),
                    Checked = false
                },
                new DeviceItem()
                {
                    DeviceName = "歩数",
                    DevicePropertyName = QsKaradaKaruteApiDeviceTypeEnum.Pedometer.ToString(),
                    Checked = false
                }
            });

                if (apiResult.LinkageItems.Count == 1)
                {
                    var linkage = apiResult.LinkageItems.First();

                    if (linkage.LinkageSystemNo.TryToValueType(int.MinValue) == QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA &&
                        !string.IsNullOrWhiteSpace(linkage.LinkageSystemId))
                    {
                        refMemberNo = linkage.LinkageSystemId;

                        var devices = new HashSet<string>(
                            linkage.Devices.ConvertAll(i =>
                                i.TryToValueType(QsKaradaKaruteApiDeviceTypeEnum.None).ToString()));

                        result.ForEach(i => i.Checked = devices.Contains(i.DevicePropertyName));
                    }
                }
            }
            catch
            {
                refMemberNo = string.Empty;
                result = new List<DeviceItem>();
            }

            return result;
        }

        /// <summary>
        /// タニタ連携入力画面の情報を取得する
        /// </summary>
        public IntegrationTanitaConnectionViewModel CreateViewModel(QolmsJotoModel mainModel)
        {
            var result = new IntegrationTanitaConnectionViewModel();

            var apiResult = _tanitaRepo.ExecuteTanitaConnectionReadApi(
                mainModel, new List<string>() { QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString(), QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_NAVITIME.ToString() });

            result.Devices.Add(new DeviceItem() { DevicePropertyName = "BodyCompositionMeter", DeviceName = "体重" });
            result.Devices.Add(new DeviceItem() { DevicePropertyName = "Sphygmomanometer", DeviceName = "血圧" });
            result.Devices.Add(new DeviceItem() { DevicePropertyName = "Pedometer", DeviceName = "歩数" });

            foreach (var linkageItem in apiResult.LinkageItems)
            {
                if (linkageItem.LinkageSystemNo == QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString())
                {
                    result.ConnectionID = linkageItem.LinkageSystemId;

                    if (!string.IsNullOrWhiteSpace(result.ConnectionID))
                    {
                        var linked = linkageItem.Devices.ConvertAll(i => byte.Parse(i));
                        foreach (var item in result.Devices)
                        {
                            var device = (QsKaradaKaruteApiDeviceTypeEnum)Enum.Parse(
                                typeof(QsKaradaKaruteApiDeviceTypeEnum), item.DevicePropertyName);
                            if (linked.Contains((byte)device))
                            {
                                item.Checked = true;
                            }
                        }
                    }
                }
                //else
                //{
                //    if (linkageItem.LinkageSystemNo == QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_NAVITIME.ToString())
                //    {
                //        result.AlkooConnectedFlag = true;
                //    }
                //}
            }

            if (string.IsNullOrWhiteSpace(result.ConnectionID))
            {
                foreach (var item in result.Devices)
                {
                    item.Checked = true;
                }
            }

            return result;
        }

        /// <summary>
        /// タニタ連携入力画面の情報を取得する
        /// </summary>
        public bool ConnectionRegister(QolmsJotoModel mainModel,
            IntegrationTanitaConnectionViewModel inputModel, ref string message)
        {
            var apiResult = new MemberAuthApiResults();

            try
            {
                apiResult = _karadaKaruteApiRepo.ExecuteKaradaKaruteConnectionApi(
                    inputModel.ID, inputModel.Password);
            }
            catch (Exception ex)
            {
                message = "タニタとの連携の呼び出しに失敗しました。";
                return false;
            }

            string memberId = string.Empty;

            if (apiResult.status == "0" && !string.IsNullOrWhiteSpace(apiResult.member_no))
            {
                memberId = apiResult.member_no;
            }
            else
            {
                message = apiResult.message;
                return false;
            }

            inputModel.Devices.Add(new DeviceItem()
            {
                DevicePropertyName = "BodyCompositionMeter",
                DeviceName = "体重",
                Checked = inputModel.BodyCompositionMeter
            });
            inputModel.Devices.Add(new DeviceItem()
            {
                DevicePropertyName = "Sphygmomanometer",
                DeviceName = "血圧",
                Checked = inputModel.Sphygmomanometer
            });
            inputModel.Devices.Add(new DeviceItem()
            {
                DevicePropertyName = "Pedometer",
                DeviceName = "歩数",
                Checked = inputModel.Pedometer
            });

            var devices = new List<byte>();
            var tags = new List<string>();

            foreach (var item in inputModel.Devices)
            {
                if (item.Checked)
                {
                    var deviceEnum = (QsKaradaKaruteApiDeviceTypeEnum)Enum.Parse(
                        typeof(QsKaradaKaruteApiDeviceTypeEnum), item.DevicePropertyName);
                    devices.Add((byte)deviceEnum);

                    switch (deviceEnum)
                    {
                        case QsKaradaKaruteApiDeviceTypeEnum.BodyCompositionMeter:
                            tags.Add("6021");
                            break;
                        case QsKaradaKaruteApiDeviceTypeEnum.Sphygmomanometer:
                            tags.AddRange(new[] { "622E", "622F" });
                            break;
                        case QsKaradaKaruteApiDeviceTypeEnum.Pedometer:
                            tags.Add("6331");
                            break;
                    }
                }
            }

            var writerResult = new QhYappliPortalTanitaConnectionWriteApiResults();

            try
            {
                writerResult = _tanitaRepo.ExecuteTanitaConnectionWriteApi(
                    mainModel, inputModel, memberId, devices, tags, 2, false);
            }
            catch (Exception ex)
            {
                message = "IDまたはパスワードが不正です。";
                return false;
            }

            if (writerResult.IsSuccess.TryToValueType(false))
            {
                try
                {
                    var parameter = QolmsLibraryV1.QolmsKaradaKaruteFirstJobWorker.MakeParamString(
                        mainModel.AuthorAccount.AccountKey, QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA,
                        QolmsLibraryV1.QhDeviceTypeEnum.BodyCompositionMeter |
                        QolmsLibraryV1.QhDeviceTypeEnum.Sphygmomanometer |
                        QolmsLibraryV1.QhDeviceTypeEnum.Pedometer);

                    var result = this.ExecuteQolmsKaradaKaruteFirstJob(
                        QjConfiguration.TanitaConnectionFirstJob, QjConfiguration.TanitaConnectionFirstJobAccount, QjConfiguration.TanitaConnectionFirstJobPassword, parameter);
                }
                catch (Exception ex)
                {
                    this.SendMail(ex.Message);
                    AccessLogWorker.WriteAccessLog(null, string.Empty,
                        AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));
                }

                try
                {
                    // ポイント付与のコメント化された処理
                }
                catch (Exception ex)
                {
                    this.SendMail(EncriptString(ex.Message));
                    AccessLogWorker.WriteAccessLog(null, string.Empty,
                        AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));
                }
            }

            return writerResult.IsSuccess.TryToValueType(false);
        }

        /// <summary>
        /// 連携を開始
        /// </summary>
        public List<byte> DeviceRegister(QolmsJotoModel mainModel, string deviceName, bool @checked)
        {
            var result = new IntegrationTanitaConnectionViewModel();

            var device = (byte)(QsKaradaKaruteApiDeviceTypeEnum)Enum.Parse(
                typeof(QsKaradaKaruteApiDeviceTypeEnum), deviceName);

            var tags = new List<string>();

            switch ((QsKaradaKaruteApiDeviceTypeEnum)device)
            {
                case QsKaradaKaruteApiDeviceTypeEnum.BodyCompositionMeter:
                    tags.Add("6021");
                    break;
                case QsKaradaKaruteApiDeviceTypeEnum.Sphygmomanometer:
                    tags.AddRange(new[] { "622E", "622F" });
                    break;
                case QsKaradaKaruteApiDeviceTypeEnum.Pedometer:
                    tags.Add("6331");
                    break;
            }

            var writerResult = _tanitaRepo.ExecuteTanitaConnectionUpdateWriteApi(
                mainModel, device, tags, @checked);

            return writerResult.Devices.ConvertAll(i => byte.Parse(i));
        }

        /// <summary>
        /// 連携を解除する
        /// </summary>
        public bool Cancel(QolmsJotoModel mainModel, ref string message)
        {
            var apiResult = _tanitaRepo.ExecuteTanitaConnectionReadApi(
                mainModel, new List<string>() { QjLinkageSystemNo.JOTO_LINKAGE_SYSTEM_NO_TANITA.ToString() });

            var cancelResult = new MemberAuthApiResults();

            try
            {
                cancelResult = _karadaKaruteApiRepo.ExecuteKaradaKaruteCancel(
                    apiResult.LinkageItems.First().LinkageSystemId);
            }
            catch (Exception ex)
            {
                this.SendMail(EncriptString(ex.Message));
                AccessLogWorker.WriteAccessLog(null, string.Empty,
                    AccessLogWorker.AccessTypeEnum.Error, string.Format(ex.Message));

                return false;
            }

            if (cancelResult.status == "0")
            {
                var writerResult = _tanitaRepo.ExecuteTanitaConnectionWriteApi(
                    mainModel, new IntegrationTanitaConnectionViewModel(), string.Empty,
                    new List<byte>(), new List<string>(), 2, true);

                return writerResult.IsSuccess.TryToValueType(false);
            }
            else
            {
                message = cancelResult.message;
                return false;
            }
        }

        #endregion
    }
}
