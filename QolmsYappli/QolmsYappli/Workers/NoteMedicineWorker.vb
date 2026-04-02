Imports System.Threading.Tasks
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsDbEntityV1
Imports MGF.QOLMS.QolmsCryptV1
Imports MGF.QOLMS.JAHISMedicineEntityV1
Imports System.Reflection

''' <summary>
''' 「レシピ動画」画面に関する機能を提供します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Friend NotInheritable Class NoteMedicineWorker



#Region "Constructor"

    ''' <summary>
    ''' デフォルトコンストラクタは使用できません。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub

#End Region

#Region "Private Method"


    ''' <summary>
    ''' 「お薬手帳」画面の取得 API を実行します。
    ''' </summary>
    ''' <param name="mainModel">メインモデル。</param>
    ''' 画面表示に必要な初期値を戻り値クラスに含めるなら True、
    ''' 含めないなら False を指定。
    ''' </param>
    ''' <param name="startDate">表示開始日。</param>
    ''' <param name="endDate">表示終了日。</param>
    ''' <param name="pageIndex">ページインデックス。</param>
    ''' <returns>
    ''' Web API 戻り値クラス。
    ''' </returns>
    ''' <remarks></remarks>
    Private Shared Function ExecuteNoteMedicineReadApi(mainModel As QolmsYappliModel) As QhYappliNoteMedicineReadApiResults

        Dim apiArgs As New QhYappliNoteMedicineReadApiArgs(
            QhApiTypeEnum.YappliNoteMedicineRead,
            QsApiSystemTypeEnum.Qolms,
            mainModel.ApiExecutor,
            mainModel.ApiExecutorName
        ) With {
            .ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
        }

        Dim apiResults As QhYappliNoteMedicineReadApiResults = QsApiManager.ExecuteQolmsApi(Of QhYappliNoteMedicineReadApiResults)(
            apiArgs,
            mainModel.SessionId,
            mainModel.ApiAuthorizeKey
        )

        With apiResults
            If .IsSuccess.TryToValueType(False) Then
                Return apiResults
            Else
                Throw New InvalidOperationException(String.Format("{0} APIの実行に失敗しました。", QsApiManager.GetQolmsApiName(apiArgs)))
            End If
        End With

    End Function


    Private Shared Function ToMedicineUsageN(target As JM_Message) As List(Of QhMedicineSetUsageItemOfJson)

        Dim result As New List(Of QhMedicineSetUsageItemOfJson)()

        For Each prescription As JM_Prescription In target.Prescription_List

            For Each rpSet As JM_RpSet In prescription.RpSet_List

                For Each rp As JM_Rp In rpSet.Rp_List
                    Dim medicineN As New List(Of QhMedicineSetEthicalDrugItemOfJson)()

                    For Each medicine As JM_Medicine In rp.Medicine_List

                        Dim medicineItem As New QhMedicineSetEthicalDrugItemOfJson With {
                         .Dose = medicine.No201.No201_4,
                         .MedicineName = medicine.No201.No201_3,
                         .Unit = medicine.No201.No201_5
                        }

                        medicineN.Add(medicineItem)

                    Next

                    result.Add(New QhMedicineSetUsageItemOfJson() With {
                                .Days = rp.No301.No301_4,
                                .DoctorId = String.Empty,
                                .DoctorName = If(rpSet IsNot Nothing AndAlso rpSet.No055 IsNot Nothing, rpSet.No055.No055_2, String.Empty),
                                .DosageForm = rp.No301.No301_6.TryToValueType(QsDbDosageFormTypeEnum.None),
                                .MedicineN = medicineN,
                                .RepresentedOrganizationId = String.Empty,
                                .RepresentedOrganizationName = If(rpSet IsNot Nothing AndAlso rpSet.No055 IsNot Nothing, rpSet.No055.No055_3, String.Empty),
                                .Unit = rp.No301.No301_5,
                                .Usage = rp.No301.No301_3
                            })
                Next

            Next

        Next

        Return result

    End Function


    ''' <summary>
    ''' 同名のプロパティ名の値をコピーする
    ''' </summary>
    ''' <param name="org"></param>
    ''' <param name="dest"></param>
    ''' <remarks></remarks>
    Private Shared Sub CopyProperty(org As Object, dest As Object)

        'それぞれのタイプを取得
        Dim fromType As Type = org.GetType()
        Dim toType As Type = dest.GetType()

        'メンバーリスト
        Dim fromMembers As MemberInfo() = fromType.GetMembers(
            BindingFlags.Public Or BindingFlags.NonPublic Or
            BindingFlags.Instance Or BindingFlags.Static Or
            BindingFlags.DeclaredOnly)
        Dim toMembers As MemberInfo() = toType.GetMembers(
            BindingFlags.Public Or BindingFlags.NonPublic Or
            BindingFlags.Instance Or BindingFlags.Static Or
            BindingFlags.DeclaredOnly)

        '名称用のリストを用意
        Dim nameList As New List(Of String)
        For Each m As MemberInfo In toMembers
            'プロパティの場合のみ
            If m.MemberType = MemberTypes.Property Then
                nameList.Add(m.Name)
            End If
        Next

        '同名プロパティの値をコピー
        For Each m As MemberInfo In fromMembers
            If m.MemberType = MemberTypes.Property Then
                '同名のプパティがあれば転送する
                If nameList.Contains(m.Name) Then
                    toType.GetProperty(m.Name).SetValue(dest, fromType.GetProperty(m.Name).GetValue(org, Nothing))
                End If
            End If
        Next

    End Sub


