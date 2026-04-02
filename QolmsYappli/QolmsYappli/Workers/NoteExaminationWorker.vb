Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsJwtAuthCore

Friend NotInheritable Class NoteExaminationWorker

#Region "Constant"

    ' とりあえずハートライフ病院の分を固定
    Private Shared ReadOnly HARTLIFE_HEALTHAGE_CALCULATION_VALUES As Dictionary(Of String, QyHealthAgeValueTypeEnum) _
        = New List(Of KeyValuePair(Of String, QyHealthAgeValueTypeEnum))() From {
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("350106", QyHealthAgeValueTypeEnum.BMI),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("350141", QyHealthAgeValueTypeEnum.Ch014),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("350137", QyHealthAgeValueTypeEnum.Ch014),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("350139", QyHealthAgeValueTypeEnum.Ch014),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("350142", QyHealthAgeValueTypeEnum.Ch016),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("350138", QyHealthAgeValueTypeEnum.Ch016),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("350140", QyHealthAgeValueTypeEnum.Ch016),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("000190", QyHealthAgeValueTypeEnum.Ch019),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("000210", QyHealthAgeValueTypeEnum.Ch021),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("000450", QyHealthAgeValueTypeEnum.Ch023),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("000080", QyHealthAgeValueTypeEnum.Ch025),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("000090", QyHealthAgeValueTypeEnum.Ch027),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("000120", QyHealthAgeValueTypeEnum.Ch029),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("701600", QyHealthAgeValueTypeEnum.Ch035),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("001000", QyHealthAgeValueTypeEnum.Ch035FBG),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("003080", QyHealthAgeValueTypeEnum.Ch037),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("003070", QyHealthAgeValueTypeEnum.Ch039)
        }.ToDictionary(Function(i) i.Key, Function(j) j.Value)


    ' JLAC10
    Private Shared ReadOnly JLAC10_HEALTHAGE_CALCULATION_VALUES As Dictionary(Of String, QyHealthAgeValueTypeEnum) _
        = New List(Of KeyValuePair(Of String, QyHealthAgeValueTypeEnum))() From {
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("9N011000000000001", QyHealthAgeValueTypeEnum.BMI),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("9A751000000000001", QyHealthAgeValueTypeEnum.Ch014),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("9A752000000000001", QyHealthAgeValueTypeEnum.Ch014),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("9A755000000000001", QyHealthAgeValueTypeEnum.Ch014),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("9A761000000000001", QyHealthAgeValueTypeEnum.Ch016),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("9A762000000000001", QyHealthAgeValueTypeEnum.Ch016),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("9A765000000000001", QyHealthAgeValueTypeEnum.Ch016),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F015000002327101", QyHealthAgeValueTypeEnum.Ch019),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F015000002327201", QyHealthAgeValueTypeEnum.Ch019),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F015000002399901", QyHealthAgeValueTypeEnum.Ch019),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F070000002327101", QyHealthAgeValueTypeEnum.Ch021),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F070000002327201", QyHealthAgeValueTypeEnum.Ch021),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F070000002399901", QyHealthAgeValueTypeEnum.Ch021),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F077000002327101", QyHealthAgeValueTypeEnum.Ch023),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F077000002327201", QyHealthAgeValueTypeEnum.Ch023),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F077000002391901", QyHealthAgeValueTypeEnum.Ch023),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3F077000002399901", QyHealthAgeValueTypeEnum.Ch023),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3B035000002327201", QyHealthAgeValueTypeEnum.Ch025),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3B035000002399901", QyHealthAgeValueTypeEnum.Ch025),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3B045000002327201", QyHealthAgeValueTypeEnum.Ch027),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3B045000002399901", QyHealthAgeValueTypeEnum.Ch027),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3B090000002327101", QyHealthAgeValueTypeEnum.Ch029),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3B090000002399901", QyHealthAgeValueTypeEnum.Ch029),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D046000001906202", QyHealthAgeValueTypeEnum.Ch035),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D046000001920402", QyHealthAgeValueTypeEnum.Ch035),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D046000001927102", QyHealthAgeValueTypeEnum.Ch035),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D046000001999902", QyHealthAgeValueTypeEnum.Ch035),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D010000001926101", QyHealthAgeValueTypeEnum.Ch035FBG),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D010000001927201", QyHealthAgeValueTypeEnum.Ch035FBG),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D010000001999901", QyHealthAgeValueTypeEnum.Ch035FBG),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("3D010000002227101", QyHealthAgeValueTypeEnum.Ch035FBG),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("1A020000000190111", QyHealthAgeValueTypeEnum.Ch037),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("1A020000000191111", QyHealthAgeValueTypeEnum.Ch037),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("1A010000000190111", QyHealthAgeValueTypeEnum.Ch039),
            New KeyValuePair(Of String, QyHealthAgeValueTypeEnum)("1A010000000191111", QyHealthAgeValueTypeEnum.Ch039)
        }.ToDictionary(Function(i) i.Key, Function(j) j.Value)


    Private Shared ReadOnly _requestUrl As New Lazy(Of String)(Function() NoteExaminationWorker.GetConfigSettings("ExaminationRequestUri"))
    Private Shared ReadOnly _jwtExecuter As New Lazy(Of String)(Function() NoteExaminationWorker.GetConfigSettings("ExaminationCryptedJWTExecuter"))
    Private Shared ReadOnly _siteUri As New Lazy(Of String)(Function() NoteExaminationWorker.GetConfigSettings("QolmsYappliSiteUri"))

    ''' <summary>
    ''' ReturnUrlに入れるパス（ドメイン以外）
    ''' </summary>
    Private Const RETURN_URL_PASS As String = "/start/ExamintaionReturn"


#End Region

#Region "Constructor"

    ''' <summary>
    ''' デフォルト コンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region
    
#Region "Property"

    Private Shared ReadOnly Property RequestUrl() As String
        Get
            Return _requestUrl.Value
        End Get
    End Property

    Private Shared ReadOnly Property JWTExecuter() As String
        Get
            Return _jwtExecuter.Value
        End Get
    End Property
        
    Private Shared ReadOnly Property SteUri() As String
        Get
            Return _siteUri.Value
        End Get
    End Property
#End Region

#Region "Private Method"

    ''' <summary>
    ''' Config設定を取得します。
    ''' </summary>
    ''' <param name="settingsName">ConfigのKey名</param>
    ''' <returns>
    ''' Configのvalue値
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetConfigSettings(settingsName As String) As String

        Dim result As String = String.Empty

        If Not String.IsNullOrWhiteSpace(settingsName) Then

            Try
                result = ConfigurationManager.AppSettings(settingsName)
            Catch
            End Try

        End If

        Return result

    End Function


    Private Shared Function ExecuteNoteExaminationReadApi(mainModel As QolmsYappliModel) As QjNoteExaminationReadApiResults

        Dim isInitialize As Boolean = False

        Dim apiArgs As New QjNoteExaminationReadApiArgs(
            QolmsApiCoreV1.QjApiTypeEnum.NoteExaminationRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
        }


        Dim apiResults As QjNoteExaminationReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjNoteExaminationReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

    End Function

    ''' <summary>
    ''' 「健診結果」画面の PDF ファイル 取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="linkageSystemNo">連携 システム 番号。</param>
    ''' <param name="linkageSystemId">連携 システム ID。</param>
    ''' <param name="recordDate">健診受診日。</param>
    ''' <param name="facilityKey">施設 キー。</param>
    ''' <param name="refOriginalName">取得した ファイル の オリジナル ファイル 名が格納される変数。</param>
    ''' <param name="refContentType">取得した ファイル の MIME タイプ が格納される変数。</param>
    ''' <returns>
    ''' 成功なら ファイルの バイト 配列、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteExaminationPdfReadApi(
        mainModel As QolmsYappliModel,
        fileKey As Guid,
        linkageSystemNo As Integer,
        linkageSystemId As String,
        recordDate As Date,
        facilityKey As Guid,
        ByRef refOriginalName As String,
        ByRef refContentType As String
    ) As Byte()

        refOriginalName = String.Empty
        refContentType = String.Empty

        Dim result As Byte() = Nothing

        Dim apiArgs As New QjNoteExaminationPdfReadApiArgs(
            QjApiTypeEnum.NoteExaminationPdfRead,
            QsApiSystemTypeEnum.QolmsJoto,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
            .FileKey = fileKey.ToApiGuidString(),
            .LinkageSystemNo = linkageSystemNo.ToString(),
            .LinkageSystemId = linkageSystemId,
            .RecordDate = recordDate.ToApiDateString(),
            .FacilityKey = facilityKey.ToApiGuidString(),
            .DataType = Convert.ToByte(QyExaminationDataTypeEnum.OverallAssessmentPdf).ToString()
        }
        Dim apiResults As QjNoteExaminationPdfReadApiResults = QsApiManager.ExecuteQolmsJotoApi(Of QjNoteExaminationPdfReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey2
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) _
                AndAlso String.Compare(IO.Path.GetExtension(.OriginalName), ".pdf", True) = 0 _
                AndAlso String.Compare(.ContentType, "application/pdf", True) = 0 _
                AndAlso Not String.IsNullOrWhiteSpace(.Data) Then

                refOriginalName = .OriginalName
                refContentType = .ContentType

                result = Convert.FromBase64String(.Data)
            Else
                Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsJotoApiName(apiArgs)))
            End If
        End With

        Return result

    End Function

    'Private Shared Function ExecuteNoteExaminationWriteApi(mainModel As QolmsYappliModel, model As PortalHospitalConnectionRequestInputModel) As QhYappliPortalHospitalConnectionRequestWriteApiResults

    '    Dim birthday As Date = New Date(Integer.Parse(model.BirthYear), Integer.Parse(model.BirthMonth), Integer.Parse(model.BirthDay))

    '    Dim apiArgs As New QhYappliNoteExaminationWriteApiArgs(
    '        QhApiTypeEnum.YappliPortalHospitalConnectionRequestWrite,
    '        QsApiSystemTypeEnum.Qolms,
    '        mainModel.ApiExecutor,
    '        mainModel.ApiExecutorName
    '    ) With {
    '        .Accountkey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
    '        .LinkageSystemNo = model.LinkageSystemNo.ToString(),
    '        .LinkageSystemId = model.LinkageSystemId,
    '        .FamilyName = model.FamilyName,
    '        .GivenName = model.GivenName,
    '        .FamilyKanaName = model.FamilyKanaName,
    '        .GivenKanaName = model.GivenKanaName,
    '        .SexType = model.SexType.ToString(),
    '        .BirthDay = birthday.ToApiDateString(),
    '        .IdentityUpdateFlag = model.IdentityUpdateFlag.ToString()
    '    }
    '    Dim apiResults As QhYappliPortalHospitalConnectionRequestWriteApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliPortalHospitalConnectionRequestWriteApiResults)(
    '        apiArgs,z
    '        mainModel.SessionId,
    '        mainModel.ApiAuthorizeKey
    '    )

    '    With apiResults
    '        If .IsSuccess.TryToValueType(False) Then
    '            Return apiResults
    '        Else
    '            Throw New InvalidOperationException(String.Format("{0} API の実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
    '        End If
    '    End With

    'End Function


    ''' <summary>
    ''' 検査結果表を作成します。
    ''' </summary>
    ''' <param name="items">検査情報のリスト。</param>
    ''' <param name="groups">検査種別（検査項目）のリスト</param>
    ''' <returns>
    ''' 検査結果表のリスト。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function CreateExaminationMatrix(mainModel As QolmsYappliModel, items As List(Of ExaminationSetItem), groups As List(Of ExaminationGroupItem)) As List(Of ExaminationMatrix)

        Dim resultN As New List(Of ExaminationMatrix)
        Dim result As New ExaminationMatrix()
        Dim cols As New List(Of ExaminationSetItem)(items)

        ' 表を作成
        If cols IsNot Nothing AndAlso cols.Any() AndAlso groups IsNot Nothing AndAlso groups.Any Then

            'ソート
            cols = cols.OrderBy(Function(i) i.RecordDate).ThenBy(Function(i) i.Sequence).ToList()

            ' 列を追加
            For Each col As ExaminationSetItem In cols
                ' 列キー　検査日＋シーケンス番号＋ID
                Dim colKey As String = String.Empty
                Dim associatedFileN As New List(Of AssociatedFileItem)()
                Dim examinationJudgementN As New Dictionary(Of String, ExaminationJudgementItem)()
                Dim healthAge As Decimal = Decimal.MinValue

                If ExaminationAxis.IsColKey(col.CacheKey, colKey) Then

                    Dim seq As Integer = Integer.MinValue
                    If col.Sequence = Integer.MinValue Then
                        seq = 0
                    Else
                        seq = col.Sequence
                    End If

                    colKey = String.Format("{0:yyyyMMdd}", col.RecordDate) + "_" + seq.ToString + "_" + col.LinkageSystemNo.ToString + "_" + col.OrganizationKey.ToString + "_" + col.CategoryId.ToString

                    If col.OverallAssessmentPdfN IsNot Nothing AndAlso col.OverallAssessmentPdfN.Any() Then
                        ' 検査手帳付随 ファイル 情報
                        col.OverallAssessmentPdfN.ToList().ForEach(
                            Sub(i)

                                If i.FacilityKey <> Guid.Empty Then

                                    Dim json As New AssociatedFileStorageReferenceJsonParameter() With {
                                        .Accountkey = mainModel.AuthorAccount.AccountKey.ToString(),
                                        .LoginAt = mainModel.AuthorAccount.LoginAt.ToString(),
                                        .RecordDate = i.RecordDate.ToString(),
                                        .FacilityKey = i.FacilityKey.ToString(),
                                        .LinkageSystemNo = i.LinkageSystemNo.ToString(),
                                        .LinkageSystemId = i.LinkageSystemId,
                                        .DataKey = i.DataKey.ToString()
                                        }

                                    associatedFileN.Add(
                                        New AssociatedFileItem() With {
                                            .DataType = QyExaminationDataTypeEnum.OverallAssessmentPdf,
                                            .DataKey = i.DataKey,
                                            .FacilityKey = i.FacilityKey,
                                            .RecordDate = i.RecordDate,
                                            .LinkageSystemNo = i.LinkageSystemNo,
                                            .LinkageSystemId = i.LinkageSystemId,
                                            .FileStorageReferenceJson = json.ToJsonString()
                                        }
                                    )
                                End If

                            End Sub
                        )
                    End If

                    If col.DicomUrlAccessKeyN IsNot Nothing AndAlso col.DicomUrlAccessKeyN.Any() Then
                        ' 検査手帳付随 DICOM画像URL 情報
                        col.DicomUrlAccessKeyN.Where(Function(i) i <> String.Empty).ToList().ForEach(
                            Sub(i)
                                Dim facilityKey As Guid = col.OrganizationKey.TryToValueType(Guid.Empty)

                                If facilityKey <> Guid.Empty Then
                                    associatedFileN.Add(
                                        New AssociatedFileItem() With {
                                            .DataType = QyExaminationDataTypeEnum.DicomData,
                                            .AdditionalKey = i,
                                            .FacilityKey = facilityKey,
                                            .RecordDate = col.RecordDate
                                        }
                                    )
                                End If

                            End Sub
                        )
                    End If


                    If col.ExaminationJudgementN IsNot Nothing AndAlso col.ExaminationJudgementN.Any() Then
                        ' 検査手帳付随 健診結果所見

                        examinationJudgementN = col.ExaminationJudgementN.ToDictionary(Of String)(Function(i) i.Name)

                    End If


                    If col.HealthAge > 0 Then
                        healthAge = col.HealthAge
                    End If

                    result.AddCol(colKey, col.OrganizationName, col.RecordDate.ToString("yyyy/M/d"), associatedFileN, examinationJudgementN, healthAge, String.Empty)

                End If
            Next

            'グループアイテムの、単位とスタンダードバリューに、
            '実際の検査として来た結果の単位とスタンダードバリューを突き合わせて入れる
            For Each grp As ExaminationGroupItem In groups

                For Each item As ExaminationItem In grp.ExaminationN

                    For Each tset As ExaminationSetItem In items
                        For Each titem As ExaminationItem In tset.ExaminationN
                            If titem.Code = item.Code Then
                                item.High = titem.High
                                item.Low = titem.Low
                                item.Unit = titem.Unit
                                item.ReferenceDisplayName = titem.ReferenceDisplayName
                                Exit For
                            End If
                        Next
                    Next
                Next
            Next

            ' 行を追加
            For Each grp As ExaminationGroupItem In groups

                '検査グループ行を追加
                result.AddGroupRow(grp)

                ' 行を追加
                For Each row As ExaminationItem In grp.ExaminationN

                    ' 検査項目コードを行キーとする
                    result.AddRow(row)
                Next
            Next

            ' 列をソート
            'result.SortColByKey()

            '' 総合判定を最終行へ移動
            'If Not String.IsNullOrWhiteSpace(totalJudgementCode) Then
            '    Dim pos As Integer = result.FindRow(totalJudgementCode)
            '    Dim children As List(Of String) = totalJudgementChildren.ToList()

            '    If pos < result.RowCount - 1 Then result.MoveRow(pos, result.RowCount - 1)
            '    If children.Count > 1 Then children.Sort() ' 子要素をソート

            '    ' 総合判定の子要素を展開
            '    children.ToList().ForEach(Sub(i) result.AddRow(i))
            'End If

            'todo-recomment
            ' 表に値をセット

            For Each col As ExaminationSetItem In cols
                Dim colKey As String = String.Empty

                If ExaminationAxis.IsColKey(col.CacheKey, colKey) Then

                    Dim dicHealthAgeValues As New Dictionary(Of String, String)()
                    Dim facilityKey As Guid = Guid.Parse(col.OrganizationKey)

                    For Each row As ExaminationItem In col.ExaminationN

                        Dim seq As Integer = Integer.MinValue
                        If col.Sequence = Integer.MinValue Then
                            seq = 0
                        Else
                            seq = col.Sequence
                        End If

                        colKey = String.Format("{0:yyyyMMdd}", col.RecordDate) + "_" + seq.ToString + "_" + col.LinkageSystemNo.ToString + "_" + col.OrganizationKey.ToString + "_" + col.CategoryId.ToString

                        'ハートライフ病院のみ固定
                        If facilityKey = Guid.Parse("4C2FC0EA-705A-42D2-B32A-85E9A4EECCDE") Then
                            If HARTLIFE_HEALTHAGE_CALCULATION_VALUES.ContainsKey(row.Code) _
                                AndAlso Not dicHealthAgeValues.ContainsKey(row.Code) Then
                                dicHealthAgeValues.Add(row.Code, row.Value)

                            End If
                        Else
                            If JLAC10_HEALTHAGE_CALCULATION_VALUES.ContainsKey(row.Code) _
                                AndAlso Not dicHealthAgeValues.ContainsKey(row.Code) Then
                                dicHealthAgeValues.Add(row.Code, row.Value)

                            End If
                        End If

                        result.SetItem(row, colKey, row.Code)

                    Next

                    'Dim seq As Integer = Integer.MinValue
                    'If col.Sequence = Integer.MinValue Then
                    '    seq = 0
                    'Else
                    '    seq = col.Sequence
                    'End If
                    'Dim key As String = String.Format("{0:yyyyMMdd}", col.RecordDate) + "_" + seq.ToString + "_" + col.CategoryId.ToString

                    'Dim healthAgeCalcFlag As Boolean = (HEALTHAGE_CALCULATION_VALUES.Keys.Count = dicHealthAgeValues.Keys.Count)
                    'キーの数が一致すれば全種の数値がそろっているので健康年齢を計算できるはず（中の数値を確認するかは別途）

                    'For Each item As String In HEALTHAGE_CALCULATION_VALUES.Keys

                    '    If Not dicHealthAgeValues.ContainsKey(item) Then
                    '        healthAgeCalcFlag = False
                    '        Exit For
                    '    End If

                    'Next

                    'result.AddColHealthAgeColcParamater(key, dicHealthAgeValues, healthAgeCalcFlag)

                End If
            Next

            ' 表の確定
            result.UpdateMatrix()

            resultN.Add(result)

        End If

        Return resultN

    End Function

    Private Shared Sub WorningLogValueCheck(values As List(Of Decimal), accountkey As Guid, recordDate As Date)

        If values.Count > 1 Then

            For Each val1 As Decimal In values

                If values.Where(Function(i) i <> val1).Count > 0 Then

                    AccessLogWorker.WriteAccessLog(Nothing, String.Empty, AccessLogWorker.AccessTypeEnum.None, String.Format("Warning ExaminationHealthAge AccountKey={0} RecordDate={1}", accountkey, recordDate))
                    Exit For

                End If

            Next
        End If

    End Sub

#End Region

#Region "共通"

    ''' <summary>
    ''' JWTトークン情報 を作成します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル</param>
    ''' <returns></returns>
    Public Shared Function GetExamintiontJwt(mainModel As QolmsYappliModel) As String


        ' 第1引数：暗号化された実行者アカウントを設定します。
        Dim executer As String = NoteExaminationWorker.JWTExecuter
        ' 第2引数：アカウントキーを設定します。
        Dim accountkey As Guid = mainModel.AuthorAccount.AccountKey
        ' 第3引数：親アカウントキーを設定します。
        Dim parentAccountkey As Guid = Guid.Empty
        ' 第4引数：2固定　この番号で健診画面を開きます。
        Dim pageType As Integer = 2

        ' トークンの有効期限を設定（初期値は60秒）
        Dim expireAddSec As Double = 60

        ' トークン発行
        Dim jwtCore As New QsJwtTokenProvider()
        Dim jwt As String = jwtCore.CreateQolmsJwtSsoKey(executer, accountkey, parentAccountkey, pageType, DateTime.Now.AddSeconds(expireAddSec))

        If String.IsNullOrWhiteSpace(jwt) Then

            Dim ErrorMessage As String = "トークンの発行に失敗しました。accountkey={0},executer={1},expire={2}"
            Throw New InvalidOperationException(String.Format(ErrorMessage,accountkey,executer,expireAddSec))
            
        End If

        Return jwt

    End Function


