Imports System.Collections.ObjectModel
Imports System.Globalization
Imports System.Web.Configuration

''' <summary>
''' 「沖縄セルラー Yappli」で使用する読み取り専用ディクショナリを表します。
''' このクラスは継承できません。
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class QyDictionary

#Region "Private Method"

    ''' <summary>
    ''' 年数と、西暦・和暦の文字列のディクショナリ―を作成します。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetCalendar() As Dictionary(Of String, String)

        Dim dic As New Dictionary(Of String, String)()

        Dim viewstr As String = String.Empty
        Dim startNo As Integer = Date.Now.Year
        Dim nowYearFirstDate As New Date(Date.Now.Year, 1, 1)
        Dim nowYearLastDate As New Date(Date.Now.Year, 12, 31)

        Dim wareki As String = String.Empty

        Dim jcalendar As JapaneseCalendar = New JapaneseCalendar()
        Dim ci As New CultureInfo("ja-JP", True)

        ci.DateTimeFormat.Calendar = New JapaneseCalendar()

        'メモ：和暦カレンダー機能
        'jcalendar.GetEra(nowYear) = 1:明治、2:大正、3:昭和、4:平成
        'jcalendar.GetYear(nowYear) = 各年号における年数

        For i As Integer = startNo To Date.Now.AddYears(-120).Year Step -1

            '該当年の、元日と大晦日の年号を取得し、差異があれば２つとも表記する
            If String.Compare(nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci), _
                nowYearLastDate.ToString("yyyy") + nowYearLastDate.ToString("（ggy）年", ci)) = 0 Then
                dic.Add(i.ToString, nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci))
            Else
                dic.Add(i.ToString, nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy,", ci) + nowYearLastDate.ToString("ggy）年", ci).Replace("1", "元"))
            End If

            nowYearFirstDate = nowYearFirstDate.AddYears(-1)
            nowYearLastDate = nowYearLastDate.AddYears(-1)

        Next

        Return dic

    End Function

    ''' <summary>
    ''' 年数と、西暦・和暦の文字列のディクショナリ―を作成します。***特定健診対象者の生年選択用***
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetTokuteiCalendar() As Dictionary(Of String, String)

        Dim dic As New Dictionary(Of String, String)()

        Dim viewstr As String = String.Empty
        Dim startNo As Integer = Date.Now.Year
        Dim endNo As Integer = Date.Now.AddYears(-120).Year
        Dim nowYearFirstDate As New Date(Date.Now.Year, 1, 1)
        Dim nowYearLastDate As New Date(Date.Now.Year, 12, 31)

        Dim wareki As String = String.Empty

        Dim jcalendar As JapaneseCalendar = New JapaneseCalendar()
        Dim ci As New CultureInfo("ja-JP", True)

        ci.DateTimeFormat.Calendar = New JapaneseCalendar()

        'メモ：和暦カレンダー機能
        'jcalendar.GetEra(nowYear) = 1:明治、2:大正、3:昭和、4:平成
        'jcalendar.GetYear(nowYear) = 各年号における年数

        Dim lower As Integer = Integer.MinValue
        Dim upper As Integer = Integer.MinValue

        Try
            lower = Integer.Parse(WebConfigurationManager.AppSettings("TokuteiKensinLower").Trim())
            upper = Integer.Parse(WebConfigurationManager.AppSettings("TokuteiKensinUpper").Trim())
        Catch
        End Try

        If lower = Integer.MinValue Then
            lower = 18
        End If
        If upper = Integer.MinValue Then
            upper = 80
        End If

        'StartNo=Lower (今年39歳の人の生まれ年）
        startNo = Date.Now.Year - lower
        'EndNo = Upper (今年75歳の人の生まれ年）
        endNo = Date.Now.Year - upper

        nowYearFirstDate = New Date(startNo, 1, 1)
        nowYearLastDate = New Date(startNo, 12, 31)

        For i As Integer = startNo To endNo Step -1

            '該当年の、元日と大晦日の年号を取得し、差異があれば２つとも表記する
            If String.Compare(nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci), _
                nowYearLastDate.ToString("yyyy") + nowYearLastDate.ToString("（ggy）年", ci)) = 0 Then
                dic.Add(i.ToString, nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy）年", ci))
            Else
                dic.Add(i.ToString, nowYearFirstDate.ToString("yyyy") + nowYearFirstDate.ToString("（ggy,", ci) + nowYearLastDate.ToString("ggy）年", ci).Replace("1", "元"))
            End If

            nowYearFirstDate = nowYearFirstDate.AddYears(-1)
            nowYearLastDate = nowYearLastDate.AddYears(-1)

        Next

        Return dic

    End Function

#End Region

#Region "Public Property"

    ''' <summary>
    ''' 性別の種別のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property SexType As ReadOnlyDictionary(Of QySexTypeEnum, String)

        Get
            Return New ReadOnlyDictionary(Of QySexTypeEnum, String)(
                New Dictionary(Of QySexTypeEnum, String)() From {
                    {QySexTypeEnum.Male, "男性"},
                    {QySexTypeEnum.Female, "女性"}
                }
            )
        End Get

    End Property

    ' ''' <summary>
    ' ''' 血液型の種別のディクショナリを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property BloodType As ReadOnlyDictionary(Of QhBloodTypeEnum, String)

    '    Get
    '        Return New ReadOnlyDictionary(Of QhBloodTypeEnum, String)(
    '            New Dictionary(Of QhBloodTypeEnum, String)() From {
    '                {QhBloodTypeEnum.A, "A"},
    '                {QhBloodTypeEnum.B, "B"},
    '                {QhBloodTypeEnum.O, "O"},
    '                {QhBloodTypeEnum.AB, "AB"}
    '            }
    '        )
    '    End Get

    'End Property

    ' ''' <summary>
    ' ''' Rh 因子の種別のディクショナリを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property RhType As ReadOnlyDictionary(Of QhRhTypeEnum, String)

    '    Get
    '        Return New ReadOnlyDictionary(Of QhRhTypeEnum, String)(
    '            New Dictionary(Of QhRhTypeEnum, String)() From {
    '                {QhRhTypeEnum.Plus, "Rh+"},
    '                {QhRhTypeEnum.Minus, "Rh-"}
    '            }
    '        )
    '    End Get

    'End Property

    ' ''' <summary>
    ' ''' 個人電話の種別のディクショナリを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property PersonalPhoneType As ReadOnlyDictionary(Of QhPhoneTypeEnum, String)

    '    Get
    '        Return New ReadOnlyDictionary(Of QhPhoneTypeEnum, String)(
    '            New Dictionary(Of QhPhoneTypeEnum, String)() From {
    '                {QhPhoneTypeEnum.Home, "電話"},
    '                {QhPhoneTypeEnum.Mobile, "携帯"},
    '                {QhPhoneTypeEnum.Fax, "FAX"}
    '            }
    '        )
    '    End Get

    'End Property

    ' ''' <summary>
    ' ''' 勤務先電話の種別のディクショナリを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property OfficePhoneType As ReadOnlyDictionary(Of QhPhoneTypeEnum, String)

    '    Get
    '        Return New ReadOnlyDictionary(Of QhPhoneTypeEnum, String)(
    '            New Dictionary(Of QhPhoneTypeEnum, String)() From {
    '                {QhPhoneTypeEnum.Office, "電話"},
    '                {QhPhoneTypeEnum.OfficeMobile, "携帯"},
    '                {QhPhoneTypeEnum.OfficeFax, "FAX"}
    '            }
    '        )
    '    End Get

    'End Property

    ' ''' <summary>
    ' ''' 保険の種別のディクショナリを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property InsuranceType As ReadOnlyDictionary(Of QhInsuranceTypeEnum, String)

    '    Get
    '        Return New ReadOnlyDictionary(Of QhInsuranceTypeEnum, String)(
    '            New Dictionary(Of QhInsuranceTypeEnum, String)() From {
    '                {QhInsuranceTypeEnum.InsuranceA00, "国民健康保険"},
    '                {QhInsuranceTypeEnum.InsuranceB00, "社会保険"},
    '                {QhInsuranceTypeEnum.InsuranceA39, "後期高齢者医療保険"}
    '            }
    '        )
    '    End Get

    'End Property

    ' ''' <summary>
    ' ''' 要介護度の種別のディクショナリを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property CareLevelType As ReadOnlyDictionary(Of QhCareLevelTypeEnum, String)

    '    Get
    '        Return New ReadOnlyDictionary(Of QhCareLevelTypeEnum, String)(
    '            New Dictionary(Of QhCareLevelTypeEnum, String)() From {
    '                {QhCareLevelTypeEnum.Support, "要支援"},
    '                {QhCareLevelTypeEnum.Care1, "要介護度1"},
    '                {QhCareLevelTypeEnum.Care2, "要介護度2"},
    '                {QhCareLevelTypeEnum.Care3, "要介護度3"},
    '                {QhCareLevelTypeEnum.Care4, "要介護度4"},
    '                {QhCareLevelTypeEnum.Care5, "要介護度5"}
    '            }
    '        )
    '    End Get

    'End Property

    ''' <summary>
    ''' バイタル情報の測定条件の種別のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property VitalConditionType As ReadOnlyDictionary(Of QyVitalConditionTypeEnum, String)

        Get
            Return New ReadOnlyDictionary(Of QyVitalConditionTypeEnum, String)(
                New Dictionary(Of QyVitalConditionTypeEnum, String)() From {
                    {QyVitalConditionTypeEnum.None, "随時"},
                    {QyVitalConditionTypeEnum.Fasting, "空腹時"},
                    {QyVitalConditionTypeEnum.LessThanTwoHours, "食後2時間未満"},
                    {QyVitalConditionTypeEnum.NotLessThanTwoHours, "食後2時間以降"}
                }
            )
        End Get

    End Property

    ''' <summary>
    ''' 都道府県のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property Prefecture As ReadOnlyDictionary(Of Byte, String)

        Get
            Return New ReadOnlyDictionary(Of Byte, String)(
                New Dictionary(Of Byte, String) From {
                    {1, "北海道"},
                    {2, "青森県"},
                    {3, "岩手県"},
                    {4, "宮城県"},
                    {5, "秋田県"},
                    {6, "山形県"},
                    {7, "福島県"},
                    {8, "茨城県"},
                    {9, "栃木県"},
                    {10, "群馬県"},
                    {11, "埼玉県"},
                    {12, "千葉県"},
                    {13, "東京都"},
                    {14, "神奈川県"},
                    {15, "新潟県"},
                    {16, "富山県"},
                    {17, "石川県"},
                    {18, "福井県"},
                    {19, "山梨県"},
                    {20, "長野県"},
                    {21, "岐阜県"},
                    {22, "静岡県"},
                    {23, "愛知県"},
                    {24, "三重県"},
                    {25, "滋賀県"},
                    {26, "京都府"},
                    {27, "大阪府"},
                    {28, "兵庫県"},
                    {29, "奈良県"},
                    {30, "和歌山県"},
                    {31, "鳥取県"},
                    {32, "島根県"},
                    {33, "岡山県"},
                    {34, "広島県"},
                    {35, "山口県"},
                    {36, "徳島県"},
                    {37, "香川県"},
                    {38, "愛媛県"},
                    {39, "高知県"},
                    {40, "福岡県"},
                    {41, "佐賀県"},
                    {42, "長崎県"},
                    {43, "熊本県"},
                    {44, "大分県"},
                    {45, "宮崎県"},
                    {46, "鹿児島県"},
                    {47, "沖縄県"}
                }
            )
        End Get

    End Property

    ''' <summary>
    ''' AM/PM のディクショナリを表します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property Meridiem As ReadOnlyDictionary(Of String, String)

        Get
            Return New ReadOnlyDictionary(Of String, String)(
                New Dictionary(Of String, String)() From {
                    {"am", "午前"},
                    {"pm", "午後"}
                }
            )
        End Get

    End Property

    ''' <summary>
    ''' 年（西暦・和暦併記）のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property Year As ReadOnlyDictionary(Of String, String)

        Get
            Dim dic As Dictionary(Of String, String) = GetCalendar()

            Return New ReadOnlyDictionary(Of String, String)(dic)

        End Get

    End Property

    ''' <summary>
    ''' 年（西暦・和暦併記）のディクショナリを取得します。***特定健診対象者の生年選択用***
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property TokuteiYear As ReadOnlyDictionary(Of String, String)

        Get
            Dim dic As Dictionary(Of String, String) = GetTokuteiCalendar()

            Return New ReadOnlyDictionary(Of String, String)(dic)

        End Get

    End Property

    ''' <summary>
    ''' 食事の種別のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property MealType As ReadOnlyDictionary(Of QyMealTypeEnum, String)

        Get
            Return New ReadOnlyDictionary(Of QyMealTypeEnum, String)(
                New Dictionary(Of QyMealTypeEnum, String)() From {
                    {QyMealTypeEnum.Breakfast, "朝食"},
                    {QyMealTypeEnum.Lunch, "昼食"},
                    {QyMealTypeEnum.Dinner, "夕食"},
                    {QyMealTypeEnum.Snacking, "間食"}
                }
            )
        End Get

    End Property

    ' ''' <summary>
    ' ''' 歯の状態の種別のディクショナリを取得します。
    ' ''' </summary>
    ' ''' <value></value>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Shared ReadOnly Property ToothStateType As ReadOnlyDictionary(Of QhToothStateTypeEnum, String)

    '    Get
    '        Return New ReadOnlyDictionary(Of QhToothStateTypeEnum, String)(
    '            New Dictionary(Of QhToothStateTypeEnum, String)() From {
    '                {QhToothStateTypeEnum.Healthy, "/"},
    '                {QhToothStateTypeEnum.Decayed, "C"},
    '                {QhToothStateTypeEnum.Treated, "◯"},
    '                {QhToothStateTypeEnum.Lost, "△"}
    '            }
    '        )
    '    End Get

    'End Property

    ''' <summary>
    ''' 薬剤の剤型の種別のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property DosageFormType As ReadOnlyDictionary(Of QyDosageFormTypeEnum, String)

        Get
            Return New ReadOnlyDictionary(Of QyDosageFormTypeEnum, String)(
                New Dictionary(Of QyDosageFormTypeEnum, String)() From {
                    {QyDosageFormTypeEnum.Oral, "内服"},
                    {QyDosageFormTypeEnum.Drip, "内滴"},
                    {QyDosageFormTypeEnum.DoseOfMedicine, "頓服"},
                    {QyDosageFormTypeEnum.InjectionDrug, "注射"},
                    {QyDosageFormTypeEnum.External, "外用"},
                    {QyDosageFormTypeEnum.DipFry, "浸煎"},
                    {QyDosageFormTypeEnum.Touzai, "湯"},
                    {QyDosageFormTypeEnum.Materials, "材料"},
                    {QyDosageFormTypeEnum.Other, "その他"}
                }
            )
        End Get

    End Property

    ''' <summary>
    ''' 連携内容の種別のディクショナリを取得します。
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property RelationContentType As ReadOnlyDictionary(Of QyRelationContentTypeEnum, String)

        Get
            Return New ReadOnlyDictionary(Of QyRelationContentTypeEnum, String)(
                New Dictionary(Of QyRelationContentTypeEnum, String)() From {
                    {QyRelationContentTypeEnum.Information, "基本情報"},
                    {QyRelationContentTypeEnum.Vital, "バイタル情報"},
                    {QyRelationContentTypeEnum.Medicine, "お薬情報"},
                    {QyRelationContentTypeEnum.Examination, "検査・健診情報"},
                    {QyRelationContentTypeEnum.Contact, "連絡情報"},
                    {QyRelationContentTypeEnum.Dental, "歯科情報"},
                    {QyRelationContentTypeEnum.Assessment, "活動情報"},
                    {QyRelationContentTypeEnum.Meal, "食事情報"}
                }
            )
        End Get

    End Property

#End Region

End Class