#End Region


#Region "Public Method"

    Public Shared Function InitializeViewModel(mainModel As QolmsYappliModel) As NoteMedicineViewModel

        ' 1ページ当たりのデータ件数
        'Const DATA_COUNT As Integer = 10

        Dim now As Date = Date.Now
        '
        'APIを実行()
        With NoteMedicineWorker.ExecuteNoteMedicineReadApi(mainModel)

            'テーブル情報を各手帳の形式へ変換
            Dim mediList As New List(Of MedicineItem)()

            If Not .MedicineN Is Nothing AndAlso .MedicineN.Count > 0 Then
                For Each item As QhApiMedicineItem In .MedicineN
                    Select Case True

                        Case item.DataType <> "4" '調剤薬
                            If Not String.IsNullOrWhiteSpace(item.ConvertedMedicineSet) Then

                                Dim jahisSetItem As String = String.Empty
                                Using crypt As New QsCrypt(QsCryptTypeEnum.QolmsSystem)
                                    jahisSetItem = crypt.DecryptString(item.ConvertedMedicineSet)
                                End Using
                                Dim ser As New QsJsonSerializer()
                                Dim jmsetitem As JM_Message = ser.Deserialize(Of JM_Message)(jahisSetItem)

                                Dim pharmacistname As String = If(jmsetitem.Prescription_List(0).No015 IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(jmsetitem.Prescription_List(0).No015.No015_2), jmsetitem.Prescription_List(0).No015.No015_2, String.Empty)
                                Dim pharmacistId As String = String.Empty
                                Dim facilityId As String = If(jmsetitem.Prescription_List(0).No051 IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(jmsetitem.Prescription_List(0).No051.No051_5), jmsetitem.Prescription_List(0).No051.No051_5, String.Empty)
                                Dim facilityName As String = If(jmsetitem.Prescription_List(0).No051 IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(jmsetitem.Prescription_List(0).No051.No051_2), jmsetitem.Prescription_List(0).No051.No051_2, String.Empty)
                                Dim pharmacyId As String = If(jmsetitem.Prescription_List(0).No011 IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(jmsetitem.Prescription_List(0).No011.No011_5), jmsetitem.Prescription_List(0).No011.No011_5, String.Empty)
                                Dim pharmacyName As String = If(jmsetitem.Prescription_List(0).No011 IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(jmsetitem.Prescription_List(0).No011.No011_2), jmsetitem.Prescription_List(0).No011.No011_2, String.Empty)
                                Dim specialNotes As String = If(jmsetitem.Prescription_List(0).No501_List IsNot Nothing AndAlso jmsetitem.Prescription_List(0).No501_List.Any() AndAlso Not String.IsNullOrWhiteSpace(jmsetitem.Prescription_List(0).No501_List(0).No501_2), jmsetitem.Prescription_List(0).No501_List(0).No501_2, String.Empty)
                                Dim memo As String = String.Empty
                                If jmsetitem.Prescription_List(0).No601_List IsNot Nothing AndAlso jmsetitem.Prescription_List(0).No601_List.Any() Then
                                    For Each item601 As JM_No601 In jmsetitem.Prescription_List(0).No601_List
                                        memo += item601.No601_2
                                    Next
                                End If

                                Dim mediItem As New MedicineItem() With {
                                    .RecordDate = Date.Parse(item.RecordDate),
                                    .Sequence = Integer.Parse(item.Sequence),
                                    .DataType = Byte.Parse(item.DataType),
                                    .OwnerType = Byte.Parse(item.OwnerType),
                                    .PrescriptionDate = Date.Parse(item.PrescriptionDate),
                                    .CacheKey = String.Empty,
                                    .PharmacistName = pharmacistname,
                                    .PharmacistId = String.Empty,
                                    .FacilityId = facilityId,
                                    .FacilityName = facilityName,
                                    .PharmacyId = pharmacistId,
                                    .PharmacyName = pharmacyName,
                                    .SpecialNotes = specialNotes,
                                    .Memo = memo,
                                    .MedicineUsageN = NoteMedicineWorker.ToMedicineUsageN(jmsetitem)
                                }
                                If item.DataType = "100" Then
                                    'SSMIXデータは調剤情報がないので薬局情報を表示しない
                                    mediItem.HidePharmacyInf = True
                                End If

                                mediList.Add(mediItem)

                            End If

                    End Select
                Next

                '1ページの表示件数に満たない場合は全件取得する
                'If .MedicineN.Count >= DATA_COUNT AndAlso mediList.Count < DATA_COUNT Then
                '    Dim viewModel As NoteMedicineViewModel = GetAllMedicineN(mainModel, Cache)
                '    With viewModel.MedicineTablePartialViewModel
                '        If .MedicineAllSetN.Count > Cache.PageIndex Then
                '            .MedicineSetN = .MedicineAllSetN(Cache.PageIndex)
                '        End If
                '        viewModel.PageCount = .MedicineAllSetN.Count
                '    End With
                '    Return viewModel
                'End If
            End If

            Dim vieModel As New NoteMedicineViewModel() With {
                .StartDate = now.AddYears(-1),
                .EndDate = now,
                .PageIndex = .PageIndex,
                .DataCount = .DataCount,
                .PageCount = .PageCount,
                .ShowPre = True,
                .ShowNonPre = False
            }

            vieModel.MedicineTablePartialViewModel = New NoteMedicineTablePartialViewModel(vieModel, mediList)

            Return vieModel

        End With

    End Function


    Public Shared Function CreateViewModel(mainModel As QolmsYappliModel) As NoteMedicineViewModel

        Dim cache As New NoteMedicineViewModel() With {
            .PageIndex = 0,
            .ShowPre = True,
            .ShowNonPre = True
        }

        ' 表示条件を設定
        mainModel.GetModelPropertyCache(cache, Function(m) m.PageIndex)
        mainModel.GetModelPropertyCache(cache, Function(m) m.ShowPre)
        mainModel.GetModelPropertyCache(cache, Function(m) m.ShowNonPre)

        Dim pageIndex As Integer = cache.PageIndex
        Dim showPre As Boolean = cache.ShowPre
        Dim showNonPre As Boolean = cache.ShowNonPre

        ' 編集対象のモデルをキャッシュから取得
        cache = mainModel.GetInputModelCache(Of NoteMedicineViewModel)()

        If cache Is Nothing Then
            cache = InitializeViewModel(mainModel)
        End If

        With cache.MedicineTablePartialViewModel

            If .PrescriptionAllSetN.Count > pageIndex Then
                For Each item As PrescriptionItem In .PrescriptionAllSetN(pageIndex)
                    Dim medicineItem As New MedicineItem
                    CopyProperty(item, medicineItem)
                    .MedicineSetN.Add(medicineItem)
                Next
            End If
            cache.PageCount = .PrescriptionAllSetN.Count

        End With

        ' 表示条件を設定
        cache.PageIndex = pageIndex
        cache.ShowPre = showPre
        cache.ShowNonPre = showNonPre

        Return cache

    End Function

#End Region

End Class