#End Region

#Region "Private Method"


    Shared Function CreateViewModel(mainModel As QolmsYappliModel) As NoteExaminationViewModel


        ' キャッシュから表示条件を取得
        Dim cache As New NoteExaminationViewModel() With {
            .StartDate = Date.MinValue,
            .EndDate = Date.MinValue
                    }

        mainModel.GetModelPropertyCache(cache, Function(m) m.StartDate)
        mainModel.GetModelPropertyCache(cache, Function(m) m.EndDate)

        ' 初期化フラグ
        ' Dim isInitialize As Boolean = (cache.StartDate = Date.MinValue OrElse cache.EndDate = Date.MinValue)

        'APIを実行()
        With NoteExaminationWorker.ExecuteNoteExaminationReadApi(mainModel)

            'If isInitialize Then
            ' 最新の検査日時と、期間が返却されるため、値を設定
            'cache.EndDate = .ExaminationLatestDate.TryToValueType(Date.Now.Date)
            'cache.StartDate = cache.EndDate.AddDays(1 - .ExaminationPeriod.TryToValueType(30))

            '表示条件を設定()
            'mainModel.SetNoteExaminationValues(cache.StartDate, cache.EndDate)

            'End If

            '検査結果項目リスト
            Dim examinationList As List(Of ExaminationSetItem) = .ExaminationSetN.ConvertAll(Function(i) i.ToExaminationSetItem)

            '検査種別リスト
            Dim groupList As List(Of ExaminationGroupItem) = .ExaminationGroupN.ConvertAll(Function(i) i.ToExaminationGroupItem)

            Dim dic As New Dictionary(Of Date, Dictionary(Of String, String))()

            For Each item As ExaminationSetItem In examinationList
                Dim dic2 As New Dictionary(Of String, String)()
                For Each item2 As ExaminationItem In item.ExaminationN

                    'ハートライフ病院のみ固定
                    If Guid.Parse(item.OrganizationKey) = Guid.Parse("4C2FC0EA-705A-42D2-B32A-85E9A4EECCDE") Then
                        If HARTLIFE_HEALTHAGE_CALCULATION_VALUES.ContainsKey(item2.Code) _
                            AndAlso Not dic2.ContainsKey(item2.Code) Then

                            dic2.Add(item2.Code, item2.Value)

                        End If
                    Else
                        If JLAC10_HEALTHAGE_CALCULATION_VALUES.ContainsKey(item2.Code) _
                           AndAlso Not dic2.ContainsKey(item2.Code) Then

                            dic2.Add(item2.Code, item2.Value)

                        End If
                    End If

                Next

                If Not dic.ContainsKey(item.RecordDate) Then dic.Add(item.RecordDate, dic2)

            Next

            Dim json As New NoteExaminationHelthAgeJsonParamater() With {
                .Accountkey = mainModel.AuthorAccount.AccountKey,
                .LoginAt = mainModel.AuthorAccount.LoginAt,
                .healthAgeCalcN = dic
            }
            Return New NoteExaminationViewModel(mainModel,
                                                cache.StartDate,
                                                cache.EndDate,
                                                NoteExaminationWorker.CreateExaminationMatrix(mainModel, examinationList, groupList),
                                                False,
                                                examinationList,
                                                groupList,
                                                json.ToJsonString)

        End With

    End Function

    ''' <summary>
    ''' 「健診結果」画面の PDF ファイル を取得します。
    ''' </summary>
    ''' <param name="mainModel">メイン モデル。</param>
    ''' <param name="fileKey">ファイル キー。</param>
    ''' <param name="linkageSystemNo">連携 システム 番号。</param>
    ''' <param name="linkageSystemId">連携 システム ID。</param>
    ''' <param name="recordDate">健診受診日。</param>
    ''' <param name="facilityKey">施設 キー。</param>
    ''' <param name="refOriginalName">取得した ファイル の オリジナル ファイル 名が格納される変数。</param>
    ''' <param name="refContentType">取得した ファイル の MIME タイプ が格納される変数。</param>
    ''' <returns>
    ''' 成功なら ファイルの バイト 配列、
    ''' 失敗なら例外を スロー。
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function GetPdfFile(
        mainModel As QolmsYappliModel,
        fileKey As Guid,
        linkageSystemNo As Integer,
        linkageSystemId As String,
        recordDate As Date,
        facilityKey As Guid,
        ByRef refOriginalName As String,
        ByRef refContentType As String
    ) As Byte()

        refOriginalName = String.Empty
        refContentType = String.Empty

        Dim result As Byte() = Nothing

        If fileKey <> Guid.Empty Then
            ' API を実行
            result = NoteExaminationWorker.ExecuteNoteExaminationPdfReadApi(mainModel, fileKey, linkageSystemNo, linkageSystemId, recordDate, facilityKey, refOriginalName, refContentType)
        End If

        Return result

    End Function


    Shared Function CreateHealthAgeEditInputModel(mainModel As QolmsYappliModel, inputModel As NoteExaminationHelthAgeJsonParamater) As List(Of HealthAgeEditInputModel)

        Dim result As New List(Of HealthAgeEditInputModel)()

        For Each item As KeyValuePair(Of Date, Dictionary(Of String, String)) In inputModel.healthAgeCalcN

            Dim dic As New Dictionary(Of QyHealthAgeValueTypeEnum, Decimal)

            For Each examination As KeyValuePair(Of String, String) In item.Value

                If HARTLIFE_HEALTHAGE_CALCULATION_VALUES.ContainsKey(examination.Key) Then

                    Dim valueType As QyHealthAgeValueTypeEnum = HARTLIFE_HEALTHAGE_CALCULATION_VALUES.Item(examination.Key)
                    Dim value As Decimal = Decimal.MinValue


                    If valueType = QyHealthAgeValueTypeEnum.Ch037 OrElse valueType = QyHealthAgeValueTypeEnum.Ch039 Then
                        Dim str As String = StrConv(examination.Value, VbStrConv.Wide).Replace("（", String.Empty).Replace("）", String.Empty) '全角に変換

                        Select Case str
                            Case "－"
                                'examination.Value.IndexOf("-") > 0
                                value = 1D
                            Case "±"
                                value = 2D
                            Case "＋", "１＋"
                                value = 3D
                            Case "＋＋", "２＋"
                                value = 4D
                            Case "＋＋＋", "３＋"
                                value = 5D

                            Case Else
                        End Select

                    Else
                        If Not Decimal.TryParse(examination.Value, value) Then
                            '変換できない数値があった場合は測定できない検査なので次へ
                            Continue For
                        End If

                    End If


                    If valueType = QyHealthAgeValueTypeEnum.Ch014 Then
                        '最高血圧
                        '平均値があれば平均を優先、ない場合は１回目→２回目
                        If examination.Key = "350141" OrElse _
                            (examination.Key = "350137" AndAlso Not item.Value.ContainsKey("350141")) OrElse _
                            (examination.Key = "350139" AndAlso Not item.Value.ContainsKey("350141") AndAlso Not item.Value.ContainsKey("350137")) Then

                            dic.Add(valueType, value)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch016 Then
                        '最低血圧
                        '平均値があれば平均を優先、ない場合は１回目→２回目
                        If examination.Key = "350142" OrElse _
                            (examination.Key = "350138" AndAlso Not item.Value.ContainsKey("350142")) OrElse _
                            (examination.Key = "350140" AndAlso Not item.Value.ContainsKey("350142") AndAlso Not item.Value.ContainsKey("350138")) Then

                            dic.Add(valueType, value)

                        End If
                    Else
                        dic.Add(valueType, value)

                    End If

                ElseIf JLAC10_HEALTHAGE_CALCULATION_VALUES.ContainsKey(examination.Key) Then

                    Dim valueType As QyHealthAgeValueTypeEnum = JLAC10_HEALTHAGE_CALCULATION_VALUES.Item(examination.Key)
                    Dim value As Decimal = Decimal.MinValue

                    If valueType = QyHealthAgeValueTypeEnum.Ch037 OrElse valueType = QyHealthAgeValueTypeEnum.Ch039 Then
                        Dim str As String = StrConv(examination.Value, VbStrConv.Wide).Replace("（", String.Empty).Replace("）", String.Empty) '全角に変換

                        Select Case str
                            Case "－"
                                'examination.Value.IndexOf("-") > 0
                                value = 1D
                            Case "±"
                                value = 2D
                            Case "＋", "１＋"
                                value = 3D
                            Case "＋＋", "２＋"
                                value = 4D
                            Case "＋＋＋", "３＋"
                                value = 5D

                            Case Else
                        End Select

                    Else
                        If Not Decimal.TryParse(examination.Value, value) Then
                            '変換できない数値があった場合は測定できない検査なので次へ
                            Continue For
                        End If

                    End If

                    If valueType = QyHealthAgeValueTypeEnum.Ch014 Then
                        '最高血圧
                        '１回目→２回目→その他
                        If (examination.Key = "9A751000000000001" AndAlso value > 0D) OrElse _
                            ((examination.Key = "9A752000000000001" AndAlso value > 0D) AndAlso _
                             (Not item.Value.ContainsKey("9A751000000000001") OrElse (item.Value.ContainsKey("9A751000000000001") AndAlso item.Value("9A751000000000001").TryToValueType(Decimal.MinValue) <= 0D))) OrElse _
                            ((examination.Key = "9A755000000000001" AndAlso value > 0D) AndAlso _
                             (Not item.Value.ContainsKey("9A751000000000001") OrElse (item.Value.ContainsKey("9A751000000000001") AndAlso item.Value("9A751000000000001").TryToValueType(Decimal.MinValue) <= 0D)) AndAlso _
                             (Not item.Value.ContainsKey("9A752000000000001") OrElse (item.Value.ContainsKey("9A752000000000001") AndAlso item.Value("9A751000000000001").TryToValueType(Decimal.MinValue) <= 0D))) Then

                            dic.Add(valueType, value)

                            ' 同じ種類の検査で数値が違うものがあれば Worning ログを書き込み
                            Dim values As List(Of Decimal) =
                                item.Value.Where(Function(i) _
                                                     i.Key = "9A755000000000001" OrElse _
                                                     i.Key = "9A751000000000001" OrElse _
                                                     i.Key = "9A755000000000001").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch016 Then
                        '最低血圧
                        '１回目→２回目→その他
                        If (examination.Key = "9A761000000000001" AndAlso value > 0D) OrElse _
                            ((examination.Key = "9A762000000000001" AndAlso value > 0D) AndAlso _
                            (Not item.Value.ContainsKey("9A761000000000001") OrElse (item.Value.ContainsKey("9A761000000000001") AndAlso item.Value("9A761000000000001").TryToValueType(Decimal.MinValue) <= 0D))) OrElse _
                            ((examination.Key = "9A765000000000001" AndAlso value > 0D) AndAlso _
                            (Not item.Value.ContainsKey("9A761000000000001") OrElse (item.Value.ContainsKey("9A761000000000001") AndAlso item.Value("9A761000000000001").TryToValueType(Decimal.MinValue) <= 0D)) AndAlso _
                            (Not item.Value.ContainsKey("9A762000000000001") OrElse (item.Value.ContainsKey("9A762000000000001") AndAlso item.Value("9A762000000000001").TryToValueType(Decimal.MinValue) <= 0D))) Then

                            dic.Add(valueType, value)

                            ' 同じ種類の検査で数値が違うものがあれば Worning ログを書き込み
                            Dim values As List(Of Decimal) =
                                item.Value.Where(Function(i) _
                                                     i.Key = "9A765000000000001" OrElse _
                                                     i.Key = "9A761000000000001" OrElse _
                                                     i.Key = "9A762000000000001").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch019 Then

                        If examination.Key = "3F015000002327101" OrElse _
                            (examination.Key = "3F015000002327201" AndAlso Not item.Value.ContainsKey("3F015000002327101")) OrElse _
                            (examination.Key = "3F015000002399901" AndAlso Not item.Value.ContainsKey("3F015000002327201") AndAlso Not item.Value.ContainsKey("3F015000002327101")) Then

                            dic.Add(valueType, value)

                            ' 同じ種類の検査で数値が違うものがあれば Worning ログを書き込み
                            Dim values As List(Of Decimal) =
                                item.Value.Where(Function(i) _
                                                     i.Key = "3F015000002327101" OrElse _
                                                     i.Key = "3F015000002327201" OrElse _
                                                     i.Key = "3F015000002399901").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch021 Then

                        If examination.Key = "3F070000002327101" OrElse _
                            (examination.Key = "3F070000002327201" AndAlso Not item.Value.ContainsKey("3F070000002327101")) OrElse _
                            (examination.Key = "3F070000002399901" AndAlso Not item.Value.ContainsKey("3F070000002327201") AndAlso Not item.Value.ContainsKey("3F070000002327101")) Then

                            dic.Add(valueType, value)

                            ' 同じ種類の検査で数値が違うものがあれば Worning ログを書き込み
                            Dim values As List(Of Decimal) =
                                item.Value.Where(Function(i) _
                                                     i.Key = "3F070000002327101" OrElse _
                                                     i.Key = "3F070000002327201" OrElse _
                                                     i.Key = "3F070000002399901").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch023 Then

                        If examination.Key = "3F077000002327101" OrElse _
                            (examination.Key = "3F077000002327201" AndAlso Not item.Value.ContainsKey("3F077000002327101")) OrElse _
                            (examination.Key = "3F077000002391901" AndAlso Not item.Value.ContainsKey("3F077000002327201") AndAlso Not item.Value.ContainsKey("3F077000002327101")) OrElse _
                            (examination.Key = "3F077000002399901" AndAlso Not item.Value.ContainsKey("3F077000002391901") AndAlso Not item.Value.ContainsKey("3F077000002327201") AndAlso Not item.Value.ContainsKey("3F077000002327101")) Then

                            dic.Add(valueType, value)

                            ' 同じ種類の検査で数値が違うものがあれば Worning ログを書き込み
                            Dim values As List(Of Decimal) =
                                item.Value.Where(Function(i) _
                                                     i.Key = "3F077000002327101" OrElse _
                                                     i.Key = "3F077000002327201" OrElse _
                                                     i.Key = "3F077000002391901" OrElse _
                                                     i.Key = "3F077000002399901").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch025 Then

                        If examination.Key = "3B035000002327201" OrElse _
                            (examination.Key = "3B035000002399901" AndAlso Not item.Value.ContainsKey("3B035000002327201")) Then

                            dic.Add(valueType, value)

                            Dim values As List(Of Decimal) =
                               item.Value.Where(Function(i) _
                                                    i.Key = "3B035000002327201" OrElse _
                                                    i.Key = "3B035000002399901").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch027 Then

                        If examination.Key = "3B045000002327201" OrElse _
                            (examination.Key = "3B045000002399901" AndAlso Not item.Value.ContainsKey("3B045000002327201")) Then

                            dic.Add(valueType, value)

                            Dim values As List(Of Decimal) =
                            item.Value.Where(Function(i) _
                                                  i.Key = "3B045000002327201" OrElse _
                                                  i.Key = "3B045000002399901").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch029 Then

                        If examination.Key = "3B090000002327101" OrElse _
                            (examination.Key = "3B090000002399901" AndAlso Not item.Value.ContainsKey("3B090000002327101")) Then

                            dic.Add(valueType, value)

                            Dim values As List(Of Decimal) =
                            item.Value.Where(Function(i) _
                                                i.Key = "3B045000002327201" OrElse _
                                                i.Key = "3B045000002399901").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch035 Then

                        If examination.Key = "3D046000001906202" OrElse _
                           (examination.Key = "3D046000001920402" AndAlso Not item.Value.ContainsKey("3D046000001906202")) OrElse _
                           (examination.Key = "3D046000001927102" AndAlso Not item.Value.ContainsKey("3D046000001920402") AndAlso Not item.Value.ContainsKey("3D046000001906202")) OrElse _
                           (examination.Key = "3D046000001999902" AndAlso Not item.Value.ContainsKey("3D046000001927102") AndAlso Not item.Value.ContainsKey("3D046000001920402") AndAlso Not item.Value.ContainsKey("3D046000001906202")) Then

                            dic.Add(valueType, value)

                            Dim values As List(Of Decimal) =
                            item.Value.Where(Function(i) _
                                                i.Key = "3D046000001906202" OrElse _
                                                i.Key = "3D046000001920402" OrElse _
                                                i.Key = "3D046000001927102" OrElse _
                                                i.Key = "3D046000001999902").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)
                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch035FBG Then

                        If examination.Key = "3D010000001926101" OrElse _
                           (examination.Key = "3D010000001927201" AndAlso Not item.Value.ContainsKey("3D010000001926101")) OrElse _
                           (examination.Key = "3D010000001999901" AndAlso Not item.Value.ContainsKey("3D010000001927201") AndAlso Not item.Value.ContainsKey("3D010000001926101")) OrElse _
                           (examination.Key = "3D010000002227101" AndAlso Not item.Value.ContainsKey("3D010000001999901") AndAlso Not item.Value.ContainsKey("3D010000001927201") AndAlso Not item.Value.ContainsKey("3D010000001926101")) Then

                            dic.Add(valueType, value)

                            Dim values As List(Of Decimal) =
                            item.Value.Where(Function(i) _
                                                i.Key = "3D010000001926101" OrElse _
                                                i.Key = "3D010000001927201" OrElse _
                                                i.Key = "3D010000001999901" OrElse _
                                                i.Key = "3D010000002227101").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch037 Then

                        If examination.Key = "1A020000000190111" OrElse _
                           (examination.Key = "1A020000000191111" AndAlso Not item.Value.ContainsKey("1A020000000190111")) Then

                            dic.Add(valueType, value)

                            Dim values As List(Of Decimal) =
                              item.Value.Where(Function(i) _
                                                  i.Key = "1A020000000190111" OrElse _
                                                  i.Key = "1A020000000191111").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    ElseIf valueType = QyHealthAgeValueTypeEnum.Ch039 Then

                        If examination.Key = "1A010000000190111" OrElse _
                           (examination.Key = "1A010000000191111" AndAlso Not item.Value.ContainsKey("1A010000000190111")) Then

                            dic.Add(valueType, value)
                            Dim values As List(Of Decimal) =
                            item.Value.Where(Function(i) _
                                                i.Key = "1A010000000190111" OrElse _
                                                i.Key = "1A010000000191111").Select(Function(i) i.Value.TryToValueType(Decimal.MinValue)).ToList()

                            NoteExaminationWorker.WorningLogValueCheck(values, mainModel.AuthorAccount.AccountKey, item.Key)

                        End If

                    Else

                        dic.Add(valueType, value)

                    End If

                End If

            Next

            If dic.Count = 13 Then '13種類で全部なので足りなかったら測定できないのでスルー
                result.Add(HealthAgeEditWorker.CreateInputModelByExamination(mainModel, item.Key, dic, QyPageNoTypeEnum.NoteExamination))

            End If

        Next

        Return result

    End Function

    Public Shared Function CreateExaminationMatrixFromItems(mainModel As QolmsYappliModel, items As List(Of ExaminationSetItem), groups As List(Of ExaminationGroupItem)) As List(Of ExaminationMatrix)

        Return CreateExaminationMatrix(mainModel, items, groups)

    End Function


    Public Shared Function GetExaminationPage(mainModel As QolmsYappliModel, fromPageNoType As QyPageNoTypeEnum) As String

        'QolmsNoteの健診ページ呼び出し
        Dim uri As String = NoteExaminationWorker.RequestUrl
        If uri.EndsWith("/") Then
           　uri = uri.TrimEnd(CType("/", Char))
        End If
        dim ReturnUrl As string = NoteExaminationWorker.SteUri
                
        ' HOMEに戻る
        If ReturnUrl.EndsWith("/") Then
           ReturnUrl = ReturnUrl.TrimEnd(CType("/", Char))
        End If

        ReturnUrl+=RETURN_URL_PASS

        If fromPageNoType = QyPageNoTypeEnum.PortalCompanyConnectionHome Then
            ReturnUrl+= "?fromPageNo=34"
        Else  If fromPageNoType = QyPageNoTypeEnum.PortalHome
            ReturnUrl+= "?fromPageNo=1"
        End If

        Dim url As String =  String.Format("{0}?pageno=2&jwt={1}&redirecturl={2}",uri, System.Uri.EscapeDataString(NoteExaminationWorker.GetExamintiontJwt(mainModel)), System.Uri.EscapeDataString(ReturnUrl))
        
        Return url

    End Function


#End Region

End Class
