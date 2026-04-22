using System;
using System.Collections.Generic;
using System.Linq;
using MGF.QOLMS.QolmsCryptV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsOpenApi.Extension;
using MGF.QOLMS.QolmsJwtAuthCore;
using MGF.QOLMS.QolmsOpenApi.Enums;
using MGF.QOLMS.QolmsOpenApi.Sql;
using System.Text.RegularExpressions;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
   
    /// <summary>
    /// 子アカウント関連
    /// </summary>
    /// <remarks></remarks>
    public sealed class AccountFamilyWorker
    {
        #region "Private Method"
        private AccountFamilyWorker()
        {
        }

        /// <summary>
        /// アカウントキーを指定して、
        /// アカウントマスタテーブルエンティティを取得します。
        /// </summary>
        /// <param name="accountKey">アカウントキー。</param>
        /// <param name="includeDelete">
        /// 削除済みも対象にするかのフラグ（オプショナル）。
        /// 対象にするなら True、
        /// 対象にしない False（デフォルト）を指定します。
        /// </param>
        /// <returns>
        /// マスタが存在するなら値がセットされたテーブルエンティティ、
        /// 存在しないなら Nothing。
        /// </returns>
        /// <remarks></remarks>
        private static QH_ACCOUNT_MST ReadAccountEntity(Guid accountKey, bool includeDelete = false)
        {
            QH_ACCOUNT_MST entity = new QH_ACCOUNT_MST() { ACCOUNTKEY = accountKey };
            QhAccountEntityReader reader = new QhAccountEntityReader();
            QhAccountEntityReaderArgs readerArgs = new QhAccountEntityReaderArgs() { Data = new List<QH_ACCOUNT_MST>() { entity } };
            QhAccountEntityReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults.IsSuccess && readerResults.Result.Count == 1 && (includeDelete | !readerResults.Result.First().DELETEFLAG))
                return readerResults.Result.First();
            else
                return null;
            
        }
        

        /// <summary>
        /// アカウントキーからAccountIndexエンティティを取得
        /// </summary>
        /// <param name="accountKey"></param>
        /// <returns></returns>
        private static QH_ACCOUNTINDEX_DAT ReadAccountIndexEntity(Guid accountKey)
        {
            if (accountKey == Guid.Empty)
                throw new ArgumentNullException("accountKey", "アカウントキーが不正です。");

            QH_ACCOUNTINDEX_DAT result = new QH_ACCOUNTINDEX_DAT();

            QhAccountIndexEntityReader reader = new QhAccountIndexEntityReader();
            QhAccountIndexEntityReaderArgs readerArgs = new QhAccountIndexEntityReaderArgs() { Data =new List<QH_ACCOUNTINDEX_DAT>() { new QH_ACCOUNTINDEX_DAT() { ACCOUNTKEY = accountKey } } };
            QhAccountIndexEntityReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Count == 1 && readerResults.Result.First().IsKeysValid())
                result = readerResults.Result.First();
            
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="entity"></param>
        /// <param name="publicAccountKey"></param>
        /// <returns></returns>
        private static QoApiAccountFamilyViewItem BuildAccountFamilyViewItem(string executor,QH_ACCOUNTINDEX_DAT entity, Guid publicAccountKey = default)
        {
            QoApiAccountFamilyViewItem result = new QoApiAccountFamilyViewItem();

            if (entity == null || !entity.IsKeysValid())
                throw new ArgumentNullException("entity", "アカウントインデックスエンティティクラスが不正です。");

            Guid publicKey = Guid.Empty ;
            if (publicAccountKey != default && publicAccountKey != entity.ACCOUNTKEY)
                publicKey = publicAccountKey;

            var tokenProvider = new QsJwtTokenProvider();
            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                var encExeCutor = crypt.EncryptString(executor.TryToValueType(Guid.Empty).ToString("N"));
                result = new QoApiAccountFamilyViewItem()
                {
                    AccountKeyReference = entity.ACCOUNTKEY.ToEncrypedReference(),
                    FamilyName = string.IsNullOrWhiteSpace(entity.FAMILYNAME) ? string.Empty : crypt.DecryptString(entity.FAMILYNAME),
                    GivenName = string.IsNullOrWhiteSpace(entity.GIVENNAME) ? string.Empty : crypt.DecryptString(entity.GIVENNAME),
                    FamilyNameKana = string.IsNullOrWhiteSpace(entity.FAMILYKANANAME) ? string.Empty : crypt.DecryptString(entity.FAMILYKANANAME),
                    GivenNameKana = string.IsNullOrWhiteSpace(entity.GIVENKANANAME) ? string.Empty : crypt.DecryptString(entity.GIVENKANANAME),
                    Sex = entity.SEXTYPE.ToString(),
                    Birthday = entity.BIRTHDAY.ToApiDateString(),
                    AccessKey = publicKey == Guid.Empty ? string.Empty : tokenProvider.CreateOpenApiJwtAccessKey(encExeCutor, entity.ACCOUNTKEY, publicKey, (int)QoApiFunctionTypeEnum.All),
                    PersonPhotoReference = entity.PHOTOKEY == Guid.Empty ? string.Empty : entity.PHOTOKEY.ToEncrypedReference(),
                    QolmsSsoAccessKey = tokenProvider.CreateQolmsJwtSsoKey(encExeCutor, entity.ACCOUNTKEY, publicKey, 1, DateTime.Now.AddMinutes(2))
                };
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static QoApiFileItem GetPersonPhoto(QoAccountFamilyPersonPhotoReadApiArgs args)
        {
            // TODO 念のためQH_ACCOUNTINDEX_DAT.PHOTOKEYと一致するかチェックしたほうが良い？

            QoApiFileItem result = new QoApiFileItem() { Sequence = "0", FileKeyReference = args.PersonPhotoReference };

            if (!string.IsNullOrWhiteSpace(args.PersonPhotoReference))
            {
                Guid fileKey = args.PersonPhotoReference.ToDecrypedReference<Guid>();

                if (fileKey != Guid.Empty)
                {
                    QhBlobStorageReadApiResults apiResults = QoBlobStorage.Read<QH_UPLOADFILE_DAT>(new QhBlobStorageReadApiArgs()
                    {
                        ActorKey = args.ActorKey,
                        ApiType = QhApiTypeEnum.FileStorageRead.ToString(),
                        ExecuteSystemType = args.ExecuteSystemType,
                        Executor = args.Executor,
                        ExecutorName = args.ExecutorName,
                        FileKey = fileKey.ToApiGuidString(),
                        FileRelationType = QsApiFileRelationTypeEnum.PersonPhoto.ToString(),
                        FileType = QsApiFileTypeEnum.Thumbnail.ToString()
                    }   );

                    if (apiResults != null && apiResults.IsSuccess == bool.TrueString && !string.IsNullOrWhiteSpace(apiResults.Data) && !string.IsNullOrWhiteSpace(apiResults.ContentType))
                    {
                        result.OriginalName = apiResults.OriginalName;
                        result.ContentType = apiResults.ContentType;
                        result.Data = apiResults.Data;
                    }
                }
            }

            return result;
        }

        private static QoAccountFamilyPersonPhotoWriteApiResults SetPersonPhoto(QoAccountFamilyPersonPhotoWriteApiArgs args)
        {
            if (args.PersonPhoto == null)
                throw new ArgumentNullException("PersonPhoto", "利用者画像情報が不正です。");
            if (string.IsNullOrWhiteSpace(args.PersonPhoto.ContentType))
                throw new ArgumentNullException("PersonPhoto.ContentType", "利用者画像MIMEタイプが不正です。");
            if (string.IsNullOrWhiteSpace(args.PersonPhoto.Data))
                throw new ArgumentNullException("PersonPhoto.Data", "利用者画像データが不正です。");

            QoAccountFamilyPersonPhotoWriteApiResults result = new QoAccountFamilyPersonPhotoWriteApiResults() { IsSuccess = bool.FalseString };

            if (args != null && args.PersonPhoto != null)
            {
                QhBlobStorageWriteApiResults apiResults = QoBlobStorage.Write<QH_UPLOADFILE_DAT>(new QhBlobStorageWriteApiArgs()
                {
                    ActorKey = args.ActorKey,
                    ApiType = QhApiTypeEnum.FileStorageWrite.ToString(),
                    ExecuteSystemType = args.ExecuteSystemType,
                    Executor = args.Executor,
                    ExecutorName = args.ExecutorName,
                    OriginalName = args.PersonPhoto.OriginalName,
                    ContentType = args.PersonPhoto.ContentType,
                    Data = args.PersonPhoto.Data,
                    FileRelationType = QsApiFileRelationTypeEnum.PersonPhoto.ToString()
                } );

                if (apiResults != null && apiResults.IsSuccess == bool.TrueString && !string.IsNullOrWhiteSpace(apiResults.FileKey))
                {
                    Guid fileKey = apiResults.FileKey.TryToValueType<Guid>(Guid.Empty);
                    if (fileKey != Guid.Empty)
                    {
                        result.PersonPhotoReference = fileKey.ToEncrypedReference();
                        result.IsSuccess = bool.TrueString;
                        result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    }
                }
            }

            return result;
        }

        private static bool UpdatePhotoKey(Guid accountKey, Guid fileKey)
        {
            bool result = false;

            // データベーステーブルから、該当する列をセレクト
            QH_ACCOUNTINDEX_DAT entity = new QH_ACCOUNTINDEX_DAT() { ACCOUNTKEY = accountKey };

            QhAccountIndexEntityReader indexReader = new QhAccountIndexEntityReader();
            QhAccountIndexEntityReaderArgs indexReaderArgs = new QhAccountIndexEntityReaderArgs() { Data = new List<QH_ACCOUNTINDEX_DAT>(){ entity }};
            QhAccountIndexEntityReaderResults indexReaderResults = QsDbManager.Read(indexReader, indexReaderArgs);

            if (indexReaderResults.Result.Count == 1)
            {
                entity = indexReaderResults.Result[0];
                entity.PHOTOKEY = fileKey;
                entity.UPDATEDDATE = DateTime.Now;
                entity.DataState = QsDbEntityStateTypeEnum.Modified;
                
                QhAccountIndexEntityWriter indexWriter = new QhAccountIndexEntityWriter();
                QhAccountIndexEntityWriterArgs indexWriterArgs = new QhAccountIndexEntityWriterArgs() { Data = new List<QH_ACCOUNTINDEX_DAT>() { entity } };
                QhAccountIndexEntityWriterResults indexWriterResults = QsDbManager.Write(indexWriter, indexWriterArgs);

                if (indexWriterResults.IsSuccess && indexWriterResults.Result == 1)
                    result = true;
                else
                    throw new InvalidOperationException("QH_ACCOUNTINDEX_DATテーブルへの登録に失敗しました。");
                
            }

            // ' IdentityApiに投げる -> 403の原因がぱっと分からなかったので保留
            // Dim apiArgs As New QiQolmsAccountConnectFamilyAccountEditWriteApiArgs(
            // QiApiTypeEnum.QolmsAccountConnectFamilyAccountEditWrite,
            // QsApiSystemTypeEnum.Qolms,
            // args.ActorKey.TryToValueType(Of Guid)(Guid.Empty),
            // args.ExecutorName
            // ) With {
            // .ActorKey = args.ActorKey,
            // .PhotoKey = fileKey.ToApiGuidString()
            // }

            // Dim apiResults As QiQolmsAccountConnectFamilyAccountEditWriteApiResults = QsApiManager.ExecuteQolmsIdentityApi(Of QiQolmsAccountConnectFamilyAccountEditWriteApiResults)(apiArgs, "qolmsopenapi", Guid.NewGuid())

            // If apiResults IsNot Nothing Then
            // With apiResults
            // result = .IsSuccess = Boolean.TrueString
            // End With
            // End If

            return result;
        }

        private static void UpdateMetadata(Guid fileKey, Guid accountKey)
        {
            bool result = QoBlobStorage.UpdateImageMetaDataAccountKey(fileKey, accountKey);
            if (result == false)
                QoAccessLog.WriteAccessLog(null, QsApiSystemTypeEnum.QolmsOpenApi, accountKey, DateTime.Now, QoAccessLog.AccessTypeEnum.Api, string.Empty, string.Format("Blobメタデータ更新に失敗しました(filekey:{0}/accountKey:{1})", fileKey, accountKey) );
        }

        private static Guid AddPrivateAccount(QoAccountFamilyAddApiArgs args)
        {
            Guid result = Guid.Empty;

            QsDbSexTypeEnum sex = QsDbSexTypeEnum.None;
            DateTime birthday = DateTime.MinValue;

            sex = (QsDbSexTypeEnum)byte.Parse(args.Account.Sex);
            if (!Enum.IsDefined(typeof(QsDbSexTypeEnum), sex))
                throw new ArgumentOutOfRangeException("Account.Sex", "性別が不正です。");

            birthday = args.Account.Birthday.TryToValueType<DateTime>(DateTime.MinValue);
            if (birthday == DateTime.MinValue)
                throw new ArgumentOutOfRangeException("Account.Birthday", "生年月日が不正です。");

            // IdentityApiに投げる
            // Identity側のExecutorのチェックでログイン可能かアカウントかどうかをチェックしているので、ActorKey（親アカウントキー）をセット
            QiQolmsAccountConnectFamilyAccountEditWriteApiResults apiResults = QoIdentityClient.ExecuteAccountConnectFamilyAccountEditWriteApi(args.ActorKey.ToValueType<Guid>(), args.ExecutorName, args.Account, sex.ToString(), new DateTime(birthday.Year, birthday.Month, birthday.Day).ToApiDateString());
            if (apiResults != null && apiResults.IsSuccess == bool.TrueString)
                result = apiResults.Accountkey;
                

            return result;
        }
#endregion
        internal static QH_CONTACT_DAT ReadContactEntity(Guid accountKey, bool includeDelete = false)
        {
            QH_CONTACT_DAT result = null;

            QhContactEntityReader reader = new QhContactEntityReader();
            QhContactEntityReaderArgs readerArgs = new QhContactEntityReaderArgs() { Data =  new List<QH_CONTACT_DAT>() { new QH_CONTACT_DAT() { ACCOUNTKEY = accountKey } } };
            QhContactEntityReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults != null && readerResults.IsSuccess && readerResults.Result != null && readerResults.Result.Any())
            {
                if (includeDelete | !readerResults.Result.First().DELETEFLAG)
                    result = readerResults.Result.First();
            }

            return result;
        }


        // TODO: 今後の修正でここを復活させて 連絡手帳に電話番号を入れる場合は、以下の修正が必要になります。
        // ・QhContactSetOfJsonは初期値を入れる必要があります。以下のようなJSONがQOLMSで想定されています（Enum系の値が入ること)。
        // {"Address1":"","Address2":"","AllergyN":[],"AttachedFileN":[],"BloodType":"None","CareLevelType":"None","DisabilityN":[],"DiseaseN":[],"EmergencyContactN":[],"Floor":"0","HasElevator":"False","InsuranceTypeN":[],"IsAboveground":"True","MedicineN":[],"PhoneN":[{"PhoneNumber":"00-1111-2222","PhoneType":"Home"}],"PostalCode":"","PrefNo":"0","RhType":"None","SurgeryN":[]}
        // ・電話番号は、QOLMS側では種別の別なく3つまでしか想定されてません。下記コードのままだと4つ目にも入る可能性があり、QOLMSで編集されると消えてしまいます。
        // 別種別にして触らないようにしてもらうなどQOLMSと協議する（QOLMS側も修正する）必要があります。
        // ・QOLMS側の電話番号はハイフンが入っているのしか許されてませんが、お薬手帳アプリではハイフンなしのみ、OpenApiではどちらも許容となっています。これの統一もQOLMS側と調整する必要ありです。
        // Friend Shared Function WriteContactEntity(accountKey As Guid, tel As String) As Boolean

        // Dim result As Boolean = False

        // 'If accountKey = Guid.Empty OrElse String.IsNullOrWhiteSpace(tel) Then Return False
        // If accountKey = Guid.Empty Then Return False

        // Dim now As Date = Date.Now
        // Dim entity As QH_CONTACT_DAT = AccountFamilyWorker.ReadContactEntity(accountKey)

        // If entity IsNot Nothing AndAlso entity.IsKeysValid() Then
        // entity.UPDATEDDATE = now
        // entity.DataState = QsDbEntityStateTypeEnum.Modified
        // Else
        // entity = New QH_CONTACT_DAT() With {.ACCOUNTKEY = accountKey, .CONTACTSET = String.Empty, .DELETEFLAG = False, .CREATEDDATE = now, .UPDATEDDATE = now}
        // entity.DataState = QsDbEntityStateTypeEnum.Added
        // End If

        // ' 電話番号をJsonに格納する（暗号化対象）
        // Dim json As QhContactSetOfJson = Nothing
        // Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)

        // Dim ser As New QsJsonSerializer()

        // If Not String.IsNullOrWhiteSpace(entity.CONTACTSET) Then
        // json = ser.Deserialize(Of QhContactSetOfJson)(crypt.DecryptString(entity.CONTACTSET))
        // Else
        // json = New QhContactSetOfJson()
        // End If

        // If String.IsNullOrWhiteSpace(tel) Then
        // '既にあれば削除
        // If json.PhoneN IsNot Nothing AndAlso json.PhoneN.Any() Then
        // Dim index As Integer = json.PhoneN.FindIndex(Function(i) i.PhoneType = QsDbPhoneTypeEnum.Home.ToString())

        // If index >= 0 Then json.PhoneN.RemoveAt(index)
        // Else
        // 'ないので更新不要
        // Return True
        // End If
        // Else
        // 'なければ追加、あれば変更
        // If json.PhoneN IsNot Nothing AndAlso json.PhoneN.Any() Then
        // Dim index As Integer = json.PhoneN.FindIndex(Function(i) i.PhoneType = QsDbPhoneTypeEnum.Home.ToString())

        // If index >= 0 Then
        // json.PhoneN(index).PhoneNumber = tel
        // Else
        // json.PhoneN.Add(New QhPhoneOfJson() With {.PhoneType = QsDbPhoneTypeEnum.Home.ToString(), .PhoneNumber = tel})
        // End If
        // Else
        // json.PhoneN = New List(Of QhPhoneOfJson)()
        // json.PhoneN.Add(New QhPhoneOfJson() With {.PhoneType = QsDbPhoneTypeEnum.Home.ToString(), .PhoneNumber = tel})
        // End If
        // End If

        // entity.CONTACTSET = crypt.EncryptString(ser.Serialize(json))

        // End Using

        // Dim writer As New QhContactEntityWriter()
        // Dim writerArgs As New QhContactEntityWriterArgs() With {.Data = {entity}.ToList()}
        // Dim writerResults As QhContactEntityWriterResults = QsDbManager.Write(writer, writerArgs)

        // If writerResults IsNot Nothing Then
        // With writerResults
        // If .IsSuccess AndAlso .Result = 1 Then
        // result = True
        // End If
        // End With
        // End If

        // Return result

        // End Function

        internal static string GetPhoneNumber(QH_CONTACT_DAT entity, QsCrypt cryptor)
        {
            string result = string.Empty;

            if (entity != null && entity.IsKeysValid())
            {
                if (!string.IsNullOrWhiteSpace(entity.CONTACTSET))
                {
                    QhContactSetOfJson json = new QsJsonSerializer().Deserialize<QhContactSetOfJson>(cryptor.DecryptString(entity.CONTACTSET));
                    if (json != null && json.PhoneN != null && json.PhoneN.Any())
                    {
                        QhPhoneOfJson item = json.PhoneN.Find(i => i.PhoneType == QsDbPhoneTypeEnum.Home.ToString());
                        if (item != null)
                            result = item.PhoneNumber;
                    }
                }
            }

            return result;
        }

        #region "Public Method

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。")]
        public static QoAccountFamilyListReadApiResults ListRead(QoAccountFamilyListReadApiArgs args)
        {
            QoAccountFamilyListReadApiResults result = new QoAccountFamilyListReadApiResults() { IsSuccess = bool.FalseString };
            Guid publicAccountKey = Guid.Parse(args.ActorKey);

            using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
            {
                
                result.AccountN = AccountFamilyWorker.ReadAccountFamily(publicAccountKey).ConvertAll(i => AccountFamilyWorker.BuildAccountFamilyViewItem(args.Executor ,i, publicAccountKey));
                foreach (QoApiAccountFamilyViewItem account in result.AccountN)
                {
                    QH_CONTACT_DAT entity = AccountFamilyWorker.ReadContactEntity(account.AccountKeyReference.ToDecrypedReference<Guid>());

                    // 電話番号を（強引に）追加
                    if (entity != null && entity.IsKeysValid())
                    {
                        if (!string.IsNullOrWhiteSpace(entity.CONTACTSET))
                        {
                            QhContactSetOfJson json = new QsJsonSerializer().Deserialize<QhContactSetOfJson>(crypt.DecryptString(entity.CONTACTSET));
                            if (json != null && json.PhoneN != null && json.PhoneN.Any())
                            {
                                QhPhoneOfJson item = json.PhoneN.Find(i => i.PhoneType == QsDbPhoneTypeEnum.Home.ToString());
                                if (item != null)
                                    account.Tel = item.PhoneNumber;
                            }
                        }
                    }
                }
                result.IsSuccess = bool.TrueString;
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                
            }

            return result;
        }

        /// <summary>
        /// 子アカウント追加処理します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。")]
        public static QoAccountFamilyAddApiResults Add(QoAccountFamilyAddApiArgs args)
        {
            QoAccountFamilyAddApiResults result = new QoAccountFamilyAddApiResults() { IsSuccess = bool.FalseString };
            List<string> errorList = new List<string>();
            if (args.Account == null)
                throw new ArgumentNullException("アカウント情報が不正です。");
            if (!args.Account.Validate(ref errorList))
                throw new ArgumentException("Account", string.Join(Environment.NewLine, errorList.ToArray()));
            if (!string.IsNullOrWhiteSpace(args.Account.Tel) && !new Regex(QoApiConfiguration.REGEX_TEL).IsMatch(args.Account.Tel))
                throw new ArgumentException("電話番号のフォーマットが不正です。");

            try
            {
                // 顔写真キーのチェック
                Guid fileKey = Guid.Empty;
                if (!string.IsNullOrWhiteSpace(args.Account.PersonPhotoReference))
                    fileKey = args.Account.PersonPhotoReference.ToDecrypedReference<Guid>();

                // プライベートアカウント追加
                Guid newAccountKey = AccountFamilyWorker.AddPrivateAccount(args);

               
                if (newAccountKey != Guid.Empty)
                {
                    // 顔写真の登録（Emptyなら登録不要）
                    if (fileKey != Guid.Empty)
                        AccountFamilyWorker.UpdatePhotoKey(newAccountKey, fileKey);

                    result.Account = AccountFamilyWorker.BuildAccountFamilyViewItem(args.Executor ,AccountFamilyWorker.ReadAccountIndexEntity(newAccountKey), args.ActorKey.ToValueType<Guid>());
                    result.IsSuccess = bool.TrueString;
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                    // メタデータの書き換え
                    UpdateMetadata(fileKey, newAccountKey);
                }
                else
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError);
                
            }
            catch (Exception ex)
            {
                result.Result = QoApiResult.Build(ex);
            }

            return result;
        }


        /// <summary>
        /// 子アカウント詳細保存します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。")]
        public static QoAccountFamilyDetailWriteApiResults DetailWrite(QoAccountFamilyDetailWriteApiArgs args)
        {
            QoAccountFamilyDetailWriteApiResults result = new QoAccountFamilyDetailWriteApiResults() { IsSuccess = bool.FalseString };
            List<string> errorList = new List<string>();
            if (args.Account == null)
                throw new ArgumentNullException("Account", "アカウント情報が不正です。");
            if (string.IsNullOrWhiteSpace(args.AccountKeyReference))
                throw new ArgumentNullException("AccountKeyReference", "アカウントキー参照文字列が不正です。");
            if (!args.Account.Validate(ref errorList))
                throw new ArgumentException("Account", string.Join(Environment.NewLine, errorList.ToArray()));
            if (!string.IsNullOrWhiteSpace(args.Account.Tel) && !new Regex(QoApiConfiguration.REGEX_TEL).IsMatch(args.Account.Tel))
                throw new ArgumentException("電話番号のフォーマットが不正です。");

            Guid accountKey = args.AccountKeyReference.ToDecrypedReference<Guid>();
            // 顔写真キーのチェック
            Guid fileKey = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(args.Account.PersonPhotoReference))
            {
                fileKey = args.Account.PersonPhotoReference.ToDecrypedReference<Guid>();
                // メタデータの書き換え
                UpdateMetadata(fileKey, accountKey);
            }

            QH_ACCOUNTINDEX_DAT indexEntity = ReadAccountIndexEntity(accountKey);

            if (indexEntity != null && indexEntity.IsKeysValid())
            {
                // アカウントインデックスの更新（更新対象は全て入力必須、氏名は暗号化対象）
                using (QsCrypt crypt = new QsCrypt(QsCryptTypeEnum.QolmsSystem))
                {                  
                    indexEntity.FAMILYNAME = crypt.EncryptString(args.Account.FamilyName);
                    indexEntity.GIVENNAME = crypt.EncryptString(args.Account.GivenName);
                    indexEntity.FAMILYKANANAME = crypt.EncryptString(args.Account.FamilyNameKana);
                    indexEntity.GIVENKANANAME = crypt.EncryptString(args.Account.GivenNameKana);
                    indexEntity.SEXTYPE = (byte)args.Account.Sex.ToValueType<QsDbSexTypeEnum>();
                    indexEntity.BIRTHDAY = args.Account.Birthday.ToValueType<DateTime>();
                    indexEntity.PHOTOKEY = string.IsNullOrWhiteSpace(args.Account.PersonPhotoReference) ? Guid.Empty : fileKey;
                    indexEntity.UPDATEDDATE = DateTime.Now;
                    indexEntity.DataState = QsDbEntityStateTypeEnum.Modified;
                }


                try
                {
                    QhAccountIndexEntityWriter writer = new QhAccountIndexEntityWriter();
                    QhAccountIndexEntityWriterArgs writerArgs = new QhAccountIndexEntityWriterArgs() { Data =new List<QH_ACCOUNTINDEX_DAT>() { indexEntity } };
                    QhAccountIndexEntityWriterResults writerResults = QsDbManager.Write(writer, writerArgs);

                    if (writerResults != null)
                    {
                        if (writerResults.IsSuccess && writerResults.Result == 1)
                        {
                            // 電話番号の更新（連絡手帳）
                            // If Not String.IsNullOrWhiteSpace(args.Account.Tel) Then AccountFamilyWorker.WriteContactEntity(accountKey, args.Account.Tel)

                            result.IsSuccess = bool.TrueString;
                            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
                        }
                    }
                }
                catch (Exception ex)
                {
                    QoAccessLog.WriteErrorLog(ex, args.Executor.TryToValueType(Guid.Empty));
                    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "アカウント情報の更新に失敗しました。" );
                }
            }
            else
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "アカウントが存在しません。");

            return result;
        }

        /// <summary>
        /// 子アカウントを削除します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Obsolete("廃止予定です。")]
        public static QoAccountFamilyDeleteApiResults Delete(QoAccountFamilyDeleteApiArgs args)
        {
            QoAccountFamilyDeleteApiResults result = new QoAccountFamilyDeleteApiResults
            { 
                IsSuccess = bool.FalseString 
            };

            if (string.IsNullOrWhiteSpace(args.AccountKeyReference))
            {
                throw new ArgumentNullException("AccountKeyReference", "アカウントキー参照文字列が不正です。");
            }

            var accountKey = args.AccountKeyReference.ToDecrypedReference<Guid>();
            var entity = ReadAccountEntity(accountKey);

            if (entity == null || !entity.IsKeysValid())
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.NotFoundError, "アカウントが存在しません。");
                return result;
            }

            // プライベートアカウントのみ削除可能
            if (!entity.PRIVATEACCOUNTFLAG)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "パブリックアカウントは削除できません。");
                return result;
            }

            
            // IdentityApiを呼んで連携解除＆アカウント削除処理
            //QiQolmsAccountConnectWriteApiResults apiResults = QoIdentityClient.ExecuteAccountConnectWriteApi(args.ActorKey.ToValueType<Guid>(), args.AccountKeyReference.ToDecrypedReference<Guid>(), args.ExecutorName);

            //if (apiResults != null && apiResults.IsSuccess == bool.TrueString)
            //{
            //    result.IsSuccess = bool.TrueString;
            //    result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
            //}

            // IdentityApiの削除処理は不完全なのでOpenApi側で完全に削除処理を行う
            var writerArgs = new FamilyDeleteWriterArgs
            {
                ParentAccountKey = args.ActorKey.TryToValueType(Guid.Empty),
                AccountKey = accountKey,
                AuthorKey = args.AuthorKey.TryToValueType(Guid.Empty)
            };

            var writerResult = QsDbManager.Write(new FamilyDeleteWriter(), writerArgs);

            if (!writerResult.IsSuccess)
            {
                result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.OperationError, "アカウント削除に失敗しました。");
                return result;
            }


            result.IsSuccess = bool.TrueString;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);            

            return result;
        }

        /// <summary>
        /// 顔写真を取得します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountFamilyPersonPhotoReadApiResults PersonPhotoRead(QoAccountFamilyPersonPhotoReadApiArgs args)
        {
            QoAccountFamilyPersonPhotoReadApiResults result = new QoAccountFamilyPersonPhotoReadApiResults() { IsSuccess = bool.FalseString };

            if (string.IsNullOrWhiteSpace(args.PersonPhotoReference))
                throw new ArgumentNullException("PersonPhotoReference", "ファイルキー参照文字列が不正です。");

            result.PersonPhoto = AccountFamilyWorker.GetPersonPhoto(args);
            result.IsSuccess =result.PersonPhoto.Data.Length >0 ?  bool.TrueString:bool.FalseString ;
            result.Result = QoApiResult.Build(QoApiResultCodeTypeEnum.Success);
      

            return result;
        }
        
        /// <summary>
        /// 顔写真を保存します。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static QoAccountFamilyPersonPhotoWriteApiResults PersonPhotoWrite(QoAccountFamilyPersonPhotoWriteApiArgs args)
        {
            return AccountFamilyWorker.SetPersonPhoto(args);
        }

        /// <summary>
        /// 親アカウントキーから子アカウントのAccountIndexを取得
        /// </summary>
        /// <param name="publicAccountKey"></param>
        /// <returns></returns>
        public static List<QH_ACCOUNTINDEX_DAT> ReadAccountFamily(Guid publicAccountKey)
        {
            List<QH_ACCOUNTINDEX_DAT> result = new List<QH_ACCOUNTINDEX_DAT>();

            AccountFamilyReader reader = new AccountFamilyReader();
            AccountFamilyReaderArgs readerArgs = new AccountFamilyReaderArgs() { AccountKey = publicAccountKey };
            AccountFamilyReaderResults readerResults = QsDbManager.Read(reader, readerArgs);

            if (readerResults != null && readerResults.IsSuccess && readerResults.Result.Any())
                result = readerResults.Result;

            return result;
        }
        #endregion
    }


}